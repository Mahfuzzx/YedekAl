using System;

namespace YedekAl
{
    public class Settings
    {
        public string serviceName { get { return Properties.Settings.Default.serviceName; } set { Properties.Settings.Default.serviceName = value; } }
        public string backupPath { get { return Properties.Settings.Default.backupPath; } set { Properties.Settings.Default.backupPath = value; } }
        public string compressCmd { get { return Properties.Settings.Default.compressCmd; } set { Properties.Settings.Default.compressCmd = value; } }
        public string dbPath { get { return Properties.Settings.Default.dbPath; } set { Properties.Settings.Default.dbPath = value; } }
        public string copyCmd { get { return Properties.Settings.Default.copyCmd; } set { Properties.Settings.Default.copyCmd = value; } }
        public bool compress { get { return Properties.Settings.Default.compress; } set { Properties.Settings.Default.compress = value; } }
        public bool justCompress { get { return Properties.Settings.Default.justCompress; } set { Properties.Settings.Default.justCompress = value; } }
        public bool copyFirst { get { return Properties.Settings.Default.copyFirst; } set { Properties.Settings.Default.copyFirst = value; } }
        public bool autoShutdown { get { return Properties.Settings.Default.autoShutdown; } set { Properties.Settings.Default.autoShutdown = value; } }

        public Settings()
        {
            try
            {
                string test = serviceName;
                test = backupPath;
                test = compressCmd;
                test = dbPath;
                test = copyCmd;
                bool btest = compress;
                btest = justCompress;
                btest = copyFirst;
                btest = autoShutdown;
            }
            catch (Exception)
            {
                serviceName = "";
                backupPath = "";
                compressCmd = "";
                dbPath = "";
                copyCmd = "";
                compress = false;
                justCompress = false;
                copyFirst = false;
                autoShutdown = false;
                save();
            }
        }

        public void save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
