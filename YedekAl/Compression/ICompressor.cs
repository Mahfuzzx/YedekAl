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
        public static ICompressor create(Settings settings)
        {
            switch (settings.format)
            {
                case ArchiveFormat.Rar: return new RarCompressor(settings.rarPath);
                case ArchiveFormat.SevenZip: return new SevenZipCompressor(settings.sevenZipPath);
                default: return new ZipCompressor();
            }
        }
    }
}
