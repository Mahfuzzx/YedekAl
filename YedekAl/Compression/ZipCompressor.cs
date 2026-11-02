using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace YedekAl
{
    internal sealed class ZipCompressor : ICompressor
    {
        private const int bufferSize = 1 << 20;
        private static readonly DateTime minZipTime = new DateTime(1980, 1, 1);
        private static readonly DateTime maxZipTime = new DateTime(2107, 12, 31);

        // .NET Framework does not check CRCs when reading, so the CRCs computed while writing are kept for verify().
        private string lastArchive;
        private Dictionary<string, uint> lastChecksums;

        public string name => "ZIP";
        public string extension => ".zip";

        public string checkAvailable() => null;

        public void compress(SourceTree tree, string archivePath, CompressionStrength strength, IProgress<int> progress, CancellationToken token)
        {
            CompressionLevel level = strength == CompressionStrength.Fast ? CompressionLevel.Fastest : CompressionLevel.Optimal;
            Dictionary<string, uint> checksums = new Dictionary<string, uint>(StringComparer.Ordinal);
            PercentReporter reporter = new PercentReporter(progress, tree.totalBytes);
            byte[] buffer = new byte[bufferSize];
            long done = 0;

            using (FileStream stream = new FileStream(archivePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                foreach (string dir in tree.directories)
                    zip.CreateEntry(entryName(dir) + "/");

                foreach (SourceFile file in tree.files)
                {
                    token.ThrowIfCancellationRequested();
                    string entryPath = entryName(file.relativePath);
                    ZipArchiveEntry entry = zip.CreateEntry(entryPath, level);
                    DateTime modified = File.GetLastWriteTime(file.fullPath);
                    if (modified >= minZipTime && modified <= maxZipTime) entry.LastWriteTime = modified;

                    Crc32 crc = new Crc32();
                    using (FileStream input = new FileStream(file.fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16, FileOptions.SequentialScan))
                    using (Stream output = entry.Open())
                    {
                        int read;
                        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            token.ThrowIfCancellationRequested();
                            output.Write(buffer, 0, read);
                            crc.update(buffer, 0, read);
                            done += read;
                            reporter.report(done);
                        }
                    }
                    checksums[entryPath] = crc.value;
                }
            }
            lastArchive = archivePath;
            lastChecksums = checksums;
        }

        public void verify(string archivePath, int expectedFiles, IProgress<int> progress, CancellationToken token)
        {
            Dictionary<string, uint> expected = string.Equals(lastArchive, archivePath, StringComparison.OrdinalIgnoreCase) ? lastChecksums : null;
            byte[] buffer = new byte[bufferSize];

            using (FileStream stream = File.OpenRead(archivePath))
            using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                List<ZipArchiveEntry> files = zip.Entries.Where(e => !e.FullName.EndsWith("/")).ToList();
                PercentReporter reporter = new PercentReporter(progress, files.Sum(e => e.Length));
                long done = 0;
                foreach (ZipArchiveEntry entry in files)
                {
                    token.ThrowIfCancellationRequested();
                    Crc32 crc = new Crc32();
                    long length = 0;
                    using (Stream input = entry.Open())
                    {
                        int read;
                        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            crc.update(buffer, 0, read);
                            length += read;
                            done += read;
                            reporter.report(done);
                        }
                    }
                    if (length != entry.Length)
                        throw new InvalidDataException(string.Format("Arşivdeki '{0}' dosyasının boyutu tutmuyor.", entry.FullName));
                    uint expectedCrc;
                    if (expected != null && (!expected.TryGetValue(entry.FullName, out expectedCrc) || expectedCrc != crc.value))
                        throw new InvalidDataException(string.Format("Arşivdeki '{0}' dosyası bozuk (CRC uyuşmuyor).", entry.FullName));
                }
                if (files.Count != expectedFiles)
                    throw new InvalidDataException(string.Format("Arşivde {0} dosya var, {1} olmalıydı.", files.Count, expectedFiles));
            }
        }

        private static string entryName(string relativePath)
        {
            return relativePath.Replace('\\', '/');
        }
    }
}
