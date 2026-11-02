using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace YedekAl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Any(a => a.Equals("/auto", StringComparison.OrdinalIgnoreCase) || a.Equals("-auto", StringComparison.OrdinalIgnoreCase)))
                return runAutomatic();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Settings settings;
            try
            {
                settings = Settings.load();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ayarlar okunamadı:\r\n" + AppPaths.settingsFile + "\r\n\r\n" + ex.Message, "YedekAl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
            Application.Run(new FrmMain(settings));
            return 0;
        }

        // Started by the scheduled task: no UI, everything goes to the log file.
        private static int runAutomatic()
        {
            try
            {
                using (BackupLogger log = new BackupLogger(null))
                {
                    Settings settings;
                    try
                    {
                        settings = Settings.load();
                    }
                    catch (Exception ex)
                    {
                        log.error("Ayarlar okunamadı: " + ex.Message);
                        return 1;
                    }

                    // Profiles run one after another; one failing does not stop the rest.
                    List<BackupResult> results = new List<BackupResult>();
                    foreach (Profile profile in settings.profiles.Where(p => p.includeInSchedule))
                        results.Add(new BackupEngine(settings, profile, log, null).run(CancellationToken.None));
                    if (results.Count == 0) log.warn("Zamanlanmış yedeklemeye dahil edilmiş profil yok.");
                    MailNotifier.notifyIfNeeded(settings, results, log);

                    bool allSucceeded = results.Count > 0 && results.All(r => r.success);
                    if (allSucceeded && settings.autoShutdown)
                    {
                        try
                        {
                            SystemShutdown.schedule(60, SystemShutdown.defaultMessage);
                            log.info("Bilgisayar 60 saniye içinde kapatılacak.");
                        }
                        catch (Exception ex)
                        {
                            log.error("Bilgisayar kapatılamadı: " + ex.Message);
                        }
                    }
                    return allSucceeded ? 0 : 1;
                }
            }
            catch (Exception)
            {
                return 2;
            }
        }
    }
}
