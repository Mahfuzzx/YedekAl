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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.cmdBackup = new System.Windows.Forms.Button();
            this.cmdStop = new System.Windows.Forms.Button();
            this.cmdStart = new System.Windows.Forms.Button();
            this.lblServiceStatus = new System.Windows.Forms.Label();
            this.imgOff = new System.Windows.Forms.PictureBox();
            this.imgOn = new System.Windows.Forms.PictureBox();
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
            this.cmdSettings});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(390, 24);
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
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtLog);
            this.groupBox1.Controls.Add(this.cmdBackup);
            this.groupBox1.Controls.Add(this.cmdStop);
            this.groupBox1.Controls.Add(this.cmdStart);
            this.groupBox1.Controls.Add(this.lblServiceStatus);
            this.groupBox1.Controls.Add(this.imgOff);
            this.groupBox1.Controls.Add(this.imgOn);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(366, 319);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Veritabaný";
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(6, 91);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(354, 222);
            this.txtLog.TabIndex = 6;
            // 
            // cmdBackup
            // 
            this.cmdBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBackup.Location = new System.Drawing.Point(6, 57);
            this.cmdBackup.Name = "cmdBackup";
            this.cmdBackup.Size = new System.Drawing.Size(354, 28);
            this.cmdBackup.TabIndex = 5;
            this.cmdBackup.Text = "&Yedekle";
            this.cmdBackup.UseVisualStyleBackColor = true;
            this.cmdBackup.Click += new System.EventHandler(this.cmdBackup_Click);
            // 
            // cmdStop
            // 
            this.cmdStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdStop.Location = new System.Drawing.Point(246, 19);
            this.cmdStop.Name = "cmdStop";
            this.cmdStop.Size = new System.Drawing.Size(54, 23);
            this.cmdStop.TabIndex = 4;
            this.cmdStop.Text = "&Durdur";
            this.cmdStop.UseVisualStyleBackColor = true;
            this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
            // 
            // cmdStart
            // 
            this.cmdStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdStart.Location = new System.Drawing.Point(306, 19);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(54, 23);
            this.cmdStart.TabIndex = 3;
            this.cmdStart.Text = "Ça&lýþtýr";
            this.cmdStart.UseVisualStyleBackColor = true;
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // lblServiceStatus
            // 
            this.lblServiceStatus.AutoSize = true;
            this.lblServiceStatus.Location = new System.Drawing.Point(44, 28);
            this.lblServiceStatus.Name = "lblServiceStatus";
            this.lblServiceStatus.Size = new System.Drawing.Size(58, 13);
            this.lblServiceStatus.TabIndex = 2;
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
            // tmrSeek
            // 
            this.tmrSeek.Enabled = true;
            this.tmrSeek.Tick += new System.EventHandler(this.tmrSeek_Tick);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 358);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mnuMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuMain;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "YedekAl";
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox imgOn;
        private System.Windows.Forms.Button cmdStop;
        private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.Label lblServiceStatus;
        private System.Windows.Forms.PictureBox imgOff;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button cmdBackup;
        private System.Windows.Forms.Timer tmrSeek;
    }
}

