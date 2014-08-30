using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using DatelHelpers;

namespace DevLink_CSharp
{

	/// <summary>
	/// TODO: Are we going to start the DevLinkWrapper FROM this app, or vice-versa?
	/// If 1) - then provide controls to get the IP Address and Password... also the ID and Timeout!
	/// 
	/// 
	/// </summary>
	public partial class DevLinkMonitor : Form
	{
		#region Private Members
		private const string BUTTON_TEXT_START = "Start";
		private const string BUTTON_TEXT_STOP = "Stop";
		
		private DevLinkWrapper _devLinkWrapper;
		private bool _subscribedToDevLinksEvent;
		private bool _isConnected;

		// This delegate enables asynchronous calls for manipulating 
		// the Text property on a TextBox control. 
		private delegate void SetTextCallback(string text);


		private ConcurrentQueue<DevLinkWrapper.DevLinkMessage> _rawDevLinkQueue;
		#endregion Private Members


		public DevLinkMonitor()
		{
			InitializeComponent();
		}
		

		#region Event Handlers

		private void Form1_Load(object sender, EventArgs e)
		{
			_isConnected = false;

			GetParametersTODO();

			StartStopButtonPressed();
		}

		private void buttonStartStop_Click(object sender, EventArgs e)
		{
			// If the Stop button was pressed, or if Connection was unsuccessful, then Stop:
			if (buttonStartStop.Text == BUTTON_TEXT_STOP || ConnectAndStartMonitoring() == false)
			{
				StopMonitoring();
			}
		}

		private bool ConnectAndStartMonitoring()
		{

			DEBUGLogger.WriteThreadInfo("ConnectAndStartMonitoring");

		// TODO: PARSE/CHECK these values:
			string ipAddress = tbIpAddress.Text.Trim();
			string password = tbPassword.Text.Trim();
			int connectionTimeoutMs = int.Parse(tbConnTimeout.Text.Trim());
			_rawDevLinkQueue = new ConcurrentQueue<DevLinkWrapper.DevLinkMessage>();


			int iPOID = 21;


			try
			{
				AddLog("Connecting to " + ipAddress + "...");
				_devLinkWrapper = new DevLinkWrapper(_rawDevLinkQueue, ipAddress, password, iPOID, connectionTimeoutMs);
				AddLog("Connection successful!");



			/*	
				// Here we're blocking till the method returns. Should not take much longer than the connectionTimeoutMs.
				// TODO: maybe Start monitoring automatically? Or pass the queue there?
			///	_isConnected = _devLinkWrapper.StartMonitoring();
			 * 
			 * if (_isConnected)
				{
					SubscribeToDevLinksEvent(true);

					StartStopButtonPressed();

					Thread dlThread = new Thread(ReceiveDevLinks);
					dlThread.Start();

					return true;
				}
				return false;*/
			}
			catch
			{
				return false;
			}
		}

		private void ReceiveDevLinks()
		{
			while (_isConnected)
			{
				DevLinkWrapper.DevLinkMessage mess;
				if (_rawDevLinkQueue.TryDequeue(out mess))
				{
					AddLog(mess.SiteId + " ||| " + mess.RawDevLink + " ### " + mess.IpAddress + " ### " + DateTime.Now);
				}
				Application.DoEvents();
			}
		}
		
		private void buttonClear_Click(object sender, EventArgs e)
		{
			textBoxLog.Text = string.Empty;
		}

