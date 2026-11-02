using System;
using System.Security.Cryptography;
using System.Text;

namespace YedekAl
{
    // DPAPI with machine scope, so the scheduled task (SYSTEM) can read what the elevated UI saved.
    // The settings file itself is restricted to Administrators and SYSTEM.
    internal static class SecretProtector
    {
        private static readonly byte[] entropy = Encoding.UTF8.GetBytes("YedekAl.ArchivePassword");

        public static string protect(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return "";
            byte[] data = ProtectedData.Protect(Encoding.UTF8.GetBytes(plainText), entropy, DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(data);
        }

        public static string unprotect(string protectedText)
        {
            if (string.IsNullOrEmpty(protectedText)) return "";
            try
            {
                byte[] data = ProtectedData.Unprotect(Convert.FromBase64String(protectedText), entropy, DataProtectionScope.LocalMachine);
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception ex) when (ex is CryptographicException || ex is FormatException)
            {
                throw new InvalidOperationException("Arşiv parolası çözülemedi. Ayarlardan parolayı yeniden girin.", ex);
            }
        }
    }
}
