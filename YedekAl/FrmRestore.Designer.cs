namespace YedekAl
{
    partial class FrmRestore
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
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblWarning = new System.Windows.Forms.Label();
            this.chkKeepCurrent = new System.Windows.Forms.CheckBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.chkConfirm = new System.Windows.Forms.CheckBox();
            this.cmdRestore = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblInfo
            //
            this.lblInfo.AutoEllipsis = true;
            this.lblInfo.Location = new System.Drawing.Point(12, 12);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(456, 60);
            this.lblInfo.TabIndex = 0;
            //
            // lblWarning
            //
            this.lblWarning.ForeColor = System.Drawing.Color.Firebrick;
            this.lblWarning.Location = new System.Drawing.Point(12, 78);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(456, 30);
            this.lblWarning.TabIndex = 1;
            this.lblWarning.Text = "Hizmet durdurulacak ve mevcut veritabanı bu yedekle değiştirilecek. Yedeğin alındığı tarihten sonra yapılan değişiklikler kaybolur.";
            //
            // chkKeepCurrent
            //
            this.chkKeepCurrent.AutoSize = true;
            this.chkKeepCurrent.Checked = true;
            this.chkKeepCurrent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkKeepCurrent.Location = new System.Drawing.Point(15, 116);
            this.chkKeepCurrent.Name = "chkKeepCurrent";
            this.chkKeepCurrent.Size = new System.Drawing.Size(330, 17);
            this.chkKeepCurrent.TabIndex = 2;
            this.chkKeepCurrent.Text = "Mevcut veritabanını silme, yanında \".yedekal_onceki_\" adıyla &sakla";
            this.chkKeepCurrent.UseVisualStyleBackColor = true;
            //
            // lblPassword
            //
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(12, 145);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(160, 13);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "&Parola (boş = profildeki parola):";
            //
            // txtPassword
            //
            this.txtPassword.Location = new System.Drawing.Point(180, 142);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(288, 20);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.UseSystemPasswordChar = true;
            //
            // chkConfirm
            //
            this.chkConfirm.AutoSize = true;
            this.chkConfirm.Location = new System.Drawing.Point(15, 180);
            this.chkConfirm.Name = "chkConfirm";
            this.chkConfirm.Size = new System.Drawing.Size(270, 17);
            this.chkConfirm.TabIndex = 5;
            this.chkConfirm.Text = "Mevcut verilerin değiştirileceğini &anlıyorum";
            this.chkConfirm.UseVisualStyleBackColor = true;
            this.chkConfirm.CheckedChanged += new System.EventHandler(this.chkConfirm_CheckedChanged);
            //
            // cmdRestore
            //
            this.cmdRestore.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdRestore.Location = new System.Drawing.Point(302, 220);
            this.cmdRestore.Name = "cmdRestore";
            this.cmdRestore.Size = new System.Drawing.Size(80, 25);
            this.cmdRestore.TabIndex = 6;
            this.cmdRestore.Text = "&Geri yükle";
            this.cmdRestore.UseVisualStyleBackColor = true;
            //
            // cmdCancel
            //
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(388, 220);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(80, 25);
            this.cmdCancel.TabIndex = 7;
            this.cmdCancel.Text = "Vazgeç";
            this.cmdCancel.UseVisualStyleBackColor = true;
            //
            // FrmRestore
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(480, 257);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.chkKeepCurrent);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.chkConfirm);
            this.Controls.Add(this.cmdRestore);
            this.Controls.Add(this.cmdCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmRestore";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "YedekAl - Geri yükle";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.CheckBox chkKeepCurrent;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkConfirm;
        private System.Windows.Forms.Button cmdRestore;
        private System.Windows.Forms.Button cmdCancel;
    }
}
