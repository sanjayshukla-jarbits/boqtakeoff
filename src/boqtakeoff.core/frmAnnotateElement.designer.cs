namespace boqtakeoff.core
{
    partial class frmAnnotateElement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAnnotateElement));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.pnlRoom = new System.Windows.Forms.Panel();
            this.dgRoomNames = new System.Windows.Forms.DataGridView();
            this.label7 = new System.Windows.Forms.Label();
            this.cmdRoomLevel = new System.Windows.Forms.ComboBox();
            this.btnPreview = new System.Windows.Forms.Button();
            this.pnlFamily = new System.Windows.Forms.Panel();
            this.rdoBritish = new System.Windows.Forms.RadioButton();
            this.rdoMetric = new System.Windows.Forms.RadioButton();
            this.dgViewFamily = new System.Windows.Forms.DataGridView();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlPreview = new System.Windows.Forms.Panel();
            this.pnlRoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgRoomNames)).BeginInit();
            this.pnlFamily.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgViewFamily)).BeginInit();
            this.panel5.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.pnlPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlRoom
            // 
            this.pnlRoom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlRoom.Controls.Add(this.dgRoomNames);
            this.pnlRoom.Controls.Add(this.label7);
            this.pnlRoom.Controls.Add(this.cmdRoomLevel);
            this.pnlRoom.Location = new System.Drawing.Point(7, 11);
            this.pnlRoom.Name = "pnlRoom";
            this.pnlRoom.Size = new System.Drawing.Size(747, 466);
            this.pnlRoom.TabIndex = 4;
            // 
            // dgRoomNames
            // 
            this.dgRoomNames.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dgRoomNames.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgRoomNames.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgRoomNames.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgRoomNames.Location = new System.Drawing.Point(3, 56);
            this.dgRoomNames.Name = "dgRoomNames";
            this.dgRoomNames.RowHeadersVisible = false;
            this.dgRoomNames.RowHeadersWidth = 51;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgRoomNames.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgRoomNames.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgRoomNames.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(33)))), ((int)(((byte)(30)))));
            this.dgRoomNames.RowTemplate.Height = 30;
            this.dgRoomNames.Size = new System.Drawing.Size(739, 405);
            this.dgRoomNames.TabIndex = 11;
            this.dgRoomNames.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgRoomNames_CellEndEdit);
            this.dgRoomNames.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgRoomNames_CellValueChanged);
            this.dgRoomNames.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgRoomNames_DataError);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(10, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(143, 19);
            this.label7.TabIndex = 7;
            this.label7.Text = "Select Room Level";
            // 
            // cmdRoomLevel
            // 
            this.cmdRoomLevel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cmdRoomLevel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdRoomLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmdRoomLevel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdRoomLevel.FormattingEnabled = true;
            this.cmdRoomLevel.Location = new System.Drawing.Point(170, 10);
            this.cmdRoomLevel.Name = "cmdRoomLevel";
            this.cmdRoomLevel.Size = new System.Drawing.Size(448, 30);
            this.cmdRoomLevel.Sorted = true;
            this.cmdRoomLevel.TabIndex = 6;
            this.cmdRoomLevel.SelectedIndexChanged += new System.EventHandler(this.cmdRoomLevel_SelectedIndexChanged);
            this.cmdRoomLevel.SelectedValueChanged += new System.EventHandler(this.cmdRoomLevel_SelectedValueChanged);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(33)))), ((int)(((byte)(30)))));
            this.btnPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreview.FlatAppearance.BorderSize = 0;
            this.btnPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreview.ForeColor = System.Drawing.Color.White;
            this.btnPreview.Location = new System.Drawing.Point(537, 482);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(102, 33);
            this.btnPreview.TabIndex = 8;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // pnlFamily
            // 
            this.pnlFamily.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlFamily.Controls.Add(this.rdoBritish);
            this.pnlFamily.Controls.Add(this.rdoMetric);
            this.pnlFamily.Controls.Add(this.dgViewFamily);
            this.pnlFamily.Location = new System.Drawing.Point(770, 11);
            this.pnlFamily.Name = "pnlFamily";
            this.pnlFamily.Size = new System.Drawing.Size(753, 478);
            this.pnlFamily.TabIndex = 5;
            this.pnlFamily.Visible = false;
            this.pnlFamily.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlFamily_Paint);
            // 
            // rdoBritish
            // 
            this.rdoBritish.AutoSize = true;
            this.rdoBritish.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoBritish.Font = new System.Drawing.Font("Arial", 9F);
            this.rdoBritish.Location = new System.Drawing.Point(155, 20);
            this.rdoBritish.Name = "rdoBritish";
            this.rdoBritish.Size = new System.Drawing.Size(70, 21);
            this.rdoBritish.TabIndex = 2;
            this.rdoBritish.Text = "British";
            this.rdoBritish.UseVisualStyleBackColor = true;
            this.rdoBritish.CheckedChanged += new System.EventHandler(this.rdoBritish_CheckedChanged);
            // 
            // rdoMetric
            // 
            this.rdoMetric.AutoSize = true;
            this.rdoMetric.Checked = true;
            this.rdoMetric.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdoMetric.Font = new System.Drawing.Font("Arial", 9F);
            this.rdoMetric.Location = new System.Drawing.Point(27, 20);
            this.rdoMetric.Name = "rdoMetric";
            this.rdoMetric.Size = new System.Drawing.Size(68, 21);
            this.rdoMetric.TabIndex = 1;
            this.rdoMetric.TabStop = true;
            this.rdoMetric.Text = "Metric";
            this.rdoMetric.UseVisualStyleBackColor = true;
            this.rdoMetric.CheckedChanged += new System.EventHandler(this.btnMetric_CheckedChanged);
            // 
            // dgViewFamily
            // 
            this.dgViewFamily.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dgViewFamily.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgViewFamily.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgViewFamily.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgViewFamily.Location = new System.Drawing.Point(8, 62);
            this.dgViewFamily.Name = "dgViewFamily";
            this.dgViewFamily.RowHeadersVisible = false;
            this.dgViewFamily.RowHeadersWidth = 51;
            this.dgViewFamily.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgViewFamily.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(33)))), ((int)(((byte)(30)))));
            this.dgViewFamily.RowTemplate.Height = 24;
            this.dgViewFamily.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgViewFamily.Size = new System.Drawing.Size(740, 407);
            this.dgViewFamily.TabIndex = 0;
            this.dgViewFamily.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgViewFamily_CellValueChanged);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel5.Controls.Add(this.btnClose);
            this.panel5.Controls.Add(this.btnNext);
            this.panel5.Controls.Add(this.btnBack);
            this.panel5.Location = new System.Drawing.Point(6, 479);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(747, 62);
            this.panel5.TabIndex = 6;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(33)))), ((int)(((byte)(30)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Location = new System.Drawing.Point(622, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(115, 40);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(33)))), ((int)(((byte)(30)))));
            this.btnNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnNext.Location = new System.Drawing.Point(503, 8);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(115, 40);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next>>";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(159)))), ((int)(((byte)(160)))));
            this.btnBack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(159)))), ((int)(((byte)(160)))));
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnBack.Location = new System.Drawing.Point(382, 8);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(115, 40);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "<<Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // pnlStatus
            // 
            this.pnlStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Location = new System.Drawing.Point(785, 12);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(743, 107);
            this.pnlStatus.TabIndex = 5;
            this.pnlStatus.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblStatus.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblStatus.Location = new System.Drawing.Point(20, 34);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(483, 35);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Annotation Completed Successfully";
            // 
            // pnlPreview
            // 
            this.pnlPreview.Controls.Add(this.btnPreview);
            this.pnlPreview.Location = new System.Drawing.Point(785, 14);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Size = new System.Drawing.Size(743, 543);
            this.pnlPreview.TabIndex = 7;
            // 
            // frmAnnotateElement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(1631, 543);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.pnlFamily);
            this.Controls.Add(this.pnlRoom);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.pnlPreview);
            this.Font = new System.Drawing.Font("Arial Narrow", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAnnotateElement";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Annotate Element(s)";
            this.Load += new System.EventHandler(this.frmAnnotateElement_Load);
            this.pnlRoom.ResumeLayout(false);
            this.pnlRoom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgRoomNames)).EndInit();
            this.pnlFamily.ResumeLayout(false);
            this.pnlFamily.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgViewFamily)).EndInit();
            this.panel5.ResumeLayout(false);
            this.pnlStatus.ResumeLayout(false);
            this.pnlStatus.PerformLayout();
            this.pnlPreview.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Panel pnlRoom;
        private System.Windows.Forms.Panel pnlFamily;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmdRoomLevel;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Panel pnlPreview;
        private System.Windows.Forms.DataGridView dgRoomNames;
        private System.Windows.Forms.DataGridView dgViewFamily;
        private System.Windows.Forms.RadioButton rdoBritish;
        private System.Windows.Forms.RadioButton rdoMetric;
        private System.Windows.Forms.Label lblStatus;
    }
}