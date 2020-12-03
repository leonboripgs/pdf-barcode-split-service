using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace BarcodeSplitWindowsService
{
	public struct DirectoryData
	{
		public int ID;
		public bool Enabled;
		public string FolderWatched;
		public string FolderOutput;
		public string FolderSuccess;
		public string FolderError;
		public string FolderLog;
		public string SplitPdfName;
		public bool Status; // runtime value
	}

	public class DirectorySettings
	{
		private DirectoryData[] _directoryList;

		public DirectorySettings()
		{
			Load();
		}

		private static string DataFileName
		{
			get
			{
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DirectorySettings.xml");
			}
		}

		public DirectoryData[] GetDirectoryData()
		{
			return _directoryList;
		}

		public void Load()
		{
			string fileName = DataFileName;

			List<DirectoryData> list = new List<DirectoryData>();
			XmlTextReader xmlReader = null;
			try
			{
				xmlReader = new XmlTextReader(fileName);

				while (xmlReader.Read())
				{
					if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Directory")
						while (xmlReader.Read())
						{
							if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Data")
							{
								int ID = 0;
								bool Enabled = false;
								string FolderWatched = "";
								string FolderOutput = "";
								string FolderSuccess = "";
								string FolderError = "";
								string FolderLog = "";
								string SplitPdfName = "";

								while (xmlReader.MoveToNextAttribute())
								{
									if (xmlReader.Name == "ID")
									{
										if (xmlReader.Value != string.Empty)
											ID = int.Parse(xmlReader.Value);
										else
											ID = 0;
									}
									else if (xmlReader.Name == "Enabled")
										Enabled = bool.Parse(xmlReader.Value);
									else if (xmlReader.Name == "FolderWatched")
										FolderWatched = xmlReader.Value;
									else if (xmlReader.Name == "FolderOutput")
										FolderOutput = xmlReader.Value;
									else if (xmlReader.Name == "FolderSuccess")
										FolderSuccess = xmlReader.Value;
									else if (xmlReader.Name == "FolderError")
										FolderError = xmlReader.Value;
									else if (xmlReader.Name == "FolderLog")
										FolderLog = xmlReader.Value;
									else if (xmlReader.Name == "SplitPdfName")
										SplitPdfName = xmlReader.Value;
								}

								if (ID > 0)
								{
									DirectoryData info = new DirectoryData();

									info.ID = ID;
									info.Enabled = Enabled;
									info.FolderWatched = FolderWatched;
									info.FolderOutput = FolderOutput;
									info.FolderSuccess = FolderSuccess;
									info.FolderError = FolderError;
									info.FolderLog = FolderLog;
									info.SplitPdfName = SplitPdfName;

									list.Add(info);
								}
							}
						}
				}
			}
			catch
			{
				Console.WriteLine("DirectorySettings.xml not found");
				return;
			}
			finally
			{
				// Finished with XmlTextReader
				if (xmlReader != null)
					xmlReader.Close();
			}

			_directoryList = list.ToArray();
		}
	}
}
