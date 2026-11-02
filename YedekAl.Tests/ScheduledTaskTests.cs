using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YedekAl.Tests
{
    [TestClass]
    public class ScheduledTaskTests
    {
        private const int TASK_VALIDATE_ONLY = 1;
        private const int TASK_LOGON_INTERACTIVE_TOKEN = 3;

        // Asks Task Scheduler to validate the task definition without registering anything (no admin rights needed).
        [TestMethod]
        public void TaskXml_IsAcceptedByTaskScheduler()
        {
            string xml = (string)typeof(ScheduledTask).GetMethod("buildXml", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, new object[] { new TimeSpan(23, 30, 0) });
            StringAssert.Contains(xml, "<Arguments>/auto</Arguments>");
            StringAssert.Contains(xml, "T23:30:00</StartBoundary>");

            dynamic service = Activator.CreateInstance(Type.GetTypeFromProgID("Schedule.Service"));
            service.Connect();
            dynamic folder = service.GetFolder("\\");
            folder.RegisterTask("YedekAl test", xml, TASK_VALIDATE_ONLY, null, null, TASK_LOGON_INTERACTIVE_TOKEN, null);
        }
    }
}
