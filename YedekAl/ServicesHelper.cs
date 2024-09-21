using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace YedekAl
{
    internal class ServicesHelper
    {
        // API Constants
        public const string SERVICES_ACTIVE_DATABASE = "ServicesActive";
        // Service Control
        public const int SERVICE_CONTROL_STOP = 0x1;
        public const int SERVICE_CONTROL_PAUSE = 0x2;
        // Service State - for CurrentState
        public const int SERVICE_STOPPED = 0x1;
        public const int SERVICE_START_PENDING = 0x2;
        public const int SERVICE_STOP_PENDING = 0x3;
        public const int SERVICE_RUNNING = 0x4;
        public const int SERVICE_CONTINUE_PENDING = 0x5;
        public const int SERVICE_PAUSE_PENDING = 0x6;
        public const int SERVICE_PAUSED = 0x7;
        // Service Control Manager object specific access types
        public const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        public const int SC_MANAGER_CONNECT = 0x1;
        public const int SC_MANAGER_CREATE_SERVICE = 0x2;
        public const int SC_MANAGER_ENUMERATE_SERVICE = 0x4;
        public const int SC_MANAGER_LOCK = 0x8;
        public const int SC_MANAGER_QUERY_LOCK_STATUS = 0x10;
        public const int SC_MANAGER_MODIFY_BOOT_CONFIG = 0x20;
        public const int SC_MANAGER_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SC_MANAGER_CONNECT | SC_MANAGER_CREATE_SERVICE | SC_MANAGER_ENUMERATE_SERVICE | SC_MANAGER_LOCK | SC_MANAGER_QUERY_LOCK_STATUS | SC_MANAGER_MODIFY_BOOT_CONFIG);
        // Service object specific access types
        public const int SERVICE_QUERY_CONFIG = 0x1;
        public const int SERVICE_CHANGE_CONFIG = 0x2;
        public const int SERVICE_QUERY_STATUS = 0x4;
        public const int SERVICE_ENUMERATE_DEPENDENTS = 0x8;
        public const int SERVICE_START = 0x10;
        public const int SERVICE_STOP = 0x20;
        public const int SERVICE_PAUSE_CONTINUE = 0x40;
        public const int SERVICE_INTERROGATE = 0x80;
        public const int SERVICE_USER_DEFINED_CONTROL = 0x100;
        public const int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG | SERVICE_QUERY_STATUS | SERVICE_ENUMERATE_DEPENDENTS | SERVICE_START | SERVICE_STOP | SERVICE_PAUSE_CONTINUE | SERVICE_INTERROGATE | SERVICE_USER_DEFINED_CONTROL);

        struct SERVICE_STATUS
        {
            public int dwServiceType;
            public int dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        }
        /*
         * 
            Declare Function CloseServiceHandle Lib "advapi32.dll" (ByVal hSCObject As Long) As Long
            Declare Function ControlService Lib "advapi32.dll" (ByVal hService As Long, ByVal dwControl As Long, lpServiceStatus As SERVICE_STATUS) As Long
            Declare Function OpenSCManager Lib "advapi32.dll" Alias "OpenSCManagerA" (ByVal lpMachineName As String, ByVal lpDatabaseName As String, ByVal dwDesiredAccess As Long) As Long
            Declare Function OpenService Lib "advapi32.dll" Alias "OpenServiceA" (ByVal hSCManager As Long, ByVal lpServiceName As String, ByVal dwDesiredAccess As Long) As Long
            Declare Function QueryServiceStatus Lib "advapi32.dll" (ByVal hService As Long, lpServiceStatus As SERVICE_STATUS) As Long
            Declare Function StartService Lib "advapi32.dll" Alias "StartServiceA" (ByVal hService As Long, ByVal dwNumServiceArgs As Long, ByVal lpServiceArgVectors As Long) As Long
         * */
        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll")]
        static extern int ControlService(IntPtr hService, int dwControl, ref SERVICE_STATUS lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, int dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, int dwDesiredAccess);

        [DllImport("advapi32.dll")]
        static extern int QueryServiceStatus(IntPtr hService, ref SERVICE_STATUS lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool StartService(IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);

        private readonly FrmMain owner;

        public ServicesHelper(FrmMain owner)
        {
            this.owner = owner;
        }

        public string serviceStatus(string ComputerName, string ServiceName)
        {
            // Declare the service status structure. 
            SERVICE_STATUS ServiceStat = new SERVICE_STATUS();
            // Declare the handles for the service manager and the service. 
            IntPtr hSManager;
            IntPtr hService;
            // Declare the result variable. 
            string ServiceStatus = "";
            // Open the service manager. 
            hSManager = OpenSCManager(ComputerName, SERVICES_ACTIVE_DATABASE, SC_MANAGER_ALL_ACCESS);
            if (hSManager != IntPtr.Zero)
            {
                // Open the service.
                hService = OpenService(hSManager, ServiceName, SERVICE_ALL_ACCESS);
                if (hService != IntPtr.Zero)
                {
                    // Query the service status.
                    if (QueryServiceStatus(hService, ref ServiceStat) > 0)
                    {
                        // Check the current state of the service.
                        switch (ServiceStat.dwCurrentState)
                        {
                            case SERVICE_STOPPED:
                                ServiceStatus = "Stopped";
                                break;
                            case SERVICE_START_PENDING:
                                ServiceStatus = "Start Pending";
                                break;
                            case SERVICE_STOP_PENDING:
                                ServiceStatus = "Stop Pending";
                                break;
                            case SERVICE_RUNNING:
                                ServiceStatus = "Running";
                                break;
                            case SERVICE_CONTINUE_PENDING:
                                ServiceStatus = "Coninue Pending";
                                break;
                            case SERVICE_PAUSE_PENDING:
                                ServiceStatus = "Pause Pending";
                                break;
                            case SERVICE_PAUSED:
                                ServiceStatus = "Paused";
                                break;
                        }
                    }
                    // Close the service handle.
                    CloseServiceHandle(hService);
                }
                // Close the service manager handle.
                CloseServiceHandle(hSManager);
            }
            // Return the service status.
            return ServiceStatus;
        }

        public bool serviceStart(string ComputerName, string ServiceName)
        {
            owner.log("Veritabanı çalıştırılıyor... ");

            //SERVICE_STATUS ServiceStatus = new SERVICE_STATUS();
            IntPtr hSManager;
            IntPtr hService;

            hSManager = OpenSCManager(ComputerName, SERVICES_ACTIVE_DATABASE, SC_MANAGER_ALL_ACCESS);
            if (hSManager.ToInt64() != 0)
            {
                hService = OpenService(hSManager, ServiceName, SERVICE_ALL_ACCESS);
                if (hService.ToInt64() != 0)
                {
                    StartService(hService, 0, 0);
                    CloseServiceHandle(hService);
                }
                CloseServiceHandle(hSManager);
            }

            Stopwatch sw = Stopwatch.StartNew();
            while (owner.sStatus != "Running")
            {
                if (sw.ElapsedMilliseconds > 10000)
                {
                    sw.Stop();
                    owner.log("HATA: Veritabanı çalıştırılamadı!\r\n");
                    return false;
                }
                Application.DoEvents();
            }
            owner.log("TAMAM!\r\n");
            return true;
        }

        public bool serviceStop(string ComputerName, string ServiceName)
        {
            owner.log("Veritabanı durduruluyor... ");

            SERVICE_STATUS ServiceStatus = new SERVICE_STATUS();
            IntPtr hSManager;
            IntPtr hService;

            hSManager = OpenSCManager(ComputerName, SERVICES_ACTIVE_DATABASE, SC_MANAGER_ALL_ACCESS);
            if (hSManager.ToInt64() != 0)
            {
                hService = OpenService(hSManager, ServiceName, SERVICE_ALL_ACCESS);
                if (hService.ToInt64() != 0)
                {
                    ControlService(hService, SERVICE_CONTROL_STOP, ref ServiceStatus);
                    CloseServiceHandle(hService);
                }
                CloseServiceHandle(hSManager);
            }

            Stopwatch sw = Stopwatch.StartNew();
            while (owner.sStatus != "Stopped")
            {
                if (sw.ElapsedMilliseconds > 10000)
                {
                    sw.Stop();
                    owner.log("HATA: Veritabanı durdurulamadı!\r\n");
                    return false;
                }
                Application.DoEvents();
            }
            owner.log("TAMAM!\r\n");
            return true;
        }
    }
}
