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
            refreshServiceStatus();
            loadHistory();
        }

        private void cmdSettings_Click(object sender, EventArgs e)
        {
            using (FrmSettings frmSettings = new FrmSettings(settings))
            {
                if (frmSettings.ShowDialog(this) == DialogResult.OK)
                {
                    settings = frmSettings.settings;
                    refreshServiceStatus();
                }
            }
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
            ServiceControllerStatus? status = ServiceManager.getStatus(settings.serviceName);
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
            string name = settings.serviceName;
            TimeSpan timeout = TimeSpan.FromSeconds(settings.serviceTimeoutSeconds);
            await runServiceAction("Hizmet durduruluyor: " + name, "Hizmet durduruldu.", () => ServiceManager.stop(name, timeout));
        }

        private async void cmdStart_Click(object sender, EventArgs e)
        {
            string name = settings.serviceName;
            TimeSpan timeout = TimeSpan.FromSeconds(settings.serviceTimeoutSeconds);
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
            setBusy(true, true);
            prgBackup.Value = 0;
            cancellation = new CancellationTokenSource();
            CancellationToken token = cancellation.Token;
            Settings current = settings;
            Progress<string> logSink = new Progress<string>(log);
            Progress<int> percent = new Progress<int>(value => prgBackup.Value = Math.Max(0, Math.Min(100, value)));

            BackupResult result = null;
            try
            {
                result = await Task.Run(() =>
                {
                    using (BackupLogger logger = new BackupLogger(logSink))
                    {
                        return new BackupEngine(current, logger, percent).run(token);
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

            if (result != null && result.success && current.autoShutdown) scheduleShutdown();
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
            cmdBackup.Enabled = !value;
            cmdSettings.Enabled = !value;
            cmdCancel.Enabled = value && backup;
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
            foreach (HistoryEntry entry in entries.OrderByDescending(x => x.start).Take(100))
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
                lvHistory.Items.Add(item);
            }
            lvHistory.EndUpdate();
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
