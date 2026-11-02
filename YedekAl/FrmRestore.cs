using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace YedekAl
{
    // Confirms a restore and collects its options; the restore itself runs in FrmMain.
    public partial class FrmRestore : Form
    {
        public bool keepCurrent => chkKeepCurrent.Checked;

        // Null means "use the profile's stored archive password".
        public string password => txtPassword.Text.Length > 0 ? txtPassword.Text : null;

        internal FrmRestore(Profile profile, HistoryEntry entry)
        {
            InitializeComponent();
            lblInfo.Text = string.Format("Profil: {0}\r\nYedek: {1}\r\nTarih: {2}   Boyut: {3}\r\nGeri yüklenecek yer: {4}",
                profile.name, Path.GetFileName(entry.path), entry.start.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture),
                SystemInfo.formatSize(entry.size), profile.dbPath);
            string extension = Path.GetExtension(entry.path).ToLowerInvariant();
            bool archiveWithPassword = !Directory.Exists(entry.path) && (extension == ".7z" || extension == ".rar");
            lblPassword.Enabled = txtPassword.Enabled = archiveWithPassword;
            updateControls();
        }

        private void chkConfirm_CheckedChanged(object sender, EventArgs e)
        {
            updateControls();
        }

        private void updateControls()
        {
            cmdRestore.Enabled = chkConfirm.Checked;
        }
    }
}
