using System;
using System.Windows.Forms;

namespace YedekAl
{
    public partial class FrmSettings : Form
    {
        private readonly Settings settings;

        public FrmSettings(Settings settings)
        {
            InitializeComponent();
            this.settings = settings;
            txtServiceName.Text = settings.serviceName;
            txtBackupDir.Text = settings.backupPath;
            txtBackupCmd.Text = settings.compressCmd;
            txtDatabaseDir.Text = settings.dbPath;
            txtXCopyCmd.Text = settings.copyCmd;
            chkCompress.Checked = settings.compress;
            chkShutdown.Checked = settings.autoShutdown;
            optCompress.Checked = settings.justCompress;
            optCopy.Checked = settings.copyFirst;
            if (!optCompress.Checked && !optCopy.Checked) optCompress.Checked = true;
            chkCompress_CheckedChanged(null, null);
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
                settings.serviceName = pServiceName;
                settings.backupPath = pBackupDir;
                settings.compressCmd = pBackupCmd;
                settings.dbPath = pDatabaseDir;
                settings.copyCmd = pCopyCmd;
                settings.compress = pCompress;
                settings.justCompress = pCompressDirect;
                settings.copyFirst = pCompressAfterCopy;
                settings.autoShutdown = pAutoShutdown;
                settings.save();
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