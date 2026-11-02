using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YedekAl.Tests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void Crc32_KnownValue()
        {
            Crc32 crc = new Crc32();
            byte[] data = Encoding.ASCII.GetBytes("123456789");
            crc.update(data, 0, 4);
            crc.update(data, 4, 5);
            Assert.AreEqual(0xCBF43926u, crc.value);
        }

        [TestMethod]
        public void PercentReporter_ReportsOnlyChanges()
        {
            List<int> seen = new List<int>();
            PercentReporter reporter = new PercentReporter(new SyncProgress<int>(seen.Add), 200);
            foreach (long done in new long[] { 0, 1, 2, 100, 101, 200, 400 }) reporter.report(done);
            CollectionAssert.AreEqual(new[] { 0, 1, 50, 100 }, seen);
        }

        [TestMethod]
        public void SecretProtector_RoundTripAndPurposes()
        {
            string archive = SecretProtector.protect("Gizli Parola\\123!ğ", SecretProtector.archivePassword);
            Assert.AreEqual("Gizli Parola\\123!ğ", SecretProtector.unprotect(archive, SecretProtector.archivePassword));
            Assert.AreEqual("", SecretProtector.protect("", SecretProtector.archivePassword));
            Assert.ThrowsException<InvalidOperationException>(() => SecretProtector.unprotect(archive, SecretProtector.smtpPassword));
            Assert.ThrowsException<InvalidOperationException>(() => SecretProtector.unprotect("bozukveri==", SecretProtector.archivePassword));
        }

        [TestMethod]
        public void History_RoundTripAndOldLines()
        {
            using (TestFolder folder = new TestFolder())
            {
                string file = folder.sub("gecmis.txt");
                HistoryEntry entry = new HistoryEntry
                {
                    start = new DateTime(2026, 11, 3, 2, 30, 0, DateTimeKind.Local),
                    duration = TimeSpan.FromSeconds(95),
                    status = BackupStatus.Success,
                    size = 1536,
                    path = @"E:\Yedek\db.7z",
                    message = "satır\tsonu\r\nvar",
                    profileId = "abc"
                };
                BackupHistory.append(entry, file);
                // A line written before profiles existed, plus junk that must be skipped.
                File.AppendAllText(file, "2026-11-01T23:00:05.0000000+03:00\t60\tFailed\t0\t\teski hata\r\nbozuk satır\r\n");

                List<HistoryEntry> entries = BackupHistory.load(file);

                Assert.AreEqual(2, entries.Count);
                Assert.AreEqual(entry.start, entries[0].start);
                Assert.AreEqual(entry.duration, entries[0].duration);
                Assert.AreEqual(BackupStatus.Success, entries[0].status);
                Assert.AreEqual(1536, entries[0].size);
                Assert.AreEqual(@"E:\Yedek\db.7z", entries[0].path);
                Assert.AreEqual("satır sonu  var", entries[0].message);
                Assert.AreEqual("abc", entries[0].profileId);
                Assert.AreEqual("", entries[1].profileId);
                Assert.AreEqual(BackupStatus.Failed, entries[1].status);

                entries[0].status = BackupStatus.Deleted;
                BackupHistory.save(entries, file);
                Assert.AreEqual(BackupStatus.Deleted, BackupHistory.load(file)[0].status);
            }
        }

        [TestMethod]
        public void SourceTree_ScanAndPathHelpers()
        {
            using (TestFolder folder = new TestFolder())
            {
                string db = folder.createDatabase();
                SourceTree tree = SourceTree.scan(db + "\\");

                Assert.AreEqual("Veri Tabanı", tree.rootName);
                Assert.AreEqual(3, tree.files.Count);
                CollectionAssert.Contains(tree.directories, @"Veri Tabanı\boş klasör");
                Assert.IsTrue(tree.files.All(f => f.relativePath.StartsWith(@"Veri Tabanı\")));
                Assert.AreEqual(tree.files.Sum(f => f.length), tree.totalBytes);

                Assert.IsTrue(SourceTree.isRoot(@"C:\"));
                Assert.IsFalse(SourceTree.isRoot(db));
                Assert.IsTrue(SourceTree.isSameOrInside(@"D:\DB\Yedek", @"D:\DB"));
                Assert.IsFalse(SourceTree.isSameOrInside(@"D:\DB2", @"D:\DB"));
            }
        }

        [TestMethod]
        public void FileCopier_CopiesTreeAndHonoursCancellation()
        {
            using (TestFolder folder = new TestFolder())
            {
                string db = folder.createDatabase();
                SourceTree tree = SourceTree.scan(db);
                string target = folder.sub("kopya");
                FileCopier.copy(tree, target, null, CancellationToken.None);
                Assert.IsTrue(TestFolder.sameTree(db, Path.Combine(target, "Veri Tabanı")));

                CancellationTokenSource cancelled = new CancellationTokenSource();
                cancelled.Cancel();
                Assert.ThrowsException<OperationCanceledException>(() => FileCopier.copy(tree, folder.sub("iptal"), null, cancelled.Token));
            }
        }

        // Progress<T> posts to a SynchronizationContext; tests want the callbacks inline.
        private sealed class SyncProgress<T> : IProgress<T>
        {
            private readonly Action<T> action;
            public SyncProgress(Action<T> action) { this.action = action; }
            public void Report(T value) { action(value); }
        }
    }
}
