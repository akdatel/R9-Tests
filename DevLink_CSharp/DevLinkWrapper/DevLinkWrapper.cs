﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
// TODO: Sort and Clean UINGS!

namespace DatelHelpers
{
	/*
	 * A long-running server (or client) program should have a built-in automatic “retry timer”. When any error is detected, 
	 * the socket should be closed and the retry timer should be started. When the retry timer goes off, then the operation 
	 * may be attempted again. The timer does not have to be very long: usually 1 second will suffice. 
	 * (TimeOut and ConnectionRefused can skip the timer and immediately retry.) http://blog.stephencleary.com/2009/05/error-handling.html
	 * 
	 TODO: Rename the file!!!
	 TODO: ADD logging!
	 TODO: WRITE TO FILE: maybe after all make it here, since this is the fist point-of-contact with the DevLinks 
	 *	(and also, we may not end up using C# Events, but a ConcurrentQueue to pass the DevLinks...)
	 *	
	 TODO: Thread safety here (IsConnected)?????? (Is it possible that when we check, IsConnected==FALSE, but there still IS an ACTIVE Connection)????????
	 * Exception Handling in CallBacks!!!!
	 *  + check thread names (native & managed)
	 * check if theConnect Callback blocks the DevLink thread or it is safe to call CloseConnection?
	 *		- closing doesn't seem to always work correctly if called too soon after connecting...
	 * 
	 */

	/// <summary>
	/// The Event Arguments for DevLinkRecieved event.
	/// </summary>
	public class DevLinkEventArgs : EventArgs
	{
		public int SiteId
		{
			get; private set;
		}
		
		public string RawDevLink
		{
			get; private set;
		}
		
		public DevLinkEventArgs(int siteId, string rawDevLink)
		{
			SiteId = siteId;
			RawDevLink = rawDevLink;
		}
	}

	/// <summary>
	/// MORE INFO on AVAYA's DevLink: https://downloads.avaya.com/elmodocs2/ip_office/DOCS3_0/DATA/Additional/mergedProjects/manuals/manual_pdfs/cti/devlinken.pdf
	/// Good explanation of IDisposeable: http://stackoverflow.com/questions/538060/proper-use-of-the-idisposable-interface
	/// </summary>
	/// <remarks>
	/// - All application call-backs [are] made on a thread which DevLink creates  (it looks like a background thread). 
	/// As a result, consideration must be given by programmers to ensure that all functions 
	/// called from within a call-back are thread-safe. 
	/// https://downloads.avaya.com/elmodocs2/ip_office/DOCS3_0/DATA/Additional/mergedProjects/manuals/manual_pdfs/cti/devlinken.pdf 
	/// </remarks>
	public class DevLinkWrapper : IDisposable
	{
		public class DevLinkMessage
		{
			public string RawDevLink { get; set; }
			public string IpAddress { get; set; }
			public int SiteId { get; set; }
		}
		#region ENUMS ------------------------------------------------------->>>

		/// <summary>
		/// Possible responses from the DevLink connection callback:
		/// </summary>
		private enum ConnectionResponse
		{
			/// <summary>
			/// Communications established. This occurs either after the initial call to DLOpen(), or after the  system unit has come back on-line after being powered off or rebooted.
			/// </summary>
			DEVLINK_COMMS_OPERATIONAL_0		= 0,

			/// <summary>
			/// No response from system unit. This occurs either after the initial call to DLOpen(), or if the system unit is powered off or rebooted. It can also occur if network problems prevent communications.
			/// </summary>
			DEVLINK_COMMS_NORESPONSE_1		= 1,

			/// <summary>
			/// Reserved for future use
			/// </summary>
			DEVLINK_COMMS_REJECTED_2		= 2,
			
			/// <summary>
			/// Packets were generated by the IP Office system unit, but were not received by DevLink. This can occur either because the IPO is under heavy load, 
			/// or because the application using DevLink did not return from a callback quickly enough. Applications should ensure that they do not take more than 100 milliseconds to process events.
			/// </summary>
			DEVLINK_COMMS_MISSEDPACKETS_3	= 3
		}

		public enum ConnectionStates
		{
			Disconnected,	// no connection tried yet, or already disconnected
			Connecting,		// in the process of connecting
			Connected,		// connected! :)
			Failed,			// DevLink returned an error
			TimedOut		// Timeout expired before getting a response from DevLink
		}

		#endregion ENUMS ----------------------<<<---------------------------------


		#region Events for the Client (Collection) ------------------------------------------------------->>>

