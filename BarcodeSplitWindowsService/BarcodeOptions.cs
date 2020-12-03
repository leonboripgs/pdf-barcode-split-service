using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Leadtools.Barcode;

namespace BarcodeSplitWindowsService
{
	[Serializable]
	public struct BarcodeOptions
	{
		public int ReadOptionsGroupIndex;
		public BarcodeSymbology[] ReadOptionsSymbologies;
		public int WriteOptionsGroupIndex;
		public BarcodeSymbology WriteOptionsSymbology;
		public bool ReadBarcodesWhenOptionsDialogCloses;
		public int ImageResolution;

		public static BarcodeOptions Default
		{
			get
			{
				BarcodeOptions obj = new BarcodeOptions();
				obj.ReadOptionsGroupIndex = 0;
				obj.ReadOptionsSymbologies = BarcodeEngine.GetSupportedSymbologies();
				obj.WriteOptionsGroupIndex = 0;
				obj.WriteOptionsSymbology = BarcodeSymbology.EAN13;
				obj.ReadBarcodesWhenOptionsDialogCloses = true;
				obj.ImageResolution = 300;

				return obj;
			}
		}

		private static string DataFileName
		{
			get
			{
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"BarcodeMainOptions.xml");
			}
		}

		private static XmlSerializer _serializer = new XmlSerializer(typeof(BarcodeOptions));

		public static BarcodeOptions Load()
		{
			try
			{
				if (File.Exists(DataFileName))
				{
					using (XmlTextReader reader = new XmlTextReader(DataFileName))
					{
						return (BarcodeOptions)_serializer.Deserialize(reader);
					}
				}
				else
				{
					return BarcodeOptions.Default;
				}
			}
			catch
			{
				return BarcodeOptions.Default;
			}
		}

		public void Save()
		{
			try
			{
				using (XmlTextWriter writer = new XmlTextWriter(DataFileName, Encoding.Unicode))
				{
					writer.Formatting = Formatting.Indented;
					writer.Indentation = 2;
					_serializer.Serialize(writer, this);
				}
			}
			catch
			{
			}
		}
	}
}
