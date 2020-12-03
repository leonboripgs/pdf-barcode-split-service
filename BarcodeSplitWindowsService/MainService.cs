using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;
using Leadtools.Twain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarcodeSplitWindowsService
{
	public partial class MainService : ServiceBase
	{
		private static BarcodeOptions _barcodeOptions = BarcodeOptions.Default;
		private DirectorySettings _directorySettings;

		IntercommsServer _intercommsServer;
		IntercommsClient _intercommsClient;

		private bool _isManageconnected;

		// Pdf splitter
		private PDFSplitter[] _pdfSplitters;
		Thread[] splitThreads;

		public MainService()
		{
			InitializeComponent();

			ServiceLog.InitLog();

			_isManageconnected = false;

			_intercommsServer = new IntercommsServer();
			_intercommsServer.IntercommsMessage += new IntercommsMessageDelegate(IntercommsMessageHandler);
			_intercommsClient = new IntercommsClient();

			_intercommsServer.Listen("PDFSCANSERVICEPIPE");
			ServiceLog.WriteLog("Intercomms Server Listening Started");

			_directorySettings = new DirectorySettings();
		}

		~MainService()
		{
			_intercommsServer.IntercommsMessage -= new IntercommsMessageDelegate(IntercommsMessageHandler);
			_intercommsServer = null;
			_intercommsClient = null;
		}

		protected override void OnStart(string[] args)
		{
			_barcodeOptions = BarcodeOptions.Load();

			//BarcodeSymbology[] supportedSymbologies = BarcodeEngine.GetSupportedSymbologies();
			Console.WriteLine(string.Format("{0} Supported symbologies:", _barcodeOptions.ReadOptionsSymbologies.Length));
			foreach (BarcodeSymbology symbology in _barcodeOptions.ReadOptionsSymbologies)
			{
				Console.WriteLine(string.Format("{0}: {1}", symbology, BarcodeEngine.GetSymbologyFriendlyName(symbology)));
			}

			_directorySettings.Load();
			DoPDFSplitStart();

			ServiceLog.WriteLog("Service started");
		}

		protected override void OnStop()
		{
			DoPDFSplitStop();

			ServiceLog.WriteLog("Service stopped");
		}

		private void DoPDFSplitStop()
		{
			if (_pdfSplitters != null && _pdfSplitters.Length > 0)
			{
				for (int i = 0; i < _pdfSplitters.Length; i++)
				{
					if (_pdfSplitters[i] != null)
					{
						_pdfSplitters[i].Stop();
					}
				}
			}
		}

		private void DoPDFSplitStart()
		{
			Dictionary<int, DirectoryData> availableData = new Dictionary<int, DirectoryData>();
			DirectoryData[] dirData = _directorySettings.GetDirectoryData();

			for (int i = 0; i < dirData.Length; i++)
			{
				dirData[i].Status = dirData[i].Enabled;

				if (dirData[i].Status)
				{
					availableData.Add(i, dirData[i]);
				}
			}

			if (availableData.Count > 0)
			{
				_pdfSplitters = new PDFSplitter[availableData.Count];
				splitThreads = new Thread[availableData.Count];

				int i = 0;

				foreach (var aData in availableData)
				{
					_pdfSplitters[i] = new PDFSplitter(this, _barcodeOptions, aData.Value);

					splitThreads[i] = new Thread(_pdfSplitters[i].Run);
					splitThreads[i].Start();
					i++;
				}
			}
		}

		private int GetDirectoryIndexById(int ID)
		{
			DirectoryData[] dirData = _directorySettings.GetDirectoryData();

			for (int i = 0; i < dirData.Length; i++)
			{
				if (dirData[i].ID == ID)
					return i;
			}

			return -1;
		}

		public void OnPDFSplitterMsgEvent(object sender, PDFSplitterEventArgs args)
		{
			int listindex = GetDirectoryIndexById(args.DirID);

			DirectoryData[] dirData = _directorySettings.GetDirectoryData();

			if (args.Kind == 1 && listindex > -1)
			{
				switch (args.Msg)
				{
					case "Stopped":
						dirData[listindex].Status = false;
						break;
				}
			}

			string strmsg = args.DirID.ToString() + "|" + args.Kind.ToString() + "|" + args.Msg + "|" + ((int)args.Evtype).ToString();

			try
			{
				if (_isManageconnected)
					_intercommsClient.Send(strmsg, "PDFSCANMANAGEPIPE");
				else
					ServiceLog.WriteLog("Intercomms Client Send Data, " + strmsg);
			}
			catch (Exception ex)
			{
				ServiceLog.WriteLog("Intercomms Client Send Error, " + ex.Message + ", " + strmsg);
			}

			for (int i = 0; i < dirData.Length; i++)
			{
				if (dirData[i].Status)
					return;
			}

			for (int i = 0; i < _pdfSplitters.Length; i++)
			{
				if (splitThreads[i] != null)
					splitThreads[i].Abort();
				_pdfSplitters[i] = null;
			}

			_pdfSplitters = null;
			splitThreads = null;
		}

		private void IntercommsMessageHandler(string message)
		{
			try
			{
				switch (message)
				{
					case "MNG_CON":
						_isManageconnected = true;
						_intercommsClient.Send("SERVICE_CON", "PDFSCANMANAGEPIPE");
						break;
					case "MNG_DIS":
						_isManageconnected = false;
						break;
					/*case "PROC_START":
						DoPDFSplitStart();
						break;
					case "PROC_STOP":
						DoPDFSplitStop();
						break;*/
				}
				ServiceLog.WriteLog("Intercomms Server Received Message, " + message + "[Length: " + message.Length + "], Connected : " + _isManageconnected.ToString());
			}
			catch (Exception ex)
			{
				ServiceLog.WriteLog("Intercomms Server Received Error, " + ex.Message);
			}
		}
		
	}
}