	//	public delegate void DevLinkRecievedEventHandler(DevLinkWrapper devLinkWrapper, DevLinkEventArgs e);
		/// <summary>
		/// NOTE: when providing an event handler for this event, plan for the even handler being executed even if you just unsubscribed
		/// (muti-threading quirks)!
		/// </summary>
		public event EventHandler<DevLinkEventArgs> DevLinkRecieved;

		#endregion Events for Client (Collection) ----------------------<<<---------------------------------


		#region IDisposable Stuff ------------------------------------------------------->>>

		private bool _disposed;  //Has the Dispose method already been called?

		#endregion  IDisposable Stuff ----------------------<<<---------------------------------


		#region DevLink Exposed Functions Stuff ------------------------------------------------------->>>

		//???	public event EventHandler<CallLogEventArgs> CallLogEvent;
		
		public delegate void DLCALLLOGEVENT(uint pbxh, StringBuilder info);
		public delegate void COMMSEVENT(uint pbxh, int commsState, int parm1);

		// We need to store references to the allocated delegate instances in private members before passing them to the native code
		// to prevent the garbage collector from collecting the delegates before the callbacks return:
		private DLCALLLOGEVENT	_handleDelta2Event;
		private COMMSEVENT		_handleCommsEvent;

		/// <summary>
		/// The _rawDevLinkQueue that the DevLink messages will be put in. It has to be passed in the constructor!
		/// </summary>
		private ConcurrentQueue<DevLinkMessage> _rawDevLinkQueue;
			
		[DllImport("devlink.dll")]
		private static extern int DLRegisterType2CallDeltas(uint pbxh, DLCALLLOGEVENT cb);

		/// <summary>
		///  This routine may return either 0 (DEVLINK_SUCCESS) or 1 (DEVLINK_UNSPECIFIEDFAIL). Please note that a return value 
		/// of DEVLINK_SUCCESS only indicates that communications with the unit has been initiated; the subsequent connection may fail for 
		/// several reasons. Further information will be provided to the COMMSEVENT callback function specified in the cb parameter.
		/// </summary>
		/// <param name="pbxh"></param>
		/// <param name="pbx_address"></param>
		/// <param name="pbx_password"></param>
		/// <param name="reserved1"></param>
		/// <param name="reserved2"></param>
		/// <param name="cb"></param>
		/// <returns></returns>
		[DllImport("devlink.dll")]
		private static extern long DLOpen(uint pbxh, string pbx_address, string pbx_password, string reserved1, string reserved2, COMMSEVENT cb);
        
        [DllImport("devlink.dll")]
		private static extern long DLClose(uint IntPtr);

		#endregion DevLink Exposed Functions Stuff ----------------------<<<---------------------------------


		#region Private Members ------------------------------------------------------->>>

		private int _connectionTimeoutSec;
		private int _iPOID;
		public string IpAddress
		{ get; private set; }
		private string _password;

		private AutoResetEvent _waitConnectionOrTimeoutHandle;

		#endregion Private Members ----------------------<<<---------------------------------


		#region Connection Status Properties ------------------------------------------------------->>>
		private readonly Object _connectionLock = new Object();

		private int _connectionState;
		public ConnectionStates ConnectionState
		{
			// NOTE:
			// If we don't lock this somehow, there is a big chance code that uses this to be "optimized" in such way, that changes will not be reflected on time.
			get
			{
				// Interlocked ensures that when this value is read, it is not read from any processor cache.
				// Interlocked class provides better performance for updates that must be atomic than lock(){}.  http://msdn.microsoft.com/en-us/library/1c9txz50.aspx
				return (ConnectionStates)Interlocked.CompareExchange(ref _connectionState, 0, 0);
				// return (ConnectionStatus)_connectionState; 
			}
			private set
			{
				// Interlocked call ensures that the processor memory cache is refreshed when Set is called
				Interlocked.Exchange(ref _connectionState, (int)value); 
				//_connectionState = (int)value;
			}
		}  

		// I thought of making this volatile, but the experts advice against using it, as it is not what we think it is...
		// And as the important reading of this happens in a lock, we should be OK.
		private bool _ignoreConnectionCallback;
		
		private string _connectionCallbackMessage;
		public string ConnectionErrorMessage
		{
			get
			{
				// TODO: ensure fresh value here!
				return _connectionCallbackMessage;
			}
			private set
			{
				_connectionCallbackMessage = value;
			}
		}

		#endregion Connection Status Properties ----------------------<<<---------------------------------


