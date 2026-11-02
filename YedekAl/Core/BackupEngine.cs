using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;

namespace YedekAl
{
    internal sealed class BackupResult
    {
        public bool success;
        public bool cancelled;
        public string outputPath;
        public string message;
    }

    // Runs one backup end to end. Used by both the UI and the scheduled task (/auto), on a worker thread.
    internal sealed class BackupEngine
    {
        private const string mutexName = @"Global\YedekAl_Yedekleme";
        private const string tempPrefix = ".yedekal_gecici_";

        private readonly Settings settings;
        private readonly BackupLogger log;
        private readonly IProgress<int> progress;

        public BackupEngine(Settings settings, BackupLogger log, IProgress<int> progress)
        {
            this.settings = settings;
            this.log = log;
            this.progress = progress;
        }

        private TimeSpan serviceTimeout => TimeSpan.FromSeconds(settings.serviceTimeoutSeconds);

        public BackupResult run(CancellationToken token)
        {
            Mutex mutex;
            try
            {
                mutex = createMutex();
            }
            catch (UnauthorizedAccessException)
            {
                return alreadyRunning();
            }

            using (mutex)
            {
                bool owned;
                try
                {
                    owned = mutex.WaitOne(0);
                }
                catch (AbandonedMutexException)
                {
                    owned = true;
                }
                if (!owned) return alreadyRunning();

                try
                {
                    return runCore(token);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        private BackupResult alreadyRunning()
        {
            const string message = "Başka bir yedekleme zaten çalışıyor.";
            log.error(message);
            return new BackupResult { message = message };
        }

        // Shared between the elevated UI and the SYSTEM task, so both must be allowed to open it.
        private static Mutex createMutex()
        {
            MutexSecurity security = new MutexSecurity();
            security.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), MutexRights.FullControl, AccessControlType.Allow));
            security.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), MutexRights.FullControl, AccessControlType.Allow));
            bool createdNew;
            return new Mutex(false, mutexName, out createdNew, security);
        }

        private BackupResult runCore(CancellationToken token)
        {
            HistoryEntry entry = new HistoryEntry { start = DateTime.Now, status = BackupStatus.Failed };
            BackupResult result = new BackupResult();
            Stopwatch watch = Stopwatch.StartNew();
            bool outputComplete = false;
            log.info("******** Yedekleme başladı ********");
            try
            {
                ICompressor compressor = settings.compress
                    ? Compressors.create(settings, settings.encrypt ? settings.getPassword() : null)
                    : null;
                if (compressor != null && settings.encrypt) log.info("Arşiv parolayla şifrelenecek.");
                entry.path = preflight(compressor, entry.start);
                bool serviceRestarted = createBackup(compressor, entry.path, token);
                outputComplete = true;
                entry.size = measure(entry.path);
                entry.status = BackupStatus.Success;
                log.info("Yedek boyutu: " + SystemInfo.formatSize(entry.size));
                if (serviceRestarted)
                {
                    result.success = true;
                }
                else
                {
                    result.message = entry.message = "Yedek alındı ancak hizmet yeniden başlatılamadı!";
                    log.error(result.message);
                }
            }
            catch (OperationCanceledException)
            {
                entry.status = BackupStatus.Cancelled;
                result.cancelled = true;
                result.message = entry.message = "Yedekleme iptal edildi.";
                log.warn(result.message);
            }
            catch (Exception ex)
            {
                result.message = entry.message = ex.Message;
                log.error(ex.Message);
            }
            finally
            {
                if (!outputComplete) deletePartialOutput(entry.path);
            }

            entry.duration = watch.Elapsed;
            result.outputPath = outputComplete ? entry.path : null;
            saveHistory(entry);
            if (entry.status == BackupStatus.Success) applyRetention(entry);
            log.info(result.success ? "******** Yedekleme bitti ********" : "******** Yedekleme başarısız ********");
            if (settings.notifyEnabled && !result.cancelled && (!result.success || settings.notifyOnSuccess))
                sendNotification(entry, result);
            return result;
        }

        // A failed e-mail never changes the backup result; it is only logged.
        private void sendNotification(HistoryEntry entry, BackupResult result)
        {
            try
            {
                log.info("Bildirim e-postası gönderiliyor: " + settings.mailTo);
                MailNotifier.sendReport(settings, entry, result, log.lines);
                log.info("Bildirim e-postası gönderildi.");
            }
            catch (Exception ex)
            {
                log.warn("Bildirim e-postası gönderilemedi: " + ex.Message);
            }
        }

        // Everything that can be checked without touching the service. Returns the output path.
        private string preflight(ICompressor compressor, DateTime start)
        {
            log.info("Ön kontroller yapılıyor...");
            List<string> errors = settings.validate();
            if (errors.Count > 0) throw new InvalidOperationException(string.Join(" ", errors));
            if (!SystemInfo.isAdministrator()) throw new InvalidOperationException("YedekAl yönetici yetkisiyle çalıştırılmalıdır.");
            if (!ServiceManager.getStatus(settings.serviceName).HasValue)
                throw new InvalidOperationException("'" + settings.serviceName + "' hizmeti bulunamadı.");
            if (!Directory.Exists(settings.dbPath) && !File.Exists(settings.dbPath))
                throw new InvalidOperationException("Veritabanı konumu bulunamadı: " + settings.dbPath);
            if (SourceTree.isRoot(settings.dbPath))
                throw new InvalidOperationException("Veritabanı konumu bir sürücünün kökü olamaz.");
            if (compressor != null)
            {
                string problem = compressor.checkAvailable();
                if (problem != null) throw new InvalidOperationException(problem);
            }

            string source = SourceTree.normalize(settings.dbPath);
            string backupDir = SourceTree.normalize(settings.backupPath);
            if (SourceTree.isSameOrInside(backupDir, source))
                throw new InvalidOperationException("Yedekleme konumu veritabanı klasörünün içinde olamaz.");
            Directory.CreateDirectory(backupDir);
            checkWritable(backupDir);

            string output = Path.Combine(backupDir, settings.formatArchiveName(start) + (compressor == null ? "" : compressor.extension));
            if (File.Exists(output) || Directory.Exists(output))
                throw new InvalidOperationException("Bu adla bir yedek zaten var: " + output);

            // Worst case the archive is as big as the source; copy-first needs room for the temporary copy too.
            SourceTree tree = SourceTree.scan(source);
            long required = compressor != null && settings.copyFirst ? tree.totalBytes * 2 : tree.totalBytes;
            required += required / 20;
            long? free = SystemInfo.freeSpace(backupDir);
            if (!free.HasValue)
                log.warn("Yedekleme konumundaki boş alan öğrenilemedi.");
            else if (free.Value < required)
                throw new InvalidOperationException(string.Format("Yedekleme konumunda yeterli boş alan yok. Gerekli: {0}, boş: {1}.",
                    SystemInfo.formatSize(required), SystemInfo.formatSize(free.Value)));

            log.info(string.Format("Ön kontroller tamam. Kaynak: {0}, boş alan: {1}.",
                SystemInfo.formatSize(tree.totalBytes), free.HasValue ? SystemInfo.formatSize(free.Value) : "?"));
            log.info("Hedef: " + output);
            return output;
        }

        // Returns false when the backup itself is fine but the service could not be started again.
        private bool createBackup(ICompressor compressor, string outputPath, CancellationToken token)
        {
            ServiceControllerStatus? initial = ServiceManager.getStatus(settings.serviceName);
            bool restartPending = initial.HasValue && initial.Value != ServiceControllerStatus.Stopped;
            bool restarted = true;
            string tempRoot = null;
            SourceTree tree;
            try
            {
                log.info("Hizmet durduruluyor: " + settings.serviceName);
                ServiceManager.stop(settings.serviceName, serviceTimeout);
                log.info("Hizmet durduruldu.");

                // Scan again now that the service is stopped and files no longer change.
                tree = SourceTree.scan(settings.dbPath);
                log.info(string.Format("{0} dosya, toplam {1}.", tree.files.Count, SystemInfo.formatSize(tree.totalBytes)));

                if (compressor == null)
                {
                    log.info("Dosyalar kopyalanıyor...");
                    Directory.CreateDirectory(outputPath);
                    FileCopier.copy(tree, outputPath, progress, token);
                }
                else
                {
                    SourceTree source = tree;
                    if (settings.copyFirst)
                    {
                        tempRoot = Path.Combine(SourceTree.normalize(settings.backupPath), tempPrefix + Guid.NewGuid().ToString("N"));
                        log.info("Dosyalar geçici klasöre kopyalanıyor...");
                        FileCopier.copy(tree, tempRoot, progress, token);
                        restartPending = false;
                        restarted = startService();
                        source = SourceTree.scan(Path.Combine(tempRoot, tree.rootName));
                    }
                    log.info(compressor.name + " ile sıkıştırılıyor...");
                    progress?.Report(0);
                    compressor.compress(source, outputPath, settings.strength, progress, token);
                }
            }
            finally
            {
                if (restartPending) restarted = startService();
                if (tempRoot != null) deleteTemp(tempRoot);
            }

            if (compressor != null && settings.verifyArchive)
            {
                log.info("Arşiv doğrulanıyor...");
                progress?.Report(0);
                compressor.verify(outputPath, tree.files.Count, progress, token);
                log.info("Arşiv sağlam.");
            }
            return restarted;
        }

        private bool startService()
        {
            try
            {
                log.info("Hizmet başlatılıyor: " + settings.serviceName);
                ServiceManager.start(settings.serviceName, serviceTimeout);
                log.info("Hizmet başlatıldı.");
                return true;
            }
            catch (Exception ex)
            {
                log.error("Hizmet başlatılamadı: " + ex.Message);
                return false;
            }
        }

        private void applyRetention(HistoryEntry current)
        {
            if (settings.keepLastCount <= 0 && settings.keepDays <= 0) return;
            try
            {
                List<HistoryEntry> history = BackupHistory.load();
                string backupDir = SourceTree.normalize(settings.backupPath);
                DateTime oldest = DateTime.Now.AddDays(-settings.keepDays);
                List<HistoryEntry> backups = history
                    .Where(e => e.status == BackupStatus.Success && isInBackupDir(e.path, backupDir))
                    .OrderByDescending(e => e.start)
                    .ToList();

                int deleted = 0;
                for (int i = 0; i < backups.Count; i++)
                {
                    HistoryEntry backup = backups[i];
                    if (string.Equals(backup.path, current.path, StringComparison.OrdinalIgnoreCase)) continue;
                    bool overCount = settings.keepLastCount > 0 && i >= settings.keepLastCount;
                    bool tooOld = settings.keepDays > 0 && backup.start < oldest;
                    if (!overCount && !tooOld) continue;
                    try
                    {
                        if (File.Exists(backup.path)) File.Delete(backup.path);
                        else if (Directory.Exists(backup.path)) Directory.Delete(backup.path, true);
                        backup.status = BackupStatus.Deleted;
                        backup.message = "Saklama kuralı gereği silindi.";
                        deleted++;
                        log.info("Eski yedek silindi: " + Path.GetFileName(backup.path));
                    }
                    catch (Exception ex)
                    {
                        log.warn("Eski yedek silinemedi: " + backup.path + " (" + ex.Message + ")");
                    }
                }
                if (deleted > 0) BackupHistory.save(history);
            }
            catch (Exception ex)
            {
                log.warn("Eski yedekler temizlenemedi: " + ex.Message);
            }
        }

        // Only backups that sit directly in the configured backup folder are candidates for deletion.
        private static bool isInBackupDir(string path, string backupDir)
        {
            if (string.IsNullOrEmpty(path)) return false;
            try
            {
                return string.Equals(Path.GetDirectoryName(SourceTree.normalize(path)), backupDir, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
            {
                return false;
            }
        }

        private void checkWritable(string dir)
        {
            string probe = Path.Combine(dir, ".yedekal_yazma_testi_" + Guid.NewGuid().ToString("N"));
            try
            {
                File.WriteAllText(probe, "");
                File.Delete(probe);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                throw new InvalidOperationException("Yedekleme konumuna yazılamıyor: " + ex.Message, ex);
            }
        }

        private void deleteTemp(string tempRoot)
        {
            try
            {
                if (Directory.Exists(tempRoot)) Directory.Delete(tempRoot, true);
            }
            catch (Exception ex)
            {
                log.warn("Geçici klasör silinemedi: " + tempRoot + " (" + ex.Message + ")");
            }
        }

        private void deletePartialOutput(string path)
        {
            if (path == null) return;
            try
            {
                if (File.Exists(path)) File.Delete(path);
                else if (Directory.Exists(path)) Directory.Delete(path, true);
                else return;
                log.info("Yarım kalan yedek silindi.");
            }
            catch (Exception ex)
            {
                log.warn("Yarım kalan yedek silinemedi: " + path + " (" + ex.Message + ")");
            }
        }

        private void saveHistory(HistoryEntry entry)
        {
            try
            {
                BackupHistory.append(entry);
            }
            catch (Exception ex)
            {
                log.warn("Yedek geçmişi kaydedilemedi: " + ex.Message);
            }
        }

        private static long measure(string path)
        {
            if (File.Exists(path)) return new FileInfo(path).Length;
            return new DirectoryInfo(path).EnumerateFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);
        }
    }
}
