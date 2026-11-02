using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;

namespace YedekAl
{
    internal static class MailNotifier
    {
        private const int timeoutMilliseconds = 30000;
        private static readonly object certificateLock = new object();

        public static List<string> splitRecipients(string value)
        {
            return (value ?? "")
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(address => address.Trim())
                .Where(address => address.Length > 0)
                .ToList();
        }

        public static void sendTest(Settings settings)
        {
            send(settings, "[YedekAl] " + Environment.MachineName + ": Test e-postası",
                "Bu bir test e-postasıdır. YedekAl bildirimleri bu adrese gönderilecek." + Environment.NewLine);
        }

        // One e-mail per run (a single profile from the UI, or every scheduled profile from /auto).
        // A failed e-mail never changes the backup results; it is only logged.
        public static void notifyIfNeeded(Settings settings, List<BackupResult> results, BackupLogger log)
        {
            List<BackupResult> reported = results.Where(r => !r.cancelled).ToList();
            if (!settings.notifyEnabled || reported.Count == 0) return;
            if (!settings.notifyOnSuccess && reported.All(r => r.success)) return;
            try
            {
                log.info("Bildirim e-postası gönderiliyor: " + settings.mailTo);
                send(settings, buildSubject(reported), buildBody(reported, log.lines));
                log.info("Bildirim e-postası gönderildi.");
            }
            catch (Exception ex)
            {
                log.warn("Bildirim e-postası gönderilemedi: " + ex.Message);
            }
        }

        public static string buildSubject(List<BackupResult> results)
        {
            int failed = results.Count(r => !r.success);
            string outcome;
            if (results.Count == 1) outcome = results[0].profileName + " yedeklemesi " + (failed == 0 ? "başarılı" : "BAŞARISIZ");
            else if (failed == 0) outcome = results.Count + " profilin tümü başarıyla yedeklendi";
            else outcome = results.Count + " profilden " + failed + " tanesi BAŞARISIZ";
            return "[YedekAl] " + Environment.MachineName + ": " + outcome;
        }

        public static string buildBody(List<BackupResult> results, IEnumerable<string> logLines)
        {
            CultureInfo invariant = CultureInfo.InvariantCulture;
            StringBuilder body = new StringBuilder();
            body.AppendLine("Bilgisayar: " + Environment.MachineName);
            foreach (BackupResult result in results)
            {
                HistoryEntry entry = result.entry;
                body.AppendLine();
                body.AppendLine("Profil: " + result.profileName);
                body.AppendLine("Başlangıç: " + entry.start.ToString("dd.MM.yyyy HH:mm:ss", invariant));
                body.AppendLine("Süre: " + entry.duration.ToString(@"h\:mm\:ss", invariant));
                body.AppendLine("Sonuç: " + (result.success ? "Başarılı" : entry.statusText));
                if (!string.IsNullOrEmpty(result.message)) body.AppendLine("Açıklama: " + result.message);
                if (result.outputPath != null) body.AppendLine("Yedek: " + result.outputPath + " (" + SystemInfo.formatSize(entry.size) + ")");
            }
            body.AppendLine();
            body.AppendLine("--- Log ---");
            foreach (string line in logLines) body.AppendLine(line);
            return body.ToString();
        }

        private static void send(Settings settings, string subject, string body)
        {
            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(settings.mailFrom.Trim(), "YedekAl");
                foreach (string recipient in splitRecipients(settings.mailTo)) message.To.Add(recipient);
                message.Subject = subject;
                message.Body = body;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;

                if (settings.smtpSecurity == SmtpSecurity.SslTls)
                {
                    ImplicitTlsSmtp.send(settings, message, timeoutMilliseconds);
                    return;
                }

                using (SmtpClient client = new SmtpClient(settings.smtpHost.Trim(), settings.smtpPort))
                {
                    bool startTls = settings.smtpSecurity == SmtpSecurity.StartTls;
                    client.EnableSsl = startTls;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Timeout = timeoutMilliseconds;
                    client.UseDefaultCredentials = false;
                    if (!string.IsNullOrWhiteSpace(settings.smtpUser))
                        client.Credentials = new NetworkCredential(settings.smtpUser.Trim(), settings.getSmtpPassword());
                    try
                    {
                        if (startTls && settings.smtpTrustAnyCertificate) sendTrustingAnyCertificate(client, message);
                        else client.Send(message);
                    }
                    catch (SmtpException ex) when (ex.InnerException != null)
                    {
                        // The inner exception usually says what went wrong (DNS, refused connection, TLS).
                        throw new SmtpException(ex.Message + " " + ex.InnerException.Message, ex);
                    }
                    catch (AuthenticationException ex)
                    {
                        throw tlsFailure(ex, settings);
                    }
                }
            }
        }

        // Same message for both TLS modes; the usual cause is a self-signed or expired server certificate.
        internal static SmtpException tlsFailure(AuthenticationException ex, Settings settings)
        {
            string hint = settings.smtpTrustAnyCertificate ? "" : " Sunucu sertifikası güvenilir değilse \"güvenilmeyen sertifikayı kabul et\" seçeneğini deneyin.";
            return new SmtpException("TLS bağlantısı kurulamadı: " + ex.Message + hint, ex);
        }

        // SmtpClient has no per-client certificate callback on .NET Framework, only the process-wide one in
        // ServicePointManager. It is swapped in for this one send and put back afterwards; the lock keeps two
        // sends from overlapping. Nothing else in YedekAl makes TLS connections.
        private static void sendTrustingAnyCertificate(SmtpClient client, MailMessage message)
        {
            lock (certificateLock)
            {
                RemoteCertificateValidationCallback previous = ServicePointManager.ServerCertificateValidationCallback;
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                try
                {
                    client.Send(message);
                }
                finally
                {
                    ServicePointManager.ServerCertificateValidationCallback = previous;
                }
            }
        }
    }
}