		#region Public Methods ------------------------------------------------------->>>

		public DevLinkWrapper(ConcurrentQueue<DevLinkMessage> rawDevLinkQueue, string ipAddress, string password, int iPOID, int connectionTimeoutSec)
		{
			_rawDevLinkQueue = rawDevLinkQueue;
			IpAddress = ipAddress;
			_password = password;
			_iPOID = iPOID;
			_connectionTimeoutSec = connectionTimeoutSec;

			ConnectionState = ConnectionStates.Disconnected;
			_connectionCallbackMessage = "";


			StartMonitoring();
		}

		private bool StartMonitoring()
		{
			// Start the connecting process. Connecting will be set to FALSE once IsConnected is assigned a value.
			ConnectionState = ConnectionStates.Connecting;
			_ignoreConnectionCallback = false;
			DEBUGLogger.WriteLine("Waiting for connection response...");


			// Store a reference to the delegate instance in a private class field before passing it to the native code - 
			// to prevent the garbage collector from collecting it!
			_handleCommsEvent = DevLink_Connection_CallBack;
			
			// Create the wait-handle. This thread will wait till the callback returns, or the timeout expires.
			_waitConnectionOrTimeoutHandle = new AutoResetEvent(false);
			

			// Call the DevLink method to connect:
			long iRet = DLOpen((uint)_iPOID, IpAddress, _password, null, null, _handleCommsEvent);
			// TODO: does it make sense to check for the return value here???
			DEBUGLogger.WriteLine("DLOpen:   iPOID = " + _iPOID + ";   iRet = " + iRet);
			
			// There is a sliiight possibility that HandleCommsEvent is called and returns before we get to the "...WaitOne()" call below.
			// But we're not going to bother with this - if so, WaitOne will just wait till the timeout.
			// (If we decide to check the ConnectionState here, before calling WaitOne, nothing will prevent it from changing 
			// juuust after our check and before the WaitOne call (unless we lock()).)
			
			// Now wait till the Connection-Callback returns, or the timeout expires:
			_waitConnectionOrTimeoutHandle.WaitOne(_connectionTimeoutSec);    // this should return 'false' in case of timeout, but we don't need the result

			// OK, the event was signaled - let's see what this means for the Connection State...
			lock (_connectionLock)
			{
				if (_connectionCallbackMessage.Length > 0)
					DEBUGLogger.WriteLine(_connectionCallbackMessage);

				switch (ConnectionState)
				{
					case ConnectionStates.Connected:
						DEBUGLogger.WriteLine("Connection within timeout!...");
						break;

					case ConnectionStates.Failed:
						DEBUGLogger.WriteLine("Connection problems! ConnectionState: " + ConnectionState.ToString());
						break;

					case ConnectionStates.Connecting:
						ConnectionState = ConnectionStates.TimedOut;
						DEBUGLogger.WriteLine("Connection Timeout expired!...");
						break;

					case ConnectionStates.Disconnected:
						DEBUGLogger.WriteLine("Unexpected connection state in StartMonitoring() - Disconnected");
						break;
				}

				// We got a response OR the waiting timed out. Either way, we're done with the CallBack, so let it know:
				_ignoreConnectionCallback = true;
			}	// un-locking

		
			if (ConnectionState == ConnectionStates.Connected)
			{
				// Start the DevLink processing:
				// TODO: Maybe move this to the lock..???
				Process();
			}
			else
			{
				// There was a connection error, or we gave up waiting 
				// (but DevLink does not know it, and could still call the callback if the timeout interval turned out to be too small)
				// We don't want surprises, so we'd better close the connection, just in case:
				CloseConnection();
			}

			// We should not need this anymore, so close it:
			_waitConnectionOrTimeoutHandle.Close();

			return (ConnectionState == ConnectionStates.Connected);
		}


		public void StopMonitoring()
		{
			CloseConnection();
		}

		#endregion Public Methods ----------------------<<<---------------------------------


		#region Private - CallBacks and Helpers ------------------------------------------------------->>>

