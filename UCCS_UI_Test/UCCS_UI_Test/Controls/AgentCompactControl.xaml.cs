using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UCCS_UI_Test.Models;

namespace UCCS_UI_Test
{
	/// <summary>
	/// Interaction logic for AgentCompactControl.xaml
	/// </summary>
	public partial class AgentCompactControl : UserControl
	{
		private const string STATUS_FORMAT = "{0} for {1}";
		private readonly Dictionary<AgentStatus, string> STATUS_NAMES = new Dictionary<AgentStatus, string>();
		private readonly Dictionary<AgentStatus, Brush> STATUS_BACKGROUNDS = new Dictionary<AgentStatus, Brush>();
		private AgentCompactModel _agentModel;
		// TODO: private AgentCompactBindingSource _agentModel;

		public AgentCompactControl()
		{
			InitializeComponent();

			InitializeStatusDictionaries();
		}

		public AgentCompactControl(AgentCompactModel agentModel)
		// TODO: public AgentCompactControl(AgentCompactBindingSource agentModel)
		{
			InitializeComponent();

			InitializeStatusDictionaries();

			_agentModel = agentModel;
			DisplayAgentData();
		}


		#region Public Properties
		public string AgentName
		{
			get { return AgentNameLabel.Content.ToString(); }
			set { AgentNameLabel.Content = value; }
		}

		public AgentStatus Status { get; set; }
		public string TimeInState { get; set; }

		#endregion Public Properties



		#region Private Helpers

		private void InitializeStatusDictionaries()
		{
			foreach (AgentStatus status in Enum.GetValues(typeof(AgentStatus)))
			{
				string statusName = Regex.Replace(status.ToString(), "[A-Z]", " $0").Trim();

				STATUS_NAMES.Add(status, statusName);

				switch (status)
				{
					case AgentStatus.Busy:
						STATUS_BACKGROUNDS.Add(status, Brushes.LightGray);
						break;
					case AgentStatus.Handle:
						STATUS_BACKGROUNDS.Add(status, Brushes.Lavender);
						break;
					case AgentStatus.Idle:
						STATUS_BACKGROUNDS.Add(status, Brushes.White);
						break;
					case AgentStatus.LoggedOut:
						STATUS_BACKGROUNDS.Add(status, Brushes.DarkSalmon);
						break;
					case AgentStatus.UnavailableIn:
					case AgentStatus.UnavailableOut:
						STATUS_BACKGROUNDS.Add(status, Brushes.DarkSeaGreen);
						break;
				}
			}
		}

		private void DisplayAgentData()
		{
			AgentName = _agentModel.Name;
			PhoneNumberOnCallLabel.Content = _agentModel.PhoneNumber;
			TimeIdle.Content = _agentModel.IdleTime;
			CallsGroup.Content = _agentModel.CallsGroup;

			DisplayStatus(_agentModel.Status, _agentModel.StatusTime);
		}

		private void DisplayStatus(AgentStatus status, TimeSpan timeInState)
		{
			AgentStatusLabel.Content = string.Format(STATUS_FORMAT, STATUS_NAMES[status], timeInState);
			ContentGrid.Background = STATUS_BACKGROUNDS[status];
		}

		#endregion Private Helpers

	}
}