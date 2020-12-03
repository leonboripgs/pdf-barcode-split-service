namespace BarcodeSplitManage
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
			this.btnOptions = new System.Windows.Forms.Button();
			this.txtLog = new System.Windows.Forms.RichTextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.chID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.chEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.chWatched = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.chStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnEdit = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnEnable = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnStopAll = new System.Windows.Forms.Button();
			this.btnRunAll = new System.Windows.Forms.Button();
			this.btnClearLog = new System.Windows.Forms.Button();
			this.tmrServiceCheck = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOptions
			// 
			this.btnOptions.Location = new System.Drawing.Point(420, 22);
			this.btnOptions.Name = "btnOptions";
			this.btnOptions.Size = new System.Drawing.Size(87, 27);
			this.btnOptions.TabIndex = 0;
			this.btnOptions.Text = "Options";
			this.btnOptions.UseVisualStyleBackColor = true;
			this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
			// 
			// txtLog
			// 
			this.txtLog.BackColor = System.Drawing.Color.Black;
			this.txtLog.ForeColor = System.Drawing.Color.White;
			this.txtLog.Location = new System.Drawing.Point(11, 441);
			this.txtLog.Name = "txtLog";
			this.txtLog.Size = new System.Drawing.Size(714, 140);
			this.txtLog.TabIndex = 1;
			this.txtLog.Text = "";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(212, 83);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// dataGridView
			// 
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.AllowUserToResizeRows = false;
			this.dataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle7.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chID,
            this.chEnabled,
            this.chWatched,
            this.chStatus});
			this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.dataGridView.Location = new System.Drawing.Point(12, 101);
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.ReadOnly = true;
			this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView.ShowCellErrors = false;
			this.dataGridView.Size = new System.Drawing.Size(621, 267);
			this.dataGridView.TabIndex = 3;
			this.dataGridView.Click += new System.EventHandler(this.dataGridView_Click);
			// 
			// chID
			// 
			dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
			this.chID.DefaultCellStyle = dataGridViewCellStyle8;
			this.chID.HeaderText = "ID";
			this.chID.Name = "chID";
			this.chID.ReadOnly = true;
			this.chID.Visible = false;
			this.chID.Width = 40;
			// 
			// chEnabled
			// 
			this.chEnabled.HeaderText = "Enabled";
			this.chEnabled.Name = "chEnabled";
			this.chEnabled.ReadOnly = true;
			this.chEnabled.Width = 80;
			// 
			// chWatched
			// 
			this.chWatched.HeaderText = "Watched Folder";
			this.chWatched.Name = "chWatched";
			this.chWatched.ReadOnly = true;
			this.chWatched.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.chWatched.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.chWatched.Width = 400;
			// 
			// chStatus
			// 
			dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.chStatus.DefaultCellStyle = dataGridViewCellStyle9;
			this.chStatus.HeaderText = "Status";
			this.chStatus.Name = "chStatus";
			this.chStatus.ReadOnly = true;
			this.chStatus.Width = 80;
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(639, 139);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(87, 27);
			this.btnAdd.TabIndex = 0;
			this.btnAdd.Text = "Add Rule";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnEdit
			// 
			this.btnEdit.Location = new System.Drawing.Point(639, 194);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(87, 27);
			this.btnEdit.TabIndex = 0;
			this.btnEdit.Text = "Edit";
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(639, 249);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(87, 27);
			this.btnDelete.TabIndex = 0;
			this.btnDelete.Text = "Delete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnEnable
			// 
			this.btnEnable.Location = new System.Drawing.Point(639, 304);
			this.btnEnable.Name = "btnEnable";
			this.btnEnable.Size = new System.Drawing.Size(87, 27);
			this.btnEnable.TabIndex = 0;
			this.btnEnable.Text = "Enable";
			this.btnEnable.UseVisualStyleBackColor = true;
			this.btnEnable.Click += new System.EventHandler(this.btnEnable_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnStopAll);
			this.groupBox1.Controls.Add(this.btnRunAll);
			this.groupBox1.Controls.Add(this.btnClearLog);
			this.groupBox1.Controls.Add(this.btnOptions);
			this.groupBox1.Location = new System.Drawing.Point(12, 374);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(621, 61);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			// 
			// btnStopAll
			// 
			this.btnStopAll.Enabled = false;
			this.btnStopAll.Location = new System.Drawing.Point(208, 22);
			this.btnStopAll.Name = "btnStopAll";
			this.btnStopAll.Size = new System.Drawing.Size(146, 27);
			this.btnStopAll.TabIndex = 0;
			this.btnStopAll.Text = "Stop all rules";
			this.btnStopAll.UseVisualStyleBackColor = true;
			this.btnStopAll.Click += new System.EventHandler(this.btnStopAll_Click);
			// 
			// btnRunAll
			// 
			this.btnRunAll.Location = new System.Drawing.Point(33, 22);
			this.btnRunAll.Name = "btnRunAll";
			this.btnRunAll.Size = new System.Drawing.Size(146, 27);
			this.btnRunAll.TabIndex = 0;
			this.btnRunAll.Text = "Run all rules";
			this.btnRunAll.UseVisualStyleBackColor = true;
			this.btnRunAll.Click += new System.EventHandler(this.btnRunAll_Click);
			// 
			// btnClearLog
			// 
			this.btnClearLog.Location = new System.Drawing.Point(524, 22);
			this.btnClearLog.Name = "btnClearLog";
			this.btnClearLog.Size = new System.Drawing.Size(87, 27);
			this.btnClearLog.TabIndex = 0;
			this.btnClearLog.Text = "Clear Log";
			this.btnClearLog.UseVisualStyleBackColor = true;
			this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
			// 
			// tmrServiceCheck
			// 
			this.tmrServiceCheck.Interval = 1000;
			this.tmrServiceCheck.Tick += new System.EventHandler(this.tmrServiceCheck_Tick);
			// 
			// MainForm
			// 
			this.AcceptButton = this.btnRunAll;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(737, 589);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.dataGridView);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.txtLog);
			this.Controls.Add(this.btnEnable);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.btnAdd);
			this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Itegrity - Barcode Split Service";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnOptions;
		private System.Windows.Forms.RichTextBox txtLog;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnEnable;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnStopAll;
		private System.Windows.Forms.Button btnRunAll;
		private System.Windows.Forms.DataGridViewTextBoxColumn chID;
		private System.Windows.Forms.DataGridViewCheckBoxColumn chEnabled;
		private System.Windows.Forms.DataGridViewTextBoxColumn chWatched;
		private System.Windows.Forms.DataGridViewTextBoxColumn chStatus;
		private System.Windows.Forms.Button btnClearLog;
		private System.Windows.Forms.Timer tmrServiceCheck;
	}
}

