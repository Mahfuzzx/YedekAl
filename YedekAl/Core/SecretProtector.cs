using System;
using System.Security.Cryptography;
using System.Text;

namespace YedekAl
{
    // DPAPI with machine scope, so the scheduled task (SYSTEM) can read what the elevated UI saved.
    // The settings file itself is restricted to Administrators and SYSTEM.
    internal static class SecretProtector
    {
        public const string archivePassword = "ArchivePassword";
        public const string smtpPassword = "SmtpPassword";

        public static string protect(string plainText, string purpose)
        {
            if (string.IsNullOrEmpty(plainText)) return "";
            byte[] data = ProtectedData.Protect(Encoding.UTF8.GetBytes(plainText), entropyFor(purpose), DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(data);
        }

        public static string unprotect(string protectedText, string purpose)
        {
            if (string.IsNullOrEmpty(protectedText)) return "";
            try
            {
                byte[] data = ProtectedData.Unprotect(Convert.FromBase64String(protectedText), entropyFor(purpose), DataProtectionScope.LocalMachine);
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception ex) when (ex is CryptographicException || ex is FormatException)
            {
                string name = purpose == smtpPassword ? "E-posta sunucusu parolası" : "Arşiv parolası";
                throw new InvalidOperationException(name + " çözülemedi. Ayarlardan parolayı yeniden girin.", ex);
            }
        }

        private static byte[] entropyFor(string purpose)
        {
            return Encoding.UTF8.GetBytes("YedekAl." + purpose);
        }
    }
}
