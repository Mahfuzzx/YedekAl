using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YedekAl.Tests
{
    [TestClass]
    public class ProfileTests
    {
        private static Profile valid()
        {
            return new Profile { serviceName = "MSSQLSERVER", dbPath = @"D:\DB", backupPath = @"E:\Yedek" };
        }

        [TestMethod]
        public void Validate_EmptyProfile()
        {
            var errors = new Profile().validate();
            Assert.IsTrue(errors.Any(e => e.Contains("Hizmet")));
            Assert.IsTrue(errors.Any(e => e.Contains("Veritabanı")));
            Assert.IsTrue(errors.Any(e => e.Contains("Yedekleme")));
            Assert.AreEqual(0, valid().validate().Count);
        }

        [TestMethod]
        public void Validate_NameTemplate()
        {
            Profile profile = valid();
            profile.nameTemplate = "yedek";
            Assert.IsTrue(profile.validate().Any(e => e.Contains("tarih alanı")));
            profile.nameTemplate = "yedek_{0:yyyy/MM}";
            Assert.IsTrue(profile.validate().Any(e => e.Contains("kullanılamayan")));
            profile.nameTemplate = "yedek_{0:yyyy";
            Assert.IsTrue(profile.validate().Any(e => e.Contains("hatalı")));
        }

        [TestMethod]
        public void Validate_EncryptionNeeds7zOrRarAndAPassword()
        {
            Profile profile = valid();
            profile.encrypt = true;
            Assert.IsTrue(profile.validate().Any(e => e.Contains("yalnızca 7z ve RAR")));
            Assert.IsTrue(profile.validate().Any(e => e.Contains("parola")));
            profile.format = ArchiveFormat.SevenZip;
            profile.setPassword("Uzun Parola 2026");
            Assert.AreEqual(0, profile.validate().Count);
            Assert.AreEqual("Uzun Parola 2026", profile.getPassword());
        }

        [TestMethod]
        public void CheckPassword()
        {
            Assert.IsNotNull(Profile.checkPassword("kisa"));
            Assert.IsNotNull(Profile.checkPassword("tırnak\"lı parola"));
            Assert.IsNotNull(Profile.checkPassword(@"sonu ters\"));
            Assert.IsNull(Profile.checkPassword(@"orta\ters bölü olur"));
        }

        [TestMethod]
        public void FormatArchiveName()
        {
            Assert.AreEqual("yedek_20261102_090507", valid().formatArchiveName(new DateTime(2026, 11, 2, 9, 5, 7)));
        }

        [TestMethod]
        public void Duplicate_GetsNewIdAndName()
        {
            Profile profile = valid();
            Profile copy = profile.duplicate("Kopya");
            Assert.AreNotEqual(profile.id, copy.id);
            Assert.AreEqual("Kopya", copy.name);
            Assert.AreEqual(profile.dbPath, copy.dbPath);
        }
    }
}
