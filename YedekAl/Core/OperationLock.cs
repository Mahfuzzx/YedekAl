using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace YedekAl
{
    // Machine-wide lock so that only one backup or restore runs at a time, whether started from the UI or the scheduled task.
    internal sealed class OperationLock : IDisposable
    {
        private const string mutexName = @"Global\YedekAl_Yedekleme";
        public const string busyMessage = "Başka bir yedekleme ya da geri yükleme zaten çalışıyor.";

        private readonly Mutex mutex;

        private OperationLock(Mutex mutex)
        {
            this.mutex = mutex;
        }

        // Null when another operation holds the lock. Must be disposed on the thread that acquired it.
        public static OperationLock tryAcquire()
        {
            Mutex mutex;
            try
            {
                mutex = createMutex();
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }

            bool owned;
            try
            {
                owned = mutex.WaitOne(0);
            }
            catch (AbandonedMutexException)
            {
                owned = true;
            }
            if (owned) return new OperationLock(mutex);
            mutex.Dispose();
            return null;
        }

        public void Dispose()
        {
            mutex.ReleaseMutex();
            mutex.Dispose();
        }

        // Shared between the elevated UI and the SYSTEM task, so both must be allowed to open it.
        private static Mutex createMutex()
        {
            MutexSecurity security = new MutexSecurity();
            security.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), MutexRights.FullControl, AccessControlType.Allow));
            security.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), MutexRights.FullControl, AccessControlType.Allow));
            bool createdNew;
            return new Mutex(false, mutexName, out createdNew, security);
        }
    }
}
