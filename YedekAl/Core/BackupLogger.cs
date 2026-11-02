using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace YedekAl
{
    // Writes to a daily log file under ProgramData and mirrors each line to the UI, if there is one.
    internal sealed class BackupLogger : IDisposable
    {
        private const int keepLogDays = 90;
        private readonly object sync = new object();
        private readonly StreamWriter writer;
        private readonly IProgress<string> sink;

        public BackupLogger(IProgress<string> sink)
        {
            this.sink = sink;
            Directory.CreateDirectory(AppPaths.logDir);
            deleteOldLogs();
            string file = Path.Combine(AppPaths.logDir, DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".log");
            FileStream stream = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            writer = new StreamWriter(stream, new UTF8Encoding(true)) { AutoFlush = true };
        }

        public void info(string message) { write("BİLGİ", "", message); }
        public void warn(string message) { write("UYARI", "UYARI: ", message); }
        public void error(string message) { write("HATA", "HATA: ", message); }

        public void Dispose()
        {
            writer.Dispose();
        }

        private void write(string level, string prefix, string message)
        {
            DateTime now = DateTime.Now;
            lock (sync)
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd HH:mm:ss} [{1}] {2}", now, level, message));
            }
            sink?.Report(string.Format(CultureInfo.InvariantCulture, "{0:HH:mm:ss}  {1}{2}", now, prefix, message));
        }

        private static void deleteOldLogs()
        {
            DateTime limit = DateTime.Now.AddDays(-keepLogDays);
            foreach (string file in Directory.GetFiles(AppPaths.logDir, "*.log"))
            {
                try
                {
                    if (File.GetLastWriteTime(file) < limit) File.Delete(file);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException) { }
            }
        }
    }
}
