using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;
using Leadtools.Twain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace BarcodeSplitManage
{
	public partial class MainForm : Form
	{
		// The RasterCodecs instance we will use to load/save images
		private BarcodeEngine _barcodeEngine;
		private RasterCodecs _rasterCodecs;
		private static BarcodeOptions _barcodeOptions = BarcodeOptions.Default;

		IntercommsServer _intercommsServer;
		IntercommsClient _intercommsClient;

		// A RasterImage that holds barcode symbology samples
		private RasterImage _sampleSymbologiesRasterImage;

		// Directory Settings
		private DirectorySettings _directorySettings;

		// Service
		static string SERVICE_NAME = "Itegrity Scan and Split Service";
		ServiceControllerStatus serviceStatus = 0;
		bool isExistService;


		public MainForm()
		{
			InitializeComponent();
		}

		#region Override

		protected override void OnLoad(EventArgs e)
		{
			try
			{
				if (!Init())
				{
					Close();
					return;
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
				Close();
			}

			_intercommsServer = new IntercommsServer();
			_intercommsServer.IntercommsMessage += new IntercommsMessageDelegate(IntercommsMessageHandler);
			_intercommsClient = new IntercommsClient();

			_intercommsServer.Listen("PDFSCANMANAGEPIPE");

			RefreshGridView();

			dataGridView_Click(null, e);

			isExistService = false;
			ServiceController sc;

			try
			{
				sc = new ServiceController(SERVICE_NAME);
				isExistService = true;
			}
			catch (Exception)
			{
			}

			//tmrServiceCheck.Start();

			base.OnLoad(e);
		}

		private string ServiceStatus()
		{
			ServiceController sc;

			try
			{
				sc = new ServiceController(SERVICE_NAME);
			}
			catch (ArgumentException)
			{
				return "Invalid service name."; // Note that just because a name is valid does not mean the service exists.
			}

			using (sc)
			{
				try
				{
					sc.Refresh(); // calling sc.Refresh() is unnecessary on the first use of `Status` but if you keep the ServiceController in-memory then be sure to call this if you're using it periodically.
					serviceStatus = sc.Status;
					isExistService = true;
				}
				catch (Exception ex)
				{
					// A Win32Exception will be raised if the service-name does not exist or the running process has insufficient permissions to query service status.
					// See Win32 QueryServiceStatus()'s documentation.
					WriteLine("Service Status Error: " + ex.Message, TraceEventType.Error);

					WriteLine("-----------------------------------------------------", TraceEventType.Information);
					WriteLine("Service Install Start...", TraceEventType.Information);
					Process.Start("InstallUtil.exe", "BarcodeSplitWindowsService.exe");
					WriteLine("Service Install End...", TraceEventType.Information);
					WriteLine("-----------------------------------------------------", TraceEventType.Information);
				}

				switch (serviceStatus)
				{
					case ServiceControllerStatus.Running:
						btnRunAll_Click(this, null);
						return "Running";
					case ServiceControllerStatus.Stopped:
						sc.Start();
						return "Stopped";
					case ServiceControllerStatus.Paused:
						sc.Start();
						return "Paused";
					case ServiceControllerStatus.StopPending:
						return "Stopping";
					case ServiceControllerStatus.StartPending:
						return "Starting";
					default:
						return "Unknown";
				}
			}
		}
		
		private void tmrServiceCheck_Tick(object sender, EventArgs e)
		{
			ServiceStatus();

			if (serviceStatus == ServiceControllerStatus.Running)
			{
				_intercommsClient.Send("MNG_CON", "PDFSCANSERVICEPIPE");
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			_intercommsClient.Send("MNG_DIS", "PDFSCANSERVICEPIPE");

			Thread.Sleep(1000);

			_intercommsServer.IntercommsMessage -= new IntercommsMessageDelegate(IntercommsMessageHandler);
			_intercommsServer = null;
			_intercommsClient = null;

			_barcodeOptions.Save();
			_directorySettings.Save();

			CleanUp();

			base.OnFormClosed(e);
		}

		private void IntercommsMessageHandler(string message)
		{
			Debug.WriteLine(message);

			try
			{
				switch(message)
				{
					case "SERVICE_CON":
						tmrServiceCheck.Stop();
						break;
					default:
						string[] items = message.Split('|');

						if (items.Length == 4)
						{
							int dirID = int.Parse(items[0]);
							int kind = int.Parse(items[1]);
							string msg = items[2];
							TraceEventType evType = (TraceEventType)int.Parse(items[3]);

							PDFSplitterEventArgs args = new PDFSplitterEventArgs(dirID, kind, msg, evType);

							this.Invoke(new EventHandler<PDFSplitterEventArgs>(OnPDFSplitterMsgEvent), this, args);
						}

						break;
				}
			}
			catch (Exception ex)
			{
				this.Invoke(new EventHandler<PDFSplitterEventArgs>(OnPDFSplitterMsgEvent), this, new PDFSplitterEventArgs(-1, 2, ex.Message, TraceEventType.Error));
			}

		}

		private void CleanUp()
		{
			// Delete all resources
			if (_sampleSymbologiesRasterImage != null)
			{
				_sampleSymbologiesRasterImage.Dispose();
			}

			if (_rasterCodecs != null)
			{
				_rasterCodecs.Dispose();
			}
		}

		private void ShowError(Exception ex)
		{
			RasterException re = ex as RasterException;
			if (re != null)
			{
				Messager.ShowError(this, string.Format(BarcodeGlobalization.GetResxString(GetType(), "Resx_LEADError"), re.Code, ex.Message));
			}
			else
			{
				TwainException tw = ex as TwainException;
				if (tw != null)
				{
					Messager.ShowError(this, string.Format(BarcodeGlobalization.GetResxString(GetType(), "Resx_TwainError"), tw.Code, ex.Message));
				}
				else
				{
					Messager.ShowError(this, ex);
				}
			}
		}

		private bool Init()
		{
			// Check support required to use this program
			if (RasterSupport.IsLocked(RasterSupportType.Barcodes1D) && RasterSupport.IsLocked(RasterSupportType.Barcodes2D))
			{
				Messager.ShowError(this, BarcodeGlobalization.GetResxString(GetType(), "Resx_LEADBarcodeSupport"));
				return false;
			}

			try
			{
				_rasterCodecs = new RasterCodecs();
			}
			catch (Exception ex)
			{
				Messager.ShowError(this, string.Format("RasterCodec initialize error: {0}", ex.Message));
				return false;
			}

			// this is very important, must be placed leadtools.engine.dll in this path
			_rasterCodecs.Options.Pdf.InitialPath = AppDomain.CurrentDomain.BaseDirectory;

			_barcodeOptions = BarcodeOptions.Load();

			//BarcodeSymbology[] supportedSymbologies = BarcodeEngine.GetSupportedSymbologies();
			WriteLine(string.Format("{0} Supported symbologies:", _barcodeOptions.ReadOptionsSymbologies.Length), TraceEventType.Information);
			foreach (BarcodeSymbology symbology in _barcodeOptions.ReadOptionsSymbologies)
			{
				WriteLine(string.Format("{0}: {1}", symbology, BarcodeEngine.GetSymbologyFriendlyName(symbology)), TraceEventType.Information);
			}
			WriteLine(string.Format("----------"), TraceEventType.Information);

			_sampleSymbologiesRasterImage = null;

			// Create the barcodes symbologies multi-frame RasterImage
			using (Stream stream = GetType().Assembly.GetManifestResourceStream("BarcodeSplitManage.Resources.Symbologies.tif"))
			{
				_rasterCodecs.Options.Load.AllPages = true;
				_sampleSymbologiesRasterImage = _rasterCodecs.Load(stream);
			}

			_barcodeEngine = new BarcodeEngine();
			_barcodeEngine.Reader.ImageType = BarcodeImageType.Unknown;
			_barcodeEngine.Reader.EnableReturnFourPoints = false;
			// Continue on errors
			_barcodeEngine.Reader.ErrorMode = BarcodeReaderErrorMode.IgnoreAll;

			_directorySettings = new DirectorySettings();

			return true;
		}

		#endregion

		#region Events

		private void btnOptions_Click(object sender, EventArgs e)
		{
			ReadBarcodeOptionsDialogBox dlg = new ReadBarcodeOptionsDialogBox(_barcodeEngine, _sampleSymbologiesRasterImage, _barcodeOptions);

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				_barcodeOptions.ReadOptionsGroupIndex = dlg.SelectedGroupIndex;
				_barcodeOptions.ReadOptionsSymbologies = dlg.GetSelectedSymbologies();
				_barcodeOptions.ImageResolution = dlg.ImageResolution;
			}
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			if (_directorySettings.Edit(-1))
			{
				RefreshGridView(true);
				dataGridView_Click(sender, e);
			}
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			int nSeletedIndex = -1;

			if (dataGridView.SelectedRows.Count > 0)
				nSeletedIndex = dataGridView.SelectedRows[0].Index;

			if (nSeletedIndex < 0)
			{
				MessageBox.Show("Select row.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return;
			}

			if (_directorySettings.Edit(nSeletedIndex))
			{
				RefreshGridView();
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			int nSeletedIndex = -1;

			if (dataGridView.SelectedRows.Count > 0)
				nSeletedIndex = dataGridView.SelectedRows[0].Index;

			if (nSeletedIndex < 0)
			{
				MessageBox.Show("Select row.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return;
			}

			if (MessageBox.Show("Permanently delete this data?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}

			if (_directorySettings.Remove(nSeletedIndex))
			{
				RefreshGridView();
				dataGridView_Click(sender, e);
			}
		}

		private void dataGridView_Click(object sender, EventArgs e)
		{
			int nSeletedIndex = -1;

			if (dataGridView.SelectedRows.Count > 0)
				nSeletedIndex = dataGridView.SelectedRows[0].Index;

			bool runable = false;

			if (btnRunAll.Enabled && nSeletedIndex >= 0 && !_directorySettings.GetDirectoryData()[nSeletedIndex].Status)
			{
				runable = true;
			}

			btnEdit.Enabled = runable;
			btnDelete.Enabled = runable;
			btnEnable.Enabled = runable;

			bool enable = false;

			if (nSeletedIndex >= 0 && _directorySettings.GetDirectoryData()[nSeletedIndex].Enabled)
			{
				enable = true;
			}

			btnEnable.Text = enable ? "Disable" : "Enable";
		}

		private void btnRunAll_Click(object sender, EventArgs e)
		{
			if (!isExistService)
			{
				ServiceController sc;

				try
				{
					sc = new ServiceController(SERVICE_NAME);
					isExistService = true;
				}
				catch (Exception)
				{
				}
			}

			if (!isExistService)
			{
				MessageBox.Show("Must install PDF Scan Split Service.");
				return;
			}

			tmrServiceCheck.Start();

			_directorySettings.Save();

			Dictionary<int, DirectoryData> availableData = new Dictionary<int, DirectoryData>();
			DirectoryData[] data = _directorySettings.GetDirectoryData();

			for (int i = 0; i < data.Length; i++)
			{
				data[i].Status = data[i].Enabled;

				if (data[i].Status)
				{
					availableData.Add(i, data[i]);
				}
			}

			//RefreshGridView();

			//dataGridView_Click(sender, e);

			if (availableData.Count > 0)
			{
				tmrServiceCheck.Start();

				btnRunAll.Enabled = false;

				btnAdd.Enabled = false;
				btnEdit.Enabled = false;
				btnDelete.Enabled = false;
				btnEnable.Enabled = false;
				btnOptions.Enabled = false;
				btnStopAll.Enabled = true;

				//-
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
					case "Ready":
						dataGridView.Rows[listindex].Cells[3].Value = "";
						break;
					case "Processing":
						dataGridView.Rows[listindex].Cells[3].Value = "Processing";
						break;
					case "Running":
						dataGridView.Rows[listindex].Cells[3].Value = "Running";
						break;
					case "Stopping":
						dataGridView.Rows[listindex].Cells[3].Value = "Stopping";
						break;
					case "Stopped":
						dataGridView.Rows[listindex].Cells[3].Value = "";
						dirData[listindex].Status = false;
						break;
				}
			}
			else if (args.Kind == 2)
			{
				WriteLine(args.Msg, args.Evtype);
			}

			for (int i = 0; i < dirData.Length; i++)
			{
				if (dirData[i].Status)
					return;
			}

			btnRunAll.Enabled = true;

			btnAdd.Enabled = true;
			btnEdit.Enabled = true;
			btnDelete.Enabled = true;
			btnEnable.Enabled = true;
			btnOptions.Enabled = true;

			dataGridView_Click(this, null);
		}

		private void btnStopAll_Click(object sender, EventArgs e)
		{
			ServiceController sc;

			try
			{
				sc = new ServiceController(SERVICE_NAME);
				sc.Stop();
			}
			catch (Exception)
			{
				Debug.WriteLine("Invalid service name."); // Note that just because a name is valid does not mean the service exists.
			}

			btnStopAll.Enabled = false;
			
			dataGridView_Click(sender, e);
		}

		private void btnEnable_Click(object sender, EventArgs e)
		{
			DirectoryData[] data = _directorySettings.GetDirectoryData();

			int nSeletedIndex = -1;

			if (dataGridView.SelectedRows.Count > 0)
				nSeletedIndex = dataGridView.SelectedRows[0].Index;

			if (nSeletedIndex < 0)
				return;

			if (data[nSeletedIndex].FolderWatched == ""
				|| data[nSeletedIndex].FolderOutput == ""
				|| data[nSeletedIndex].FolderSuccess == ""
				|| data[nSeletedIndex].FolderError == ""
				|| data[nSeletedIndex].FolderLog == ""
				|| data[nSeletedIndex].SplitPdfName == "")
			{
				if (MessageBox.Show("Invalid directory settings. Edit this data?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}

				btnEdit_Click(sender, e);

				return;
			}

			data[nSeletedIndex].Enabled = btnEnable.Text == "Enable" ? true : false;

			RefreshGridView();

			dataGridView_Click(sender, e);
		}

		private void btnClearLog_Click(object sender, EventArgs e)
		{
			txtLog.Clear();
		}

		#endregion

		private void RefreshGridView(bool newrow = false)
		{
			dataGridView.Enabled = false;

			int nSeletedIndex = -1;

			if (dataGridView.SelectedRows.Count > 0)
				nSeletedIndex = dataGridView.SelectedRows[0].Index;

			dataGridView.Rows.Clear();

			DirectoryData[] data = _directorySettings.GetDirectoryData();

			for (int i = 0; i < data.Length; i++)
			{
				int index = dataGridView.Rows.Add();

				using (DataGridViewRow row = dataGridView.Rows[index])
				{
					row.Cells[0].Value = data[i].ID;
					row.Cells[1].Value = data[i].Enabled;
					row.Cells[2].Value = data[i].FolderWatched;
				}
			}

			if (newrow)
			{
				nSeletedIndex = dataGridView.Rows.Count - 1;
			}

			if (dataGridView.Rows.Count == 0)
				nSeletedIndex = -1;
			else if (nSeletedIndex >= dataGridView.Rows.Count)
				nSeletedIndex = dataGridView.Rows.Count - 1;

			if (nSeletedIndex >= 0)
			{
				dataGridView.Rows[nSeletedIndex].Selected = true;
				dataGridView.CurrentCell = dataGridView.Rows[nSeletedIndex].Cells[1];
			}

			dataGridView.Enabled = true;
		}

		public void WriteLine(string message, TraceEventType level)
		{
			txtLog.Select(txtLog.TextLength, 0);

			switch (level)
			{
				case TraceEventType.Critical:
					txtLog.SelectionColor = Color.Red;
					break;
				case TraceEventType.Debug:
					txtLog.SelectionColor = Color.Gray;
					break;
				case TraceEventType.Error:
					txtLog.SelectionColor = Color.FromArgb(255, 57, 32);
					break;
				case TraceEventType.Information:
					txtLog.SelectionColor = Color.FromArgb(81, 255, 106);
					break;
				case TraceEventType.None:
					txtLog.SelectionColor = Color.White;
					break;
				case TraceEventType.Verbose:
					txtLog.SelectionColor = Color.Red;
					break;
				case TraceEventType.Warning:
					txtLog.SelectionColor = Color.Yellow;
					break;
			}

			message = DateTime.Now.ToString("[H:mm] ") + message;

			txtLog.AppendText(message);
			txtLog.AppendText("\n");

			txtLog.SelectionStart = txtLog.Text.Length;
			txtLog.ScrollToCaret();
		}
	}
}
