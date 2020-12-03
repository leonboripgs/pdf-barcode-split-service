using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace BarcodeSplitManage
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

		public void Save()
		{
			string fileName = DataFileName;

			XmlWriter xmlWriter = null;
			try
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.IndentChars = "\t";
				settings.OmitXmlDeclaration = false;
				settings.Encoding = Encoding.UTF8;

				xmlWriter = XmlWriter.Create(fileName, settings);

				xmlWriter.WriteStartElement("Directory");

				for (int i = 0; i < _directoryList.Length; i++)
				{
					DirectoryData data = _directoryList[i];

					xmlWriter.WriteStartElement("Data");

					xmlWriter.WriteAttributeString("ID", data.ID.ToString());
					xmlWriter.WriteAttributeString("Enabled", data.Enabled.ToString());
					xmlWriter.WriteAttributeString("FolderWatched", data.FolderWatched);
					xmlWriter.WriteAttributeString("FolderOutput", data.FolderOutput);
					xmlWriter.WriteAttributeString("FolderSuccess", data.FolderSuccess);
					xmlWriter.WriteAttributeString("FolderError", data.FolderError);
					xmlWriter.WriteAttributeString("FolderLog", data.FolderLog);
					xmlWriter.WriteAttributeString("SplitPdfName", data.SplitPdfName);

					xmlWriter.WriteEndElement();
				}

				xmlWriter.WriteEndElement();
			}
			catch
			{
				Console.WriteLine("DirectorySettings.xml write error");
				return;
			}
			finally
			{
				// Finished with XmlTextReader
				if (xmlWriter != null)
					xmlWriter.Close();
			}
		}

		public bool Edit(int listIndex)
		{
			DirectoryData data;

			if (listIndex >= 0)
			{
				data = _directoryList[listIndex];
			}
			else
			{
				data = new DirectoryData();
			}

			EditDirecoryDialogBox dlg = new EditDirecoryDialogBox(data);

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				data = dlg.GetDirectoryData();

				if (listIndex >= 0)
				{
					_directoryList[listIndex] = data;
				}
				else
				{
					if (_directoryList.Length == 0)
						data.ID = 1;
					else
						data.ID = _directoryList[_directoryList.Length - 1].ID + 1;

					DirectoryData[] catData = new DirectoryData[] { data };

					_directoryList = _directoryList.Concat(catData).ToArray();
				}

				Save();

				return true;
			}

			return false;
		}

		public bool Remove(int listIndex)
		{
			if (listIndex >= 0)
			{
				_directoryList = _directoryList.Where((source, index) => index != listIndex).ToArray();

				Save();

				return true;
			}

			return false;
		}
	}
}
