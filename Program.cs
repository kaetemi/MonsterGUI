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
			System.Net.ServicePointManager.DefaultConnectionLimit = 128;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainWindow());
		}
	}
}
