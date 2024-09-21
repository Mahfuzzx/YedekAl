using System;
using System.Windows.Forms;

namespace YedekAl
{
    public partial class FrmIcon : Form
    {
        public FrmIcon()
        {
            InitializeComponent();
            txtServiceName.Text = Properties.Settings.Default.serviceName;
            txtBackupDir.Text = Properties.Settings.Default.backupPath;
            txtBackupCmd.Text = Properties.Settings.Default.compressCmd;
            txtDatabaseDir.Text = Properties.Settings.Default.dbPath;
            txtXCopyCmd.Text = Properties.Settings.Default.copyCmd;
            chkCompress.Checked = Properties.Settings.Default.compress;
            chkShutdown.Checked = Properties.Settings.Default.autoShutdown;
            optCompress.Checked = Properties.Settings.Default.justCompress;
            optCopy.Checked = Properties.Settings.Default.copyFirst;
            if (!optCompress.Checked && !optCopy.Checked) optCompress.Checked = true;
            chkCompress_CheckedChanged(this, null);
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            string pServiceName = txtServiceName.Text.Trim();
            string pBackupDir = txtBackupDir.Text.Trim();
            string pBackupCmd = txtBackupCmd.Text.Trim();
            string pDatabaseDir = txtDatabaseDir.Text.Trim();
            string pCopyCmd = txtXCopyCmd.Text.Trim();
            bool pCompress = chkCompress.Checked;
            bool pCompressDirect = optCompress.Checked;
            bool pCompressAfterCopy = optCopy.Checked;
            bool pAutoShutdown = chkShutdown.Checked;
            if (pServiceName == "" || pBackupDir == "" || pBackupCmd == "" || pDatabaseDir == "" || pCopyCmd == "") MessageBox.Show("Alanlarýn hepsi doldurulmak zorundadýr.", "Dur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                Properties.Settings.Default.serviceName = pServiceName;
                Properties.Settings.Default.backupPath = pBackupDir;
                Properties.Settings.Default.compressCmd = pBackupCmd;
                Properties.Settings.Default.dbPath = pDatabaseDir;
                Properties.Settings.Default.copyCmd = pCopyCmd;
                Properties.Settings.Default.compress = pCompress;
                Properties.Settings.Default.justCompress = pCompressDirect;
                Properties.Settings.Default.copyFirst = pCompressAfterCopy;
                Properties.Settings.Default.autoShutdown = pAutoShutdown;
                Properties.Settings.Default.Save();
                Close();
            }
        }

        private void chkCompress_CheckedChanged(object sender, EventArgs e)
        {
            optCopy.Enabled = chkCompress.Checked;
            optCompress.Enabled = chkCompress.Checked;
        }
    }
}