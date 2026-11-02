using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YedekAl.Tests
{
    // Just enough SMTP for one message (EHLO, STARTTLS, AUTH PLAIN/LOGIN, MAIL, RCPT, DATA, QUIT), optionally over TLS
    // with a self-signed certificate. Records what the client sent, with credentials decoded as USER= / PASS= lines.
    internal sealed class FakeSmtpServer : IDisposable
    {
        private readonly TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
        private readonly List<string> lines = new List<string>();
        private readonly SmtpSecurity security;
        private readonly X509Certificate2 certificate;
        private readonly Task task;

        public FakeSmtpServer(SmtpSecurity security = SmtpSecurity.None)
        {
            this.security = security;
            if (security != SmtpSecurity.None) certificate = createSelfSignedCertificate();
            listener.Start();
            port = ((IPEndPoint)listener.LocalEndpoint).Port;
            task = Task.Run(() => serve());
        }

        public int port { get; }
        public string data { get; private set; } = "";

        public List<string> conversation()
        {
            Assert.IsTrue(task.Wait(TimeSpan.FromSeconds(30)), "SMTP conversation did not finish");
            lock (lines) return new List<string>(lines);
        }

        private void serve()
        {
            try
            {
                using (TcpClient client = listener.AcceptTcpClient())
                {
                    Stream stream = client.GetStream();
                    if (security == SmtpSecurity.SslTls) stream = startTls(stream);
                    talk(stream);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is SocketException || ex is System.Security.Authentication.AuthenticationException || ex is ObjectDisposedException)
            {
                add("CLOSED: " + ex.GetType().Name);
            }
        }

        // After STARTTLS the client speaks first (EHLO again), so there is no second greeting.
        private void talk(Stream stream, bool afterStartTls = false)
        {
            StreamReader reader = new StreamReader(stream, Encoding.ASCII);
            StreamWriter writer = new StreamWriter(stream, Encoding.ASCII) { NewLine = "\r\n", AutoFlush = true };
            if (!afterStartTls) writer.WriteLine("220 test");
            bool upgraded = security != SmtpSecurity.StartTls || afterStartTls;
            StringBuilder message = null;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (message != null)
                {
                    if (line == ".") { data = message.ToString(); message = null; writer.WriteLine("250 ok"); }
                    else message.AppendLine(line);
                    continue;
                }
                add(line);
                string upper = line.ToUpperInvariant();
                if (upper.StartsWith("EHLO"))
                {
                    writer.WriteLine("250-test");
                    writer.WriteLine(upgraded ? "250 AUTH LOGIN PLAIN" : "250 STARTTLS");
                }
                else if (upper == "STARTTLS")
                {
                    writer.WriteLine("220 go ahead");
                    talk(startTls(stream), true);
                    return;
                }
                else if (upper.StartsWith("AUTH PLAIN "))
                {
                    string[] parts = decode(line.Substring(11)).Split('\0');
                    add("USER=" + parts[1]);
                    add("PASS=" + parts[2]);
                    writer.WriteLine("235 ok");
                }
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
                else if (upper == "DATA") { message = new StringBuilder(); writer.WriteLine("354 go"); }
                else if (upper == "QUIT") { writer.WriteLine("221 bye"); return; }
                else writer.WriteLine("250 ok");
            }
        }

        private Stream startTls(Stream stream)
        {
            SslStream ssl = new SslStream(stream, false);
            ssl.AuthenticateAsServer(certificate);
            return ssl;
        }

        private void add(string line)
        {
            lock (lines) lines.Add(line);
        }

        private static string decode(string base64)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }

        // SslStream on Windows needs a persisted private key, hence the round trip through PFX.
        private static X509Certificate2 createSelfSignedCertificate()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                CertificateRequest request = new CertificateRequest("CN=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                using (X509Certificate2 created = request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1)))
                {
                    return new X509Certificate2(created.Export(X509ContentType.Pfx), (string)null, X509KeyStorageFlags.Exportable);
                }
            }
        }

        public void Dispose()
        {
            listener.Stop();
            certificate?.Dispose();
        }
    }
}
