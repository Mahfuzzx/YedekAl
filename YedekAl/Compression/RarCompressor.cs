using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace YedekAl
{
    internal sealed class RarCompressor : ExternalCompressor
    {
        public RarCompressor(string configuredPath, string password = null) : base(configuredPath, findExe, password) { }

        public override string name => "RAR";
        public override string extension => ".rar";
        protected override Encoding outputEncoding => Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);

        protected override string compressArguments(SourceTree tree, string archivePath, CompressionStrength strength)
        {
            int level = strength == CompressionStrength.Fast ? 1 : strength == CompressionStrength.Best ? 5 : 3;
            string solid = strength == CompressionStrength.Fast ? "" : " -s";
            // -hp encrypts the file names as well as the data.
            return string.Format("a -ep1 -r -m{0}{1}{2} -idc -y -- {3} {4}", level, solid, passwordSwitch("-hp"), quote(archivePath), quote(tree.rootPath));
        }

        protected override string verifyArguments(string archivePath)
        {
            return "t" + passwordSwitch("-p") + " -idc -y -- " + quote(archivePath);
        }

        protected override string extractArguments(string archivePath)
        {
            return "x" + passwordSwitch("-p") + " -idc -y -- " + quote(archivePath);
        }

        // rar.exe only prints per-file percentages, so progress is counted by lines that end with "OK".
        protected override Action<string> createProgressParser(int fileCount, IProgress<int> progress)
        {
            PercentReporter reporter = new PercentReporter(progress, fileCount);
            StringBuilder line = new StringBuilder();
            int done = 0;
            return chunk =>
            {
                foreach (char c in chunk)
                {
                    if (c != '\n')
                    {
                        line.Append(c);
                        continue;
                    }
                    if (line.ToString().TrimEnd().EndsWith("OK")) reporter.report(++done);
                    line.Clear();
                }
            };
        }

        protected override string describeExitCode(int code)
        {
            switch (code)
            {
                case 1: return "bazı dosyalar işlenemedi";
                case 2: return "ölümcül hata";
                case 3: return "CRC hatası";
                case 4: return "arşiv kilitli";
                case 5: return "yazma hatası";
                case 6: return "dosya açma hatası";
                case 7: return "komut satırı hatası";
                case 8: return "yetersiz bellek";
                case 9: return "dosya oluşturma hatası";
                case 10: return "işlenecek dosya yok";
                case 255: return "işlem durduruldu";
                default: return "bilinmeyen hata";
            }
        }

        private static string findExe()
        {
            string winRar = readRegistry(@"SOFTWARE\WinRAR", "exe64") ?? readRegistry(@"SOFTWARE\WinRAR", "exe32");
            return firstExisting(
                winRar == null ? null : Path.Combine(Path.GetDirectoryName(winRar), "Rar.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WinRAR", "Rar.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "WinRAR", "Rar.exe"));
        }
    }
}
