namespace YedekAl
{
    partial class FrmProfile
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
            this.grpDatabase = new System.Windows.Forms.GroupBox();
            this.lblProfileName = new System.Windows.Forms.Label();
            this.txtProfileName = new System.Windows.Forms.TextBox();
            this.chkIncludeInSchedule = new System.Windows.Forms.CheckBox();
            this.lblServiceName = new System.Windows.Forms.Label();
            this.txtServiceName = new System.Windows.Forms.TextBox();
            this.lblDatabaseDir = new System.Windows.Forms.Label();
            this.txtDatabaseDir = new System.Windows.Forms.TextBox();
            this.cmdBrowseDatabase = new System.Windows.Forms.Button();
            this.grpBackup = new System.Windows.Forms.GroupBox();
            this.lblBackupDir = new System.Windows.Forms.Label();
            this.txtBackupDir = new System.Windows.Forms.TextBox();
            this.cmdBrowseBackup = new System.Windows.Forms.Button();
            this.lblNameTemplate = new System.Windows.Forms.Label();
            this.txtNameTemplate = new System.Windows.Forms.TextBox();
            this.lblNameHint = new System.Windows.Forms.Label();
            this.chkCompress = new System.Windows.Forms.CheckBox();
            this.lblFormat = new System.Windows.Forms.Label();
            this.cboFormat = new System.Windows.Forms.ComboBox();
            this.lblStrength = new System.Windows.Forms.Label();
            this.cboStrength = new System.Windows.Forms.ComboBox();
            this.optCompress = new System.Windows.Forms.RadioButton();
            this.optCopy = new System.Windows.Forms.RadioButton();
            this.chkVerify = new System.Windows.Forms.CheckBox();
            this.chkEncrypt = new System.Windows.Forms.CheckBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPasswordConfirm = new System.Windows.Forms.Label();
            this.txtPasswordConfirm = new System.Windows.Forms.TextBox();
            this.lblPasswordNote = new System.Windows.Forms.Label();
            this.lblExePath = new System.Windows.Forms.Label();
            this.txtExePath = new System.Windows.Forms.TextBox();
            this.cmdBrowseExe = new System.Windows.Forms.Button();
            this.lblExeStatus = new System.Windows.Forms.Label();
            this.grpRetention = new System.Windows.Forms.GroupBox();
            this.lblKeepCount = new System.Windows.Forms.Label();
            this.numKeepCount = new System.Windows.Forms.NumericUpDown();
            this.lblKeepCountHint = new System.Windows.Forms.Label();
            this.lblKeepDays = new System.Windows.Forms.Label();
            this.numKeepDays = new System.Windows.Forms.NumericUpDown();
            this.lblKeepDaysHint = new System.Windows.Forms.Label();
            this.lblRetentionNote = new System.Windows.Forms.Label();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.grpDatabase.SuspendLayout();
            this.grpBackup.SuspendLayout();
            this.grpRetention.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKeepCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeepDays)).BeginInit();
            this.SuspendLayout();
            //
            // grpDatabase
            //
            this.grpDatabase.Controls.Add(this.lblProfileName);
            this.grpDatabase.Controls.Add(this.txtProfileName);
            this.grpDatabase.Controls.Add(this.chkIncludeInSchedule);
            this.grpDatabase.Controls.Add(this.lblServiceName);
            this.grpDatabase.Controls.Add(this.txtServiceName);
            this.grpDatabase.Controls.Add(this.lblDatabaseDir);
            this.grpDatabase.Controls.Add(this.txtDatabaseDir);
            this.grpDatabase.Controls.Add(this.cmdBrowseDatabase);
            this.grpDatabase.Location = new System.Drawing.Point(12, 12);
            this.grpDatabase.Name = "grpDatabase";
            this.grpDatabase.Size = new System.Drawing.Size(536, 110);
            this.grpDatabase.TabIndex = 0;
            this.grpDatabase.TabStop = false;
            this.grpDatabase.Text = "Profil";
            //
            // lblProfileName
            //
            this.lblProfileName.AutoSize = true;
            this.lblProfileName.Location = new System.Drawing.Point(12, 25);
            this.lblProfileName.Name = "lblProfileName";
            this.lblProfileName.Size = new System.Drawing.Size(57, 13);
            this.lblProfileName.TabIndex = 0;
            this.lblProfileName.Text = "Profil &adı:";
            //
            // txtProfileName
            //
            this.txtProfileName.Location = new System.Drawing.Point(140, 22);
            this.txtProfileName.Name = "txtProfileName";
            this.txtProfileName.Size = new System.Drawing.Size(200, 20);
            this.txtProfileName.TabIndex = 1;
            //
            // chkIncludeInSchedule
            //
            this.chkIncludeInSchedule.AutoSize = true;
            this.chkIncludeInSchedule.Location = new System.Drawing.Point(352, 24);
            this.chkIncludeInSchedule.Name = "chkIncludeInSchedule";
            this.chkIncludeInSchedule.Size = new System.Drawing.Size(160, 17);
            this.chkIncludeInSchedule.TabIndex = 2;
            this.chkIncludeInSchedule.Text = "Zamanlanmış yedeklemeye dahil";
            this.chkIncludeInSchedule.UseVisualStyleBackColor = true;
            //
            // lblServiceName
            //
            this.lblServiceName.AutoSize = true;
            this.lblServiceName.Location = new System.Drawing.Point(12, 53);
            this.lblServiceName.Name = "lblServiceName";
            this.lblServiceName.Size = new System.Drawing.Size(59, 13);
            this.lblServiceName.TabIndex = 3;
            this.lblServiceName.Text = "&Hizmet adı:";
            //
            // txtServiceName
            //
            this.txtServiceName.Location = new System.Drawing.Point(140, 50);
            this.txtServiceName.Name = "txtServiceName";
            this.txtServiceName.Size = new System.Drawing.Size(382, 20);
            this.txtServiceName.TabIndex = 4;
            //
            // lblDatabaseDir
            //
            this.lblDatabaseDir.AutoSize = true;
            this.lblDatabaseDir.Location = new System.Drawing.Point(12, 81);
            this.lblDatabaseDir.Name = "lblDatabaseDir";
            this.lblDatabaseDir.Size = new System.Drawing.Size(98, 13);
            this.lblDatabaseDir.TabIndex = 5;
            this.lblDatabaseDir.Text = "&Veritabanı konumu:";
            //
            // txtDatabaseDir
            //
            this.txtDatabaseDir.Location = new System.Drawing.Point(140, 78);
            this.txtDatabaseDir.Name = "txtDatabaseDir";
            this.txtDatabaseDir.Size = new System.Drawing.Size(344, 20);
            this.txtDatabaseDir.TabIndex = 6;
            //
            // cmdBrowseDatabase
            //
            this.cmdBrowseDatabase.Location = new System.Drawing.Point(490, 77);
            this.cmdBrowseDatabase.Name = "cmdBrowseDatabase";
            this.cmdBrowseDatabase.Size = new System.Drawing.Size(32, 22);
            this.cmdBrowseDatabase.TabIndex = 7;
            this.cmdBrowseDatabase.Text = "...";
            this.cmdBrowseDatabase.UseVisualStyleBackColor = true;
            this.cmdBrowseDatabase.Click += new System.EventHandler(this.cmdBrowseDatabase_Click);
            //
            // grpBackup
            //
            this.grpBackup.Controls.Add(this.lblBackupDir);
            this.grpBackup.Controls.Add(this.txtBackupDir);
            this.grpBackup.Controls.Add(this.cmdBrowseBackup);
            this.grpBackup.Controls.Add(this.lblNameTemplate);
            this.grpBackup.Controls.Add(this.txtNameTemplate);
            this.grpBackup.Controls.Add(this.lblNameHint);
            this.grpBackup.Controls.Add(this.chkCompress);
            this.grpBackup.Controls.Add(this.lblFormat);
            this.grpBackup.Controls.Add(this.cboFormat);
            this.grpBackup.Controls.Add(this.lblStrength);
            this.grpBackup.Controls.Add(this.cboStrength);
            this.grpBackup.Controls.Add(this.optCompress);
            this.grpBackup.Controls.Add(this.optCopy);
            this.grpBackup.Controls.Add(this.chkVerify);
            this.grpBackup.Controls.Add(this.chkEncrypt);
            this.grpBackup.Controls.Add(this.lblPasswordNote);
            this.grpBackup.Controls.Add(this.lblPassword);
            this.grpBackup.Controls.Add(this.txtPassword);
            this.grpBackup.Controls.Add(this.lblPasswordConfirm);
            this.grpBackup.Controls.Add(this.txtPasswordConfirm);
            this.grpBackup.Controls.Add(this.lblExePath);
            this.grpBackup.Controls.Add(this.txtExePath);
            this.grpBackup.Controls.Add(this.cmdBrowseExe);
            this.grpBackup.Controls.Add(this.lblExeStatus);
            this.grpBackup.Location = new System.Drawing.Point(12, 128);
            this.grpBackup.Name = "grpBackup";
            this.grpBackup.Size = new System.Drawing.Size(536, 320);
            this.grpBackup.TabIndex = 1;
            this.grpBackup.TabStop = false;
            this.grpBackup.Text = "Yedek";
            //
            // lblBackupDir
            //
            this.lblBackupDir.AutoSize = true;
            this.lblBackupDir.Location = new System.Drawing.Point(12, 25);
            this.lblBackupDir.Name = "lblBackupDir";
            this.lblBackupDir.Size = new System.Drawing.Size(104, 13);
            this.lblBackupDir.TabIndex = 0;
            this.lblBackupDir.Text = "&Yedekleme konumu:";
            //
            // txtBackupDir
            //
            this.txtBackupDir.Location = new System.Drawing.Point(140, 22);
            this.txtBackupDir.Name = "txtBackupDir";
            this.txtBackupDir.Size = new System.Drawing.Size(344, 20);
            this.txtBackupDir.TabIndex = 1;
            //
            // cmdBrowseBackup
            //
            this.cmdBrowseBackup.Location = new System.Drawing.Point(490, 21);
            this.cmdBrowseBackup.Name = "cmdBrowseBackup";
            this.cmdBrowseBackup.Size = new System.Drawing.Size(32, 22);
            this.cmdBrowseBackup.TabIndex = 2;
            this.cmdBrowseBackup.Text = "...";
            this.cmdBrowseBackup.UseVisualStyleBackColor = true;
            this.cmdBrowseBackup.Click += new System.EventHandler(this.cmdBrowseBackup_Click);
            //
            // lblNameTemplate
            //
            this.lblNameTemplate.AutoSize = true;
            this.lblNameTemplate.Location = new System.Drawing.Point(12, 53);
            this.lblNameTemplate.Name = "lblNameTemplate";
            this.lblNameTemplate.Size = new System.Drawing.Size(57, 13);
            this.lblNameTemplate.TabIndex = 3;
            this.lblNameTemplate.Text = "Arşi&v adı:";
            //
            // txtNameTemplate
            //
            this.txtNameTemplate.Location = new System.Drawing.Point(140, 50);
            this.txtNameTemplate.Name = "txtNameTemplate";
            this.txtNameTemplate.Size = new System.Drawing.Size(382, 20);
            this.txtNameTemplate.TabIndex = 4;
            //
            // lblNameHint
            //
            this.lblNameHint.AutoSize = true;
            this.lblNameHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblNameHint.Location = new System.Drawing.Point(137, 73);
            this.lblNameHint.Name = "lblNameHint";
            this.lblNameHint.Size = new System.Drawing.Size(330, 13);
            this.lblNameHint.TabIndex = 5;
            this.lblNameHint.Text = "{0:yyyyMMdd_HHmmss} yerine yedeğin tarihi ve saati yazılır.";
            //
            // chkCompress
            //
            this.chkCompress.AutoSize = true;
            this.chkCompress.Location = new System.Drawing.Point(15, 96);
            this.chkCompress.Name = "chkCompress";
            this.chkCompress.Size = new System.Drawing.Size(140, 17);
            this.chkCompress.TabIndex = 6;
            this.chkCompress.Text = "Veritabanını &sıkıştır";
            this.chkCompress.UseVisualStyleBackColor = true;
            this.chkCompress.CheckedChanged += new System.EventHandler(this.chkCompress_CheckedChanged);
            //
            // lblFormat
            //
            this.lblFormat.AutoSize = true;
            this.lblFormat.Location = new System.Drawing.Point(33, 124);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(36, 13);
            this.lblFormat.TabIndex = 7;
            this.lblFormat.Text = "&Biçim:";
            //
            // cboFormat
            //
            this.cboFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFormat.Location = new System.Drawing.Point(140, 121);
            this.cboFormat.Name = "cboFormat";
            this.cboFormat.Size = new System.Drawing.Size(120, 21);
            this.cboFormat.TabIndex = 8;
            this.cboFormat.SelectedIndexChanged += new System.EventHandler(this.cboFormat_SelectedIndexChanged);
            //
            // lblStrength
            //
            this.lblStrength.AutoSize = true;
            this.lblStrength.Location = new System.Drawing.Point(286, 124);
            this.lblStrength.Name = "lblStrength";
            this.lblStrength.Size = new System.Drawing.Size(42, 13);
            this.lblStrength.TabIndex = 9;
            this.lblStrength.Text = "Se&viye:";
            //
            // cboStrength
            //
            this.cboStrength.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStrength.Location = new System.Drawing.Point(340, 121);
            this.cboStrength.Name = "cboStrength";
            this.cboStrength.Size = new System.Drawing.Size(120, 21);
            this.cboStrength.TabIndex = 10;
            //
            // optCompress
            //
            this.optCompress.AutoSize = true;
            this.optCompress.Location = new System.Drawing.Point(36, 152);
            this.optCompress.Name = "optCompress";
            this.optCompress.Size = new System.Drawing.Size(201, 17);
            this.optCompress.TabIndex = 11;
            this.optCompress.TabStop = true;
            this.optCompress.Text = "Veritabanını doğrudan hedefe sıkıştır";
            this.optCompress.UseVisualStyleBackColor = true;
            //
            // optCopy
            //
            this.optCopy.AutoSize = true;
            this.optCopy.Location = new System.Drawing.Point(36, 174);
            this.optCopy.Name = "optCopy";
            this.optCopy.Size = new System.Drawing.Size(390, 17);
            this.optCopy.TabIndex = 12;
            this.optCopy.TabStop = true;
            this.optCopy.Text = "Önce kopyala, sonra sıkıştır (hizmet daha kısa süre durur, iki kat yer gerekir)";
            this.optCopy.UseVisualStyleBackColor = true;
            //
            // chkVerify
            //
            this.chkVerify.AutoSize = true;
            this.chkVerify.Location = new System.Drawing.Point(36, 197);
            this.chkVerify.Name = "chkVerify";
            this.chkVerify.Size = new System.Drawing.Size(205, 17);
            this.chkVerify.TabIndex = 13;
            this.chkVerify.Text = "Sıkıştırdıktan sonra arşivi &doğrula";
            this.chkVerify.UseVisualStyleBackColor = true;
            //
            // chkEncrypt
            //
            this.chkEncrypt.AutoSize = true;
            this.chkEncrypt.Location = new System.Drawing.Point(36, 219);
            this.chkEncrypt.Name = "chkEncrypt";
            this.chkEncrypt.Size = new System.Drawing.Size(250, 17);
            this.chkEncrypt.TabIndex = 14;
            this.chkEncrypt.Text = "Arşivi parolayla ş&ifrele (yalnızca 7z ve RAR)";
            this.chkEncrypt.UseVisualStyleBackColor = true;
            this.chkEncrypt.CheckedChanged += new System.EventHandler(this.chkEncrypt_CheckedChanged);
            //
            // lblPassword
            //
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(54, 245);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(40, 13);
            this.lblPassword.TabIndex = 15;
            this.lblPassword.Text = "Parol&a:";
            //
            // txtPassword
            //
            this.txtPassword.Location = new System.Drawing.Point(140, 242);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(150, 20);
            this.txtPassword.TabIndex = 16;
            this.txtPassword.UseSystemPasswordChar = true;
            //
            // lblPasswordConfirm
            //
            this.lblPasswordConfirm.AutoSize = true;
            this.lblPasswordConfirm.Location = new System.Drawing.Point(300, 245);
            this.lblPasswordConfirm.Name = "lblPasswordConfirm";
            this.lblPasswordConfirm.Size = new System.Drawing.Size(42, 13);
            this.lblPasswordConfirm.TabIndex = 17;
            this.lblPasswordConfirm.Text = "Te&krar:";
            //
            // txtPasswordConfirm
            //
            this.txtPasswordConfirm.Location = new System.Drawing.Point(350, 242);
            this.txtPasswordConfirm.Name = "txtPasswordConfirm";
            this.txtPasswordConfirm.Size = new System.Drawing.Size(172, 20);
            this.txtPasswordConfirm.TabIndex = 18;
            this.txtPasswordConfirm.UseSystemPasswordChar = true;
            //
            // lblPasswordNote
            //
            this.lblPasswordNote.AutoEllipsis = true;
            this.lblPasswordNote.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPasswordNote.Location = new System.Drawing.Point(292, 220);
            this.lblPasswordNote.Name = "lblPasswordNote";
            this.lblPasswordNote.Size = new System.Drawing.Size(230, 13);
            this.lblPasswordNote.TabIndex = 19;
            this.lblPasswordNote.Text = "Boş bırakılırsa kayıtlı parola korunur.";
            //
            // lblExePath
            //
            this.lblExePath.AutoSize = true;
            this.lblExePath.Location = new System.Drawing.Point(33, 273);
            this.lblExePath.Name = "lblExePath";
            this.lblExePath.Size = new System.Drawing.Size(73, 13);
            this.lblExePath.TabIndex = 20;
            this.lblExePath.Text = "&Program yolu:";
            //
            // txtExePath
            //
            this.txtExePath.Location = new System.Drawing.Point(140, 270);
            this.txtExePath.Name = "txtExePath";
            this.txtExePath.Size = new System.Drawing.Size(344, 20);
            this.txtExePath.TabIndex = 21;
            this.txtExePath.TextChanged += new System.EventHandler(this.txtExePath_TextChanged);
            //
            // cmdBrowseExe
            //
            this.cmdBrowseExe.Location = new System.Drawing.Point(490, 269);
            this.cmdBrowseExe.Name = "cmdBrowseExe";
            this.cmdBrowseExe.Size = new System.Drawing.Size(32, 22);
            this.cmdBrowseExe.TabIndex = 22;
            this.cmdBrowseExe.Text = "...";
            this.cmdBrowseExe.UseVisualStyleBackColor = true;
            this.cmdBrowseExe.Click += new System.EventHandler(this.cmdBrowseExe_Click);
            //
            // lblExeStatus
            //
            this.lblExeStatus.AutoEllipsis = true;
            this.lblExeStatus.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblExeStatus.Location = new System.Drawing.Point(137, 294);
            this.lblExeStatus.Name = "lblExeStatus";
            this.lblExeStatus.Size = new System.Drawing.Size(385, 15);
            this.lblExeStatus.TabIndex = 23;
            //
            // grpRetention
            //
            this.grpRetention.Controls.Add(this.lblKeepCount);
            this.grpRetention.Controls.Add(this.numKeepCount);
            this.grpRetention.Controls.Add(this.lblKeepCountHint);
            this.grpRetention.Controls.Add(this.lblKeepDays);
            this.grpRetention.Controls.Add(this.numKeepDays);
            this.grpRetention.Controls.Add(this.lblKeepDaysHint);
            this.grpRetention.Controls.Add(this.lblRetentionNote);
            this.grpRetention.Location = new System.Drawing.Point(12, 454);
            this.grpRetention.Name = "grpRetention";
            this.grpRetention.Size = new System.Drawing.Size(536, 96);
            this.grpRetention.TabIndex = 2;
            this.grpRetention.TabStop = false;
            this.grpRetention.Text = "Eski yedekler";
            //
            // lblKeepCount
            //
            this.lblKeepCount.AutoSize = true;
            this.lblKeepCount.Location = new System.Drawing.Point(12, 24);
            this.lblKeepCount.Name = "lblKeepCount";
            this.lblKeepCount.Size = new System.Drawing.Size(96, 13);
            this.lblKeepCount.TabIndex = 0;
            this.lblKeepCount.Text = "Saklanacak &yedek:";
            //
            // numKeepCount
            //
            this.numKeepCount.Location = new System.Drawing.Point(140, 22);
            this.numKeepCount.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numKeepCount.Name = "numKeepCount";
            this.numKeepCount.Size = new System.Drawing.Size(60, 20);
            this.numKeepCount.TabIndex = 1;
            //
            // lblKeepCountHint
            //
            this.lblKeepCountHint.AutoSize = true;
            this.lblKeepCountHint.Location = new System.Drawing.Point(206, 24);
            this.lblKeepCountHint.Name = "lblKeepCountHint";
            this.lblKeepCountHint.Size = new System.Drawing.Size(100, 13);
            this.lblKeepCountHint.TabIndex = 2;
            this.lblKeepCountHint.Text = "adet (0 = sınırsız)";
            //
            // lblKeepDays
            //
            this.lblKeepDays.AutoSize = true;
            this.lblKeepDays.Location = new System.Drawing.Point(12, 50);
            this.lblKeepDays.Name = "lblKeepDays";
            this.lblKeepDays.Size = new System.Drawing.Size(72, 13);
            this.lblKeepDays.TabIndex = 3;
            this.lblKeepDays.Text = "En fazla ya&ş:";
            //
            // numKeepDays
            //
            this.numKeepDays.Location = new System.Drawing.Point(140, 48);
            this.numKeepDays.Maximum = new decimal(new int[] {
            3650,
            0,
            0,
            0});
            this.numKeepDays.Name = "numKeepDays";
            this.numKeepDays.Size = new System.Drawing.Size(60, 20);
            this.numKeepDays.TabIndex = 4;
            //
            // lblKeepDaysHint
            //
            this.lblKeepDaysHint.AutoSize = true;
            this.lblKeepDaysHint.Location = new System.Drawing.Point(206, 50);
            this.lblKeepDaysHint.Name = "lblKeepDaysHint";
            this.lblKeepDaysHint.Size = new System.Drawing.Size(95, 13);
            this.lblKeepDaysHint.TabIndex = 5;
            this.lblKeepDaysHint.Text = "gün (0 = sınırsız)";
            //
            // lblRetentionNote
            //
            this.lblRetentionNote.AutoSize = true;
            this.lblRetentionNote.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblRetentionNote.Location = new System.Drawing.Point(12, 74);
            this.lblRetentionNote.Name = "lblRetentionNote";
            this.lblRetentionNote.Size = new System.Drawing.Size(380, 13);
            this.lblRetentionNote.TabIndex = 6;
            this.lblRetentionNote.Text = "Yalnızca YedekAl\'ın bu klasöre aldığı yedekler silinir. Son yedek her zaman korunur.";
            //
            // cmdOK
            //
            this.cmdOK.Location = new System.Drawing.Point(382, 562);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(80, 25);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "Kaydet";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            //
            // cmdCancel
            //
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(468, 562);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(80, 25);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Text = "Vazgeç";
            this.cmdCancel.UseVisualStyleBackColor = true;
            //
            // FrmProfile
            //
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(560, 599);
            this.Controls.Add(this.grpDatabase);
            this.Controls.Add(this.grpBackup);
            this.Controls.Add(this.grpRetention);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmProfile";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "YedekAl - Profil";
            this.grpDatabase.ResumeLayout(false);
            this.grpDatabase.PerformLayout();
            this.grpBackup.ResumeLayout(false);
            this.grpBackup.PerformLayout();
            this.grpRetention.ResumeLayout(false);
            this.grpRetention.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKeepCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeepDays)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDatabase;
        private System.Windows.Forms.Label lblProfileName;
        private System.Windows.Forms.TextBox txtProfileName;
        private System.Windows.Forms.CheckBox chkIncludeInSchedule;
        private System.Windows.Forms.Label lblServiceName;
        private System.Windows.Forms.TextBox txtServiceName;
        private System.Windows.Forms.Label lblDatabaseDir;
        private System.Windows.Forms.TextBox txtDatabaseDir;
        private System.Windows.Forms.Button cmdBrowseDatabase;
        private System.Windows.Forms.GroupBox grpBackup;
        private System.Windows.Forms.Label lblBackupDir;
        private System.Windows.Forms.TextBox txtBackupDir;
        private System.Windows.Forms.Button cmdBrowseBackup;
        private System.Windows.Forms.Label lblNameTemplate;
        private System.Windows.Forms.TextBox txtNameTemplate;
        private System.Windows.Forms.Label lblNameHint;
        private System.Windows.Forms.CheckBox chkCompress;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.ComboBox cboFormat;
        private System.Windows.Forms.Label lblStrength;
        private System.Windows.Forms.ComboBox cboStrength;
        private System.Windows.Forms.RadioButton optCompress;
        private System.Windows.Forms.RadioButton optCopy;
        private System.Windows.Forms.CheckBox chkVerify;
        private System.Windows.Forms.CheckBox chkEncrypt;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPasswordConfirm;
        private System.Windows.Forms.TextBox txtPasswordConfirm;
        private System.Windows.Forms.Label lblPasswordNote;
        private System.Windows.Forms.Label lblExePath;
        private System.Windows.Forms.TextBox txtExePath;
        private System.Windows.Forms.Button cmdBrowseExe;
        private System.Windows.Forms.Label lblExeStatus;
        private System.Windows.Forms.GroupBox grpRetention;
        private System.Windows.Forms.Label lblKeepCount;
        private System.Windows.Forms.NumericUpDown numKeepCount;
        private System.Windows.Forms.Label lblKeepCountHint;
        private System.Windows.Forms.Label lblKeepDays;
        private System.Windows.Forms.NumericUpDown numKeepDays;
        private System.Windows.Forms.Label lblKeepDaysHint;
        private System.Windows.Forms.Label lblRetentionNote;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
    }
}
