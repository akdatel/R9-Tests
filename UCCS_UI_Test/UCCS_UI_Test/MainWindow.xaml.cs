using System;
using System.Collections.Generic;
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
using UCCS_UI_Test.Data;
using UCCS_UI_Test.Models;

namespace UCCS_UI_Test
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		public override void OnApplyTemplate()
		{
			// Create an AgentCompactPanel with mock data:
			AgentCompactPanel acp = new AgentCompactPanel(AgentsData.GetAgentsData());  // TODO: GetAgentCompactBindingSource
			ContentGrid.Children.Add(acp);

			this.DataContext = AgentsData.GetAgentCompactBindingSource();
		}
	}
}
