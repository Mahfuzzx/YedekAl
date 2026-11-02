using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YedekAl
{
    // Edits one backup profile. Saving is left to the caller, which owns the Settings.
    public partial class FrmProfile : Form
    {
        private readonly Profile original;
        private readonly List<string> otherNames;
        private string rarPath;
        private string sevenZipPath;
        private ArchiveFormat shownFormat;
        private bool loading;

        public Profile profile { get; private set; }

        public FrmProfile(Profile current, IEnumerable<string> otherProfileNames)
        {
            InitializeComponent();
            original = current;
            otherNames = otherProfileNames.ToList();
            loading = true;
            // Item order must match the ArchiveFormat and CompressionStrength enums.
            cboFormat.Items.AddRange(new object[] { "ZIP", "RAR", "7z" });
            cboStrength.Items.AddRange(new object[] { "Hızlı", "Normal", "En iyi" });

            txtProfileName.Text = current.name;
            chkIncludeInSchedule.Checked = current.includeInSchedule;
            txtServiceName.Text = current.serviceName;
            txtDatabaseDir.Text = current.dbPath;
            txtBackupDir.Text = current.backupPath;
            txtNameTemplate.Text = current.nameTemplate;
            chkCompress.Checked = current.compress;
            cboFormat.SelectedIndex = (int)current.format;
            cboStrength.SelectedIndex = (int)current.strength;
            optCopy.Checked = current.copyFirst;
            optCompress.Checked = !current.copyFirst;
            chkVerify.Checked = current.verifyArchive;
            chkEncrypt.Checked = current.encrypt;
            lblPasswordNote.Visible = current.protectedPassword.Length > 0;
            rarPath = current.rarPath;
            sevenZipPath = current.sevenZipPath;
            shownFormat = current.format;
            txtExePath.Text = exePathFor(shownFormat);
            numKeepCount.Value = Math.Min(numKeepCount.Maximum, Math.Max(0, current.keepLastCount));
            numKeepDays.Value = Math.Min(numKeepDays.Maximum, Math.Max(0, current.keepDays));
            loading = false;
            updateControls();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            storeExePath();
            Profile result = original.clone();
            result.name = txtProfileName.Text.Trim();
            result.includeInSchedule = chkIncludeInSchedule.Checked;
            result.serviceName = txtServiceName.Text.Trim();
            result.dbPath = txtDatabaseDir.Text.Trim();
            result.backupPath = txtBackupDir.Text.Trim();
            result.nameTemplate = txtNameTemplate.Text.Trim();
            result.compress = chkCompress.Checked;
            result.format = (ArchiveFormat)cboFormat.SelectedIndex;
            result.strength = (CompressionStrength)cboStrength.SelectedIndex;
            result.copyFirst = optCopy.Checked;
            result.verifyArchive = chkVerify.Checked;
            result.rarPath = rarPath;
            result.sevenZipPath = sevenZipPath;
            result.keepLastCount = (int)numKeepCount.Value;
            result.keepDays = (int)numKeepDays.Value;

            // An empty password box keeps the stored password; turning encryption off forgets it.
            result.encrypt = chkEncrypt.Checked;
            bool passwordChanged = false;
            if (!result.encrypt)
            {
                result.protectedPassword = "";
            }
            else if (txtPassword.Text.Length > 0 || txtPasswordConfirm.Text.Length > 0)
            {
                string problem = txtPassword.Text != txtPasswordConfirm.Text ? "Parolalar eşleşmiyor." : Profile.checkPassword(txtPassword.Text);
                if (problem != null)
                {
                    MessageBox.Show(this, problem, "Dur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                result.setPassword(txtPassword.Text);
                passwordChanged = true;
            }

            List<string> errors = result.validate();
            if (otherNames.Any(n => string.Equals(n.Trim(), result.name, StringComparison.CurrentCultureIgnoreCase)))
                errors.Add("Bu adda başka bir profil var.");
            if (errors.Count > 0)
            {
                MessageBox.Show(this, string.Join("\r\n", errors), "Dur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (result.compress)
            {
                string problem = Compressors.create(result).checkAvailable();
                if (problem != null && MessageBox.Show(this, problem + "\r\n\r\nBu programla yedek alınamaz. Profil yine de kaydedilsin mi?",
                        "YedekAl", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }

            if (passwordChanged)
                MessageBox.Show(this, "Parolayı güvenli bir yere not edin. YedekAl parolayı size geri gösteremez; parola olmadan şifreli yedekler açılamaz.",
                    "YedekAl", MessageBoxButtons.OK, MessageBoxIcon.Information);

            profile = result;
            DialogResult = DialogResult.OK;
        }

        private void cboFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading) return;
            storeExePath();
            shownFormat = (ArchiveFormat)cboFormat.SelectedIndex;
            loading = true;
            txtExePath.Text = exePathFor(shownFormat);
            loading = false;
            updateControls();
        }

        private void chkCompress_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading) updateControls();
        }

        private void chkEncrypt_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading) updateControls();
        }

        private void txtExePath_TextChanged(object sender, EventArgs e)
        {
            if (!loading) updateExeStatus();
        }

        private void cmdBrowseDatabase_Click(object sender, EventArgs e)
        {
            browseFolder(txtDatabaseDir, "Veritabanı klasörünü seçin.");
        }

        private void cmdBrowseBackup_Click(object sender, EventArgs e)
        {
            browseFolder(txtBackupDir, "Yedeklerin kaydedileceği klasörü seçin.");
        }

        private void cmdBrowseExe_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = shownFormat == ArchiveFormat.Rar ? "Rar.exe dosyasını seçin" : "7z.exe veya 7za.exe dosyasını seçin";
                dialog.Filter = "Programlar (*.exe)|*.exe";
                if (dialog.ShowDialog(this) == DialogResult.OK) txtExePath.Text = dialog.FileName;
            }
        }

        private void browseFolder(TextBox target, string description)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = description;
                dialog.SelectedPath = target.Text;
                if (dialog.ShowDialog(this) == DialogResult.OK) target.Text = dialog.SelectedPath;
            }
        }

        private string exePathFor(ArchiveFormat format)
        {
            if (format == ArchiveFormat.Rar) return rarPath;
            if (format == ArchiveFormat.SevenZip) return sevenZipPath;
            return "";
        }

        private void storeExePath()
        {
            if (shownFormat == ArchiveFormat.Rar) rarPath = txtExePath.Text.Trim();
            else if (shownFormat == ArchiveFormat.SevenZip) sevenZipPath = txtExePath.Text.Trim();
        }

        private void updateControls()
        {
            bool compress = chkCompress.Checked;
            bool external = compress && shownFormat != ArchiveFormat.Zip;
            lblFormat.Enabled = cboFormat.Enabled = compress;
            lblStrength.Enabled = cboStrength.Enabled = compress;
            optCompress.Enabled = optCopy.Enabled = chkVerify.Enabled = chkEncrypt.Enabled = compress;
            bool password = compress && chkEncrypt.Checked;
            lblPassword.Enabled = txtPassword.Enabled = lblPasswordConfirm.Enabled = txtPasswordConfirm.Enabled = password;
            lblExePath.Enabled = txtExePath.Enabled = cmdBrowseExe.Enabled = external;
            updateExeStatus();
        }

        private void updateExeStatus()
        {
            lblExeStatus.ForeColor = SystemColors.GrayText;
            if (!chkCompress.Checked)
            {
                lblExeStatus.Text = "";
                return;
            }
            if (shownFormat == ArchiveFormat.Zip)
            {
                lblExeStatus.Text = "ZIP için ek program gerekmez.";
                return;
            }
            ExternalCompressor compressor = shownFormat == ArchiveFormat.Rar
                ? (ExternalCompressor)new RarCompressor(txtExePath.Text)
                : new SevenZipCompressor(txtExePath.Text);
            string problem = compressor.checkAvailable();
            if (problem != null) lblExeStatus.ForeColor = Color.Firebrick;
            lblExeStatus.Text = problem ?? "Kullanılacak: " + compressor.exePath;
        }
    }
}
