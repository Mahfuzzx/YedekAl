using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace YedekAl
{
    // Replaces "shutdown -f -s -t 60 -d p:0:0 -c ..." with the underlying Win32 calls.
    internal static class SystemShutdown
    {
        public const string defaultMessage = "YedekAl: Yedekleme tamamlandı, bilgisayar kapatılıyor.";

        private const int TOKEN_ADJUST_PRIVILEGES = 0x20;
        private const int TOKEN_QUERY = 0x8;
        private const int SE_PRIVILEGE_ENABLED = 0x2;
        private const int ERROR_NOT_ALL_ASSIGNED = 1300;
        private const uint SHTDN_REASON_FLAG_PLANNED = 0x80000000;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public long Luid;
            public int Attributes;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr processHandle, int desiredAccess, out IntPtr tokenHandle);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LookupPrivilegeValue(string systemName, string name, out long luid);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, bool disableAllPrivileges, ref TOKEN_PRIVILEGES newState, int bufferLength, IntPtr previousState, IntPtr returnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool InitiateSystemShutdownEx(string machineName, string message, uint timeout, bool forceAppsClosed, bool rebootAfterShutdown, uint reason);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool AbortSystemShutdown(string machineName);

        public static void schedule(int seconds, string message)
        {
            enableShutdownPrivilege();
            if (!InitiateSystemShutdownEx(null, message, (uint)seconds, true, false, SHTDN_REASON_FLAG_PLANNED))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static void abort()
        {
            enableShutdownPrivilege();
            if (!AbortSystemShutdown(null))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        private static void enableShutdownPrivilege()
        {
            IntPtr token;
            if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out token))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            try
            {
                TOKEN_PRIVILEGES privileges = new TOKEN_PRIVILEGES { PrivilegeCount = 1, Attributes = SE_PRIVILEGE_ENABLED };
                if (!LookupPrivilegeValue(null, "SeShutdownPrivilege", out privileges.Luid))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                if (!AdjustTokenPrivileges(token, false, ref privileges, 0, IntPtr.Zero, IntPtr.Zero))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                // AdjustTokenPrivileges succeeds even when the privilege was not granted.
                if (Marshal.GetLastWin32Error() == ERROR_NOT_ALL_ASSIGNED)
                    throw new Win32Exception(ERROR_NOT_ALL_ASSIGNED);
            }
            finally
            {
                CloseHandle(token);
            }
        }
    }
}
