using System;
using System.IO;
using System.Reflection;

namespace YedekAl
{
    internal static class AppPaths
    {
        // ProgramData is shared by the UI and the scheduled task (which runs as SYSTEM).
        public static readonly string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "YedekAl");
        public static readonly string settingsFile = Path.Combine(dataDir, "ayarlar.xml");
        public static readonly string historyFile = Path.Combine(dataDir, "gecmis.txt");
        public static readonly string logDir = Path.Combine(dataDir, "Loglar");
        public static readonly string appDir = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string exePath = Assembly.GetExecutingAssembly().Location;
    }
}
