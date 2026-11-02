using System;
using System.IO;
using System.Text;

namespace YedekAl
{
    internal sealed class SevenZipCompressor : ExternalCompressor
    {
        public SevenZipCompressor(string configuredPath) : base(configuredPath, findExe) { }

        public override string name => "7-Zip";
        public override string extension => ".7z";
        protected override Encoding outputEncoding => new UTF8Encoding(false);

        protected override string compressArguments(SourceTree tree, string archivePath, CompressionStrength strength)
        {
            int level = strength == CompressionStrength.Fast ? 1 : strength == CompressionStrength.Best ? 9 : 5;
            return string.Format("a -t7z -mx={0} -ssw -bsp1 -bso0 -sccUTF-8 -y -- {1} {2}", level, quote(archivePath), quote(tree.rootPath));
        }

        protected override string verifyArguments(string archivePath)
        {
            return "t -bsp1 -bso0 -sccUTF-8 -y -- " + quote(archivePath);
        }

        protected override string describeExitCode(int code)
        {
            switch (code)
            {
                case 1: return "bazı dosyalar işlenemedi";
                case 2: return "ölümcül hata";
                case 7: return "komut satırı hatası";
                case 8: return "yetersiz bellek";
                case 255: return "işlem durduruldu";
                default: return "bilinmeyen hata";
            }
        }

        // The bundled 7za.exe comes first, then an installed 7-Zip.
        private static string findExe()
        {
            string installDir = readRegistry(@"SOFTWARE\7-Zip", "Path64") ?? readRegistry(@"SOFTWARE\7-Zip", "Path");
            return firstExisting(
                Path.Combine(AppPaths.appDir, "7za.exe"),
                installDir == null ? null : Path.Combine(installDir, "7z.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "7-Zip", "7z.exe"));
        }
    }
}
