using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace YedekAl
{
    public enum ArchiveFormat { Zip, Rar, SevenZip }

    public enum CompressionStrength { Fast, Normal, Best }

    // One backup job: which service to stop, what to back up, where and how.
    public class Profile
    {
        public const string defaultNameTemplate = "yedek_{0:yyyyMMdd_HHmmss}";

        public string id { get; set; } = Guid.NewGuid().ToString("N");
        public string name { get; set; } = "Varsayılan";
        public bool includeInSchedule { get; set; } = true;
        public string serviceName { get; set; } = "";
        public string dbPath { get; set; } = "";
        public string backupPath { get; set; } = "";
        public string nameTemplate { get; set; } = defaultNameTemplate;
        public bool compress { get; set; } = true;
        public ArchiveFormat format { get; set; } = ArchiveFormat.Zip;
        public CompressionStrength strength { get; set; } = CompressionStrength.Normal;
        public bool copyFirst { get; set; }
        public bool verifyArchive { get; set; } = true;
        public bool encrypt { get; set; }
        public string protectedPassword { get; set; } = "";
        public string rarPath { get; set; } = "";
        public string sevenZipPath { get; set; } = "";
        public int keepLastCount { get; set; }
        public int keepDays { get; set; }
        public int serviceTimeoutSeconds { get; set; } = 60;

        public Profile clone()
        {
            return (Profile)MemberwiseClone();
        }

        // A copy that is a separate profile: new id, and history/retention start from scratch.
        public Profile duplicate(string newName)
        {
            Profile copy = clone();
            copy.id = Guid.NewGuid().ToString("N");
            copy.name = newName;
            return copy;
        }

        public override string ToString()
        {
            return name;
        }

        public string getPassword()
        {
            return SecretProtector.unprotect(protectedPassword, SecretProtector.archivePassword);
        }

        public void setPassword(string password)
        {
            protectedPassword = SecretProtector.protect(password, SecretProtector.archivePassword);
        }

        /// <summary>Null when the password is acceptable, otherwise the reason it is not.</summary>
        public static string checkPassword(string password)
        {
            if (password.Length < 8) return "Parola en az 8 karakter olmalıdır.";
            // Keeps the value safe to pass as a quoted command line argument to rar.exe / 7za.exe.
            if (password.Contains("\"") || password.EndsWith("\\")) return "Parola \" karakteri içeremez ve \\ ile bitemez.";
            return null;
        }

        public string formatArchiveName(DateTime time)
        {
            return string.Format(CultureInfo.InvariantCulture, nameTemplate, time);
        }

        public List<string> validate()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(name)) errors.Add("Profil adı boş olamaz.");
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
                    string archiveName = formatArchiveName(DateTime.Now);
                    if (archiveName.Trim().Length == 0 || archiveName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                        errors.Add("Arşiv adı, dosya adlarında kullanılamayan karakterler içeriyor.");
                }
                catch (FormatException)
                {
                    errors.Add("Arşiv adı şablonu hatalı. Örnek: " + defaultNameTemplate);
                }
            }
            if (encrypt)
            {
                if (!compress || format == ArchiveFormat.Zip) errors.Add("Şifreleme yalnızca 7z ve RAR biçimlerinde kullanılabilir.");
                if (string.IsNullOrEmpty(protectedPassword)) errors.Add("Şifreleme için bir parola girilmelidir.");
            }
            if (keepLastCount < 0 || keepDays < 0) errors.Add("Saklama değerleri negatif olamaz.");
            if (serviceTimeoutSeconds <= 0) errors.Add("Hizmet zaman aşımı sıfırdan büyük olmalıdır.");
            return errors;
        }
    }
}
