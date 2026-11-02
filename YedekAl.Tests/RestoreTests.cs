using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YedekAl.Tests
{
    [TestClass]
    public class RestoreTests
    {
        private static readonly DateTime now = new DateTime(2026, 11, 3, 10, 0, 0);

        [TestMethod]
        public void Swap_PutsRestoredCopyInAndKeepsOldAside()
        {
            using (TestFolder folder = new TestFolder())
            {
                string source = folder.createDatabase("kaynak");
                string restored = folder.sub("geri", "Veri Tabanı");
                FileCopier.copy(SourceTree.scan(source), folder.sub("geri"), null, System.Threading.CancellationToken.None);
                Directory.Move(folder.sub("geri", "kaynak"), restored);
                string db = folder.sub("sunucu", "Veri Tabanı");
                Directory.CreateDirectory(db);
                File.WriteAllText(Path.Combine(db, "eski.txt"), "eski veri");

                string previous = RestoreEngine.swap(restored, db, now);

                Assert.AreEqual(db + ".yedekal_onceki_20261103_100000", previous);
                Assert.IsTrue(TestFolder.sameTree(source, db));
                Assert.IsFalse(Directory.Exists(restored));
                Assert.AreEqual("eski veri", File.ReadAllText(Path.Combine(previous, "eski.txt")));
            }
        }

        [TestMethod]
        public void Swap_WithoutCurrentDatabase()
        {
            using (TestFolder folder = new TestFolder())
            {
                string restored = folder.createDatabase("geri");
                string db = folder.sub("Veri Tabanı");
                Assert.IsNull(RestoreEngine.swap(restored, db, now));
                Assert.IsTrue(File.Exists(Path.Combine(db, "a.txt")));
            }
        }

        [TestMethod]
        public void Swap_FailureRollsBack()
        {
            using (TestFolder folder = new TestFolder())
            {
                string db = folder.sub("Diger");
                Directory.CreateDirectory(db);
                File.WriteAllText(Path.Combine(db, "eski.txt"), "dokunulmamalı");

                InvalidOperationException error = Assert.ThrowsException<InvalidOperationException>(
                    () => RestoreEngine.swap(folder.sub("yok", "Diger"), db, now));

                StringAssert.Contains(error.Message, "yerinde bırakıldı");
                Assert.AreEqual("dokunulmamalı", File.ReadAllText(Path.Combine(db, "eski.txt")));
                Assert.IsFalse(Directory.Exists(db + ".yedekal_onceki_20261103_100000"));
            }
        }

        [TestMethod]
        public void CopyPermissions_ReachesFolderAndFiles()
        {
            using (TestFolder folder = new TestFolder())
            {
                string from = folder.sub("kaynak");
                string to = folder.sub("hedef");
                Directory.CreateDirectory(from);
                Directory.CreateDirectory(to);
                File.WriteAllText(Path.Combine(to, "dosya.txt"), "x");
                SecurityIdentifier networkService = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null);
                DirectorySecurity security = Directory.GetAccessControl(from);
                security.AddAccessRule(new FileSystemAccessRule(networkService, FileSystemRights.Modify,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                Directory.SetAccessControl(from, security);

                Assert.IsTrue(RestoreEngine.copyPermissions(from, to));

                Assert.IsTrue(hasRule(Directory.GetAccessControl(to), networkService), "folder");
                Assert.IsTrue(hasRule(File.GetAccessControl(Path.Combine(to, "dosya.txt")), networkService), "file inside");
                Assert.IsFalse(RestoreEngine.copyPermissions(folder.sub("yok"), to));
            }
        }

        private static bool hasRule(FileSystemSecurity security, SecurityIdentifier sid)
        {
            return security.GetAccessRules(true, true, typeof(SecurityIdentifier)).Cast<FileSystemAccessRule>().Any(r => r.IdentityReference.Equals(sid));
        }
    }
}
