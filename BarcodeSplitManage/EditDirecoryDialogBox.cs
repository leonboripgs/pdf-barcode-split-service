using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BarcodeSplitManage
{
	public partial class EditDirecoryDialogBox : Form
	{
		DirectoryData _data;

		public EditDirecoryDialogBox()
		{
			InitializeComponent();
		}

		public EditDirecoryDialogBox(DirectoryData data)
		{
			InitializeComponent();

			_data = data;

			txtFolderWatched.Text = _data.FolderWatched;
			txtFolderOutput.Text = _data.FolderOutput;
			txtFolderSuccess.Text = _data.FolderSuccess;
			txtFolderError.Text = _data.FolderError;
			txtFolderLog.Text = _data.FolderLog;
			txtSplitPdfName.Text = _data.SplitPdfName;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			_data.FolderWatched = txtFolderWatched.Text;
			_data.FolderOutput = txtFolderOutput.Text;
			_data.FolderSuccess = txtFolderSuccess.Text;
			_data.FolderError = txtFolderError.Text;
			_data.FolderLog = txtFolderLog.Text;
			_data.SplitPdfName = txtSplitPdfName.Text;

			this.DialogResult = DialogResult.OK;
		}

		public DirectoryData GetDirectoryData()
		{
			return _data;
		}

		private void btnBrowseWatched_Click(object sender, EventArgs e)
		{
			try
			{
				FolderBrowserDialog dlg = new FolderBrowserDialog();
				dlg.Description = "Select folder";
				dlg.SelectedPath = txtFolderWatched.Text;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					txtFolderWatched.Text = dlg.SelectedPath;
					Console.WriteLine("Changed watched folder");
				}
			}
			catch { }
		}

		private void btnBrowseOutput_Click(object sender, EventArgs e)
		{
			try
			{
				FolderBrowserDialog dlg = new FolderBrowserDialog();
				dlg.Description = "Select folder";
				dlg.SelectedPath = txtFolderOutput.Text;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					txtFolderOutput.Text = dlg.SelectedPath;
					Console.WriteLine("Changed output directory");
				}
			}
			catch { }
		}

		private void btnBrowseSuccess_Click(object sender, EventArgs e)
		{
			try
			{
				FolderBrowserDialog dlg = new FolderBrowserDialog();
				dlg.Description = "Select folder";
				dlg.SelectedPath = txtFolderSuccess.Text;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					txtFolderSuccess.Text = dlg.SelectedPath;
					Console.WriteLine("Changed success directory");
				}
			}
			catch { }
		}

		private void btnBrowseError_Click(object sender, EventArgs e)
		{
			try
			{
				FolderBrowserDialog dlg = new FolderBrowserDialog();
				dlg.Description = "Select folder";
				dlg.SelectedPath = txtFolderError.Text;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					txtFolderError.Text = dlg.SelectedPath;
					Console.WriteLine("Changed error directory");
				}
			}
			catch { }
		}

		private void btnBrowseLog_Click(object sender, EventArgs e)
		{
			try
			{
				FolderBrowserDialog dlg = new FolderBrowserDialog();
				dlg.Description = "Select folder";
				dlg.SelectedPath = txtFolderLog.Text;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					txtFolderLog.Text = dlg.SelectedPath;
					Console.WriteLine("Changed log file directory");
				}
			}
			catch { }
		}
	}
}
