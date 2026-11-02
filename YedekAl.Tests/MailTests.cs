using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
        public void SendTest_TalksSmtpWithLogin()
        {
            using (FakeSmtpServer server = new FakeSmtpServer())
            {
                Settings settings = new Settings
                {
                    smtpHost = "127.0.0.1",
                    smtpPort = server.port,
                    smtpSsl = false,
                    smtpUser = "yedek@ornek.com",
                    mailFrom = "yedek@ornek.com",
                    mailTo = "admin@ornek.com; ikinci@ornek.com"
                };
                settings.setSmtpPassword("Smtp Parola 1");

                MailNotifier.sendTest(settings);

                List<string> lines = server.conversation();
                CollectionAssert.Contains(lines, "USER=yedek@ornek.com");
                CollectionAssert.Contains(lines, "PASS=Smtp Parola 1");
                CollectionAssert.Contains(lines, "MAIL FROM:<yedek@ornek.com>");
                CollectionAssert.Contains(lines, "RCPT TO:<admin@ornek.com>");
                CollectionAssert.Contains(lines, "RCPT TO:<ikinci@ornek.com>");
            }
        }

        // Just enough SMTP (EHLO, AUTH LOGIN, MAIL, RCPT, DATA, QUIT) for one message; records what the client sent.
        private sealed class FakeSmtpServer : IDisposable
        {
            private readonly TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            private readonly List<string> lines = new List<string>();
            private readonly Task task;

            public FakeSmtpServer()
            {
                listener.Start();
                port = ((IPEndPoint)listener.LocalEndpoint).Port;
                task = Task.Run(() => serve());
            }

            public int port { get; }

            public List<string> conversation()
            {
                Assert.IsTrue(task.Wait(TimeSpan.FromSeconds(30)), "SMTP conversation did not finish");
                lock (lines) return new List<string>(lines);
            }

            private void serve()
            {
                using (TcpClient client = listener.AcceptTcpClient())
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII) { NewLine = "\r\n", AutoFlush = true })
                {
                    writer.WriteLine("220 test");
                    bool data = false;
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        add(line);
                        if (data)
                        {
                            if (line == ".") { data = false; writer.WriteLine("250 ok"); }
                            continue;
                        }
                        string upper = line.ToUpperInvariant();
                        if (upper.StartsWith("EHLO")) { writer.WriteLine("250-test"); writer.WriteLine("250 AUTH LOGIN"); }
                        else if (upper.StartsWith("AUTH LOGIN"))
                        {
                            string[] parts = line.Split(' ');
                            string user;
                            if (parts.Length > 2) user = parts[2];
                            else { writer.WriteLine("334 VXNlcm5hbWU6"); user = reader.ReadLine(); }
                            add("USER=" + decode(user));
                            writer.WriteLine("334 UGFzc3dvcmQ6");
                            add("PASS=" + decode(reader.ReadLine()));
                            writer.WriteLine("235 ok");
                        }
                        else if (upper.StartsWith("DATA")) { data = true; writer.WriteLine("354 go"); }
                        else if (upper.StartsWith("QUIT")) { writer.WriteLine("221 bye"); break; }
                        else writer.WriteLine("250 ok");
                    }
                }
            }

            private void add(string line)
            {
                lock (lines) lines.Add(line);
            }

            private static string decode(string base64)
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            }

            public void Dispose()
            {
                listener.Stop();
            }
        }
    }
}
