namespace YedekAl
{
    partial class FrmGeneral
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
            this.grpSchedule = new System.Windows.Forms.GroupBox();
            this.chkSchedule = new System.Windows.Forms.CheckBox();
            this.dtpScheduleTime = new System.Windows.Forms.DateTimePicker();
            this.lblScheduleNote = new System.Windows.Forms.Label();
            this.lblScheduleProfiles = new System.Windows.Forms.Label();
            this.chkShutdown = new System.Windows.Forms.CheckBox();
            this.chkNotify = new System.Windows.Forms.CheckBox();
            this.cmdNotifySettings = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.grpSchedule.SuspendLayout();
            this.SuspendLayout();
            //
            // grpSchedule
            //
            this.grpSchedule.Controls.Add(this.chkSchedule);
            this.grpSchedule.Controls.Add(this.dtpScheduleTime);
            this.grpSchedule.Controls.Add(this.lblScheduleNote);
            this.grpSchedule.Controls.Add(this.lblScheduleProfiles);
            this.grpSchedule.Location = new System.Drawing.Point(12, 12);
            this.grpSchedule.Name = "grpSchedule";
            this.grpSchedule.Size = new System.Drawing.Size(456, 96);
            this.grpSchedule.TabIndex = 0;
            this.grpSchedule.TabStop = false;
            this.grpSchedule.Text = "Zamanlama";
            //
            // chkSchedule
            //
            this.chkSchedule.AutoSize = true;
            this.chkSchedule.Location = new System.Drawing.Point(15, 23);
            this.chkSchedule.Name = "chkSchedule";
            this.chkSchedule.Size = new System.Drawing.Size(190, 17);
            this.chkSchedule.TabIndex = 0;
            this.chkSchedule.Text = "Her gün &otomatik yedek al, saat:";
            this.chkSchedule.UseVisualStyleBackColor = true;
            this.chkSchedule.CheckedChanged += new System.EventHandler(this.chkSchedule_CheckedChanged);
            //
            // dtpScheduleTime
            //
            this.dtpScheduleTime.CustomFormat = "HH:mm";
            this.dtpScheduleTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpScheduleTime.Location = new System.Drawing.Point(230, 21);
            this.dtpScheduleTime.Name = "dtpScheduleTime";
            this.dtpScheduleTime.ShowUpDown = true;
            this.dtpScheduleTime.Size = new System.Drawing.Size(70, 20);
            this.dtpScheduleTime.TabIndex = 1;
            //
            // lblScheduleNote
            //
            this.lblScheduleNote.AutoSize = true;
            this.lblScheduleNote.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblScheduleNote.Location = new System.Drawing.Point(12, 48);
            this.lblScheduleNote.Name = "lblScheduleNote";
            this.lblScheduleNote.Size = new System.Drawing.Size(420, 13);
            this.lblScheduleNote.TabIndex = 2;
            this.lblScheduleNote.Text = "Görev SYSTEM hesabıyla çalışır; kimse oturum açmamış olsa da yedek alınır.";
            //
            // lblScheduleProfiles
            //
            this.lblScheduleProfiles.AutoEllipsis = true;
            this.lblScheduleProfiles.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblScheduleProfiles.Location = new System.Drawing.Point(12, 68);
            this.lblScheduleProfiles.Name = "lblScheduleProfiles";
            this.lblScheduleProfiles.Size = new System.Drawing.Size(432, 15);
            this.lblScheduleProfiles.TabIndex = 3;
            //
            // chkShutdown
            //
            this.chkShutdown.AutoSize = true;
            this.chkShutdown.Location = new System.Drawing.Point(15, 120);
            this.chkShutdown.Name = "chkShutdown";
            this.chkShutdown.Size = new System.Drawing.Size(231, 17);
            this.chkShutdown.TabIndex = 1;
            this.chkShutdown.Text = "Yedekleme tamamlanınca bilgisayarı &kapat";
            this.chkShutdown.UseVisualStyleBackColor = true;
            //
            // chkNotify
            //
            this.chkNotify.AutoSize = true;
            this.chkNotify.Location = new System.Drawing.Point(15, 147);
            this.chkNotify.Name = "chkNotify";
            this.chkNotify.Size = new System.Drawing.Size(240, 17);
            this.chkNotify.TabIndex = 2;
            this.chkNotify.Text = "Yedekleme başarısız olursa &e-posta gönder";
            this.chkNotify.UseVisualStyleBackColor = true;
            //
            // cmdNotifySettings
            //
            this.cmdNotifySettings.Location = new System.Drawing.Point(318, 143);
            this.cmdNotifySettings.Name = "cmdNotifySettings";
            this.cmdNotifySettings.Size = new System.Drawing.Size(150, 23);
            this.cmdNotifySettings.TabIndex = 3;
            this.cmdNotifySettings.Text = "E-posta ayarları...";
            this.cmdNotifySettings.UseVisualStyleBackColor = true;
            this.cmdNotifySettings.Click += new System.EventHandler(this.cmdNotifySettings_Click);
            //
            // cmdOK
            //
            this.cmdOK.Location = new System.Drawing.Point(302, 190);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(80, 25);
            this.cmdOK.TabIndex = 4;
            this.cmdOK.Text = "Kaydet";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            //
            // cmdCancel
            //
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(388, 190);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(80, 25);
            this.cmdCancel.TabIndex = 5;
            this.cmdCancel.Text = "Vazgeç";
            this.cmdCancel.UseVisualStyleBackColor = true;
            //
            // FrmGeneral
            //
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(480, 227);
            this.Controls.Add(this.grpSchedule);
            this.Controls.Add(this.chkShutdown);
            this.Controls.Add(this.chkNotify);
            this.Controls.Add(this.cmdNotifySettings);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmGeneral";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "YedekAl - Genel ayarlar";
            this.grpSchedule.ResumeLayout(false);
            this.grpSchedule.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSchedule;
        private System.Windows.Forms.CheckBox chkSchedule;
        private System.Windows.Forms.DateTimePicker dtpScheduleTime;
        private System.Windows.Forms.Label lblScheduleNote;
        private System.Windows.Forms.Label lblScheduleProfiles;
        private System.Windows.Forms.CheckBox chkShutdown;
        private System.Windows.Forms.CheckBox chkNotify;
        private System.Windows.Forms.Button cmdNotifySettings;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
    }
}
