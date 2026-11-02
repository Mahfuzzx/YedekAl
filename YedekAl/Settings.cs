using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace YedekAl
{
    public enum ArchiveFormat { Zip, Rar, SevenZip }

    public enum CompressionStrength { Fast, Normal, Best }

    // Kept in ProgramData (not in user.config) so the scheduled task, running as SYSTEM, sees the same settings.
    public class Settings
    {
        public const string defaultNameTemplate = "yedek_{0:yyyyMMdd_HHmmss}";
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(Settings));

        public string serviceName { get; set; } = "";
        public string dbPath { get; set; } = "";
        public string backupPath { get; set; } = "";
        public string nameTemplate { get; set; } = defaultNameTemplate;
        public bool compress { get; set; } = true;
        public ArchiveFormat format { get; set; } = ArchiveFormat.Zip;
        public CompressionStrength strength { get; set; } = CompressionStrength.Normal;
        public bool copyFirst { get; set; }
        public bool verifyArchive { get; set; } = true;
        public string rarPath { get; set; } = "";
        public string sevenZipPath { get; set; } = "";
        public int keepLastCount { get; set; }
        public int keepDays { get; set; }
        public bool scheduleEnabled { get; set; }
        public string scheduleTime { get; set; } = "23:00";
        public bool autoShutdown { get; set; }
        public int serviceTimeoutSeconds { get; set; } = 60;

        public static Settings load()
        {
            if (File.Exists(AppPaths.settingsFile))
            {
                using (FileStream stream = File.OpenRead(AppPaths.settingsFile))
                {
                    return (Settings)serializer.Deserialize(stream);
                }
            }
            Settings settings = migrateFromUserSettings();
            settings.save();
            return settings;
        }

        public void save()
        {
            Directory.CreateDirectory(AppPaths.dataDir);
            string tempFile = AppPaths.settingsFile + ".tmp";
            using (FileStream stream = File.Create(tempFile))
            {
                serializer.Serialize(stream, this);
            }
            if (File.Exists(AppPaths.settingsFile)) File.Replace(tempFile, AppPaths.settingsFile, null);
            else File.Move(tempFile, AppPaths.settingsFile);
        }

        public Settings clone()
        {
            return (Settings)MemberwiseClone();
        }

        public string formatArchiveName(DateTime time)
        {
            return string.Format(CultureInfo.InvariantCulture, nameTemplate, time);
        }

        public TimeSpan getScheduleTime()
        {
            TimeSpan time;
            return TimeSpan.TryParseExact(scheduleTime, @"hh\:mm", CultureInfo.InvariantCulture, out time) ? time : new TimeSpan(23, 0, 0);
        }

        public List<string> validate()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(serviceName)) errors.Add("Hizmet adı boş olamaz.");
            if (string.IsNullOrWhiteSpace(dbPath)) errors.Add("Veritabanı konumu boş olamaz.");
            if (string.IsNullOrWhiteSpace(backupPath)) errors.Add("Yedekleme konumu boş olamaz.");
            if (string.IsNullOrWhiteSpace(nameTemplate) || !nameTemplate.Contains("{0"))
            {
                errors.Add("Arşiv adı, " + defaultNameTemplate + " örneğindeki gibi bir tarih alanı içermelidir; yoksa her yedek aynı adı alır.");
            }
            else
            {
                try
                {
                    string name = formatArchiveName(DateTime.Now);
                    if (name.Trim().Length == 0 || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                        errors.Add("Arşiv adı, dosya adlarında kullanılamayan karakterler içeriyor.");
                }
                catch (FormatException)
                {
                    errors.Add("Arşiv adı şablonu hatalı. Örnek: " + defaultNameTemplate);
                }
            }
            if (keepLastCount < 0 || keepDays < 0) errors.Add("Saklama değerleri negatif olamaz.");
            if (serviceTimeoutSeconds <= 0) errors.Add("Hizmet zaman aşımı sıfırdan büyük olmalıdır.");
            return errors;
        }

        // First run after the update: carry over what still applies from the old per-user settings.
        private static Settings migrateFromUserSettings()
        {
            Settings settings = new Settings();
            try
            {
                Properties.Settings old = Properties.Settings.Default;
                settings.serviceName = old.serviceName ?? "";
                settings.dbPath = old.dbPath ?? "";
                settings.compress = old.compress;
                settings.copyFirst = old.compress && old.copyFirst;
                settings.autoShutdown = old.autoShutdown;

                // Old backup paths could end in date placeholders such as D:\Yedek\yedek_%y%a%g; keep the folder only.
                string oldBackupPath = old.backupPath ?? "";
                int placeholder = oldBackupPath.IndexOf('%');
                if (placeholder > 0) settings.backupPath = Path.GetDirectoryName(oldBackupPath.Substring(0, placeholder)) ?? "";
                else if (placeholder < 0) settings.backupPath = oldBackupPath;

                string oldCompressCmd = old.compressCmd ?? "";
                if (oldCompressCmd.IndexOf("rar", StringComparison.OrdinalIgnoreCase) >= 0) settings.format = ArchiveFormat.Rar;
                else if (oldCompressCmd.IndexOf("7z", StringComparison.OrdinalIgnoreCase) >= 0) settings.format = ArchiveFormat.SevenZip;
            }
            catch (Exception) { }
            return settings;
        }
    }
}