		/// <summary>
		/// DLOpen callback
		/// </summary>
		/// <param name="pbxh"></param>
		/// <param name="comms_state"></param>
		/// <param name="parm1"></param>
		private void DevLink_Connection_CallBack(uint pbxh, int comms_state, int parm1)		// oCommsEvent   CommEvent
		{
			DEBUGLogger.WriteThreadInfo("DevLink_Connection_CallBack");

			// TODO: TRY - CATCH
			DEBUGLogger.WriteLine("DevLink_Connection_CallBack:   iPOID = " + pbxh + ";   comms_state = " + comms_state + ";   parm1 = " + parm1);
			if (_ignoreConnectionCallback)
				return;		// Whatever!

			lock (_connectionLock)
			{
				if (_ignoreConnectionCallback)
				{
					// It is too late, we have already given up on this method returning! 
					DEBUGLogger.WriteLine("DevLink_Connection_CallBack: Too late, the connection callback result (comms_state=" + comms_state + "; parm1=" + parm1 + ") is being ignored.");
					return;
				}

				switch ((ConnectionResponse)comms_state)
				{
					// Communications established. This occurs either after the initial call to DLOpen(), 
					// or after the  system unit has come back on-line after being powered off or rebooted.
					// TODO: does the above mean that this method will be called again if the system is rebooted...?? Think of how to handle this case!
					case ConnectionResponse.DEVLINK_COMMS_OPERATIONAL_0:
					{
						ConnectionState = ConnectionStates.Connected;

						// OK, we're done here, notify the main thread:
						// TODO: what if this happens before the WaitOne call? OK I guess???
						_waitConnectionOrTimeoutHandle.Set();
						return;
					}
					// No response from system unit. This occurs either (1) after the initial call to DLOpen(), or 
					// (2) if the system unit is powered off/rebooted, or (3) if network problems prevent communications. 
					case ConnectionResponse.DEVLINK_COMMS_NORESPONSE_1:
					{
						// TODO: LOG a message, and wait more for possible recovery.
						ConnectionState = ConnectionStates.Failed;
						return;
					}
					// Reserved for future use OR incorrect system password specified (????)
					case ConnectionResponse.DEVLINK_COMMS_REJECTED_2:
					{
						_connectionCallbackMessage = 
							"Received response DEVLINK_COMMS_REJECTED - Reserved for future use! Do we have to implement this case too?";
						break;
					}

					// Packets were generated by IPO, but were not received by DevLink. 
					// This can occur either because the IPO is under heavy load (IPPO always prioritizes data routing and call handling above CTI events 
					// - parm1 contains the number of packets missed) 
					// or because the application using DevLink did not return from a callback quickly enough. 
					// Applications should ensure that they do not take more than 100 milliseconds to process events.
					case ConnectionResponse.DEVLINK_COMMS_MISSEDPACKETS_3:
					{
						_connectionCallbackMessage = "There are " + parm1 +
						                    " lost packets! IPO may be under heavy load. Ensure that it takes no more than 100 ms to process events!";
						break;
					}

					default:
						_connectionCallbackMessage = "Ooops, got unexpected result! comms_state = " + comms_state;
						break;
				}

				// Hmm, we were not able to connect successfully. We WON'T call _waitConnectionOrTimeoutHandle.Set() now, to give it some more time/chance to connect till the timeout.
				ConnectionState = ConnectionStates.Failed;
			}
		}

		private void DevLink_Received_CallBack(uint pbxh, StringBuilder rawDevLink)		// CallEvent
		{
			// TODO: TRY - CATCH

			DEBUGLogger.WriteThreadInfo("DevLink_Received_CallBack");

			DEBUGLogger.WriteLine("pbxh=" + pbxh + ";  info=" + rawDevLink);

			_rawDevLinkQueue.Enqueue(new DevLinkMessage() { IpAddress = IpAddress, RawDevLink = rawDevLink.ToString(), SiteId = _iPOID });
			
			// Make thread-safe: since this method is a CallBack, it will be called by Avaya's DevLink code
			// in its's thread, which will be different from any of the possible subscriber's threads.
			// http://broadcast.oreilly.com/2010/09/understanding-c-raising-events.html
			var devLinkRecieved_threadSafe = DevLinkRecieved;
			// Multi-cast delegates are immutable, so whatever happens to DevLinkRecieved from here on will NOT affect devLinkRecieved_threadSafe
			if (devLinkRecieved_threadSafe != null)
			{
				devLinkRecieved_threadSafe(this, new DevLinkEventArgs((int)pbxh, rawDevLink.ToString()));

				// NOTE:
				// When an event fires, all of the registered handlers execute on the same thread (the one that raised the event - 
				// in our case - the thread belongs to Avaya's DevLink!). 
				// There's no built-in facility for events to fire on multiple threads.
				// http://stackoverflow.com/questions/3480036/multiple-threads-subscribing-same-event
			}

		}

