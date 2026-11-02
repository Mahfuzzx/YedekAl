using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YedekAl
{
    // E-mail notification settings. Edits the mail fields of the Settings object it is given, only on OK.
    public partial class FrmNotify : Form
    {
        private readonly Settings target;
        private bool sending;

        public FrmNotify(Settings target)
        {
            InitializeComponent();
            this.target = target;
            txtSmtpHost.Text = target.smtpHost;
            numSmtpPort.Value = Math.Min(numSmtpPort.Maximum, Math.Max(numSmtpPort.Minimum, target.smtpPort));
            chkSmtpSsl.Checked = target.smtpSsl;
            chkTrustAnyCertificate.Checked = target.smtpTrustAnyCertificate;
            txtSmtpUser.Text = target.smtpUser;
            lblPasswordNote.Visible = target.protectedSmtpPassword.Length > 0;
            txtMailFrom.Text = target.mailFrom;
            txtMailTo.Text = target.mailTo;
            chkNotifySuccess.Checked = target.notifyOnSuccess;
            updateControls();
        }

        private void chkSmtpSsl_CheckedChanged(object sender, EventArgs e)
        {
            updateControls();
        }

        // The certificate is only checked on an encrypted connection.
        private void updateControls()
        {
            chkTrustAnyCertificate.Enabled = chkSmtpSsl.Checked;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Settings edited = collect();
            if (edited == null) return;
            target.smtpHost = edited.smtpHost;
            target.smtpPort = edited.smtpPort;
            target.smtpSsl = edited.smtpSsl;
            target.smtpTrustAnyCertificate = edited.smtpTrustAnyCertificate;
            target.smtpUser = edited.smtpUser;
            target.protectedSmtpPassword = edited.protectedSmtpPassword;
            target.mailFrom = edited.mailFrom;
            target.mailTo = edited.mailTo;
            target.notifyOnSuccess = edited.notifyOnSuccess;
            DialogResult = DialogResult.OK;
        }

        private async void cmdTest_Click(object sender, EventArgs e)
        {
            Settings edited = collect();
            if (edited == null) return;
            sending = true;
            cmdTest.Enabled = cmdOK.Enabled = cmdCancel.Enabled = false;
            UseWaitCursor = true;
            try
            {
                await Task.Run(() => MailNotifier.sendTest(edited));
                MessageBox.Show(this, "Test e-postası gönderildi: " + edited.mailTo, "YedekAl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Test e-postası gönderilemedi:\r\n" + ex.Message, "YedekAl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sending = false;
                cmdTest.Enabled = cmdOK.Enabled = cmdCancel.Enabled = true;
                UseWaitCursor = false;
            }
        }

        private void FrmNotify_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sending) e.Cancel = true;
        }

        // Returns a copy holding the form values, or null after showing what is wrong.
        private Settings collect()
        {
            Settings edited = target.clone();
            edited.smtpHost = txtSmtpHost.Text.Trim();
            edited.smtpPort = (int)numSmtpPort.Value;
            edited.smtpSsl = chkSmtpSsl.Checked;
            edited.smtpTrustAnyCertificate = chkSmtpSsl.Checked && chkTrustAnyCertificate.Checked;
            edited.smtpUser = txtSmtpUser.Text.Trim();
            // An empty password box keeps the stored password; no user name means no password.
            if (edited.smtpUser.Length == 0) edited.protectedSmtpPassword = "";
            else if (txtSmtpPassword.Text.Length > 0) edited.setSmtpPassword(txtSmtpPassword.Text);
            edited.mailFrom = txtMailFrom.Text.Trim();
            edited.mailTo = txtMailTo.Text.Trim();
            edited.notifyOnSuccess = chkNotifySuccess.Checked;

            List<string> errors = edited.validateMail();
            if (errors.Count == 0) return edited;
            MessageBox.Show(this, string.Join("\r\n", errors), "Dur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }
}
