using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Threading;

namespace YedekAl
{
    internal sealed class RestoreResult
    {
        public bool success;
        public bool cancelled;
        public string message;
        // Where the database that was replaced now lives, when it was kept.
        public string previousPath;
    }

    // Puts a backup back in place of the profile's database:
    // 1. check and unpack the backup next to the database while the service keeps running,
    // 2. stop the service, rename the current database aside, move the restored copy in, start the service.
    // If the move fails the old database is moved back, so the service never starts on a half-restored folder.
    internal sealed class RestoreEngine
    {
        private const string workPrefix = ".yedekal_geriyukleme_";
        private const string previousMarker = ".yedekal_onceki_";

        private readonly Profile profile;
        private readonly BackupLogger log;
        private readonly IProgress<int> progress;

        public RestoreEngine(Profile profile, BackupLogger log, IProgress<int> progress)
        {
            this.profile = profile;
            this.log = log;
            this.progress = progress;
        }

        private TimeSpan serviceTimeout => TimeSpan.FromSeconds(profile.serviceTimeoutSeconds);

        // password: null uses the profile's stored archive password (if any).
        public RestoreResult run(string backupPath, string password, bool keepCurrent, CancellationToken token)
        {
            using (OperationLock operationLock = OperationLock.tryAcquire())
            {
                if (operationLock == null)
                {
                    log.error(OperationLock.busyMessage);
                    return new RestoreResult { message = OperationLock.busyMessage };
                }
                return runCore(backupPath, password, keepCurrent, token);
            }
        }

        private RestoreResult runCore(string backupPath, string password, bool keepCurrent, CancellationToken token)
        {
            RestoreResult result = new RestoreResult();
            string workDir = null;
            log.info("******** Geri yükleme başladı: " + profile.name + " ********");
            log.info("Yedek: " + backupPath);
            try
            {
                if (password == null && profile.encrypt) password = profile.getPassword();
                string target = SourceTree.normalize(profile.dbPath);
                ICompressor compressor = Compressors.forBackup(backupPath, profile, password);
                preflight(compressor, backupPath, target);

                workDir = Path.Combine(Path.GetDirectoryName(target), workPrefix + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(workDir);
                string restored = unpack(compressor, backupPath, workDir, token);
                checkKind(restored, target);
                token.ThrowIfCancellationRequested();

                // From here on the database is being replaced; cancelling is no longer honoured.
                bool restarted;
                string previous = replace(restored, target, out restarted);
                if (!restarted)
                {
                    result.message = "Veritabanı geri yüklendi ancak hizmet yeniden başlatılamadı!";
                    log.error(result.message);
                }
                else
                {
                    result.success = true;
                }

                // The replaced database is only thrown away once the service is back up on the restored one.
                if (previous != null && !keepCurrent && restarted)
                {
                    deletePath(previous, "Eski veritabanı silinemedi");
                    previous = null;
                }
                if (previous != null) log.info("Eski veritabanı saklandı: " + previous);
                result.previousPath = previous;
            }
            catch (OperationCanceledException)
            {
                result.cancelled = true;
                result.message = "Geri yükleme iptal edildi; veritabanına dokunulmadı.";
                log.warn(result.message);
            }
            catch (Exception ex)
            {
                result.message = ex.Message;
                log.error(ex.Message);
            }
            finally
            {
                if (workDir != null) deletePath(workDir, "Geçici klasör silinemedi");
            }
            log.info(result.success ? "******** Geri yükleme bitti ********" : "******** Geri yükleme başarısız ********");
            return result;
        }

        private void preflight(ICompressor compressor, string backupPath, string target)
        {
            log.info("Ön kontroller yapılıyor...");
            if (!SystemInfo.isAdministrator()) throw new InvalidOperationException("YedekAl yönetici yetkisiyle çalıştırılmalıdır.");
            if (!File.Exists(backupPath) && !Directory.Exists(backupPath)) throw new InvalidOperationException("Yedek bulunamadı: " + backupPath);
            if (!ServiceManager.getStatus(profile.serviceName).HasValue)
                throw new InvalidOperationException("'" + profile.serviceName + "' hizmeti bulunamadı.");
            if (SourceTree.isRoot(target)) throw new InvalidOperationException("Veritabanı konumu bir sürücünün kökü olamaz.");
            string parent = Path.GetDirectoryName(target);
            if (!Directory.Exists(parent)) throw new InvalidOperationException("Veritabanının bulunduğu klasör yok: " + parent);
            if (SourceTree.isSameOrInside(SourceTree.normalize(backupPath), target))
                throw new InvalidOperationException("Yedek, geri yüklenecek veritabanı klasörünün içinde olamaz.");
            if (compressor != null)
            {
                string problem = compressor.checkAvailable();
                if (problem != null) throw new InvalidOperationException(problem);
            }

            // The restored copy is unpacked next to the database, so that volume needs room for all of it.
            long required = estimateSize(backupPath);
            long? free = SystemInfo.freeSpace(parent);
            if (free.HasValue && free.Value < required + required / 20)
                throw new InvalidOperationException(string.Format("Veritabanı diskinde yeterli boş alan yok. Gerekli: {0}, boş: {1}.",
                    SystemInfo.formatSize(required), SystemInfo.formatSize(free.Value)));
            log.info("Ön kontroller tamam. Geri yüklenecek boyut: en az " + SystemInfo.formatSize(required) + ".");
        }

        // Exact for folder copies and ZIP; for 7z/RAR the archive size is a lower bound.
        private static long estimateSize(string backupPath)
        {
            if (Directory.Exists(backupPath))
                return new DirectoryInfo(backupPath).EnumerateFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);
            if (string.Equals(Path.GetExtension(backupPath), ".zip", StringComparison.OrdinalIgnoreCase))
            {
                using (FileStream stream = File.OpenRead(backupPath))
                using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    return zip.Entries.Sum(e => e.Length);
                }
            }
            return new FileInfo(backupPath).Length;
        }

