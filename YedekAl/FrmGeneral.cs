using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace YedekAl
{
    // Settings shared by all profiles: schedule, shutdown and e-mail. Saves and applies the scheduled task on OK.
    public partial class FrmGeneral : Form
    {
        // Copy of the current settings; FrmNotify writes the e-mail fields into it.
        private readonly Settings working;

        public Settings settings { get; private set; }

        public FrmGeneral(Settings current)
        {
            InitializeComponent();
            working = current.clone();
            chkSchedule.Checked = current.scheduleEnabled;
            dtpScheduleTime.Value = DateTime.Today.Add(current.getScheduleTime());
            chkShutdown.Checked = current.autoShutdown;
            chkNotify.Checked = current.notifyEnabled;
            List<string> scheduled = current.profiles.Where(p => p.includeInSchedule).Select(p => p.name).ToList();
            lblScheduleProfiles.Text = "Dahil profiller: " + (scheduled.Count > 0 ? string.Join(", ", scheduled) : "(yok)");
            updateControls();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Settings result = working.clone();
            result.scheduleEnabled = chkSchedule.Checked;
            result.scheduleTime = dtpScheduleTime.Value.ToString("HH:mm", CultureInfo.InvariantCulture);
            result.autoShutdown = chkShutdown.Checked;
            result.notifyEnabled = chkNotify.Checked;

            List<string> errors = result.validate();
            if (errors.Count > 0)
            {
                MessageBox.Show(this, string.Join("\r\n", errors), "Dur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                result.save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Ayarlar kaydedilemedi:\r\n" + ex.Message, "YedekAl", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                if (result.scheduleEnabled) ScheduledTask.createOrUpdate(result.getScheduleTime());
                else ScheduledTask.delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Ayarlar kaydedildi ancak zamanlanmış görev ayarlanamadı:\r\n" + ex.Message, "YedekAl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            settings = result;
            DialogResult = DialogResult.OK;
        }

        private void cmdNotifySettings_Click(object sender, EventArgs e)
        {
            using (FrmNotify frmNotify = new FrmNotify(working))
            {
                // Configuring e-mail implies wanting it; the checkbox can still be cleared afterwards.
                if (frmNotify.ShowDialog(this) == DialogResult.OK) chkNotify.Checked = true;
            }
        }

        private void chkSchedule_CheckedChanged(object sender, EventArgs e)
        {
            updateControls();
        }

        private void updateControls()
        {
            dtpScheduleTime.Enabled = chkSchedule.Checked;
        }
    }
}
