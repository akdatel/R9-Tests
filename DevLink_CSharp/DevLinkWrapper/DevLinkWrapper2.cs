using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

namespace DevLinkWrapper
{
	class DevLinkWrapper2 : IDisposable
	{
		// internal fields
		private object _devLink;
		private Queue<string> _messageQueue; 
		private Thread _worker;
		private bool _connected;
		private volatile bool _shutdown;

		#region [ Construction/Disposal ]

		public DevLinkWrapper2( /* ipAddress */)
		{
			_messageQueue = new Queue<string>(100);
		}

		public void Dispose()
		{
			this.Close();
		}

		#endregion

		#region [ Properties ]

		public bool Connected
		{
			get { return _connected; }
		}

		#endregion

		#region [ Operations ]

		public void Connect()
		{
			if (_worker == null)
			{
				_connected = false;
				_shutdown = false;
				_worker = new Thread(new ThreadStart(Worker));
				_worker.Start();
			}
		}

		public string GetMessage()
		{
			lock (_messageQueue)
			{
				if (_messageQueue.Count > 0)
				{
					return _messageQueue.Dequeue();
				}
				else
				{
					return null;
				}
			}
		}

		public void Close()
		{
			if (_worker != null)
			{
				_shutdown = true;
				_worker.Join();
				_worker = null;
			}
		}

		#endregion

		#region [ Internals ]

		private void Worker()
		{
			while (!_connected)
			{
				_devLink = new object();
				// _devLink.Connect(DevLinkConnectionCallBack)
				// try to connect
				// wait for the call back
				// if timeout {
				// dispose _devLink
				// Sleep(3000);// }
				if (_shutdown) return;
			}

			// if(retryCount > X) 
			//{
			//	_worker = null;
			//	return;
			//}

			// _devLink.Start(DevLinkMessageCallback)
			// start checking for messages
			while (!_shutdown)
			{
				Thread.Sleep(500);
			}
		}

		private void DevLinkConnectionCallBack()
		{
			_connected = true;
		}

		private void DevLinkMessageCallback(string msg)
		{
			// read message
			lock (_messageQueue)
			{
				// push message in queue
				_messageQueue.Enqueue(msg);
			}

		}

		#endregion
	}
}
