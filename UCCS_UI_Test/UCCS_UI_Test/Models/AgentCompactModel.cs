using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UCCS_UI_Test.Annotations;

namespace UCCS_UI_Test.Models
{
	public enum AgentStatus
	{
		Idle,
		LoggedOut,
		Busy,
		Handle,
		UnavailableIn,
		UnavailableOut
	}

	public class AgentCompactModel
	{
		public string Name { get; set; }

		public AgentStatus Status { get; set; }

		public TimeSpan StatusTime { get; set; }

		public TimeSpan IdleTime { get; set; }

		public string PhoneNumber { get; set; }

		public int CallsGroup { get; set; }
	}



	public class AgentCompactBindingSource : INotifyPropertyChanged
	{
		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (value != _name)
				{
					_name = value;
					OnPropertyChanged();
				}
			}
		}

		private AgentStatus _status;
		public AgentStatus Status
		{
			get { return _status; }
			set
			{
				if (value != _status)
				{
					_status = value;
					OnPropertyChanged();
				}
			}
		}

		private TimeSpan _statusTime;
		public TimeSpan StatusTime
		{
			get { return _statusTime; }
			set
			{
				if (value != _statusTime)
				{
					_statusTime = value;
					OnPropertyChanged();
				}
			}
		}

		private TimeSpan _idleTime;
		public TimeSpan IdleTime
		{
			get { return _idleTime; }
			set
			{
				if (value != _idleTime)
				{
					_idleTime = value;
					OnPropertyChanged();
				}
			}
		}

		private string _phoneNumber;
		public string PhoneNumber
		{
			get { return _phoneNumber; }
			set
			{
				if (value != _phoneNumber)
				{
					_phoneNumber = value;
					OnPropertyChanged();
				}
			}
		}

		private int _callsGroup;
		public int CallsGroup 
		{
			get { return _callsGroup;	 }
			set
			{
				if (value != _callsGroup)
				{
					_callsGroup = value;
					OnPropertyChanged();
				}
			}
		}

		// ...

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) 
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
