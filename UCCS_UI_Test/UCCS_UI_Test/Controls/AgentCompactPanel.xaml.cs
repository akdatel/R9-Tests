using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
	/// Interaction logic for AgentCompactPanel.xaml
	/// </summary>
	public partial class AgentCompactPanel : UserControl
	{
		private List<AgentCompactModel> _agentData;
		// TODO: private ObservableCollection<AgentCompactBindingSource> _agentData;

		public AgentCompactPanel(List<AgentCompactModel> agentData)
		// TODO: public AgentCompactPanel(ObservableCollection<AgentCompactBindingSource> agentData)
		{
			_agentData = agentData;

			InitializeComponent();
		}

		public override void OnApplyTemplate()
		{
			// This CANNOT be done in the constructor!
			InitializeContent();

			base.OnApplyTemplate();
		}


		#region Private Helpers

		private void InitializeContent()
		{
			foreach (var acm in _agentData)
			{
				AgentCompactControl acc = new AgentCompactControl(acm);
				ContentPanel.Children.Add(acc);
			}
		}
		
		#endregion Private Helpers
	}
}
