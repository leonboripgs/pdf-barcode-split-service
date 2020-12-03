using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;
using Leadtools.Forms;
using Leadtools.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace BarcodeSplitWindowsService
{
	public enum ServiceState
	{
		Ready = 0,
		Running = 1,
		Processing = 2,
		Stopping = 3,
		Stopped = 4
	}
	public class PDFSplitterEventArgs : EventArgs
	{
		public PDFSplitterEventArgs(int dirID, int kind, string msg, TraceEventType etype = 0)
		{
			DirID = dirID;
			Kind = kind;
			Msg = msg;
			Evtype = etype;
		}
		public int DirID { get; set; }
		public int Kind { get; set; }
		public string Msg { get; set; }
		public TraceEventType Evtype { get; set; }
	}

	public class PDFSplitter
	{
		private MainService _parent;

		private BarcodeOptions _barcodeOptions;
		private DirectoryData _directoryData;

		private bool _runable;

		private ServiceState _runningState;
		public ServiceState RunnigState
		{ get { return _runningState; } }

		public PDFSplitter(MainService parent, BarcodeOptions options, DirectoryData dirdata)
		{
			_parent = parent;
			_barcodeOptions = options;
			_directoryData = dirdata;

			_runningState = ServiceState.Ready;
			_runable = false;
		}

		public void Run()
		{
			_runable = true;
			_runningState = ServiceState.Running;

			SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 1, "Running"));

			while (_runable)
			{
				Thread.Sleep(1000);

				if (Directory.Exists(_directoryData.FolderWatched))
				{
					string[] fileEntries = Directory.GetFiles(_directoryData.FolderWatched);

					foreach (string fileName in fileEntries)
					{
						if (Path.GetExtension(fileName).ToLower() != ".pdf")
							continue;

						_runningState = ServiceState.Processing;
						SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 1, "Processing"));

						Split(fileName);

						if (!_runable)
							break;
					}
				}

				_runningState = ServiceState.Running;
				SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 1, "Running"));
			}
			
			_runningState = ServiceState.Ready;
			SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 1, "Stopped"));
		}

		public void Stop()
		{
			_runable = false;

			if (_runningState != ServiceState.Ready)
			{
				_runningState = ServiceState.Stopping;

				SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 1, "Stopping"));
			}
		}

		private bool Split(string srcFileName)
		{
			// Open a document from disk

			if (!File.Exists(srcFileName))
			{
				SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 2, string.Format("Load error: {0} no exist", srcFileName), TraceEventType.Error));
				return false;
			}

			string dstFileName;
			PDFDocument pdfDoc;

			try
			{
				pdfDoc = new PDFDocument(srcFileName);
				SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 2, string.Format("Load success: {0} has {1} pages", srcFileName, pdfDoc.Pages.Count), TraceEventType.Information));
			}
			catch (Exception ex)
			{
				SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 2, string.Format("Load pdf error: {0}", ex.Message), TraceEventType.Error));

				dstFileName = Path.Combine(_directoryData.FolderError, Path.GetFileName(srcFileName));

				FileCopy(srcFileName, dstFileName);

				return false;
			}

			int allPagesCount = pdfDoc.Pages.Count;

			pdfDoc.Resolution = _barcodeOptions.ImageResolution;

			Dictionary<int, BarcodeData[]> findedData = new Dictionary<int, BarcodeData[]>();
			Dictionary<int, BarcodeData[]> errorData = new Dictionary<int, BarcodeData[]>();

			RasterCodecs _rasterCodecs = new RasterCodecs();
			_rasterCodecs.Options.Pdf.InitialPath = AppDomain.CurrentDomain.BaseDirectory;

			for (int i = 1; i <= pdfDoc.Pages.Count; i++)
			{
				try
				{
					BarcodeData[] detectBarcodes = ReadBarcode(pdfDoc.GetPageImage(_rasterCodecs, i));
					BarcodeData[] barcodes = new BarcodeData[] { };

					if (detectBarcodes.Count() > 0)
					{
						// check barcode value contain "-";

						for (int k = 0; k < detectBarcodes.Count(); k++)
						{
							if (detectBarcodes[k].Value.IndexOf("-") > 0)
							{
								barcodes = barcodes.Concat(new BarcodeData[] { detectBarcodes[k] }).ToArray();
							}
						}

						if (barcodes.Length == 1)
							findedData.Add(i, barcodes);
						else if (barcodes.Length > 1)
							errorData.Add(i, barcodes); // this pdf is error file.
					}
				}
				catch (Exception ex)
				{
					SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 2, string.Format("Detect barcodes error in pdf: {0}, page: {1}, {2}", srcFileName, i, ex.Message), TraceEventType.Error));
				}
			}

			pdfDoc.Dispose();

			if (errorData.Count > 0)
			{
				dstFileName = Path.Combine(_directoryData.FolderError, Path.GetFileName(srcFileName));

				string errPages = "";

				foreach (var fData in findedData)
				{
					errPages += fData.Key + ",";
				}
				SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 2, string.Format("Detect two or more barcodes in pdf: {0}, page(s): {1}", srcFileName, errPages), TraceEventType.Error));
			}
			else if (findedData.Count <= 0)
			{
				dstFileName = Path.Combine(_directoryData.FolderError, Path.GetFileName(srcFileName));
				SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 2, string.Format("No detect barcodes: {0}", srcFileName), TraceEventType.Error));
			}
			else
			{
				bool errorFlag = false;

				if (findedData.Count > 0)
				{
					findedData.Add(allPagesCount + 1, null);
				}

				PDFFile pdffile = new PDFFile(srcFileName);

				int pageStart = -1, pageEnd;
				BarcodeData prevBarcodeData = null;

				foreach (var fData in findedData)
				{
					if (pageStart == -1)
					{
						pageStart = fData.Key;
						prevBarcodeData = fData.Value[0];
						continue;
					}

					string splitName = "";

					pageEnd = fData.Key - 1;

					try
					{
						splitName = GetNewPdfFileName(srcFileName, prevBarcodeData, pageStart, pageEnd);

						pdffile.ExtractPages(pageStart, pageEnd, splitName);
						SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 2, string.Format("Extract pdf success: {0}, {1} ~ {2} pages", splitName, pageStart, pageEnd), TraceEventType.Debug));
					}
					catch (Exception ex)
					{
						SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 2, string.Format("PDF split error, {0} : {1} ~ {2} pages, {3}", splitName, pageStart, pageEnd, ex.Message), TraceEventType.Error));
						errorFlag = true;
						break;
					}

					if (fData.Value == null)
						break;

					pageStart = fData.Key;					
					prevBarcodeData = fData.Value[0];
				}

				if (!errorFlag)
				{
					dstFileName = Path.Combine(_directoryData.FolderSuccess, Path.GetFileName(srcFileName));
					SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 2, string.Format("Split success: {0}", srcFileName), TraceEventType.Information));
				}
				else
				{
					dstFileName = Path.Combine(_directoryData.FolderError, Path.GetFileName(srcFileName));
				}
			}

			bool ret = FileCopy(srcFileName, dstFileName);
			File.Delete(srcFileName);
			return ret;
		}

		private string GetNewPdfFileName(string pdfFileName, BarcodeData barcodes, int pageStart, int pageEnd)
		{
			string str = _directoryData.SplitPdfName;

			pdfFileName = Path.GetFileNameWithoutExtension(pdfFileName);

			if (barcodes != null)
			{
				string codeVal = barcodes.Value;
				str = str.Replace("{barcode}", "{" + codeVal + "}");
			}
			else
			{
				str = str.Replace("{barcode}", "{#ERROR}");
			}

			int unixtimestamp = (int)Math.Truncate((DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);

			str = str.Replace("{timestamp}", "{" + unixtimestamp.ToString() + "}");

			str = str.Replace("{basename}", "{" + pdfFileName + "}");
			str = str.Replace("{startpage}", "{" + pageStart.ToString() + "}");
			str = str.Replace("{endpage}", "{" + pageEnd.ToString() + "}");
			str = str.Replace("{pages}", "{" + (pageEnd-pageStart).ToString() + "}");

			str = str.Replace("}{", "");
			str = str.Replace("{", "");
			str = str.Replace("}", "");

			str = Path.Combine(_directoryData.FolderOutput, str);

			str += ".pdf";

			return str;
		}

		private bool FileCopy(string srcFileName, string dstFileName, bool overwrite = true)
		{
			try
			{
				File.Copy(srcFileName, dstFileName, overwrite);
			}
			catch (Exception ex)
			{
				SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 2, string.Format("PDF file copying  error, {0} -> {1} : {2}", srcFileName, dstFileName, ex.Message), TraceEventType.Error));
				return false;
			}

			return true;
		}

		private BarcodeData[] ReadBarcode(RasterImage image)
		{
			BarcodeEngine _barcodeEngine = new BarcodeEngine();
			_barcodeEngine.Reader.ImageType = BarcodeImageType.Unknown;
			_barcodeEngine.Reader.EnableReturnFourPoints = false;
			// Continue on errors
			_barcodeEngine.Reader.ErrorMode = BarcodeReaderErrorMode.IgnoreAll;

			BarcodeData[] barcodes = new BarcodeData[] { };

			try
			{
				barcodes = _barcodeEngine.Reader.ReadBarcodes(image, LogicalRectangle.Empty, 0, _barcodeOptions.ReadOptionsSymbologies, null);
			}
			catch (Exception ex)
			{
				SendMsgEvent(this, new PDFSplitterEventArgs(_directoryData.ID, 2, string.Format("Detect barcodes error: {0}", ex.Message), TraceEventType.Error));
			}

			return barcodes;
		}

		void SendMsgEvent(object sender, PDFSplitterEventArgs e)
		{
			if (e.Evtype == TraceEventType.Error)
			{
				string message = DateTime.Now.ToString("[H:mm] ") + e.Msg;

				string path = Path.Combine(_directoryData.FolderLog, "error.log.txt");
				// This text is added only once to the file.
				try
				{
					// Create a file to write to.
					using (StreamWriter sw = new StreamWriter(path, true))
					{
						sw.WriteLine(message);
						sw.Flush();
						sw.Close();
					}
				}
				catch
				{

				}
			}

			//if (_parent != null)
			//	_parent.Invoke(new EventHandler<PDFSplitterEventArgs>(Progress), sender, e);
			Progress(sender, e);
		}

		void Progress(object sender, PDFSplitterEventArgs e)
		{
			if (_parent != null)
				_parent.OnPDFSplitterMsgEvent(sender, e);
		}
	}
}
