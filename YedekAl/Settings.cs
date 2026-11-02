using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace YedekAl
{
    // Machine-wide settings: the backup profiles plus what is shared by all of them (schedule, shutdown, e-mail).
    // Kept in ProgramData (not in user.config) so the scheduled task, running as SYSTEM, sees the same settings.
    public class Settings
    {
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(Settings));

        public List<Profile> profiles { get; set; } = new List<Profile>();
        // History lines written before profiles existed belong to this profile.
        public string legacyProfileId { get; set; } = "";
        public bool scheduleEnabled { get; set; }
        public string scheduleTime { get; set; } = "23:00";
        public bool autoShutdown { get; set; }
        public bool notifyEnabled { get; set; }
        public bool notifyOnSuccess { get; set; }
        public string smtpHost { get; set; } = "";
        public int smtpPort { get; set; } = 587;
        public bool smtpSsl { get; set; } = true;
        public string smtpUser { get; set; } = "";
        public string protectedSmtpPassword { get; set; } = "";
        public string mailFrom { get; set; } = "";
        public string mailTo { get; set; } = "";

        public static Settings load()
        {
            Settings settings;
            if (File.Exists(AppPaths.settingsFile))
            {
                XDocument document = XDocument.Load(AppPaths.settingsFile);
                using (XmlReader reader = document.CreateReader())
                {
                    settings = (Settings)serializer.Deserialize(reader);
                }
                if (document.Root.Element("profiles") == null)
                {
                    settings.adoptLegacyProfile(readLegacyProfile(document));
                    settings.save();
                }
            }
            else
            {
                settings = new Settings();
                settings.autoShutdown = migrateFromUserSettings(out Profile profile);
                settings.adoptLegacyProfile(profile);
                settings.save();
            }
            if (settings.profiles.Count == 0) settings.profiles.Add(new Profile());
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
            restrictAccess(AppPaths.settingsFile);
        }

        public Settings clone()
        {
            Settings copy = (Settings)MemberwiseClone();
            copy.profiles = profiles.Select(p => p.clone()).ToList();
            return copy;
        }

        public Profile findProfile(string id)
        {
            return profiles.FirstOrDefault(p => p.id == id);
        }

        internal bool belongsTo(HistoryEntry entry, Profile profile)
        {
            return entry.profileId == profile.id || (entry.profileId.Length == 0 && profile.id == legacyProfileId);
        }

        public string getSmtpPassword()
        {
            return SecretProtector.unprotect(protectedSmtpPassword, SecretProtector.smtpPassword);
        }

        public void setSmtpPassword(string password)
        {
            protectedSmtpPassword = SecretProtector.protect(password, SecretProtector.smtpPassword);
        }

        public TimeSpan getScheduleTime()
        {
            TimeSpan time;
            return TimeSpan.TryParseExact(scheduleTime, @"hh\:mm", CultureInfo.InvariantCulture, out time) ? time : new TimeSpan(23, 0, 0);
        }

        // Settings shared by all profiles; each profile is checked with Profile.validate().
        public List<string> validate()
        {
            List<string> errors = new List<string>();
            if (profiles.Count == 0) errors.Add("En az bir profil olmalıdır.");
            foreach (var group in profiles.GroupBy(p => p.name.Trim(), StringComparer.CurrentCultureIgnoreCase).Where(g => g.Count() > 1))
                errors.Add("Aynı adda birden fazla profil var: " + group.Key);
            if (scheduleEnabled && !profiles.Any(p => p.includeInSchedule))
                errors.Add("Zamanlama açık ama zamanlanmış yedeklemeye dahil edilmiş bir profil yok.");
            if (notifyEnabled) errors.AddRange(validateMail());
            return errors;
        }

        public List<string> validateMail()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(smtpHost)) errors.Add("E-posta sunucusu boş olamaz.");
            if (smtpPort < 1 || smtpPort > 65535) errors.Add("E-posta sunucusu bağlantı noktası geçersiz.");
            // System.Net.Mail only does STARTTLS; implicit TLS on 465 would hang until the timeout.
            if (smtpPort == 465) errors.Add("465 numaralı bağlantı noktası (SSL) desteklenmiyor; 587 ve güvenli bağlantı (STARTTLS) kullanın.");
            if (!isMailAddress(mailFrom)) errors.Add("Gönderen adresi geçersiz.");
            List<string> recipients = MailNotifier.splitRecipients(mailTo);
            if (recipients.Count == 0) errors.Add("En az bir alıcı adresi girilmelidir.");
            foreach (string recipient in recipients)
                if (!isMailAddress(recipient)) errors.Add("Alıcı adresi geçersiz: " + recipient);
            return errors;
        }

        private static bool isMailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            try
            {
                return new System.Net.Mail.MailAddress(value.Trim()).Address == value.Trim();
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void adoptLegacyProfile(Profile profile)
        {
            profiles.Insert(0, profile);
            legacyProfileId = profile.id;
        }

        // Before profiles, the job fields sat directly under <Settings>; they have the same names on Profile.
        private static Profile readLegacyProfile(XDocument document)
        {
            XmlSerializer legacySerializer = new XmlSerializer(typeof(Profile), new XmlRootAttribute(document.Root.Name.LocalName));
            using (XmlReader reader = document.CreateReader())
            {
                return (Profile)legacySerializer.Deserialize(reader);
            }
        }

        // The file may hold passwords, so only Administrators and SYSTEM may read it.
        private static void restrictAccess(string file)
        {
            FileSecurity security = new FileSecurity();
            security.SetAccessRuleProtection(true, false);
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), FileSystemRights.FullControl, AccessControlType.Allow));
            security.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), FileSystemRights.FullControl, AccessControlType.Allow));
            File.SetAccessControl(file, security);
        }

        // Very first run: carry over what still applies from the old per-user settings. Returns autoShutdown.
        private static bool migrateFromUserSettings(out Profile profile)
        {
            profile = new Profile();
            try
            {
                Properties.Settings old = Properties.Settings.Default;
                profile.serviceName = old.serviceName ?? "";
                profile.dbPath = old.dbPath ?? "";
                profile.compress = old.compress;
                profile.copyFirst = old.compress && old.copyFirst;

                // Old backup paths could end in date placeholders such as D:\Yedek\yedek_%y%a%g; keep the folder only.
                string oldBackupPath = old.backupPath ?? "";
                int placeholder = oldBackupPath.IndexOf('%');
                if (placeholder > 0) profile.backupPath = Path.GetDirectoryName(oldBackupPath.Substring(0, placeholder)) ?? "";
                else if (placeholder < 0) profile.backupPath = oldBackupPath;

                string oldCompressCmd = old.compressCmd ?? "";
                if (oldCompressCmd.IndexOf("rar", StringComparison.OrdinalIgnoreCase) >= 0) profile.format = ArchiveFormat.Rar;
                else if (oldCompressCmd.IndexOf("7z", StringComparison.OrdinalIgnoreCase) >= 0) profile.format = ArchiveFormat.SevenZip;
                return old.autoShutdown;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
