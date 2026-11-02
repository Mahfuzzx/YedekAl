using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace YedekAl
{
    internal static class SystemInfo
    {
        private static readonly CultureInfo turkish = CultureInfo.GetCultureInfo("tr-TR");

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpaceEx(string directoryName, out ulong freeBytesAvailable, out ulong totalBytes, out ulong totalFreeBytes);

        public static bool isAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        // Works for local folders and UNC shares; null when the size cannot be read.
        public static long? freeSpace(string directory)
        {
            ulong free, total, totalFree;
            string path = directory.EndsWith("\\") ? directory : directory + "\\";
            if (!GetDiskFreeSpaceEx(path, out free, out total, out totalFree)) return null;
            return (long)free;
        }

        public static string formatSize(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int unit = 0;
            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }
            return size.ToString(unit == 0 ? "0" : "0.#", turkish) + " " + units[unit];
        }
    }
}
