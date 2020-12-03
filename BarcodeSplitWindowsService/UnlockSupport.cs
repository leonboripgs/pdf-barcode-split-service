using System;
using System.IO;

using Leadtools;

namespace BarcodeSplitWindowsService
{
	internal static class Support
	{
#if LEADTOOLS_V19_OR_LATER
		public static bool SetLicense(bool silent)
		{
			try
			{
				// TODO: Change this to use your license file and developer key */
				string licenseFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\full_license.lic";
				string developerKey = "gcxLXptTi5bPbVDDR9E+k4CRC8BLLm0IuN383qJp6jPqoMTYamOe1yuYzHqrCmFEN5zDcumaaCTXpO9GpeGal0wjSKF8nxnu";
				var lic = File.ReadAllBytes(licenseFilePath);
				RasterSupport.SetLicense(lic, developerKey);
				//RasterSupport.SetLicense(licenseFilePath, developerKey);
			}
			catch (Exception ex)
			{
				//System.Diagnostics.Debug.Write(ex.Message);
				ServiceLog.WriteLog("Set License Error, " + ex.Message);
			}

			if (RasterSupport.KernelExpired)
			{
				if (silent == false)
				{
					string msg = "Your license file is missing, invalid or expired. LEADTOOLS will not function. Please contact LEAD Sales for information on obtaining a valid license.";
					string logmsg = string.Format("*** NOTE: {0} ***{1}", msg, Environment.NewLine);
					ServiceLog.WriteLog("*******************************************************************************" + Environment.NewLine);
					ServiceLog.WriteLog(logmsg);
					ServiceLog.WriteLog("*******************************************************************************" + Environment.NewLine);
				}

				return false;
			}
			return true;
		}

		public static bool SetLicense()
		{
			return SetLicense(false);
		}

#elif LTV18_CONFIG
      public static void SetLicense()
      {
         try
         {
            RasterSupport.SetLicense("", "Nag");
            /* Uncomment this and add your license file and developer key
            string licenseFilePath = "Replace this with the path to the LEADTOOLS license file";
            string developerKey = "Replace this with your developer key";
            RasterSupport.SetLicense(licenseFilePath, developerKey);
            */
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.Write(ex.Message);
         }
      }
#endif  //LEADTOOLS_V19_OR_LATER

	}
}
