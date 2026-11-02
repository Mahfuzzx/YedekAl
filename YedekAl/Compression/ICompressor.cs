using System;
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

        void verify(string archivePath, int expectedFiles, IProgress<int> progress, CancellationToken token);
    }

    internal static class Compressors
    {
        // password is null for an unencrypted archive; ZIP never gets one (Settings.validate() prevents it).
        public static ICompressor create(Profile profile, string password = null)
        {
            switch (profile.format)
            {
                case ArchiveFormat.Rar: return new RarCompressor(profile.rarPath, password);
                case ArchiveFormat.SevenZip: return new SevenZipCompressor(profile.sevenZipPath, password);
                default: return new ZipCompressor();
            }
        }
    }
}
