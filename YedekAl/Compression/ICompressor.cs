using System;
using System.IO;
using System.Threading;

namespace YedekAl
{
    internal interface ICompressor
    {
        string name { get; }
        string extension { get; }

        /// <summary>Null when the compressor can be used, otherwise the reason it cannot.</summary>
        string checkAvailable();

        void compress(SourceTree tree, string archivePath, CompressionStrength strength, IProgress<int> progress, CancellationToken token);

        /// <summary>expectedFiles &lt; 0 when the file count is not known.</summary>
        void verify(string archivePath, int expectedFiles, IProgress<int> progress, CancellationToken token);

        /// <summary>Extracts into destDir, which must exist; the archive's root folder ends up directly under it.</summary>
        void extract(string archivePath, string destDir, IProgress<int> progress, CancellationToken token);
    }

    internal static class Compressors
    {
        // password is null for an unencrypted archive; ZIP never gets one (Settings.validate() prevents it).
        public static ICompressor create(Profile profile, string password = null)
        {
            return create(profile.format, profile, password);
        }

        // For restoring: the archive's own extension decides, since the profile may have changed format since.
        // Returns null for a plain folder copy (a backup taken without compression).
        public static ICompressor forBackup(string backupPath, Profile profile, string password)
        {
            if (Directory.Exists(backupPath)) return null;
            switch (Path.GetExtension(backupPath).ToLowerInvariant())
            {
                case ".rar": return create(ArchiveFormat.Rar, profile, password);
                case ".7z": return create(ArchiveFormat.SevenZip, profile, password);
                case ".zip": return create(ArchiveFormat.Zip, profile, password);
                default: throw new InvalidOperationException("Yedek biçimi tanınmadı: " + backupPath);
            }
        }

        private static ICompressor create(ArchiveFormat format, Profile profile, string password)
        {
            switch (format)
            {
                case ArchiveFormat.Rar: return new RarCompressor(profile.rarPath, password);
                case ArchiveFormat.SevenZip: return new SevenZipCompressor(profile.sevenZipPath, password);
                default: return new ZipCompressor();
            }
        }
    }
}
