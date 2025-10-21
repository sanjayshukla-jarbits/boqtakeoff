namespace boqtakeoff.core
{
    partial class Main
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lblSelectfile = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnFileBrowse = new System.Windows.Forms.Button();
            this.linkDownload = new System.Windows.Forms.LinkLabel();
            this.grpErrorPanel = new System.Windows.Forms.GroupBox();
            this.grdErrorgrid = new System.Windows.Forms.DataGridView();
            this.SKU = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UOM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.error_log = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grpButtonPanel = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.grpSelectProject = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtVersionLevel = new System.Windows.Forms.TextBox();
            this.cmbProjects = new System.Windows.Forms.ComboBox();
            this.lblVersionLabel = new System.Windows.Forms.Label();
            this.lblSelectProject = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.rdoExcel = new System.Windows.Forms.RadioButton();
            this.rdoModel = new System.Windows.Forms.RadioButton();
            this.rdoBritish = new System.Windows.Forms.RadioButton();
            this.rdoMetric = new System.Windows.Forms.RadioButton();
            this.grpExcelModel = new System.Windows.Forms.GroupBox();
            this.grpSelectFile = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlModelCategory = new System.Windows.Forms.GroupBox();
            this.gvCategory = new System.Windows.Forms.DataGridView();
            this.checkbox = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BuiltInCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CategoryValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlRoom = new System.Windows.Forms.Panel();
            this.btnViewImage = new System.Windows.Forms.Button();
            this.dgRoomNames = new System.Windows.Forms.DataGridView();
            this.label7 = new System.Windows.Forms.Label();
            this.cmdRoomLevel = new System.Windows.Forms.ComboBox();
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.btnPreview = new System.Windows.Forms.Button();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.grpErrorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdErrorgrid)).BeginInit();
            this.grpButtonPanel.SuspendLayout();
            this.grpSelectProject.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.grpExcelModel.SuspendLayout();
            this.grpSelectFile.SuspendLayout();
            this.pnlModelCategory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvCategory)).BeginInit();
            this.pnlRoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgRoomNames)).BeginInit();
            this.pnlPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "CSV";
            // 
            // lblSelectfile
            // 
            this.lblSelectfile.AutoSize = true;
            this.lblSelectfile.Font = new System.Drawing.Font("Arial Narrow", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectfile.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.lblSelectfile.Location = new System.Drawing.Point(30, 50);
            this.lblSelectfile.Name = "lblSelectfile";
            this.lblSelectfile.Size = new System.Drawing.Size(118, 16);
            this.lblSelectfile.TabIndex = 8;
            this.lblSelectfile.Text = "Supported (.csv,.xlsx,xls)";
            // 
            // txtFileName
            // 
            this.txtFileName.AccessibleDescription = "";
            this.txtFileName.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtFileName.Font = new System.Drawing.Font("Arial", 10F);
            this.txtFileName.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtFileName.Location = new System.Drawing.Point(155, 22);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(364, 27);
            this.txtFileName.TabIndex = 7;
            // 
            // btnFileBrowse
            // 
            this.btnFileBrowse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(33)))), ((int)(((byte)(30)))));
            this.btnFileBrowse.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnFileBrowse.FlatAppearance.BorderSize = 0;
            this.btnFileBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFileBrowse.Font = new System.Drawing.Font("Arial", 8F);
            this.btnFileBrowse.ForeColor = System.Drawing.Color.White;
            this.btnFileBrowse.Location = new System.Drawing.Point(589, 22);
            this.btnFileBrowse.Name = "btnFileBrowse";
            this.btnFileBrowse.Size = new System.Drawing.Size(98, 30);
            this.btnFileBrowse.TabIndex = 6;
            this.btnFileBrowse.Text = "Browse";
            this.btnFileBrowse.UseVisualStyleBackColor = false;
            this.btnFileBrowse.Click += new System.EventHandler(this.btnFileBrowse_Click);
            // 
            // linkDownload
            // 
            this.linkDownload.AutoSize = true;
            this.linkDownload.BackColor = System.Drawing.SystemColors.Control;
            this.linkDownload.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkDownload.Location = new System.Drawing.Point(690, 32);
            this.linkDownload.Name = "linkDownload";
            this.linkDownload.Size = new System.Drawing.Size(136, 17);
            this.linkDownload.TabIndex = 8;
            this.linkDownload.TabStop = true;
            this.linkDownload.Text = "Download Template";
            this.linkDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDownload_LinkClicked);
            // 
            // grpErrorPanel
            // 
            this.grpErrorPanel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.grpErrorPanel.Controls.Add(this.grdErrorgrid);
            this.grpErrorPanel.Location = new System.Drawing.Point(838, 12);
            this.grpErrorPanel.Name = "grpErrorPanel";
            this.grpErrorPanel.Size = new System.Drawing.Size(814, 161);
            this.grpErrorPanel.TabIndex = 21;
            this.grpErrorPanel.TabStop = false;
            this.grpErrorPanel.Visible = false;
            // 
            // grdErrorgrid
            // 
            this.grdErrorgrid.AllowUserToAddRows = false;
            this.grdErrorgrid.AllowUserToDeleteRows = false;
            this.grdErrorgrid.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.grdErrorgrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.grdErrorgrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(33)))), ((int)(((byte)(30)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdErrorgrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.grdErrorgrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdErrorgrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SKU,
            this.UOM,
            this.Column2,
            this.error_log});
            this.grdErrorgrid.Location = new System.Drawing.Point(6, 11);
            this.grdErrorgrid.Name = "grdErrorgrid";
            this.grdErrorgrid.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.HotTrack;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdErrorgrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.grdErrorgrid.RowHeadersVisible = false;
            this.grdErrorgrid.RowHeadersWidth = 51;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdErrorgrid.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.grdErrorgrid.RowTemplate.Height = 26;
            this.grdErrorgrid.Size = new System.Drawing.Size(802, 104);
            this.grdErrorgrid.TabIndex = 17;
            // 
            // SKU
            // 
            this.SKU.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SKU.DataPropertyName = "sku_label";
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            this.SKU.DefaultCellStyle = dataGridViewCellStyle2;
            this.SKU.Frozen = true;
            this.SKU.HeaderText = "SKU";
            this.SKU.MinimumWidth = 6;
            this.SKU.Name = "SKU";
            this.SKU.ReadOnly = true;
            this.SKU.Width = 200;
            // 
            // UOM
            // 
            this.UOM.DataPropertyName = "UOM";
            this.UOM.Frozen = true;
            this.UOM.HeaderText = "UOM";
            this.UOM.MinimumWidth = 6;
            this.UOM.Name = "UOM";
            this.UOM.ReadOnly = true;
            this.UOM.Width = 200;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Quantity";
            this.Column2.Frozen = true;
            this.Column2.HeaderText = "Quantity";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 80;
            // 
            // error_log
            // 
            this.error_log.DataPropertyName = "error_log";
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Red;
            this.error_log.DefaultCellStyle = dataGridViewCellStyle3;
            this.error_log.Frozen = true;
            this.error_log.HeaderText = "Error";
            this.error_log.MinimumWidth = 6;
            this.error_log.Name = "error_log";
            this.error_log.ReadOnly = true;
            this.error_log.Width = 400;
            // 
            // grpButtonPanel
            // 
            this.grpButtonPanel.BackColor = System.Drawing.SystemColors.Control;
            this.grpButtonPanel.Controls.Add(this.btnCancel);
            this.grpButtonPanel.Controls.Add(this.btnNext);
            this.grpButtonPanel.Controls.Add(this.btnBack);
            this.grpButtonPanel.Location = new System.Drawing.Point(5, 566);
            this.grpButtonPanel.Name = "grpButtonPanel";
            this.grpButtonPanel.Size = new System.Drawing.Size(827, 69);
            this.grpButtonPanel.TabIndex = 9;
            this.grpButtonPanel.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(33)))), ((int)(((byte)(30)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(723, 21);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 36);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(33)))), ((int)(((byte)(30)))));
            this.btnNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNext.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.ForeColor = System.Drawing.Color.White;
            this.btnNext.Location = new System.Drawing.Point(619, 22);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(98, 36);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Finish";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(159)))), ((int)(((byte)(160)))));
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(515, 22);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(98, 36);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "<Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Visible = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // grpSelectProject
            // 
            this.grpSelectProject.BackColor = System.Drawing.SystemColors.Control;
            this.grpSelectProject.Controls.Add(this.tableLayoutPanel1);
            this.grpSelectProject.Location = new System.Drawing.Point(5, 127);
            this.grpSelectProject.Name = "grpSelectProject";
            this.grpSelectProject.Size = new System.Drawing.Size(827, 92);
            this.grpSelectProject.TabIndex = 12;
            this.grpSelectProject.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 299F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtVersionLevel, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.cmbProjects, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblVersionLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblSelectProject, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(11, 13);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 44.73684F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55.26316F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(810, 70);
            this.tableLayoutPanel1.TabIndex = 8;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(117, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 19);
            this.label2.TabIndex = 6;
            this.label2.Text = " *";
            // 
            // txtVersionLevel
            // 
            this.txtVersionLevel.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVersionLevel.Location = new System.Drawing.Point(144, 34);
            this.txtVersionLevel.Name = "txtVersionLevel";
            this.txtVersionLevel.Size = new System.Drawing.Size(364, 27);
            this.txtVersionLevel.TabIndex = 1;
            // 
            // cmbProjects
            // 
            this.cmbProjects.AllowDrop = true;
            this.cmbProjects.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmbProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProjects.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbProjects.FormattingEnabled = true;
            this.cmbProjects.Location = new System.Drawing.Point(144, 2);
            this.cmbProjects.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbProjects.Name = "cmbProjects";
            this.cmbProjects.Size = new System.Drawing.Size(364, 25);
            this.cmbProjects.TabIndex = 4;
            // 
            // lblVersionLabel
            // 
            this.lblVersionLabel.AutoSize = true;
            this.lblVersionLabel.Font = new System.Drawing.Font("Arial", 10F);
            this.lblVersionLabel.Location = new System.Drawing.Point(3, 31);
            this.lblVersionLabel.Name = "lblVersionLabel";
            this.lblVersionLabel.Size = new System.Drawing.Size(108, 19);
            this.lblVersionLabel.TabIndex = 0;
            this.lblVersionLabel.Text = "Version Label";
            this.lblVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSelectProject
            // 
            this.lblSelectProject.AutoSize = true;
            this.lblSelectProject.Font = new System.Drawing.Font("Arial", 10F);
            this.lblSelectProject.Location = new System.Drawing.Point(3, 0);
            this.lblSelectProject.Name = "lblSelectProject";
            this.lblSelectProject.Size = new System.Drawing.Size(107, 19);
            this.lblSelectProject.TabIndex = 3;
            this.lblSelectProject.Text = "Project Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(117, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 19);
            this.label1.TabIndex = 7;
            this.label1.Text = " *";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // rdoExcel
            // 
            this.rdoExcel.AutoSize = true;
            this.rdoExcel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.rdoExcel.Checked = true;
            this.rdoExcel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoExcel.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoExcel.ForeColor = System.Drawing.Color.Black;
            this.rdoExcel.Location = new System.Drawing.Point(11, 29);
            this.rdoExcel.Name = "rdoExcel";
            this.rdoExcel.Size = new System.Drawing.Size(69, 23);
            this.rdoExcel.TabIndex = 9;
            this.rdoExcel.TabStop = true;
            this.rdoExcel.Text = "Excel";
            this.rdoExcel.UseVisualStyleBackColor = false;
            this.rdoExcel.CheckedChanged += new System.EventHandler(this.rdoExcel_CheckedChanged);
            // 
            // rdoModel
            // 
            this.rdoModel.AutoSize = true;
            this.rdoModel.BackColor = System.Drawing.SystemColors.Control;
            this.rdoModel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoModel.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoModel.ForeColor = System.Drawing.Color.Black;
            this.rdoModel.Location = new System.Drawing.Point(137, 29);
            this.rdoModel.Name = "rdoModel";
            this.rdoModel.Size = new System.Drawing.Size(73, 23);
            this.rdoModel.TabIndex = 10;
            this.rdoModel.Text = "Model";
            this.rdoModel.UseVisualStyleBackColor = false;
            this.rdoModel.CheckedChanged += new System.EventHandler(this.rdoModel_CheckedChanged);
            // 
            // rdoBritish
            // 
            this.rdoBritish.AutoSize = true;
            this.rdoBritish.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoBritish.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoBritish.ForeColor = System.Drawing.Color.Black;
            this.rdoBritish.Location = new System.Drawing.Point(137, 24);
            this.rdoBritish.Name = "rdoBritish";
            this.rdoBritish.Size = new System.Drawing.Size(76, 23);
            this.rdoBritish.TabIndex = 4;
            this.rdoBritish.Text = "British";
            this.rdoBritish.UseVisualStyleBackColor = true;
            this.rdoBritish.CheckedChanged += new System.EventHandler(this.rdoBritish_CheckedChanged);
            // 
            // rdoMetric
            // 
            this.rdoMetric.AutoSize = true;
            this.rdoMetric.Checked = true;
            this.rdoMetric.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoMetric.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoMetric.ForeColor = System.Drawing.Color.Black;
            this.rdoMetric.Location = new System.Drawing.Point(9, 24);
            this.rdoMetric.Name = "rdoMetric";
            this.rdoMetric.Size = new System.Drawing.Size(75, 23);
            this.rdoMetric.TabIndex = 3;
            this.rdoMetric.TabStop = true;
            this.rdoMetric.Text = "Metric";
            this.rdoMetric.UseVisualStyleBackColor = true;
            this.rdoMetric.CheckedChanged += new System.EventHandler(this.rdoMetric_CheckedChanged);
            // 
            // grpExcelModel
            // 
            this.grpExcelModel.Controls.Add(this.rdoModel);
            this.grpExcelModel.Controls.Add(this.rdoExcel);
            this.grpExcelModel.Location = new System.Drawing.Point(5, -4);
            this.grpExcelModel.Name = "grpExcelModel";
            this.grpExcelModel.Size = new System.Drawing.Size(827, 66);
            this.grpExcelModel.TabIndex = 24;
            this.grpExcelModel.TabStop = false;
            // 
            // grpSelectFile
            // 
            this.grpSelectFile.BackColor = System.Drawing.SystemColors.Control;
            this.grpSelectFile.Controls.Add(this.linkDownload);
            this.grpSelectFile.Controls.Add(this.label3);
            this.grpSelectFile.Controls.Add(this.lblSelectfile);
            this.grpSelectFile.Controls.Add(this.txtFileName);
            this.grpSelectFile.Controls.Add(this.btnFileBrowse);
            this.grpSelectFile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.grpSelectFile.Location = new System.Drawing.Point(5, 63);
            this.grpSelectFile.Name = "grpSelectFile";
            this.grpSelectFile.Size = new System.Drawing.Size(826, 70);
            this.grpSelectFile.TabIndex = 8;
            this.grpSelectFile.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 19);
            this.label3.TabIndex = 8;
            this.label3.Text = "Select File";
            // 
            // pnlModelCategory
            // 
            this.pnlModelCategory.BackColor = System.Drawing.SystemColors.Control;
            this.pnlModelCategory.Controls.Add(this.gvCategory);
            this.pnlModelCategory.Controls.Add(this.rdoBritish);
            this.pnlModelCategory.Controls.Add(this.rdoMetric);
            this.pnlModelCategory.Location = new System.Drawing.Point(8, 68);
            this.pnlModelCategory.Name = "pnlModelCategory";
            this.pnlModelCategory.Size = new System.Drawing.Size(827, 403);
            this.pnlModelCategory.TabIndex = 25;
            this.pnlModelCategory.TabStop = false;
            this.pnlModelCategory.Enter += new System.EventHandler(this.grpSelectFile_Enter);
            // 
            // gvCategory
            // 
            this.gvCategory.AllowUserToAddRows = false;
            this.gvCategory.AllowUserToDeleteRows = false;
            this.gvCategory.BackgroundColor = System.Drawing.SystemColors.Control;
            this.gvCategory.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.gvCategory.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.gvCategory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvCategory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.checkbox,
            this.Category,
            this.BuiltInCategory,
            this.CategoryValue});
            this.gvCategory.GridColor = System.Drawing.SystemColors.Control;
            this.gvCategory.Location = new System.Drawing.Point(7, 79);
            this.gvCategory.Name = "gvCategory";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvCategory.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.gvCategory.RowHeadersVisible = false;
            this.gvCategory.RowHeadersWidth = 51;
            this.gvCategory.RowTemplate.Height = 24;
            this.gvCategory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvCategory.Size = new System.Drawing.Size(801, 326);
            this.gvCategory.TabIndex = 5;
            this.gvCategory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvCategory_CellContentClick);
            // 
            // checkbox
            // 
            this.checkbox.HeaderText = "";
            this.checkbox.MinimumWidth = 6;
            this.checkbox.Name = "checkbox";
            this.checkbox.Width = 50;
            // 
            // Category
            // 
            this.Category.DataPropertyName = "Category";
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            this.Category.DefaultCellStyle = dataGridViewCellStyle6;
            this.Category.HeaderText = "Category";
            this.Category.MinimumWidth = 6;
            this.Category.Name = "Category";
            this.Category.Width = 380;
            // 
            // BuiltInCategory
            // 
            this.BuiltInCategory.DataPropertyName = "BuiltInCategory";
            this.BuiltInCategory.HeaderText = "Built In Category";
            this.BuiltInCategory.MinimumWidth = 6;
            this.BuiltInCategory.Name = "BuiltInCategory";
            this.BuiltInCategory.Width = 300;
            // 
            // CategoryValue
            // 
            this.CategoryValue.DataPropertyName = "CategoryValue";
            this.CategoryValue.HeaderText = "";
            this.CategoryValue.MinimumWidth = 6;
            this.CategoryValue.Name = "CategoryValue";
            this.CategoryValue.Visible = false;
            this.CategoryValue.Width = 125;
            // 
            // pnlRoom
            // 
            this.pnlRoom.Controls.Add(this.btnViewImage);
            this.pnlRoom.Controls.Add(this.dgRoomNames);
            this.pnlRoom.Controls.Add(this.label7);
            this.pnlRoom.Controls.Add(this.cmdRoomLevel);
            this.pnlRoom.Location = new System.Drawing.Point(5, 203);
            this.pnlRoom.Name = "pnlRoom";
            this.pnlRoom.Size = new System.Drawing.Size(826, 362);
            this.pnlRoom.TabIndex = 26;
            this.pnlRoom.Visible = false;
            // 
            // btnViewImage
            // 
            this.btnViewImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(33)))), ((int)(((byte)(30)))));
            this.btnViewImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnViewImage.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
            this.btnViewImage.FlatAppearance.BorderSize = 0;
            this.btnViewImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewImage.Font = new System.Drawing.Font("Arial", 8F);
            this.btnViewImage.ForeColor = System.Drawing.Color.White;
            this.btnViewImage.Location = new System.Drawing.Point(589, 14);
            this.btnViewImage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnViewImage.Name = "btnViewImage";
            this.btnViewImage.Size = new System.Drawing.Size(98, 29);
            this.btnViewImage.TabIndex = 5;
            this.btnViewImage.Text = "View Image";
            this.btnViewImage.UseVisualStyleBackColor = false;
            this.btnViewImage.Click += new System.EventHandler(this.btnViewImage_Click);
            // 
            // dgRoomNames
            // 
            this.dgRoomNames.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dgRoomNames.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgRoomNames.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgRoomNames.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgRoomNames.Location = new System.Drawing.Point(10, 55);
            this.dgRoomNames.Name = "dgRoomNames";
            this.dgRoomNames.RowHeadersVisible = false;
            this.dgRoomNames.RowHeadersWidth = 51;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgRoomNames.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.dgRoomNames.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgRoomNames.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(33)))), ((int)(((byte)(30)))));
            this.dgRoomNames.RowTemplate.Height = 30;
            this.dgRoomNames.Size = new System.Drawing.Size(807, 291);
            this.dgRoomNames.TabIndex = 12;
            this.dgRoomNames.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgRoomNames_DataError);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(13, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 19);
            this.label7.TabIndex = 9;
            this.label7.Text = "Room Level";
            // 
            // cmdRoomLevel
            // 
            this.cmdRoomLevel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cmdRoomLevel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdRoomLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmdRoomLevel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdRoomLevel.FormattingEnabled = true;
            this.cmdRoomLevel.Location = new System.Drawing.Point(155, 18);
            this.cmdRoomLevel.Name = "cmdRoomLevel";
            this.cmdRoomLevel.Size = new System.Drawing.Size(364, 24);
            this.cmdRoomLevel.Sorted = true;
            this.cmdRoomLevel.TabIndex = 8;
            this.cmdRoomLevel.SelectedIndexChanged += new System.EventHandler(this.cmdRoomLevel_SelectedIndexChanged);
            // 
            // pnlPreview
            // 
            this.pnlPreview.Controls.Add(this.btnPreview);
            this.pnlPreview.Location = new System.Drawing.Point(837, 3);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(815, 632);
            this.pnlPreview.TabIndex = 27;
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(33)))), ((int)(((byte)(30)))));
            this.btnPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreview.FlatAppearance.BorderSize = 0;
            this.btnPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(648, 584);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(102, 33);
            this.btnPreview.TabIndex = 8;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(836, 636);
            this.Controls.Add(this.pnlPreview);
            this.Controls.Add(this.pnlRoom);
            this.Controls.Add(this.grpExcelModel);
            this.Controls.Add(this.grpSelectFile);
            this.Controls.Add(this.grpErrorPanel);
            this.Controls.Add(this.grpButtonPanel);
            this.Controls.Add(this.grpSelectProject);
            this.Controls.Add(this.pnlModelCategory);
            this.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BOQ Extractor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Main_Paint);
            this.grpErrorPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdErrorgrid)).EndInit();
            this.grpButtonPanel.ResumeLayout(false);
            this.grpSelectProject.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.grpExcelModel.ResumeLayout(false);
            this.grpExcelModel.PerformLayout();
            this.grpSelectFile.ResumeLayout(false);
            this.grpSelectFile.PerformLayout();
            this.pnlModelCategory.ResumeLayout(false);
            this.pnlModelCategory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvCategory)).EndInit();
            this.pnlRoom.ResumeLayout(false);
            this.pnlRoom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgRoomNames)).EndInit();
            this.pnlPreview.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnFileBrowse;
        private System.Windows.Forms.GroupBox grpButtonPanel;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.GroupBox grpSelectProject;
        private System.Windows.Forms.ComboBox cmbProjects;
        private System.Windows.Forms.Label lblSelectProject;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.LinkLabel linkDownload;
        private System.Windows.Forms.Label lblSelectfile;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TextBox txtVersionLevel;
        private System.Windows.Forms.Label lblVersionLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox grpErrorPanel;
        private System.Windows.Forms.DataGridView grdErrorgrid;
        private System.Windows.Forms.RadioButton rdoModel;
        private System.Windows.Forms.RadioButton rdoExcel;
        private System.Windows.Forms.RadioButton rdoBritish;
        private System.Windows.Forms.RadioButton rdoMetric;
        private System.Windows.Forms.GroupBox grpExcelModel;
        private System.Windows.Forms.GroupBox grpSelectFile;
        private System.Windows.Forms.GroupBox pnlModelCategory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewTextBoxColumn SKU;
        private System.Windows.Forms.DataGridViewTextBoxColumn UOM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn error_log;
        private System.Windows.Forms.Panel pnlRoom;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmdRoomLevel;
        private System.Windows.Forms.DataGridView dgRoomNames;
        private System.Windows.Forms.Panel pnlPreview;
        private System.Windows.Forms.Button btnPreview;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.DataGridView gvCategory;
        private System.Windows.Forms.DataGridViewCheckBoxColumn checkbox;
        private System.Windows.Forms.DataGridViewTextBoxColumn Category;
        private System.Windows.Forms.DataGridViewTextBoxColumn BuiltInCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn CategoryValue;
        private System.Windows.Forms.Button btnViewImage;
    }
}