using System;
using System.IO;

namespace BarcodeSplitWindowsService
{
	public static class ServiceLog
	{
		public static void InitLog()
		{
			StreamWriter sw = null;

			try
			{
				sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\ServiceLog.txt");
				sw.WriteLine("[" + DateTime.Now.ToString() + "] : Logging Started");
				sw.Flush();
				sw.Close();
			}
			catch
			{

			}
		}

		public static void WriteLog(string Msg)
		{
			StreamWriter sw = null;

			try
			{
				sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\ServiceLog.txt", true);
				sw.WriteLine("[" + DateTime.Now.ToString() + "] : " + Msg);
				sw.Flush();
				sw.Close();
			}
			catch
			{

			}
		}
	}
}
