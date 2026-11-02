using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YedekAl
{
    public partial class FrmMain : Form
    {
        private Settings settings;
        private CancellationTokenSource cancellation;
        private bool busy;

        public FrmMain(Settings settings)
        {
            InitializeComponent();
            this.settings = settings;
            fillProfiles(settings.profiles[0].id);
        }

        private Profile currentProfile => cboProfile.SelectedItem as Profile ?? settings.profiles[0];

        private void fillProfiles(string selectedId)
        {
            cboProfile.BeginUpdate();
            cboProfile.Items.Clear();
            foreach (Profile profile in settings.profiles) cboProfile.Items.Add(profile);
            cboProfile.EndUpdate();
            Profile selected = settings.findProfile(selectedId) ?? settings.profiles[0];
            cboProfile.SelectedIndex = settings.profiles.IndexOf(selected);
            cmdDeleteProfile.Enabled = settings.profiles.Count > 1;
        }

        private void cboProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshServiceStatus();
            loadHistory();
        }

        private void cmdSettings_Click(object sender, EventArgs e)
        {
            using (FrmGeneral frmGeneral = new FrmGeneral(settings))
            {
                if (frmGeneral.ShowDialog(this) == DialogResult.OK) settings = frmGeneral.settings;
            }
        }

        private void cmdNewProfile_Click(object sender, EventArgs e)
        {
            string name = uniqueProfileName("Yeni profil");
            Profile profile = new Profile { name = name };
            editAndAdd(profile);
        }

        private void cmdDuplicateProfile_Click(object sender, EventArgs e)
        {
            editAndAdd(currentProfile.duplicate(uniqueProfileName(currentProfile.name + " (kopya)")));
        }

        private void cmdEditProfile_Click(object sender, EventArgs e)
        {
            Profile current = currentProfile;
            using (FrmProfile frmProfile = new FrmProfile(current, settings.profiles.Where(p => p.id != current.id).Select(p => p.name)))
            {
                if (frmProfile.ShowDialog(this) != DialogResult.OK) return;
                Settings updated = settings.clone();
                updated.profiles[settings.profiles.IndexOf(current)] = frmProfile.profile;
                if (applySettings(updated)) fillProfiles(current.id);
            }
        }

        private void cmdDeleteProfile_Click(object sender, EventArgs e)
        {
            Profile current = currentProfile;
            if (settings.profiles.Count < 2) return;
            if (MessageBox.Show(this, "'" + current.name + "' profili silinsin mi?\r\n\r\nBu profille alınmış yedek dosyaları silinmez.",
                    "YedekAl", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;
            Settings updated = settings.clone();
            updated.profiles.RemoveAll(p => p.id == current.id);
            if (applySettings(updated)) fillProfiles(updated.profiles[0].id);
        }

        private void editAndAdd(Profile profile)
        {
            using (FrmProfile frmProfile = new FrmProfile(profile, settings.profiles.Select(p => p.name)))
            {
                if (frmProfile.ShowDialog(this) != DialogResult.OK) return;
                Settings updated = settings.clone();
                updated.profiles.Add(frmProfile.profile);
                if (applySettings(updated)) fillProfiles(frmProfile.profile.id);
            }
        }

        private string uniqueProfileName(string baseName)
        {
            string name = baseName;
            for (int i = 2; settings.profiles.Any(p => string.Equals(p.name, name, StringComparison.CurrentCultureIgnoreCase)); i++)
                name = baseName + " " + i;
            return name;
        }

        // Validates and saves the shared settings; on success they become the current ones.
        private bool applySettings(Settings updated)
        {
            List<string> errors = updated.validate();
            if (errors.Count > 0)
            {
                MessageBox.Show(this, string.Join("\r\n", errors), "Dur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            try
            {
                updated.save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Ayarlar kaydedilemedi:\r\n" + ex.Message, "YedekAl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            settings = updated;
            return true;
        }

        private void cmdLogs_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(AppPaths.logDir);
            Process.Start("explorer.exe", "\"" + AppPaths.logDir + "\"");
        }

        private void tmrSeek_Tick(object sender, EventArgs e)
        {
            refreshServiceStatus();
        }

        private void refreshServiceStatus()
        {
            ServiceControllerStatus? status = ServiceManager.getStatus(currentProfile.serviceName);
            bool running = status == ServiceControllerStatus.Running;
            imgOn.Visible = running;
            imgOff.Visible = !running;
            lblServiceStatus.Text = ServiceManager.describe(status);
            if (!busy)
            {
                cmdStart.Enabled = status == ServiceControllerStatus.Stopped || status == ServiceControllerStatus.Paused;
                cmdStop.Enabled = running || status == ServiceControllerStatus.Paused;
            }
        }

        private async void cmdStop_Click(object sender, EventArgs e)
        {
            string name = currentProfile.serviceName;
            TimeSpan timeout = TimeSpan.FromSeconds(currentProfile.serviceTimeoutSeconds);
            await runServiceAction("Hizmet durduruluyor: " + name, "Hizmet durduruldu.", () => ServiceManager.stop(name, timeout));
        }

        private async void cmdStart_Click(object sender, EventArgs e)
        {
            string name = currentProfile.serviceName;
            TimeSpan timeout = TimeSpan.FromSeconds(currentProfile.serviceTimeoutSeconds);
            await runServiceAction("Hizmet başlatılıyor: " + name, "Hizmet başlatıldı.", () => ServiceManager.start(name, timeout));
        }

        private async Task runServiceAction(string startMessage, string doneMessage, Action action)
        {
            setBusy(true, false);
            logNow(startMessage);
            try
            {
                await Task.Run(action);
                logNow(doneMessage);
            }
            catch (Exception ex)
            {
                logNow("HATA: " + ex.Message);
            }
            finally
            {
                setBusy(false, false);
            }
        }

        private async void cmdBackup_Click(object sender, EventArgs e)
        {
            await runBackups(new List<Profile> { currentProfile });
        }

        private async void cmdBackupAll_Click(object sender, EventArgs e)
        {
            List<Profile> profiles = settings.profiles.Where(p => p.includeInSchedule).ToList();
            if (profiles.Count == 0)
            {
                MessageBox.Show(this, "Zamanlanmış yedeklemeye dahil edilmiş profil yok. Profil ayarlarından seçebilirsiniz.",
                    "YedekAl", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            await runBackups(profiles);
        }

        // Backs up the given profiles one after another, then sends one e-mail for all of them.
        private async Task runBackups(List<Profile> profiles)
        {
            setBusy(true, true);
            prgBackup.Value = 0;
            cancellation = new CancellationTokenSource();
            CancellationToken token = cancellation.Token;
            Settings current = settings;
            Progress<string> logSink = new Progress<string>(log);
            Progress<int> percent = new Progress<int>(value => prgBackup.Value = Math.Max(0, Math.Min(100, value)));

            List<BackupResult> results = null;
            try
            {
                results = await Task.Run(() =>
                {
                    using (BackupLogger logger = new BackupLogger(logSink))
                    {
                        List<BackupResult> done = new List<BackupResult>();
                        foreach (Profile profile in profiles)
                        {
                            if (token.IsCancellationRequested) break;
                            done.Add(new BackupEngine(current, profile, logger, percent).run(token));
                        }
                        MailNotifier.notifyIfNeeded(current, done, logger);
                        return done;
                    }
                });
            }
            catch (Exception ex)
            {
                logNow("HATA: " + ex.Message);
            }
            finally
            {
                setBusy(false, false);
                cancellation.Dispose();
                cancellation = null;
                loadHistory();
            }

            bool allSucceeded = results != null && results.Count == profiles.Count && results.All(r => r.success);
            if (allSucceeded && current.autoShutdown) scheduleShutdown();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            if (cancellation == null) return;
            cancellation.Cancel();
            cmdCancel.Enabled = false;
            logNow("İptal ediliyor...");
        }

        private void scheduleShutdown()
        {
            try
            {
                SystemShutdown.schedule(60, SystemShutdown.defaultMessage);
            }
            catch (Exception ex)
            {
                logNow("HATA: Bilgisayar kapatılamadı: " + ex.Message);
                return;
            }
            logNow("Bilgisayar 60 saniye içinde kapatılacak.");
            DialogResult answer = MessageBox.Show(this,
                "Yedekleme tamamlandı. Bilgisayar 60 saniye içinde kapatılacak.\r\n\r\nKapatmayı iptal etmek için İptal düğmesine basın.",
                "YedekAl", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (answer != DialogResult.Cancel) return;
            try
            {
                SystemShutdown.abort();
                logNow("Kapatma iptal edildi.");
            }
            catch (Exception ex)
            {
                logNow("HATA: Kapatma iptal edilemedi: " + ex.Message);
            }
        }

        private void setBusy(bool value, bool backup)
        {
            busy = value;
            cmdBackup.Enabled = cmdBackupAll.Enabled = !value;
            cmdSettings.Enabled = mnuProfile.Enabled = cboProfile.Enabled = !value;
            cmdCancel.Enabled = value && backup;
            cmdRestore.Enabled = !value && selectedBackup() != null;
            if (value)
            {
                cmdStart.Enabled = false;
                cmdStop.Enabled = false;
            }
            else
            {
                refreshServiceStatus();
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!busy || e.CloseReason == CloseReason.WindowsShutDown) return;
            e.Cancel = true;
            MessageBox.Show(this, "Bir işlem sürüyor. Kapatmadan önce bitmesini bekleyin ya da yedeklemeyi iptal edin.", "YedekAl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void loadHistory()
        {
            List<HistoryEntry> entries;
            try
            {
                entries = BackupHistory.load();
            }
            catch (Exception ex)
            {
                logNow("UYARI: Yedek geçmişi okunamadı: " + ex.Message);
                return;
            }

            lvHistory.BeginUpdate();
            lvHistory.Items.Clear();
            Profile profile = currentProfile;
            foreach (HistoryEntry entry in entries.Where(x => settings.belongsTo(x, profile)).OrderByDescending(x => x.start).Take(100))
            {
                bool hasFile = entry.status == BackupStatus.Success || entry.status == BackupStatus.Deleted;
                ListViewItem item = new ListViewItem(entry.start.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture));
                item.SubItems.Add(formatDuration(entry.duration));
                item.SubItems.Add(entry.size > 0 ? SystemInfo.formatSize(entry.size) : "");
                item.SubItems.Add(entry.statusText);
                item.SubItems.Add(hasFile ? Path.GetFileName(entry.path) : entry.message);
                item.ToolTipText = entry.message.Length > 0 ? entry.message : entry.path;
                if (entry.status == BackupStatus.Failed) item.ForeColor = Color.Firebrick;
                else if (entry.status != BackupStatus.Success) item.ForeColor = SystemColors.GrayText;
                item.Tag = entry;
                lvHistory.Items.Add(item);
            }
            lvHistory.EndUpdate();
            cmdRestore.Enabled = !busy && selectedBackup() != null;
        }

        private void lvHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmdRestore.Enabled = !busy && selectedBackup() != null;
        }

        // The selected history line, if it is a successful backup whose file is still there.
        private HistoryEntry selectedBackup()
        {
            if (lvHistory.SelectedItems.Count != 1) return null;
            HistoryEntry entry = lvHistory.SelectedItems[0].Tag as HistoryEntry;
            if (entry == null || entry.status != BackupStatus.Success) return null;
            return File.Exists(entry.path) || Directory.Exists(entry.path) ? entry : null;
        }

        private async void cmdRestore_Click(object sender, EventArgs e)
        {
            HistoryEntry entry = selectedBackup();
            if (entry == null) return;
            Profile profile = currentProfile;
            bool keepCurrent;
            string password;
            using (FrmRestore frmRestore = new FrmRestore(profile, entry))
            {
                if (frmRestore.ShowDialog(this) != DialogResult.OK) return;
                keepCurrent = frmRestore.keepCurrent;
                password = frmRestore.password;
            }

            setBusy(true, true);
            prgBackup.Value = 0;
            cancellation = new CancellationTokenSource();
            CancellationToken token = cancellation.Token;
            Progress<string> logSink = new Progress<string>(log);
            Progress<int> percent = new Progress<int>(value => prgBackup.Value = Math.Max(0, Math.Min(100, value)));
            RestoreResult result = null;
            try
            {
                result = await Task.Run(() =>
                {
                    using (BackupLogger logger = new BackupLogger(logSink))
                    {
                        return new RestoreEngine(profile, logger, percent).run(entry.path, password, keepCurrent, token);
                    }
                });
            }
            catch (Exception ex)
            {
                logNow("HATA: " + ex.Message);
            }
            finally
            {
                setBusy(false, false);
                cancellation.Dispose();
                cancellation = null;
            }

            if (result == null || result.cancelled) return;
            if (result.success)
                MessageBox.Show(this, "Veritabanı geri yüklendi." + (result.previousPath != null ? "\r\n\r\nEski veritabanı burada saklandı:\r\n" + result.previousPath : ""),
                    "YedekAl", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(this, "Geri yükleme tamamlanamadı:\r\n" + result.message, "YedekAl", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static string formatDuration(TimeSpan duration)
        {
            return duration.TotalHours >= 1 ? duration.ToString(@"h\:mm\:ss") : duration.ToString(@"m\:ss");
        }

        private void logNow(string message)
        {
            log(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture) + "  " + message);
        }

        private void log(string line)
        {
            txtLog.AppendText(line + "\r\n");
        }
    }
}
