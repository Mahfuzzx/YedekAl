using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace YedekAl
{
    public enum BackupStatus { Success, Failed, Cancelled, Deleted }

    internal sealed class HistoryEntry
    {
        public DateTime start;
        public TimeSpan duration;
        public BackupStatus status;
        public long size;
        public string path = "";
        public string message = "";

        public string statusText
        {
            get
            {
                switch (status)
                {
                    case BackupStatus.Success: return "Başarılı";
                    case BackupStatus.Cancelled: return "İptal edildi";
                    case BackupStatus.Deleted: return "Silindi";
                    default: return "Hatalı";
                }
            }
        }
    }

    // One tab-separated line per backup. Retention only ever deletes paths recorded here.
    internal static class BackupHistory
    {
        private static readonly CultureInfo invariant = CultureInfo.InvariantCulture;

        public static List<HistoryEntry> load()
        {
            List<HistoryEntry> entries = new List<HistoryEntry>();
            if (!File.Exists(AppPaths.historyFile)) return entries;

            using (FileStream stream = new FileStream(AppPaths.historyFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split('\t');
                    DateTime start;
                    long seconds, size;
                    BackupStatus status;
                    if (parts.Length < 6
                        || !DateTime.TryParseExact(parts[0], "o", invariant, DateTimeStyles.RoundtripKind, out start)
                        || !long.TryParse(parts[1], NumberStyles.Integer, invariant, out seconds)
                        || !Enum.TryParse(parts[2], out status)
                        || !long.TryParse(parts[3], NumberStyles.Integer, invariant, out size))
                        continue;
                    entries.Add(new HistoryEntry
                    {
                        start = start,
                        duration = TimeSpan.FromSeconds(seconds),
                        status = status,
                        size = size,
                        path = parts[4],
                        message = parts[5]
                    });
                }
            }
            return entries;
        }

        public static void append(HistoryEntry entry)
        {
            Directory.CreateDirectory(AppPaths.dataDir);
            File.AppendAllText(AppPaths.historyFile, format(entry) + Environment.NewLine, new UTF8Encoding(false));
        }

        public static void save(IEnumerable<HistoryEntry> entries)
        {
            string tempFile = AppPaths.historyFile + ".tmp";
            using (StreamWriter writer = new StreamWriter(tempFile, false, new UTF8Encoding(false)))
            {
                foreach (HistoryEntry entry in entries)
                    writer.WriteLine(format(entry));
            }
            if (File.Exists(AppPaths.historyFile)) File.Replace(tempFile, AppPaths.historyFile, null);
            else File.Move(tempFile, AppPaths.historyFile);
        }

        private static string format(HistoryEntry entry)
        {
            return string.Join("\t",
                entry.start.ToString("o", invariant),
                ((long)entry.duration.TotalSeconds).ToString(invariant),
                entry.status.ToString(),
                entry.size.ToString(invariant),
                clean(entry.path),
                clean(entry.message));
        }

        private static string clean(string value)
        {
            return (value ?? "").Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
        }
    }
}
