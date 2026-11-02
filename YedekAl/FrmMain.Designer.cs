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
            this.mnuProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdNewProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdEditProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdDuplicateProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdDeleteProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdLogs = new System.Windows.Forms.ToolStripMenuItem();
            this.lblProfile = new System.Windows.Forms.Label();
            this.cboProfile = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdStop = new System.Windows.Forms.Button();
            this.cmdStart = new System.Windows.Forms.Button();
            this.lblServiceStatus = new System.Windows.Forms.Label();
            this.imgOff = new System.Windows.Forms.PictureBox();
            this.imgOn = new System.Windows.Forms.PictureBox();
            this.cmdBackup = new System.Windows.Forms.Button();
            this.cmdBackupAll = new System.Windows.Forms.Button();
            this.cmdRestore = new System.Windows.Forms.Button();
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
            this.mnuProfile,
            this.cmdSettings,
            this.cmdLogs});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(520, 24);
            this.mnuMain.TabIndex = 0;
            this.mnuMain.Text = "menuStrip1";
            //
            // mnuProfile
            //
            this.mnuProfile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdNewProfile,
            this.cmdEditProfile,
            this.cmdDuplicateProfile,
            this.cmdDeleteProfile});
            this.mnuProfile.Name = "mnuProfile";
            this.mnuProfile.Size = new System.Drawing.Size(45, 20);
            this.mnuProfile.Text = "&Profil";
            //
            // cmdNewProfile
            //
            this.cmdNewProfile.Name = "cmdNewProfile";
            this.cmdNewProfile.Size = new System.Drawing.Size(180, 22);
            this.cmdNewProfile.Text = "&Yeni profil...";
            this.cmdNewProfile.Click += new System.EventHandler(this.cmdNewProfile_Click);
            //
            // cmdEditProfile
            //
            this.cmdEditProfile.Name = "cmdEditProfile";
            this.cmdEditProfile.Size = new System.Drawing.Size(180, 22);
            this.cmdEditProfile.Text = "Profili &düzenle...";
            this.cmdEditProfile.Click += new System.EventHandler(this.cmdEditProfile_Click);
            //
            // cmdDuplicateProfile
            //
            this.cmdDuplicateProfile.Name = "cmdDuplicateProfile";
            this.cmdDuplicateProfile.Size = new System.Drawing.Size(180, 22);
            this.cmdDuplicateProfile.Text = "Profili &kopyala...";
            this.cmdDuplicateProfile.Click += new System.EventHandler(this.cmdDuplicateProfile_Click);
            //
            // cmdDeleteProfile
            //
            this.cmdDeleteProfile.Name = "cmdDeleteProfile";
            this.cmdDeleteProfile.Size = new System.Drawing.Size(180, 22);
            this.cmdDeleteProfile.Text = "Profili &sil";
            this.cmdDeleteProfile.Click += new System.EventHandler(this.cmdDeleteProfile_Click);
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
            // lblProfile
            //
            this.lblProfile.AutoSize = true;
            this.lblProfile.Location = new System.Drawing.Point(12, 34);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(33, 13);
            this.lblProfile.TabIndex = 1;
            this.lblProfile.Text = "Pr&ofil:";
            //
            // cboProfile
            //
            this.cboProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfile.Location = new System.Drawing.Point(60, 31);
            this.cboProfile.Name = "cboProfile";
            this.cboProfile.Size = new System.Drawing.Size(448, 21);
            this.cboProfile.TabIndex = 2;
            this.cboProfile.SelectedIndexChanged += new System.EventHandler(this.cboProfile_SelectedIndexChanged);
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.cmdStop);
            this.groupBox1.Controls.Add(this.cmdStart);
            this.groupBox1.Controls.Add(this.lblServiceStatus);
            this.groupBox1.Controls.Add(this.imgOff);
            this.groupBox1.Controls.Add(this.imgOn);
            this.groupBox1.Location = new System.Drawing.Point(12, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(496, 60);
            this.groupBox1.TabIndex = 3;
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
            this.cmdBackup.Location = new System.Drawing.Point(12, 126);
            this.cmdBackup.Name = "cmdBackup";
            this.cmdBackup.Size = new System.Drawing.Size(200, 30);
            this.cmdBackup.TabIndex = 4;
            this.cmdBackup.Text = "&Yedekle";
            this.cmdBackup.UseVisualStyleBackColor = true;
            this.cmdBackup.Click += new System.EventHandler(this.cmdBackup_Click);
            //
            // cmdBackupAll
            //
            this.cmdBackupAll.Location = new System.Drawing.Point(218, 126);
            this.cmdBackupAll.Name = "cmdBackupAll";
            this.cmdBackupAll.Size = new System.Drawing.Size(194, 30);
            this.cmdBackupAll.TabIndex = 5;
            this.cmdBackupAll.Text = "Tü&münü yedekle";
            this.cmdBackupAll.UseVisualStyleBackColor = true;
            this.cmdBackupAll.Click += new System.EventHandler(this.cmdBackupAll_Click);
            //
            // cmdCancel
            //
            this.cmdCancel.Enabled = false;
            this.cmdCancel.Location = new System.Drawing.Point(418, 126);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(90, 30);
            this.cmdCancel.TabIndex = 6;
            this.cmdCancel.Text = "İp&tal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            //
            // prgBackup
            //
            this.prgBackup.Location = new System.Drawing.Point(12, 162);
            this.prgBackup.Name = "prgBackup";
            this.prgBackup.Size = new System.Drawing.Size(496, 18);
            this.prgBackup.TabIndex = 7;
            //
            // txtLog
            //
            this.txtLog.Location = new System.Drawing.Point(12, 186);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(496, 200);
            this.txtLog.TabIndex = 8;
            //
            // lblHistory
            //
            this.lblHistory.AutoSize = true;
            this.lblHistory.Location = new System.Drawing.Point(12, 394);
            this.lblHistory.Name = "lblHistory";
            this.lblHistory.Size = new System.Drawing.Size(73, 13);
            this.lblHistory.TabIndex = 9;
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
            this.lvHistory.Location = new System.Drawing.Point(12, 411);
            this.lvHistory.MultiSelect = false;
            this.lvHistory.Name = "lvHistory";
            this.lvHistory.ShowItemToolTips = true;
            this.lvHistory.Size = new System.Drawing.Size(496, 168);
            this.lvHistory.TabIndex = 10;
            this.lvHistory.UseCompatibleStateImageBehavior = false;
            this.lvHistory.View = System.Windows.Forms.View.Details;
            this.lvHistory.SelectedIndexChanged += new System.EventHandler(this.lvHistory_SelectedIndexChanged);
            //
            // cmdRestore
            //
            this.cmdRestore.Enabled = false;
            this.cmdRestore.Location = new System.Drawing.Point(388, 387);
            this.cmdRestore.Name = "cmdRestore";
            this.cmdRestore.Size = new System.Drawing.Size(120, 22);
            this.cmdRestore.TabIndex = 11;
            this.cmdRestore.Text = "&Geri yükle...";
            this.cmdRestore.UseVisualStyleBackColor = true;
            this.cmdRestore.Click += new System.EventHandler(this.cmdRestore_Click);
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
            this.ClientSize = new System.Drawing.Size(520, 591);
            this.Controls.Add(this.lvHistory);
            this.Controls.Add(this.lblHistory);
            this.Controls.Add(this.cmdRestore);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.prgBackup);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdBackupAll);
            this.Controls.Add(this.cmdBackup);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cboProfile);
            this.Controls.Add(this.lblProfile);
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
        private System.Windows.Forms.ToolStripMenuItem mnuProfile;
        private System.Windows.Forms.ToolStripMenuItem cmdNewProfile;
        private System.Windows.Forms.ToolStripMenuItem cmdEditProfile;
        private System.Windows.Forms.ToolStripMenuItem cmdDuplicateProfile;
        private System.Windows.Forms.ToolStripMenuItem cmdDeleteProfile;
        private System.Windows.Forms.ToolStripMenuItem cmdSettings;
        private System.Windows.Forms.ToolStripMenuItem cmdLogs;
        private System.Windows.Forms.Label lblProfile;
        private System.Windows.Forms.ComboBox cboProfile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox imgOn;
        private System.Windows.Forms.Button cmdStop;
        private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.Label lblServiceStatus;
        private System.Windows.Forms.PictureBox imgOff;
        private System.Windows.Forms.Button cmdBackup;
        private System.Windows.Forms.Button cmdBackupAll;
        private System.Windows.Forms.Button cmdRestore;
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
