using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace YedekAl
{
    public partial class FrmMain : Form
    {
        public string sStatus = "";
        private bool backUpMode = false;
        private string srcPath = "";
        private string bckUpPath = "";
        private readonly ServicesHelper servicesHelper;

        public FrmMain()
        {
            InitializeComponent();
            servicesHelper = new ServicesHelper(this);
            try
            {
                string test = Properties.Settings.Default.serviceName;
                test = Properties.Settings.Default.backupPath;
                test = Properties.Settings.Default.compressCmd;
                test = Properties.Settings.Default.dbPath;
                test = Properties.Settings.Default.copyCmd;
                bool btest = Properties.Settings.Default.compress;
                btest = Properties.Settings.Default.justCompress;
                btest = Properties.Settings.Default.copyFirst;
                btest = Properties.Settings.Default.autoShutdown;
            }
            catch (Exception)
            {
                Properties.Settings.Default.serviceName = "";
                Properties.Settings.Default.backupPath = "";
                Properties.Settings.Default.compressCmd = "";
                Properties.Settings.Default.dbPath = "";
                Properties.Settings.Default.copyCmd = "";
                Properties.Settings.Default.compress = false;
                Properties.Settings.Default.justCompress = false;
                Properties.Settings.Default.copyFirst = false;
                Properties.Settings.Default.autoShutdown = false;
                Properties.Settings.Default.Save();
            }
        }

        private void cmdSettings_Click(object sender, EventArgs e)
        {
            using (FrmSettings frmIcon = new FrmSettings())
            {
                frmIcon.ShowDialog(this);
            }
        }

        private void shellAndWait(string program_name)
        {
            int tLoc = program_name.IndexOf(' ');
            string args = program_name.Substring(tLoc + 1);
            Process process = Process.Start(program_name.Substring(0, tLoc), args);
            // Wait for the program to finish. 
            process.WaitForExit();
        }

        private void tmrSeek_Tick(object sender, EventArgs e)
        {
            sStatus = servicesHelper.serviceStatus("", Properties.Settings.Default.serviceName);
            imgOn.Visible = sStatus == "Running";
            imgOff.Visible = sStatus != "Running";
            lblServiceStatus.Text = (sStatus == "Running") ? "Çalýþýyor" : (sStatus == "Stopped" || sStatus == "Paused") ? "Çalýþmýyor" : "Meþgul/Yok";
            if (!backUpMode)
            {
                cmdStart.Enabled = sStatus == "Stopped" || sStatus == "Paused";
                cmdStop.Enabled = sStatus == "Running";
            }
        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            servicesHelper.serviceStop("", Properties.Settings.Default.serviceName);
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            servicesHelper.serviceStart("", Properties.Settings.Default.serviceName);
        }

        private void backupModeSwitch(bool bckpMode)
        {
            cmdBackup.Enabled = !bckpMode;
            cmdSettings.Enabled = !bckpMode;
            cmdStart.Enabled = !bckpMode;
            cmdStop.Enabled = !bckpMode;
            backUpMode = bckpMode;
        }

        private string replaceVars(string src)
        {
            return src.Replace("%y", DateTime.Now.Year.ToString())
                .Replace("%1", "\"" + srcPath + "\"")
                .Replace("%2", "\"" + bckUpPath + "\"\\")
                .Replace("%a", DateTime.Now.Month.ToString())
                .Replace("%g", DateTime.Now.Day.ToString())
                .Replace("%s", DateTime.Now.Hour.ToString())
                .Replace("%d", DateTime.Now.Minute.ToString())
                .Replace("%n", DateTime.Now.Second.ToString());
        }

        private void cmdBackup_Click(object sender, EventArgs e)
        {
            log("******** Yedekleme baþladý! ********\r\n");
            backupModeSwitch(true);
            srcPath = Properties.Settings.Default.dbPath;
            bckUpPath = Properties.Settings.Default.backupPath;
            string copyDest = bckUpPath + "\\" + Guid.NewGuid().ToString();
            if (!servicesHelper.serviceStop("", Properties.Settings.Default.serviceName))
            {
                log("******** Yedekleme durdu! ********\r\n");
                backupModeSwitch(false);
                return;
            }
            if ((Properties.Settings.Default.copyFirst && Properties.Settings.Default.compress) || !Properties.Settings.Default.compress)
            {
                if (!Properties.Settings.Default.compress)
                {
                    // D:\TEMP\yedek_%y%a%g%s%d%n
                    copyDest = replaceVars(bckUpPath);
                }
                srcPath = copyDest;
                // xcopy %1 %2 /e /c /y /v
                string copyCmd = Properties.Settings.Default.copyCmd
                    .Replace("%1", "\"" + Properties.Settings.Default.dbPath + "\"")
                    .Replace("%2", "\"" + copyDest + "\"");
                log("Dosyalar kopyalanýyor... ");
                try
                {
                    System.IO.Directory.CreateDirectory(copyDest);
                    shellAndWait(copyCmd);
                }
                catch (Exception ex)
                {
                    log("HATA: " + ex.Message + "!\r\n");
                    log("******** Yedekleme durdu! ********\r\n");
                    backupModeSwitch(false);
                    return;
                }
                log("TAMAM!\r\n");
                servicesHelper.serviceStart("", Properties.Settings.Default.serviceName);
            }
            if (Properties.Settings.Default.compress)
            {
                // Rar a -ag -m5 -r -s -y %2yedek_%y%a%g%s%d%n %1
                string compCmd = replaceVars(Properties.Settings.Default.compressCmd);
                log("Dosyalar sýkýþtýrýlýyor... ");
                try
                {
                    shellAndWait(compCmd);
                }
                catch (Exception ex)
                {
                    log("HATA: " + ex.Message + "!\r\n");
                    log("******** Yedekleme durdu! ********\r\n");
                    backupModeSwitch(false);
                    return;
                }
                log("TAMAM!\r\n");
                if (!Properties.Settings.Default.copyFirst)
                {
                    if (!servicesHelper.serviceStart("", Properties.Settings.Default.serviceName))
                    {
                        log("******** Yedekleme durdu! ********\r\n");
                        backupModeSwitch(false);
                        return;
                    }
                }
                else
                {
                    log("Önbellek siliniyor... ");
                    System.IO.Directory.Delete(copyDest, true);
                    log("TAMAM!\r\n");
                }
            }
            log("******** Yedekleme bitti! ********\r\n");
            backupModeSwitch(false);
            if (Properties.Settings.Default.autoShutdown) shellAndWait("shutdown -f -s -t 60 -d p:0:0 -c \"Yedekleme sonu.\"");
        }

        public void log(string line)
        {
            txtLog.Text += line;
            txtLog.Select(txtLog.Text.Length - 1, 0);
            txtLog.ScrollToCaret();
            Refresh();
        }
    }
}