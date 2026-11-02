using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YedekAl.Tests
{
    [TestClass]
    public class MailTests
    {
        private static BackupResult result(string profile, bool success, string message = null)
        {
            return new BackupResult
            {
                success = success,
                profileName = profile,
                message = message,
                outputPath = success ? @"E:\Yedek\db_20261103.7z" : null,
                entry = new HistoryEntry
                {
                    start = new DateTime(2026, 11, 3, 2, 30, 0),
                    duration = TimeSpan.FromSeconds(95),
                    status = success ? BackupStatus.Success : BackupStatus.Failed,
                    size = success ? 1536 : 0
                }
            };
        }

        private static Settings settingsFor(FakeSmtpServer server, SmtpSecurity security, bool trustAnyCertificate)
        {
            Settings settings = new Settings
            {
                smtpHost = "127.0.0.1",
                smtpPort = server.port,
                smtpSecurity = security,
                smtpTrustAnyCertificate = trustAnyCertificate,
                smtpUser = "yedek@ornek.com",
                mailFrom = "yedek@ornek.com",
                mailTo = "admin@ornek.com; ikinci@ornek.com"
            };
            settings.setSmtpPassword("Smtp Parola 1");
            return settings;
        }

        private static void assertDelivered(FakeSmtpServer server)
        {
            List<string> lines = server.conversation();
            CollectionAssert.Contains(lines, "USER=yedek@ornek.com");
            CollectionAssert.Contains(lines, "PASS=Smtp Parola 1");
            CollectionAssert.Contains(lines, "MAIL FROM:<yedek@ornek.com>");
            CollectionAssert.Contains(lines, "RCPT TO:<admin@ornek.com>");
            CollectionAssert.Contains(lines, "RCPT TO:<ikinci@ornek.com>");
            StringAssert.Contains(server.data, "Subject: =?utf-8?");
            Assert.IsFalse(server.data.Contains("X-Sender:"), "pickup-folder headers must not be sent");
        }

        [TestMethod]
        public void SplitRecipients()
        {
            CollectionAssert.AreEqual(new[] { "a@x.com", "b@x.com", "c@x.com" }, MailNotifier.splitRecipients(" a@x.com; b@x.com ,,c@x.com ;"));
            Assert.AreEqual(0, MailNotifier.splitRecipients(null).Count);
        }

        [TestMethod]
        public void Subject_DescribesTheRun()
        {
            StringAssert.EndsWith(MailNotifier.buildSubject(new List<BackupResult> { result("Varsayılan", true), result("Muhasebe", false) }), "2 profilden 1 tanesi BAŞARISIZ");
            StringAssert.EndsWith(MailNotifier.buildSubject(new List<BackupResult> { result("Muhasebe", false) }), "Muhasebe yedeklemesi BAŞARISIZ");
            StringAssert.EndsWith(MailNotifier.buildSubject(new List<BackupResult> { result("A", true), result("B", true) }), "2 profilin tümü başarıyla yedeklendi");
        }

        [TestMethod]
        public void Body_ListsEveryProfileAndTheLog()
        {
            string body = MailNotifier.buildBody(new List<BackupResult> { result("Varsayılan", true), result("Muhasebe", false, "'B' hizmeti bulunamadı.") }, new[] { "02:30:00  log satırı" });
            StringAssert.Contains(body, "Profil: Varsayılan");
            StringAssert.Contains(body, @"Yedek: E:\Yedek\db_20261103.7z (1,5 KB)");
            StringAssert.Contains(body, "Profil: Muhasebe");
            StringAssert.Contains(body, "Sonuç: Hatalı");
            StringAssert.Contains(body, "Açıklama: 'B' hizmeti bulunamadı.");
            StringAssert.Contains(body, "02:30:00  log satırı");
        }

        [TestMethod]
        public void Send_PlainText()
        {
            using (FakeSmtpServer server = new FakeSmtpServer())
            {
                MailNotifier.sendTest(settingsFor(server, SmtpSecurity.None, false));
                assertDelivered(server);
            }
        }

        [TestMethod]
        public void Send_SslTls_WithUntrustedCertificateAllowed()
        {
            using (FakeSmtpServer server = new FakeSmtpServer(SmtpSecurity.SslTls))
            {
                MailNotifier.sendTest(settingsFor(server, SmtpSecurity.SslTls, true));
                assertDelivered(server);
                Assert.IsTrue(server.conversation().Any(l => l.StartsWith("AUTH PLAIN")), "PLAIN is preferred when offered");
            }
        }

        [TestMethod]
        public void Send_SslTls_RejectsUntrustedCertificateByDefault()
        {
            using (FakeSmtpServer server = new FakeSmtpServer(SmtpSecurity.SslTls))
            {
                SmtpException error = Assert.ThrowsException<SmtpException>(() => MailNotifier.sendTest(settingsFor(server, SmtpSecurity.SslTls, false)));
                StringAssert.Contains(error.Message, "TLS");
                Assert.IsFalse(server.conversation().Any(l => l.StartsWith("USER=")), "no credentials may be sent over an untrusted connection");
            }
        }

        [TestMethod]
        public void Send_StartTls_WithUntrustedCertificateAllowed()
        {
            using (FakeSmtpServer server = new FakeSmtpServer(SmtpSecurity.StartTls))
            {
                MailNotifier.sendTest(settingsFor(server, SmtpSecurity.StartTls, true));
                assertDelivered(server);
                CollectionAssert.Contains(server.conversation(), "STARTTLS");
                Assert.IsNull(ServicePointManager.ServerCertificateValidationCallback, "the process-wide callback must be put back");
            }
        }

        [TestMethod]
        public void Send_StartTls_RejectsUntrustedCertificateByDefault()
        {
            using (FakeSmtpServer server = new FakeSmtpServer(SmtpSecurity.StartTls))
            {
                SmtpException error = Assert.ThrowsException<SmtpException>(() => MailNotifier.sendTest(settingsFor(server, SmtpSecurity.StartTls, false)));
                StringAssert.Contains(error.Message, "güvenilmeyen sertifikayı kabul et");
                Assert.IsFalse(server.conversation().Any(l => l.StartsWith("USER=")), "no credentials may be sent over an untrusted connection");
            }
        }
    }
}
