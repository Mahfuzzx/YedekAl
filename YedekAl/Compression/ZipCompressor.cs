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

        public string name => "ZIP";
        public string extension => ".zip";

        public string checkAvailable() => null;

        public void compress(SourceTree tree, string archivePath, CompressionStrength strength, IProgress<int> progress, CancellationToken token)
        {
            CompressionLevel level = strength == CompressionStrength.Fast ? CompressionLevel.Fastest : CompressionLevel.Optimal;
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
                    ZipArchiveEntry entry = zip.CreateEntry(entryName(file.relativePath), level);
                    DateTime modified = File.GetLastWriteTime(file.fullPath);
                    if (modified >= minZipTime && modified <= maxZipTime) entry.LastWriteTime = modified;

                    using (FileStream input = new FileStream(file.fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16, FileOptions.SequentialScan))
                    using (Stream output = entry.Open())
                    {
                        int read;
                        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            token.ThrowIfCancellationRequested();
                            output.Write(buffer, 0, read);
                            done += read;
                            reporter.report(done);
                        }
                    }
                }
            }
        }

        // expectedFiles < 0 skips the file count check (restoring a backup whose source is not known).
        public void verify(string archivePath, int expectedFiles, IProgress<int> progress, CancellationToken token)
        {
            int files = readAll(archivePath, null, progress, token);
            if (expectedFiles >= 0 && files != expectedFiles)
                throw new InvalidDataException(string.Format("Arşivde {0} dosya var, {1} olmalıydı.", files, expectedFiles));
        }

        public void extract(string archivePath, string destDir, IProgress<int> progress, CancellationToken token)
        {
            readAll(archivePath, destDir, progress, token);
        }

        // Reads every entry and checks its size and CRC against the central directory.
        // With destDir set the entries are also written there. Returns the number of files.
        private static int readAll(string archivePath, string destDir, IProgress<int> progress, CancellationToken token)
        {
            Dictionary<string, uint> checksums = ZipCentralDirectory.readChecksums(archivePath);
            string destRoot = destDir == null ? null : SourceTree.normalize(destDir);
            byte[] buffer = new byte[bufferSize];

            using (FileStream stream = File.OpenRead(archivePath))
            using (ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                List<ZipArchiveEntry> files = zip.Entries.Where(e => !e.FullName.EndsWith("/")).ToList();
                PercentReporter reporter = new PercentReporter(progress, files.Sum(e => e.Length));
                long done = 0;

                if (destRoot != null)
                    foreach (ZipArchiveEntry dir in zip.Entries.Where(e => e.FullName.EndsWith("/")))
                        Directory.CreateDirectory(targetPath(destRoot, dir.FullName));

                foreach (ZipArchiveEntry entry in files)
                {
                    token.ThrowIfCancellationRequested();
                    string target = destRoot == null ? null : targetPath(destRoot, entry.FullName);
                    if (target != null) Directory.CreateDirectory(Path.GetDirectoryName(target));

                    Crc32 crc = new Crc32();
                    long length = 0;
                    using (Stream input = entry.Open())
                    using (Stream output = target == null ? null : new FileStream(target, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1 << 16))
                    {
                        int read;
                        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            token.ThrowIfCancellationRequested();
                            output?.Write(buffer, 0, read);
                            crc.update(buffer, 0, read);
                            length += read;
                            done += read;
                            reporter.report(done);
                        }
                    }
                    if (target != null) File.SetLastWriteTime(target, entry.LastWriteTime.LocalDateTime);

                    uint expectedCrc;
                    if (length != entry.Length)
                        throw new InvalidDataException(string.Format("Arşivdeki '{0}' dosyasının boyutu tutmuyor.", entry.FullName));
                    if (!checksums.TryGetValue(entry.FullName, out expectedCrc) || expectedCrc != crc.value)
                        throw new InvalidDataException(string.Format("Arşivdeki '{0}' dosyası bozuk (CRC uyuşmuyor).", entry.FullName));
                }
                return files.Count;
            }
        }

        // Refuses entry names such as "..\..\Windows\x" that would land outside the destination.
        private static string targetPath(string destRoot, string entryName)
        {
            string target = Path.GetFullPath(Path.Combine(destRoot, entryName.Replace('/', '\\').TrimEnd('\\')));
            if (!SourceTree.isSameOrInside(target, destRoot))
                throw new InvalidDataException("Arşivde hedef klasörün dışına yazmaya çalışan bir kayıt var: " + entryName);
            return target;
        }

        private static string entryName(string relativePath)
        {
            return relativePath.Replace('\\', '/');
        }
    }
}