        // Returns the restored database (the single top-level item the backup contains).
        private string unpack(ICompressor compressor, string backupPath, string workDir, CancellationToken token)
        {
            if (compressor == null)
            {
                log.info("Yedek klasörü kopyalanıyor...");
                SourceTree tree = SourceTree.scan(singleItem(backupPath));
                FileCopier.copy(tree, workDir, progress, token);
            }
            else
            {
                log.info("Yedek doğrulanıyor...");
                progress?.Report(0);
                compressor.verify(backupPath, -1, progress, token);
                log.info("Yedek sağlam. Açılıyor...");
                progress?.Report(0);
                compressor.extract(backupPath, workDir, progress, token);
            }
            return singleItem(workDir);
        }

        // Backups always hold exactly one root: the database folder (or file) itself.
        private static string singleItem(string folder)
        {
            string[] items = Directory.GetFileSystemEntries(folder);
            if (items.Length != 1)
                throw new InvalidDataException("Yedekte tek bir veritabanı klasörü bekleniyordu, " + items.Length + " öğe bulundu.");
            return items[0];
        }

        private static void checkKind(string restored, string target)
        {
            if ((Directory.Exists(target) && !Directory.Exists(restored)) || (File.Exists(target) && !File.Exists(restored)))
                throw new InvalidDataException("Yedeğin içeriği veritabanı konumuyla uyuşmuyor (klasör / dosya farkı).");
        }

        // Swaps the restored copy in with the service stopped. Returns where the old database went (null if there was none).
        private string replace(string restored, string target, out bool restarted)
        {
            ServiceControllerStatus? initial = ServiceManager.getStatus(profile.serviceName);
            bool restartNeeded = initial.HasValue && initial.Value != ServiceControllerStatus.Stopped;
            restarted = true;
            string previous = null;
            try
            {
                try
                {
                    if (copyPermissions(target, restored)) log.info("Dosya izinleri mevcut veritabanından kopyalandı.");
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException || ex is InvalidOperationException)
                {
                    log.warn("Dosya izinleri kopyalanamadı: " + ex.Message);
                }
                log.info("Hizmet durduruluyor: " + profile.serviceName);
                ServiceManager.stop(profile.serviceName, serviceTimeout);
                log.info("Hizmet durduruldu.");
                previous = swap(restored, target, DateTime.Now);
                log.info("Veritabanı geri yüklendi: " + target);
            }
            finally
            {
                if (restartNeeded) restarted = startService();
            }
            return previous;
        }

        // Renames the current database aside and moves the restored copy into its place. If the second move
        // fails, the first is undone so the database is left exactly as it was. Returns the aside path, or null.
        internal static string swap(string restored, string target, DateTime now)
        {
            string previous = null;
            if (File.Exists(target) || Directory.Exists(target))
            {
                previous = target + previousMarker + now.ToString("yyyyMMdd_HHmmss");
                move(target, previous);
            }
            try
            {
                move(restored, target);
            }
            catch (Exception ex)
            {
                if (previous == null) throw;
                try
                {
                    move(previous, target);
                }
                catch (Exception rollback)
                {
                    throw new InvalidOperationException(string.Format(
                        "Geri yükleme başarısız oldu ve eski veritabanı yerine taşınamadı! Eski veritabanı: {0}. Hata: {1} / {2}",
                        previous, ex.Message, rollback.Message), ex);
                }
                throw new InvalidOperationException("Geri yüklenen veritabanı yerine taşınamadı; mevcut veritabanı yerinde bırakıldı. Hata: " + ex.Message, ex);
            }
            return previous;
        }

        // Extracted files inherit the permissions of the parent folder; give them the database's own permissions instead.
        // Returns false when there is no current database to copy from.
        internal static bool copyPermissions(string from, string to)
        {
            // A security object read from another path counts as unmodified and SetAccessControl would write nothing,
            // so the descriptor is copied into a fresh object, which marks the access rules as changed.
            if (Directory.Exists(from))
            {
                DirectorySecurity security = new DirectorySecurity();
                security.SetSecurityDescriptorBinaryForm(Directory.GetAccessControl(from, AccessControlSections.Access).GetSecurityDescriptorBinaryForm(), AccessControlSections.Access);
                Directory.SetAccessControl(to, security);
            }
            else if (File.Exists(from))
            {
                FileSecurity security = new FileSecurity();
                security.SetSecurityDescriptorBinaryForm(File.GetAccessControl(from, AccessControlSections.Access).GetSecurityDescriptorBinaryForm(), AccessControlSections.Access);
                File.SetAccessControl(to, security);
            }
            else
            {
                return false;
            }
            return true;
        }

        private static void move(string from, string to)
        {
            if (Directory.Exists(from)) Directory.Move(from, to);
            else File.Move(from, to);
        }

        private bool startService()
        {
            try
            {
                log.info("Hizmet başlatılıyor: " + profile.serviceName);
                ServiceManager.start(profile.serviceName, serviceTimeout);
                log.info("Hizmet başlatıldı.");
                return true;
            }
            catch (Exception ex)
            {
                log.error("Hizmet başlatılamadı: " + ex.Message);
                return false;
            }
        }

        private void deletePath(string path, string failure)
        {
            try
            {
                if (Directory.Exists(path)) Directory.Delete(path, true);
                else if (File.Exists(path)) File.Delete(path);
            }
            catch (Exception ex)
            {
                log.warn(failure + ": " + path + " (" + ex.Message + ")");
            }
        }
    }
}
