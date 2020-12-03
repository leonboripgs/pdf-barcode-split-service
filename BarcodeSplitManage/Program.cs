using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BarcodeSplitManage
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			if (!Support.SetLicense())
				return;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
