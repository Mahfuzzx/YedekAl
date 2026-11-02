using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;

namespace YedekAl
{
    // SMTP over TLS from the first byte (SSL/TLS, usually port 465), which System.Net.Mail.SmtpClient cannot do.
    // The message itself is still rendered by SmtpClient (into a pickup folder), so MIME encoding stays .NET's own;
    // this class only speaks the SMTP dialogue: EHLO, AUTH PLAIN/LOGIN, MAIL, RCPT, DATA, QUIT.
    internal static class ImplicitTlsSmtp
    {
        public static void send(Settings settings, MailMessage message, int timeoutMilliseconds)
        {
            string host = settings.smtpHost.Trim();
            byte[] data = Encoding.UTF8.GetBytes(dotStuff(render(message)));

            using (TcpClient tcp = new TcpClient())
            {
                if (!tcp.ConnectAsync(host, settings.smtpPort).Wait(timeoutMilliseconds))
                    throw new SmtpException(string.Format("{0}:{1} adresine bağlanılamadı (zaman aşımı).", host, settings.smtpPort));
                tcp.ReceiveTimeout = tcp.SendTimeout = timeoutMilliseconds;

                RemoteCertificateValidationCallback validate = (sender, certificate, chain, errors) =>
                    errors == SslPolicyErrors.None || settings.smtpTrustAnyCertificate;
                using (SslStream ssl = new SslStream(tcp.GetStream(), false, validate))
                {
                    try
                    {
                        // SslProtocols.None lets Windows pick the best protocol it supports (TLS 1.2 / 1.3).
                        ssl.AuthenticateAsClient(host, null, SslProtocols.None, false);
                    }
                    catch (AuthenticationException ex)
                    {
                        throw MailNotifier.tlsFailure(ex, settings);
                    }

                    Session session = new Session(ssl);
                    session.expect(220);
                    List<string> capabilities = session.command("EHLO " + Dns.GetHostName(), 250);
                    if (!string.IsNullOrWhiteSpace(settings.smtpUser))
                        authenticate(session, capabilities, settings.smtpUser.Trim(), settings.getSmtpPassword());
                    session.command("MAIL FROM:<" + message.From.Address + ">", 250);
                    foreach (MailAddress recipient in message.To.Concat(message.CC).Concat(message.Bcc))
                        session.command("RCPT TO:<" + recipient.Address + ">", 250, 251);
                    session.command("DATA", 354);
                    ssl.Write(data, 0, data.Length);
                    session.command(".", 250);
                    session.quit();
                }
            }
        }

        private static void authenticate(Session session, List<string> capabilities, string user, string password)
        {
            string auth = capabilities.Select(l => l.Substring(4).Trim().ToUpperInvariant()).FirstOrDefault(l => l.StartsWith("AUTH")) ?? "";
            string[] mechanisms = auth.Split(new[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (mechanisms.Contains("PLAIN"))
            {
                session.command("AUTH PLAIN " + base64("\0" + user + "\0" + password), 235);
            }
            else if (mechanisms.Contains("LOGIN"))
            {
                session.command("AUTH LOGIN", 334);
                session.command(base64(user), 334);
                session.command(base64(password), 235);
            }
            else
            {
                throw new SmtpException("Sunucu desteklenen bir kimlik doğrulama yöntemi (PLAIN / LOGIN) sunmuyor.");
            }
        }

        // Lets SmtpClient write the message as an .eml file, then drops the X-Sender / X-Receiver lines it adds for pickup folders.
        private static string render(MailMessage message)
        {
            string folder = Path.Combine(Path.GetTempPath(), "yedekal_eposta_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            try
            {
                using (SmtpClient writer = new SmtpClient())
                {
                    writer.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    writer.PickupDirectoryLocation = folder;
                    writer.Send(message);
                }
                string text = File.ReadAllText(Directory.GetFiles(folder, "*.eml").Single(), Encoding.UTF8);
                string[] lines = text.Split(new[] { "\r\n" }, StringSplitOptions.None);
                int skip = lines.TakeWhile(l => l.StartsWith("X-Sender:") || l.StartsWith("X-Receiver:")).Count();
                return string.Join("\r\n", lines.Skip(skip));
            }
            finally
            {
                try { Directory.Delete(folder, true); } catch (IOException) { }
            }
        }

        // SMTP ends DATA with a line holding a single dot, so dots at the start of a line are doubled.
        private static string dotStuff(string message)
        {
            string text = message.Replace("\r\n.", "\r\n..");
            if (text.StartsWith(".")) text = "." + text;
            return text.EndsWith("\r\n") ? text : text + "\r\n";
        }

        private static string base64(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        private sealed class Session
        {
            private readonly Stream stream;
            private readonly StreamReader reader;

            public Session(Stream stream)
            {
                this.stream = stream;
                reader = new StreamReader(stream, Encoding.ASCII);
            }

            public void expect(params int[] codes)
            {
                read(codes);
            }

            public List<string> command(string line, params int[] codes)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(line + "\r\n");
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                return read(codes);
            }

            public void quit()
            {
                try
                {
                    command("QUIT", 221);
                }
                catch (Exception ex) when (ex is IOException || ex is SmtpException) { }
            }

            // Reads a (possibly multi-line "250-...") reply and checks its code.
            private List<string> read(int[] codes)
            {
                List<string> lines = new List<string>();
                string line;
                do
                {
                    line = reader.ReadLine();
                    if (line == null || line.Length < 3) throw new SmtpException("Sunucu bağlantıyı beklenmedik şekilde kapattı.");
                    lines.Add(line);
                }
                while (line.Length > 3 && line[3] == '-');

                int code;
                if (!int.TryParse(line.Substring(0, 3), out code) || !codes.Contains(code))
                    throw new SmtpException("Sunucu yanıtı: " + string.Join(" ", lines));
                return lines;
            }
        }
    }
}