		private void Process()
		{
			DEBUGLogger.WriteThreadInfo("Process");

			_handleDelta2Event = DevLink_Received_CallBack;

			/*This routine may return:- 
				0 = DEVLINK_SUCCESS 
				1 = DEVLINK_UNSPECIFIEDFAIL - Returned in the event of an error. 
				2 = DEVLINK_LICENCENOTFOUND - If no CTI license is activated on the IP Office system. */
			long iRet = DLRegisterType2CallDeltas((uint)_iPOID, _handleDelta2Event					// DLCALLLOGEVENT
				/*	(pbxh, rawDevLink) => {	lock (thisLock)
											if (default(EventHandler<DevLinkEventArgs>) != DevLinkRecieved)
											{
												DevLinkRecieved(this, new DevLinkEventArgs(pbxh, rawDevLink));
											} }  */
			);

			DEBUGLogger.WriteLine("DLRegisterType2CallDeltas return value = " + iRet);
		}

		private void CloseConnection()
		{
			// We may want to call this just in case (when connection timeout expires) so we don't get any more calls back from DevLink!
			if (ConnectionState != ConnectionStates.Disconnected)	
			{
				// Disconnect from an IP Office system:
				// This routine may return 0 (DEVLINK_SUCCESS) or 1 (DEVLINK_UNSPECIFIEDFAIL) in the event of an error. 
				// [AK:] OR a big number >> 1, in the event it is called twice for the same IPO.
				// TODO: should we check for ret > 0 ???
				long iRet = DLClose((uint)_iPOID);

				ConnectionState = ConnectionStates.Disconnected;

				// TODO: set the callback delegates to NULL???
				// OR Just do NOTHING in the methods, if we are disconnected???

				_disposed = true;
				DEBUGLogger.WriteLine("DLClose:   iPOID = " + _iPOID + ";   iRet = " + iRet);
			}
		}

		#endregion   Private - CallBacks and Helpers ----------------------<<<---------------------------------


		#region IDisposable Interface ------------------------------------------------------->>>

		/// <summary>
		/// It frees the resources used by the class.
		/// From http://msdn.microsoft.com/en-us/library/fs2xkftw%28v=vs.110%29.aspx:
		/// It has primarily two code blacks:
		/// 1) The block that frees unmanaged resources. This block executes regardless of the value of the disposing parameter.
		/// 2) A conditional block that frees managed resources. This block executes if the value of disposing is true.
		/// If the method call comes from a finalizer (that is, if disposing is false), only the code that frees unmanaged resources executes.
		/// Because the order in which the garbage collector destroys managed objects during finalization is not defined, calling this Dispose overload
		/// with a value of false prevents the finalizer from trying to release managed resources that may have already been reclaimed. 
		/// </summary>
		public void Dispose()
		{
			if (!_disposed)
			{
				CloseConnection();
			}
		}

		~DevLinkWrapper()
        {
			//we're being finalized (i.e. destroyed), call Dispose in case the user forgot to:
            Dispose();
		}

		#endregion IDisposable Interface --------------------------<<<-----------------------------

	}


	/// <summary>
	/// Helper class to log "Debug" info
	/// </summary>
	[DebuggerStepThrough]
	public static class DEBUGLogger
	{
		private const string PREFIX = ">>>>>>>>>>>>>> ";
		private const string ThreadInfoFormat = "### {0} ### Thread.CurrentThread INFO: Name = {1};  ManagedThreadId = {2};  IsBackground = {3};  IsThreadPoolThread = {4};  ThreadState = {5}";
		private static object _locker = new object();

		public static void WriteLine(string message)
		{
			Debug.WriteLine(PREFIX + message);
		}

		/// <summary>
		/// Logs information about the thread. Public so it can be accessed from other classes.
		/// TODO: move this to a Utils class!
		/// </summary>
		/// <param name="methodName"></param>
		public static void WriteThreadInfo(string methodName)
		{
			// TODO: comment these out if we don't want debug thread logging:
			lock (_locker)
			{
				Thread ct = Thread.CurrentThread;
				Debug.WriteLine(ThreadInfoFormat, methodName, ct.Name, ct.ManagedThreadId, ct.IsBackground, ct.IsThreadPoolThread,
					ct.ThreadState);
			}
		}
	}

}



/* 
		
 ---- STOPWATCH: ------------------------------------------
	var DEBUG_watch = Stopwatch.StartNew();
	//.....
	DEBUG_watch.Stop();
	Debug.WriteLine(">>>>>>>>>>>> ....... completed in: " + DEBUG_watch.ElapsedMilliseconds + " milliseconds!");
 
 */
