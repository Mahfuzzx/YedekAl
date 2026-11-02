namespace YedekAl
{
    partial class FrmNotify
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
            this.lblSmtpHost = new System.Windows.Forms.Label();
            this.txtSmtpHost = new System.Windows.Forms.TextBox();
            this.lblSmtpPort = new System.Windows.Forms.Label();
            this.numSmtpPort = new System.Windows.Forms.NumericUpDown();
            this.chkSmtpSsl = new System.Windows.Forms.CheckBox();
            this.chkTrustAnyCertificate = new System.Windows.Forms.CheckBox();
            this.lblSmtpUser = new System.Windows.Forms.Label();
            this.txtSmtpUser = new System.Windows.Forms.TextBox();
            this.lblSmtpPassword = new System.Windows.Forms.Label();
            this.txtSmtpPassword = new System.Windows.Forms.TextBox();
            this.lblPasswordNote = new System.Windows.Forms.Label();
            this.lblMailFrom = new System.Windows.Forms.Label();
            this.txtMailFrom = new System.Windows.Forms.TextBox();
            this.lblMailTo = new System.Windows.Forms.Label();
            this.txtMailTo = new System.Windows.Forms.TextBox();
            this.lblMailToHint = new System.Windows.Forms.Label();
            this.chkNotifySuccess = new System.Windows.Forms.CheckBox();
            this.cmdTest = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numSmtpPort)).BeginInit();
            this.SuspendLayout();
            //
            // lblSmtpHost
            //
            this.lblSmtpHost.AutoSize = true;
            this.lblSmtpHost.Location = new System.Drawing.Point(12, 15);
            this.lblSmtpHost.Name = "lblSmtpHost";
            this.lblSmtpHost.Size = new System.Drawing.Size(47, 13);
            this.lblSmtpHost.TabIndex = 0;
            this.lblSmtpHost.Text = "&Sunucu:";
            //
            // txtSmtpHost
            //
            this.txtSmtpHost.Location = new System.Drawing.Point(130, 12);
            this.txtSmtpHost.Name = "txtSmtpHost";
            this.txtSmtpHost.Size = new System.Drawing.Size(200, 20);
            this.txtSmtpHost.TabIndex = 1;
            //
            // lblSmtpPort
            //
            this.lblSmtpPort.AutoSize = true;
            this.lblSmtpPort.Location = new System.Drawing.Point(340, 15);
            this.lblSmtpPort.Name = "lblSmtpPort";
            this.lblSmtpPort.Size = new System.Drawing.Size(29, 13);
            this.lblSmtpPort.TabIndex = 2;
            this.lblSmtpPort.Text = "P&ort:";
            //
            // numSmtpPort
            //
            this.numSmtpPort.Location = new System.Drawing.Point(375, 12);
            this.numSmtpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numSmtpPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSmtpPort.Name = "numSmtpPort";
            this.numSmtpPort.Size = new System.Drawing.Size(57, 20);
            this.numSmtpPort.TabIndex = 3;
            this.numSmtpPort.Value = new decimal(new int[] {
            587,
            0,
            0,
            0});
            //
            // chkSmtpSsl
            //
            this.chkSmtpSsl.AutoSize = true;
            this.chkSmtpSsl.Location = new System.Drawing.Point(130, 38);
            this.chkSmtpSsl.Name = "chkSmtpSsl";
            this.chkSmtpSsl.Size = new System.Drawing.Size(170, 17);
            this.chkSmtpSsl.TabIndex = 4;
            this.chkSmtpSsl.Text = "&Güvenli bağlantı (STARTTLS)";
            this.chkSmtpSsl.UseVisualStyleBackColor = true;
            this.chkSmtpSsl.CheckedChanged += new System.EventHandler(this.chkSmtpSsl_CheckedChanged);
            // 
            // chkTrustAnyCertificate
            // 
            this.chkTrustAnyCertificate.AutoSize = true;
            this.chkTrustAnyCertificate.Location = new System.Drawing.Point(130, 60);
            this.chkTrustAnyCertificate.Name = "chkTrustAnyCertificate";
            this.chkTrustAnyCertificate.Size = new System.Drawing.Size(290, 17);
            this.chkTrustAnyCertificate.TabIndex = 5;
            this.chkTrustAnyCertificate.Text = "Güvenilmeyen sunucu sertifikasını da kabul et (önerilmez)";
            this.chkTrustAnyCertificate.UseVisualStyleBackColor = true;
            //
            // lblSmtpUser
            //
            this.lblSmtpUser.AutoSize = true;
            this.lblSmtpUser.Location = new System.Drawing.Point(12, 89);
            this.lblSmtpUser.Name = "lblSmtpUser";
            this.lblSmtpUser.Size = new System.Drawing.Size(67, 13);
            this.lblSmtpUser.TabIndex = 6;
            this.lblSmtpUser.Text = "&Kullanıcı adı:";
            //
            // txtSmtpUser
            //
            this.txtSmtpUser.Location = new System.Drawing.Point(130, 86);
            this.txtSmtpUser.Name = "txtSmtpUser";
            this.txtSmtpUser.Size = new System.Drawing.Size(302, 20);
            this.txtSmtpUser.TabIndex = 7;
            //
            // lblSmtpPassword
            //
            this.lblSmtpPassword.AutoSize = true;
            this.lblSmtpPassword.Location = new System.Drawing.Point(12, 115);
            this.lblSmtpPassword.Name = "lblSmtpPassword";
            this.lblSmtpPassword.Size = new System.Drawing.Size(40, 13);
            this.lblSmtpPassword.TabIndex = 8;
            this.lblSmtpPassword.Text = "&Parola:";
            //
            // txtSmtpPassword
            //
            this.txtSmtpPassword.Location = new System.Drawing.Point(130, 112);
            this.txtSmtpPassword.Name = "txtSmtpPassword";
            this.txtSmtpPassword.Size = new System.Drawing.Size(302, 20);
            this.txtSmtpPassword.TabIndex = 9;
            this.txtSmtpPassword.UseSystemPasswordChar = true;
            //
            // lblPasswordNote
            //
            this.lblPasswordNote.AutoSize = true;
            this.lblPasswordNote.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPasswordNote.Location = new System.Drawing.Point(127, 135);
            this.lblPasswordNote.Name = "lblPasswordNote";
            this.lblPasswordNote.Size = new System.Drawing.Size(200, 13);
            this.lblPasswordNote.TabIndex = 10;
            this.lblPasswordNote.Text = "Boş bırakılırsa kayıtlı parola korunur.";
            //
            // lblMailFrom
            //
            this.lblMailFrom.AutoSize = true;
            this.lblMailFrom.Location = new System.Drawing.Point(12, 159);
            this.lblMailFrom.Name = "lblMailFrom";
            this.lblMailFrom.Size = new System.Drawing.Size(53, 13);
            this.lblMailFrom.TabIndex = 11;
            this.lblMailFrom.Text = "Gö&nderen:";
            //
            // txtMailFrom
            //
            this.txtMailFrom.Location = new System.Drawing.Point(130, 156);
            this.txtMailFrom.Name = "txtMailFrom";
            this.txtMailFrom.Size = new System.Drawing.Size(302, 20);
            this.txtMailFrom.TabIndex = 12;
            //
            // lblMailTo
            //
            this.lblMailTo.AutoSize = true;
            this.lblMailTo.Location = new System.Drawing.Point(12, 185);
            this.lblMailTo.Name = "lblMailTo";
            this.lblMailTo.Size = new System.Drawing.Size(47, 13);
            this.lblMailTo.TabIndex = 13;
            this.lblMailTo.Text = "&Alıcılar:";
            //
            // txtMailTo
            //
            this.txtMailTo.Location = new System.Drawing.Point(130, 182);
            this.txtMailTo.Name = "txtMailTo";
            this.txtMailTo.Size = new System.Drawing.Size(302, 20);
            this.txtMailTo.TabIndex = 14;
            //
            // lblMailToHint
            //
            this.lblMailToHint.AutoSize = true;
            this.lblMailToHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblMailToHint.Location = new System.Drawing.Point(127, 205);
            this.lblMailToHint.Name = "lblMailToHint";
            this.lblMailToHint.Size = new System.Drawing.Size(190, 13);
            this.lblMailToHint.TabIndex = 15;
            this.lblMailToHint.Text = "Birden fazla adresi virgülle ayırın.";
            //
            // chkNotifySuccess
            //
            this.chkNotifySuccess.AutoSize = true;
            this.chkNotifySuccess.Location = new System.Drawing.Point(15, 232);
            this.chkNotifySuccess.Name = "chkNotifySuccess";
            this.chkNotifySuccess.Size = new System.Drawing.Size(220, 17);
            this.chkNotifySuccess.TabIndex = 16;
            this.chkNotifySuccess.Text = "&Başarılı yedeklerde de e-posta gönder";
            this.chkNotifySuccess.UseVisualStyleBackColor = true;
            //
            // cmdTest
            //
            this.cmdTest.Location = new System.Drawing.Point(12, 284);
            this.cmdTest.Name = "cmdTest";
            this.cmdTest.Size = new System.Drawing.Size(150, 25);
            this.cmdTest.TabIndex = 17;
            this.cmdTest.Text = "&Test e-postası gönder";
            this.cmdTest.UseVisualStyleBackColor = true;
            this.cmdTest.Click += new System.EventHandler(this.cmdTest_Click);
            //
            // cmdOK
            //
            this.cmdOK.Location = new System.Drawing.Point(266, 284);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(80, 25);
            this.cmdOK.TabIndex = 18;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            //
            // cmdCancel
            //
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(352, 284);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(80, 25);
            this.cmdCancel.TabIndex = 19;
            this.cmdCancel.Text = "Vazgeç";
            this.cmdCancel.UseVisualStyleBackColor = true;
            //
            // FrmNotify
            //
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(444, 321);
            this.Controls.Add(this.lblSmtpHost);
            this.Controls.Add(this.txtSmtpHost);
            this.Controls.Add(this.lblSmtpPort);
            this.Controls.Add(this.numSmtpPort);
            this.Controls.Add(this.chkSmtpSsl);
            this.Controls.Add(this.chkTrustAnyCertificate);
            this.Controls.Add(this.lblSmtpUser);
            this.Controls.Add(this.txtSmtpUser);
            this.Controls.Add(this.lblSmtpPassword);
            this.Controls.Add(this.txtSmtpPassword);
            this.Controls.Add(this.lblPasswordNote);
            this.Controls.Add(this.lblMailFrom);
            this.Controls.Add(this.txtMailFrom);
            this.Controls.Add(this.lblMailTo);
            this.Controls.Add(this.txtMailTo);
            this.Controls.Add(this.lblMailToHint);
            this.Controls.Add(this.chkNotifySuccess);
            this.Controls.Add(this.cmdTest);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmNotify";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "YedekAl - E-posta bildirimi";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmNotify_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numSmtpPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSmtpHost;
        private System.Windows.Forms.TextBox txtSmtpHost;
        private System.Windows.Forms.Label lblSmtpPort;
        private System.Windows.Forms.NumericUpDown numSmtpPort;
        private System.Windows.Forms.CheckBox chkSmtpSsl;
        private System.Windows.Forms.CheckBox chkTrustAnyCertificate;
        private System.Windows.Forms.Label lblSmtpUser;
        private System.Windows.Forms.TextBox txtSmtpUser;
        private System.Windows.Forms.Label lblSmtpPassword;
        private System.Windows.Forms.TextBox txtSmtpPassword;
        private System.Windows.Forms.Label lblPasswordNote;
        private System.Windows.Forms.Label lblMailFrom;
        private System.Windows.Forms.TextBox txtMailFrom;
        private System.Windows.Forms.Label lblMailTo;
        private System.Windows.Forms.TextBox txtMailTo;
        private System.Windows.Forms.Label lblMailToHint;
        private System.Windows.Forms.CheckBox chkNotifySuccess;
        private System.Windows.Forms.Button cmdTest;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
    }
}
