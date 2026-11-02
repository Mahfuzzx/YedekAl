using System;
using System.ServiceProcess;

namespace YedekAl
{
    internal static class ServiceManager
    {
        public static ServiceControllerStatus? getStatus(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) return null;
            try
            {
                using (ServiceController service = new ServiceController(serviceName))
                {
                    return service.Status;
                }
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public static string describe(ServiceControllerStatus? status)
        {
            if (!status.HasValue) return "Bulunamadı";
            switch (status.Value)
            {
                case ServiceControllerStatus.Running: return "Çalışıyor";
                case ServiceControllerStatus.Stopped: return "Çalışmıyor";
                case ServiceControllerStatus.Paused: return "Duraklatıldı";
                case ServiceControllerStatus.StartPending: return "Başlatılıyor...";
                case ServiceControllerStatus.StopPending: return "Durduruluyor...";
                case ServiceControllerStatus.ContinuePending: return "Devam ettiriliyor...";
                case ServiceControllerStatus.PausePending: return "Duraklatılıyor...";
                default: return status.Value.ToString();
            }
        }

        public static void stop(string serviceName, TimeSpan timeout)
        {
            using (ServiceController service = new ServiceController(serviceName))
            {
                if (service.Status == ServiceControllerStatus.Stopped) return;
                if (service.Status != ServiceControllerStatus.StopPending) service.Stop();
                waitFor(service, ServiceControllerStatus.Stopped, timeout, "durmadı");
            }
        }

        public static void start(string serviceName, TimeSpan timeout)
        {
            using (ServiceController service = new ServiceController(serviceName))
            {
                if (service.Status == ServiceControllerStatus.Running) return;
                if (service.Status == ServiceControllerStatus.StopPending)
                    waitFor(service, ServiceControllerStatus.Stopped, timeout, "durmadı");

                if (service.Status == ServiceControllerStatus.Paused) service.Continue();
                else if (service.Status == ServiceControllerStatus.Stopped) service.Start();
                waitFor(service, ServiceControllerStatus.Running, timeout, "başlamadı");
            }
        }

        private static void waitFor(ServiceController service, ServiceControllerStatus status, TimeSpan timeout, string failure)
        {
            try
            {
                service.WaitForStatus(status, timeout);
            }
            catch (System.ServiceProcess.TimeoutException)
            {
                throw new InvalidOperationException(string.Format("'{0}' hizmeti {1} saniye içinde {2}.", service.ServiceName, (int)timeout.TotalSeconds, failure));
            }
        }
    }
}
