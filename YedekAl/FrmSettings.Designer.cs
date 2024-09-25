namespace YedekAl
{
    partial class FrmSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtServiceName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDatabaseDir = new System.Windows.Forms.TextBox();
            this.txtBackupDir = new System.Windows.Forms.TextBox();
            this.txtXCopyCmd = new System.Windows.Forms.TextBox();
            this.txtBackupCmd = new System.Windows.Forms.TextBox();
            this.chkCompress = new System.Windows.Forms.CheckBox();
            this.optCopy = new System.Windows.Forms.RadioButton();
            this.optCompress = new System.Windows.Forms.RadioButton();
            this.chkShutdown = new System.Windows.Forms.CheckBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Hizmet adý:";
            // 
            // txtServiceName
            // 
            this.txtServiceName.Location = new System.Drawing.Point(122, 12);
            this.txtServiceName.Name = "txtServiceName";
            this.txtServiceName.Size = new System.Drawing.Size(281, 20);
            this.txtServiceName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Veritabaný konumu:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "&Yedekleme konumu:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Kopyalama komutu:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Sýkýþtýrma k&omutu:";
            // 
            // txtDatabaseDir
            // 
            this.txtDatabaseDir.Location = new System.Drawing.Point(122, 38);
            this.txtDatabaseDir.Name = "txtDatabaseDir";
            this.txtDatabaseDir.Size = new System.Drawing.Size(281, 20);
            this.txtDatabaseDir.TabIndex = 6;
            // 
            // txtBackupDir
            // 
            this.txtBackupDir.Location = new System.Drawing.Point(122, 64);
            this.txtBackupDir.Name = "txtBackupDir";
            this.txtBackupDir.Size = new System.Drawing.Size(281, 20);
            this.txtBackupDir.TabIndex = 7;
            // 
            // txtXCopyCmd
            // 
            this.txtXCopyCmd.Location = new System.Drawing.Point(122, 90);
            this.txtXCopyCmd.Name = "txtXCopyCmd";
            this.txtXCopyCmd.Size = new System.Drawing.Size(281, 20);
            this.txtXCopyCmd.TabIndex = 8;
            // 
            // txtBackupCmd
            // 
            this.txtBackupCmd.Location = new System.Drawing.Point(122, 116);
            this.txtBackupCmd.Name = "txtBackupCmd";
            this.txtBackupCmd.Size = new System.Drawing.Size(281, 20);
            this.txtBackupCmd.TabIndex = 9;
            // 
            // chkCompress
            // 
            this.chkCompress.AutoSize = true;
            this.chkCompress.Location = new System.Drawing.Point(12, 153);
            this.chkCompress.Name = "chkCompress";
            this.chkCompress.Size = new System.Drawing.Size(118, 17);
            this.chkCompress.TabIndex = 10;
            this.chkCompress.Text = "Veri tabanýný sýkýþtýr.";
            this.chkCompress.UseVisualStyleBackColor = true;
            this.chkCompress.CheckedChanged += new System.EventHandler(this.chkCompress_CheckedChanged);
            // 
            // optCopy
            // 
            this.optCopy.AutoSize = true;
            this.optCopy.Location = new System.Drawing.Point(40, 176);
            this.optCopy.Name = "optCopy";
            this.optCopy.Size = new System.Drawing.Size(216, 17);
            this.optCopy.TabIndex = 11;
            this.optCopy.TabStop = true;
            this.optCopy.Text = "Veri tabanýný önce kopyala, sonra sýkýþtýr.";
            this.optCopy.UseVisualStyleBackColor = true;
            // 
            // optCompress
            // 
            this.optCompress.AutoSize = true;
            this.optCompress.Location = new System.Drawing.Point(40, 199);
            this.optCompress.Name = "optCompress";
            this.optCompress.Size = new System.Drawing.Size(201, 17);
            this.optCompress.TabIndex = 12;
            this.optCompress.TabStop = true;
            this.optCompress.Text = "Veri tabanýný doðrudan hedefe sýkýþtýr.";
            this.optCompress.UseVisualStyleBackColor = true;
            // 
            // chkShutdown
            // 
            this.chkShutdown.AutoSize = true;
            this.chkShutdown.Location = new System.Drawing.Point(12, 237);
            this.chkShutdown.Name = "chkShutdown";
            this.chkShutdown.Size = new System.Drawing.Size(231, 17);
            this.chkShutdown.TabIndex = 13;
            this.chkShutdown.Text = "Yedek alma tamamlanýnca bilgisayarý kapat.";
            this.chkShutdown.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.Location = new System.Drawing.Point(328, 270);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 14;
            this.cmdOK.Text = "&Kaydet";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // FrmIcon
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 305);
            this.ControlBox = false;
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.chkShutdown);
            this.Controls.Add(this.optCompress);
            this.Controls.Add(this.optCopy);
            this.Controls.Add(this.chkCompress);
            this.Controls.Add(this.txtBackupCmd);
            this.Controls.Add(this.txtXCopyCmd);
            this.Controls.Add(this.txtBackupDir);
            this.Controls.Add(this.txtDatabaseDir);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtServiceName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmIcon";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "YedekAl - Ayarlar";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServiceName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDatabaseDir;
        private System.Windows.Forms.TextBox txtBackupDir;
        private System.Windows.Forms.TextBox txtXCopyCmd;
        private System.Windows.Forms.TextBox txtBackupCmd;
        private System.Windows.Forms.CheckBox chkCompress;
        private System.Windows.Forms.RadioButton optCopy;
        private System.Windows.Forms.RadioButton optCompress;
        private System.Windows.Forms.CheckBox chkShutdown;
        private System.Windows.Forms.Button cmdOK;
    }
}