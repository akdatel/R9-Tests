using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevLink_CSharp
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// TODO: PAss a DLW (TODO: who shoudl initialize it??? Maybe just subscribe to the events..??
			Application.Run(new DevLinkMonitor());
		}
	}
}
