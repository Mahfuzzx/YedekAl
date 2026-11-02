using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YedekAl.Tests
{
    [TestClass]
    public class CompressorTests
    {
        private static readonly CancellationToken none = CancellationToken.None;

        // The bundled 7za.exe is copied next to the test assembly along with YedekAl.exe.
        private static string sevenZipExe()
        {
            string exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7za.exe");
            if (!File.Exists(exe)) Assert.Inconclusive("7za.exe bulunamadı: " + exe);
            return exe;
        }

        [TestMethod]
        public void Zip_CompressVerifyExtract()
        {
            using (TestFolder folder = new TestFolder())
            {
                string db = folder.createDatabase();
                SourceTree tree = SourceTree.scan(db);
                string archive = folder.sub("yedek.zip");
                new ZipCompressor().compress(tree, archive, CompressionStrength.Normal, null, none);

                Assert.AreEqual(tree.files.Count + tree.directories.Count, ZipCentralDirectory.readChecksums(archive).Count);
                new ZipCompressor().verify(archive, tree.files.Count, null, none);
                Assert.ThrowsException<InvalidDataException>(() => new ZipCompressor().verify(archive, tree.files.Count + 1, null, none));

                string output = folder.sub("acilan");
                Directory.CreateDirectory(output);
                new ZipCompressor().extract(archive, output, null, none);
                Assert.IsTrue(TestFolder.sameTree(db, Path.Combine(output, "Veri Tabanı")));
            }
        }

        [TestMethod]
        public void Zip_FreshVerifyCatchesCorruption()
        {
            using (TestFolder folder = new TestFolder())
            {
                SourceTree tree = SourceTree.scan(folder.createDatabase());
                string archive = folder.sub("yedek.zip");
                new ZipCompressor().compress(tree, archive, CompressionStrength.Normal, null, none);
                byte[] bytes = File.ReadAllBytes(archive);

                bytes[bytes.Length / 2] ^= 0xFF;   // inside the incompressible 3 MB file, stored as-is
                string flipped = folder.sub("bozuk.zip");
                File.WriteAllBytes(flipped, bytes);
                InvalidDataException crc = Assert.ThrowsException<InvalidDataException>(() => new ZipCompressor().verify(flipped, -1, null, none));
                StringAssert.Contains(crc.Message, "CRC");

                string truncated = folder.sub("eksik.zip");
                File.WriteAllBytes(truncated, bytes.Take(bytes.Length / 2).ToArray());
                Assert.ThrowsException<InvalidDataException>(() => new ZipCompressor().verify(truncated, -1, null, none));
            }
        }

        [TestMethod]
        public void Zip_Zip64WithManyEntries()
        {
            using (TestFolder folder = new TestFolder())
            {
                string archive = folder.sub("cok.zip");
                using (ZipArchive zip = new ZipArchive(File.Create(archive), ZipArchiveMode.Create))
                    for (int i = 0; i < 66000; i++)
                        using (Stream entry = zip.CreateEntry("d/f" + i).Open()) entry.WriteByte((byte)i);

                Assert.AreEqual(66000, ZipCentralDirectory.readChecksums(archive).Count);
                new ZipCompressor().verify(archive, 66000, null, none);
            }
        }

        [TestMethod]
        public void Zip_ExtractRefusesPathTraversal()
        {
            using (TestFolder folder = new TestFolder())
            {
                string archive = folder.sub("kotu.zip");
                using (ZipArchive zip = new ZipArchive(File.Create(archive), ZipArchiveMode.Create))
                using (StreamWriter writer = new StreamWriter(zip.CreateEntry("../../kacak.txt").Open()))
                    writer.Write("x");
                string output = folder.sub("a", "b");
                Directory.CreateDirectory(output);

                InvalidDataException error = Assert.ThrowsException<InvalidDataException>(() => new ZipCompressor().extract(archive, output, null, none));
                StringAssert.Contains(error.Message, "dışına");
                Assert.IsFalse(File.Exists(folder.sub("kacak.txt")));
            }
        }

        [TestMethod]
        public void SevenZip_EncryptedRoundTrip()
        {
            string exe = sevenZipExe();
            using (TestFolder folder = new TestFolder())
            {
                string db = folder.createDatabase();
                SourceTree tree = SourceTree.scan(db);
                SevenZipCompressor compressor = new SevenZipCompressor(exe, "Uzun Parola 2026");
                string archive = folder.sub("yedek.7z");
                compressor.compress(tree, archive, CompressionStrength.Fast, null, none);
                compressor.verify(archive, -1, null, none);

                string output = folder.sub("acilan");
                Directory.CreateDirectory(output);
                compressor.extract(archive, output, null, none);
                Assert.IsTrue(TestFolder.sameTree(db, Path.Combine(output, "Veri Tabanı")));

                Assert.ThrowsException<InvalidOperationException>(() => new SevenZipCompressor(exe, "yanlış parola").verify(archive, -1, null, none));
                Assert.ThrowsException<InvalidOperationException>(() => new SevenZipCompressor(exe).verify(archive, -1, null, none), "no password must fail, not hang");

                Process list = Process.Start(new ProcessStartInfo(exe, "l -p- -ba \"" + archive + "\"") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true });
                string listing = list.StandardOutput.ReadToEnd();
                list.WaitForExit();
                Assert.IsFalse(listing.Contains("buyuk.bin"), "file names must be encrypted too");
            }
        }

        [TestMethod]
        public void SevenZip_CancelKillsTheProcess()
        {
            string exe = sevenZipExe();
            using (TestFolder folder = new TestFolder())
            {
                string db = folder.sub("db");
                Directory.CreateDirectory(db);
                byte[] big = new byte[200 << 20];
                new Random(2).NextBytes(big);
                File.WriteAllBytes(Path.Combine(db, "big.bin"), big);
                int before = Process.GetProcessesByName("7za").Length;

                using (CancellationTokenSource cancel = new CancellationTokenSource(1000))
                {
                    Stopwatch watch = Stopwatch.StartNew();
                    Assert.ThrowsException<OperationCanceledException>(() =>
                        new SevenZipCompressor(exe).compress(SourceTree.scan(db), folder.sub("c.7z"), CompressionStrength.Best, null, cancel.Token));
                    Assert.IsTrue(watch.Elapsed < TimeSpan.FromSeconds(20));
                }
                Assert.AreEqual(before, Process.GetProcessesByName("7za").Length);
            }
        }

        [TestMethod]
        public void Rar_MissingProgramIsReported()
        {
            RarCompressor rar = new RarCompressor(@"C:\yok\Rar.exe");
            StringAssert.Contains(rar.checkAvailable(), "bulunamadı");
        }

        [TestMethod]
        public void ForBackup_ChoosesByExtension()
        {
            Profile profile = new Profile();
            Assert.IsInstanceOfType(Compressors.forBackup(@"E:\y\a.zip", profile, null), typeof(ZipCompressor));
            Assert.IsInstanceOfType(Compressors.forBackup(@"E:\y\a.7z", profile, null), typeof(SevenZipCompressor));
            Assert.IsInstanceOfType(Compressors.forBackup(@"E:\y\a.RAR", profile, null), typeof(RarCompressor));
            Assert.ThrowsException<InvalidOperationException>(() => Compressors.forBackup(@"E:\y\a.tar", profile, null));
        }
    }
}
