using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace YedekAl
{
    // Daily "YedekAl.exe /auto" task, registered through the Task Scheduler COM API and run as SYSTEM
    // so it works without anyone logged in.
    internal static class ScheduledTask
    {
        public const string taskName = "YedekAl Otomatik Yedekleme";
        private const int TASK_CREATE_OR_UPDATE = 6;
        private const int TASK_LOGON_SERVICE_ACCOUNT = 5;
        private const int HRESULT_FILE_NOT_FOUND = unchecked((int)0x80070002);

        public static void createOrUpdate(TimeSpan time)
        {
            string xml = buildXml(time);
            withRootFolder(folder => folder.RegisterTask(taskName, xml, TASK_CREATE_OR_UPDATE, "SYSTEM", null, TASK_LOGON_SERVICE_ACCOUNT, null));
        }

        public static void delete()
        {
            withRootFolder(folder =>
            {
                try
                {
                    folder.DeleteTask(taskName, 0);
                }
                catch (FileNotFoundException) { }
                catch (COMException ex) when (ex.HResult == HRESULT_FILE_NOT_FOUND) { }
            });
        }

        private static void withRootFolder(Action<dynamic> action)
        {
            dynamic service = Activator.CreateInstance(Type.GetTypeFromProgID("Schedule.Service"));
            try
            {
                service.Connect();
                dynamic folder = service.GetFolder("\\");
                try
                {
                    action(folder);
                }
                finally
                {
                    Marshal.ReleaseComObject(folder);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(service);
            }
        }

        private static string buildXml(TimeSpan time)
        {
            string start = DateTime.Today.Add(time).ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
            string command = SecurityElement.Escape(AppPaths.exePath);
            string workingDir = SecurityElement.Escape(Path.GetDirectoryName(AppPaths.exePath));
            return $@"<?xml version=""1.0"" encoding=""UTF-16""?>
<Task version=""1.2"" xmlns=""http://schemas.microsoft.com/windows/2004/02/mit/task"">
  <RegistrationInfo>
    <Description>YedekAl tarafından oluşturuldu. Her gün veritabanının yedeğini alır.</Description>
  </RegistrationInfo>
  <Triggers>
    <CalendarTrigger>
      <StartBoundary>{start}</StartBoundary>
      <Enabled>true</Enabled>
      <ScheduleByDay>
        <DaysInterval>1</DaysInterval>
      </ScheduleByDay>
    </CalendarTrigger>
  </Triggers>
  <Principals>
    <Principal id=""Author"">
      <UserId>S-1-5-18</UserId>
      <RunLevel>HighestAvailable</RunLevel>
    </Principal>
  </Principals>
  <Settings>
    <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>
    <DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>
    <StopIfGoingOnBatteries>false</StopIfGoingOnBatteries>
    <StartWhenAvailable>true</StartWhenAvailable>
    <ExecutionTimeLimit>PT12H</ExecutionTimeLimit>
    <Enabled>true</Enabled>
  </Settings>
  <Actions Context=""Author"">
    <Exec>
      <Command>{command}</Command>
      <Arguments>/auto</Arguments>
      <WorkingDirectory>{workingDir}</WorkingDirectory>
    </Exec>
  </Actions>
</Task>";
        }
    }
}