		/// <summary>
		/// Handling the DevLinkReceived event. NOTE: This will be executed in Avaya DevLink's Thread! 
		/// It MUST be very light-weight and NOT throw exceptions! 
		/// TODO:LATER: if this doesn't work well, try using ConcurrentQueue
		/// </summary>
		/// <param name="devLinkWrapper"></param>
		/// <param name="e"></param>
		private void devLinkWrapper_DevLinkRecieved(object sender, DevLinkEventArgs e)
		{
			// It is sliiightly possible for us to have un-subscribed juust before the event fires.
			// In this case (because of the way the event is fired to make it thread-safe) this would still get called, so make sure we don't execute it:
			if (_subscribedToDevLinksEvent == false)
			{
				AddLog("devLinkWrapper_DevLinkRecieved - we've already unsubscribed! Ignoring...");
				return;
			}

			DEBUGLogger.WriteThreadInfo("devLinkWrapper_DevLinkRecieved EH");

			DevLinkWrapper dlWrapper = (DevLinkWrapper) sender;
			// TODO: THINK ABOUT:
			// - backing up the data if connection is lost -- in the Collection??

			AddLog(e.SiteId + " ||| " + e.RawDevLink + " ### " + dlWrapper.IpAddress + " ### " + DateTime.Now);
		}

		#endregion Event Handlers


		#region Private Helpers

		private void GetParametersTODO()
		{
			// TODO: Maybe read these from EventArgs???

			// TODO: remove these defaults - make them read from config OR be passed from another process
			tbIpAddress.Text = "10.10.10.51";
			tbPassword.Text = "password";

			// TODO: limit this to numbers and to a specific range
			tbConnTimeout.Text = 5000.ToString();
		}

		/// <summary>
		/// Switch the UI when pressing the Start/Stop button
		/// </summary>
		/// <param name="isStart"></param>
		private void StartStopButtonPressed()
		{
			groupBoxControls.Enabled = !_isConnected;
			buttonStartStop.Text = _isConnected ? BUTTON_TEXT_STOP : BUTTON_TEXT_START;
		}

		/// <summary>
		/// The devLinkWrapper_DevLinkRecieved event handler is called from a different thread, so it CANNOT access the controls on this form directly. 
		/// What this method does: if the calling thread is different from the thread that 
		/// created the TextBox control, this method calls itself asynchronously using the Invoke method.
		/// Otherwise, the Text property is accessed directly.  
		/// Source: http://msdn.microsoft.com/query/dev11.query?appId=Dev11IDEF1&k=k(EHInvalidOperation.WinForms.IllegalCrossThreadCall);
		/// </summary>
		/// <param name="message"></param>
		[DebuggerStepThrough]
		private void AddLog(string message)
		{
			// TODO: make sure the text does not get TOO LONG!

			// InvokeRequired compares the thread ID of the calling thread 
			// to the thread ID of the creating thread. 
			// If these threads are different, it returns true. 
			if (textBoxLog.InvokeRequired)
			{
				SetTextCallback d = new SetTextCallback(AddLog);
				Invoke(d, new object[] { message });
			}
			else
			{
				textBoxLog.Text += message + Environment.NewLine;
			}
		}

		private void StopMonitoring()
		{
			AddLog("Disconnecting...");
			if (_devLinkWrapper != null && _devLinkWrapper.ConnectionState != DevLinkWrapper.ConnectionStates.Disconnected)
			{
				// subscribers should unsubscribe themselves when they are done or they will never get GC’d.
				SubscribeToDevLinksEvent(false);
				_devLinkWrapper.StopMonitoring();
			}
			AddLog("Disconnected.");

			_isConnected = false;
			// Update the UI as if the Stop button was pressed:
			StartStopButtonPressed();
		}


		/// <summary>
		/// A helper to both un/subscribe to the DevLinkRecieved event AND set the flag that keeps track of this.
		/// </summary>
		/// <param name="unSubscribe"></param>
		private void SubscribeToDevLinksEvent(bool doSubscribe)
		{
		/*	if (doSubscribe == false && _subscribedToDevLinksEvent)
			{
				//devLinkWrapper.DevLinkRecieved -= new DevLinkWrapper.DevLinkRecievedEventHandler(devLinkWrapper_DevLinkRecieved);
				_devLinkWrapper.DevLinkRecieved -= devLinkWrapper_DevLinkRecieved;
				_subscribedToDevLinksEvent = false;
			}
			else
			{
				_devLinkWrapper.DevLinkRecieved += devLinkWrapper_DevLinkRecieved;
				_subscribedToDevLinksEvent = true;
			}*/
		}
		#endregion Private Helpers
	}
	


}
