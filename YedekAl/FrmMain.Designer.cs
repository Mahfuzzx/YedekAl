namespace YedekAl
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.cmdSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdLogs = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdStop = new System.Windows.Forms.Button();
            this.cmdStart = new System.Windows.Forms.Button();
            this.lblServiceStatus = new System.Windows.Forms.Label();
            this.imgOff = new System.Windows.Forms.PictureBox();
            this.imgOn = new System.Windows.Forms.PictureBox();
            this.cmdBackup = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.prgBackup = new System.Windows.Forms.ProgressBar();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblHistory = new System.Windows.Forms.Label();
            this.lvHistory = new System.Windows.Forms.ListView();
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDuration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tmrSeek = new System.Windows.Forms.Timer(this.components);
            this.mnuMain.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgOff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgOn)).BeginInit();
            this.SuspendLayout();
            //
            // mnuMain
            //
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdSettings,
            this.cmdLogs});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(520, 24);
            this.mnuMain.TabIndex = 0;
            this.mnuMain.Text = "menuStrip1";
            //
            // cmdSettings
            //
            this.cmdSettings.Name = "cmdSettings";
            this.cmdSettings.Size = new System.Drawing.Size(54, 20);
            this.cmdSettings.Text = "&Ayarlar";
            this.cmdSettings.Click += new System.EventHandler(this.cmdSettings_Click);
            //
            // cmdLogs
            //
            this.cmdLogs.Name = "cmdLogs";
            this.cmdLogs.Size = new System.Drawing.Size(53, 20);
            this.cmdLogs.Text = "&Loglar";
            this.cmdLogs.Click += new System.EventHandler(this.cmdLogs_Click);
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.cmdStop);
            this.groupBox1.Controls.Add(this.cmdStart);
            this.groupBox1.Controls.Add(this.lblServiceStatus);
            this.groupBox1.Controls.Add(this.imgOff);
            this.groupBox1.Controls.Add(this.imgOn);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(496, 60);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Veritabanı";
            //
            // cmdStop
            //
            this.cmdStop.Location = new System.Drawing.Point(344, 22);
            this.cmdStop.Name = "cmdStop";
            this.cmdStop.Size = new System.Drawing.Size(70, 25);
            this.cmdStop.TabIndex = 2;
            this.cmdStop.Text = "&Durdur";
            this.cmdStop.UseVisualStyleBackColor = true;
            this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
            //
            // cmdStart
            //
            this.cmdStart.Location = new System.Drawing.Point(420, 22);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(70, 25);
            this.cmdStart.TabIndex = 3;
            this.cmdStart.Text = "Ça&lıştır";
            this.cmdStart.UseVisualStyleBackColor = true;
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            //
            // lblServiceStatus
            //
            this.lblServiceStatus.AutoSize = true;
            this.lblServiceStatus.Location = new System.Drawing.Point(44, 28);
            this.lblServiceStatus.Name = "lblServiceStatus";
            this.lblServiceStatus.Size = new System.Drawing.Size(58, 13);
            this.lblServiceStatus.TabIndex = 1;
            this.lblServiceStatus.Text = "Son durum";
            //
            // imgOff
            //
            this.imgOff.Image = ((System.Drawing.Image)(resources.GetObject("imgOff.Image")));
            this.imgOff.Location = new System.Drawing.Point(6, 19);
            this.imgOff.Name = "imgOff";
            this.imgOff.Size = new System.Drawing.Size(32, 32);
            this.imgOff.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgOff.TabIndex = 1;
            this.imgOff.TabStop = false;
            //
            // imgOn
            //
            this.imgOn.Image = ((System.Drawing.Image)(resources.GetObject("imgOn.Image")));
            this.imgOn.Location = new System.Drawing.Point(6, 19);
            this.imgOn.Name = "imgOn";
            this.imgOn.Size = new System.Drawing.Size(32, 32);
            this.imgOn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgOn.TabIndex = 0;
            this.imgOn.TabStop = false;
            //
            // cmdBackup
            //
            this.cmdBackup.Location = new System.Drawing.Point(12, 95);
            this.cmdBackup.Name = "cmdBackup";
            this.cmdBackup.Size = new System.Drawing.Size(400, 30);
            this.cmdBackup.TabIndex = 2;
            this.cmdBackup.Text = "&Yedekle";
            this.cmdBackup.UseVisualStyleBackColor = true;
            this.cmdBackup.Click += new System.EventHandler(this.cmdBackup_Click);
            //
            // cmdCancel
            //
            this.cmdCancel.Enabled = false;
            this.cmdCancel.Location = new System.Drawing.Point(418, 95);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(90, 30);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "İp&tal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            //
            // prgBackup
            //
            this.prgBackup.Location = new System.Drawing.Point(12, 131);
            this.prgBackup.Name = "prgBackup";
            this.prgBackup.Size = new System.Drawing.Size(496, 18);
            this.prgBackup.TabIndex = 4;
            //
            // txtLog
            //
            this.txtLog.Location = new System.Drawing.Point(12, 155);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(496, 200);
            this.txtLog.TabIndex = 5;
            //
            // lblHistory
            //
            this.lblHistory.AutoSize = true;
            this.lblHistory.Location = new System.Drawing.Point(12, 363);
            this.lblHistory.Name = "lblHistory";
            this.lblHistory.Size = new System.Drawing.Size(73, 13);
            this.lblHistory.TabIndex = 6;
            this.lblHistory.Text = "Son yedekler:";
            //
            // lvHistory
            //
            this.lvHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDate,
            this.colDuration,
            this.colSize,
            this.colStatus,
            this.colFile});
            this.lvHistory.FullRowSelect = true;
            this.lvHistory.HideSelection = false;
            this.lvHistory.Location = new System.Drawing.Point(12, 380);
            this.lvHistory.MultiSelect = false;
            this.lvHistory.Name = "lvHistory";
            this.lvHistory.ShowItemToolTips = true;
            this.lvHistory.Size = new System.Drawing.Size(496, 168);
            this.lvHistory.TabIndex = 7;
            this.lvHistory.UseCompatibleStateImageBehavior = false;
            this.lvHistory.View = System.Windows.Forms.View.Details;
            //
            // colDate
            //
            this.colDate.Text = "Tarih";
            this.colDate.Width = 105;
            //
            // colDuration
            //
            this.colDuration.Text = "Süre";
            this.colDuration.Width = 55;
            //
            // colSize
            //
            this.colSize.Text = "Boyut";
            this.colSize.Width = 70;
            //
            // colStatus
            //
            this.colStatus.Text = "Durum";
            this.colStatus.Width = 75;
            //
            // colFile
            //
            this.colFile.Text = "Dosya / Açıklama";
            this.colFile.Width = 170;
            //
            // tmrSeek
            //
            this.tmrSeek.Enabled = true;
            this.tmrSeek.Interval = 1000;
            this.tmrSeek.Tick += new System.EventHandler(this.tmrSeek_Tick);
            //
            // FrmMain
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 560);
            this.Controls.Add(this.lvHistory);
            this.Controls.Add(this.lblHistory);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.prgBackup);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdBackup);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mnuMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuMain;
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "YedekAl";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgOff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgOn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem cmdSettings;
        private System.Windows.Forms.ToolStripMenuItem cmdLogs;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox imgOn;
        private System.Windows.Forms.Button cmdStop;
        private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.Label lblServiceStatus;
        private System.Windows.Forms.PictureBox imgOff;
        private System.Windows.Forms.Button cmdBackup;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.ProgressBar prgBackup;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblHistory;
        private System.Windows.Forms.ListView lvHistory;
        private System.Windows.Forms.ColumnHeader colDate;
        private System.Windows.Forms.ColumnHeader colDuration;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.ColumnHeader colStatus;
        private System.Windows.Forms.ColumnHeader colFile;
        private System.Windows.Forms.Timer tmrSeek;
    }
}
