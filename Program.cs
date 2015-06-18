using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MonsterGUI
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			System.Net.ServicePointManager.EnableDnsRoundRobin = true;
			System.Net.ServicePointManager.UseNagleAlgorithm = false;
			System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
			System.Net.ServicePointManager.MaxServicePoints = int.MaxValue;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainWindow());
		}
	}
}
