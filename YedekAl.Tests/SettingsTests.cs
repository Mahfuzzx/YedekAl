using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YedekAl.Tests
{
    [TestClass]
    public class SettingsTests
    {
        private const string legacyXml = @"<?xml version=""1.0""?>
<Settings xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <serviceName>MSSQLSERVER</serviceName><dbPath>D:\Veri\DB</dbPath><backupPath>E:\Yedek</backupPath>
  <nameTemplate>db_{0:yyyyMMdd}</nameTemplate><compress>true</compress><format>SevenZip</format><strength>Best</strength>
  <copyFirst>true</copyFirst><verifyArchive>true</verifyArchive><encrypt>false</encrypt><protectedPassword />
  <rarPath /><sevenZipPath>C:\x\7za.exe</sevenZipPath><keepLastCount>7</keepLastCount><keepDays>30</keepDays>
  <scheduleEnabled>true</scheduleEnabled><scheduleTime>02:30</scheduleTime><autoShutdown>true</autoShutdown>
  <serviceTimeoutSeconds>90</serviceTimeoutSeconds><notifyEnabled>true</notifyEnabled><smtpHost>smtp.ornek.com</smtpHost>
  <smtpPort>587</smtpPort><mailFrom>a@ornek.com</mailFrom><mailTo>b@ornek.com</mailTo>
</Settings>";

        private static Settings twoProfiles()
        {
            Settings settings = new Settings();
            settings.profiles.Add(new Profile { name = "Varsayılan", serviceName = "A", dbPath = @"D:\A", backupPath = @"E:\Yedek" });
            settings.profiles.Add(new Profile { name = "Muhasebe", serviceName = "B", dbPath = @"D:\B", backupPath = @"E:\Yedek", includeInSchedule = false });
            return settings;
        }

        [TestMethod]
        public void LegacyFile_IsMigratedIntoOneProfileAndRewritten()
        {
            using (TestFolder folder = new TestFolder())
            {
                string file = folder.sub("ayarlar.xml");
                File.WriteAllText(file, legacyXml);

                Settings settings = Settings.loadFrom(file);

                Assert.AreEqual(1, settings.profiles.Count);
                Profile profile = settings.profiles[0];
                Assert.AreEqual("Varsayılan", profile.name);
                Assert.AreEqual("MSSQLSERVER", profile.serviceName);
                Assert.AreEqual(@"D:\Veri\DB", profile.dbPath);
                Assert.AreEqual(@"E:\Yedek", profile.backupPath);
                Assert.AreEqual("db_{0:yyyyMMdd}", profile.nameTemplate);
                Assert.AreEqual(ArchiveFormat.SevenZip, profile.format);
                Assert.AreEqual(CompressionStrength.Best, profile.strength);
                Assert.IsTrue(profile.copyFirst);
                Assert.AreEqual(7, profile.keepLastCount);
                Assert.AreEqual(30, profile.keepDays);
                Assert.AreEqual(90, profile.serviceTimeoutSeconds);
                Assert.AreEqual(@"C:\x\7za.exe", profile.sevenZipPath);
                Assert.IsTrue(profile.includeInSchedule);

                Assert.IsTrue(settings.scheduleEnabled);
                Assert.AreEqual("02:30", settings.scheduleTime);
                Assert.IsTrue(settings.autoShutdown);
                Assert.IsTrue(settings.notifyEnabled);
                Assert.AreEqual("smtp.ornek.com", settings.smtpHost);
                Assert.AreEqual(profile.id, settings.legacyProfileId);

                Assert.IsNotNull(XDocument.Load(file).Root.Element("profiles"), "the file should be rewritten in the new format");
                Settings again = Settings.loadFrom(file);
                Assert.AreEqual(profile.id, again.profiles.Single().id, "loading the rewritten file must not migrate twice");
            }
        }

        [TestMethod]
        public void SaveAndLoad_KeepsProfiles()
        {
            using (TestFolder folder = new TestFolder())
            {
                string file = folder.sub("ayarlar.xml");
                Settings settings = twoProfiles();
                settings.smtpTrustAnyCertificate = true;
                settings.saveTo(file);

                Settings back = Settings.loadFrom(file);

                CollectionAssert.AreEqual(settings.profiles.Select(p => p.id).ToList(), back.profiles.Select(p => p.id).ToList());
                Assert.AreEqual("Muhasebe", back.profiles[1].name);
                Assert.IsFalse(back.profiles[1].includeInSchedule);
                Assert.IsTrue(back.smtpTrustAnyCertificate);
            }
        }

        [TestMethod]
        public void Clone_IsDeep()
        {
            Settings settings = twoProfiles();
            Settings copy = settings.clone();
            copy.profiles[0].name = "değişti";
            copy.profiles.RemoveAt(1);
            Assert.AreEqual("Varsayılan", settings.profiles[0].name);
            Assert.AreEqual(2, settings.profiles.Count);
        }

        [TestMethod]
        public void HistoryWithoutProfile_BelongsToLegacyProfileOnly()
        {
            Settings settings = twoProfiles();
            settings.legacyProfileId = settings.profiles[0].id;
            HistoryEntry old = new HistoryEntry();
            HistoryEntry second = new HistoryEntry { profileId = settings.profiles[1].id };

            Assert.IsTrue(settings.belongsTo(old, settings.profiles[0]));
            Assert.IsFalse(settings.belongsTo(old, settings.profiles[1]));
            Assert.IsTrue(settings.belongsTo(second, settings.profiles[1]));
            Assert.IsFalse(settings.belongsTo(second, settings.profiles[0]));
        }

        [TestMethod]
        public void Validate_SharedRules()
        {
            Settings settings = twoProfiles();
            Assert.AreEqual(0, settings.validate().Count);

            Settings duplicate = settings.clone();
            duplicate.profiles[1].name = "VARSAYILAN";
            Assert.IsTrue(duplicate.validate().Any(e => e.Contains("Aynı adda")));

            Settings noneScheduled = settings.clone();
            noneScheduled.scheduleEnabled = true;
            noneScheduled.profiles.ForEach(p => p.includeInSchedule = false);
            Assert.IsTrue(noneScheduled.validate().Any(e => e.Contains("dahil")));

            Settings noProfiles = new Settings();
            Assert.IsTrue(noProfiles.validate().Any(e => e.Contains("En az bir profil")));
        }

        [TestMethod]
        public void ValidateMail_RejectsBadValues()
        {
            Settings settings = new Settings { smtpHost = "smtp.ornek.com", mailFrom = "a@ornek.com", mailTo = "b@ornek.com; c@ornek.com" };
            Assert.AreEqual(0, settings.validateMail().Count);

            settings.smtpPort = 465;
            settings.mailFrom = "kötü adres";
            settings.mailTo = "b@ornek.com, yanlis";
            var errors = settings.validateMail();
            Assert.IsTrue(errors.Any(e => e.Contains("465")));
            Assert.IsTrue(errors.Any(e => e.Contains("Gönderen")));
            Assert.IsTrue(errors.Any(e => e.Contains("yanlis")));
        }

        [TestMethod]
        public void BadMailSettings_DoNotBlockProfiles()
        {
            Settings settings = twoProfiles();
            settings.notifyEnabled = true;
            Assert.IsTrue(settings.validate().Count > 0, "shared settings report the e-mail problem");
            Assert.AreEqual(0, settings.profiles[0].validate().Count, "the profile itself can still be backed up");
        }

        [TestMethod]
        public void ScheduleTime_FallsBackTo2300()
        {
            Assert.AreEqual(new TimeSpan(2, 30, 0), new Settings { scheduleTime = "02:30" }.getScheduleTime());
            Assert.AreEqual(new TimeSpan(23, 0, 0), new Settings { scheduleTime = "saçma" }.getScheduleTime());
        }

        [TestMethod]
        public void SmtpPassword_IsStoredEncrypted()
        {
            Settings settings = new Settings();
            settings.setSmtpPassword("Smtp Parola 1");
            Assert.AreNotEqual("Smtp Parola 1", settings.protectedSmtpPassword);
            Assert.IsFalse(settings.protectedSmtpPassword.Contains("Parola"));
            Assert.AreEqual("Smtp Parola 1", settings.getSmtpPassword());
        }
    }
}
