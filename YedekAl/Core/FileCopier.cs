using System;
using System.IO;
using System.Threading;

namespace YedekAl
{
    internal static class FileCopier
    {
        private const int bufferSize = 1 << 20;

        // Copies the tree into destRoot, keeping the root folder name (destRoot\<rootName>\...).
        public static void copy(SourceTree tree, string destRoot, IProgress<int> progress, CancellationToken token)
        {
            PercentReporter reporter = new PercentReporter(progress, tree.totalBytes);
            foreach (string dir in tree.directories)
                Directory.CreateDirectory(Path.Combine(destRoot, dir));

            byte[] buffer = new byte[bufferSize];
            long done = 0;
            foreach (SourceFile file in tree.files)
            {
                token.ThrowIfCancellationRequested();
                string target = Path.Combine(destRoot, file.relativePath);
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(target));
                    using (FileStream input = new FileStream(file.fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16, FileOptions.SequentialScan))
                    using (FileStream output = new FileStream(target, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1 << 16))
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
                    File.SetLastWriteTimeUtc(target, File.GetLastWriteTimeUtc(file.fullPath));
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    throw new IOException(string.Format("'{0}' kopyalanamadı: {1}", file.fullPath, ex.Message), ex);
                }
            }
            reporter.reportPercent(100);
        }
    }
}
