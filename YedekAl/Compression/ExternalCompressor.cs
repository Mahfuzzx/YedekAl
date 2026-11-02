using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace YedekAl
{
    // Base for formats that need their own console tool (rar.exe, 7za.exe). Arguments are built here, never typed by the user.
    internal abstract class ExternalCompressor : ICompressor
    {
        private static readonly Regex percentPattern = new Regex(@"(\d{1,3})%", RegexOptions.Compiled);

        protected ExternalCompressor(string configuredPath, Func<string> findDefault, string password)
        {
            exePath = string.IsNullOrWhiteSpace(configuredPath) ? findDefault() : configuredPath.Trim();
            this.password = string.IsNullOrEmpty(password) ? null : password;
        }

        public string exePath { get; }
        protected string password { get; }
        public abstract string name { get; }
        public abstract string extension { get; }
        protected abstract Encoding outputEncoding { get; }

        protected abstract string compressArguments(SourceTree tree, string archivePath, CompressionStrength strength);
        protected abstract string verifyArguments(string archivePath);
        // Extracts into the current directory, which run() sets to the destination.
        protected abstract string extractArguments(string archivePath);
        protected abstract string describeExitCode(int code);

        public string checkAvailable()
        {
            if (string.IsNullOrEmpty(exePath)) return name + " programı bulunamadı.";
            if (!File.Exists(exePath)) return name + " programı bulunamadı: " + exePath;
            return null;
        }

        public void compress(SourceTree tree, string archivePath, CompressionStrength strength, IProgress<int> progress, CancellationToken token)
        {
            run(compressArguments(tree, archivePath, strength), createProgressParser(tree.files.Count, progress), token);
        }

        public void verify(string archivePath, int expectedFiles, IProgress<int> progress, CancellationToken token)
        {
            run(verifyArguments(archivePath), createProgressParser(Math.Max(0, expectedFiles), progress), token);
        }

        public void extract(string archivePath, string destDir, IProgress<int> progress, CancellationToken token)
        {
            run(extractArguments(archivePath), createProgressParser(0, progress), token, destDir);
        }

        // Default: the tool prints an overall percentage (7-Zip with -bsp1).
        protected virtual Action<string> createProgressParser(int fileCount, IProgress<int> progress)
        {
            PercentReporter reporter = new PercentReporter(progress, 100);
            return chunk =>
            {
                MatchCollection matches = percentPattern.Matches(chunk);
                if (matches.Count > 0)
                    reporter.reportPercent(Math.Min(100, int.Parse(matches[matches.Count - 1].Groups[1].Value)));
            };
        }

        private void run(string arguments, Action<string> onOutput, CancellationToken token, string workingDirectory = null)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(exePath, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = outputEncoding,
                StandardErrorEncoding = outputEncoding,
                WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(exePath)
            };

            using (Process process = Process.Start(startInfo))
            using (token.Register(() => kill(process)))
            {
                // Closed stdin makes any unexpected prompt (e.g. "Enter password") fail instead of hanging.
                process.StandardInput.Close();
                Task<string> errorTask = process.StandardError.ReadToEndAsync();
                StringBuilder tail = new StringBuilder();
                char[] buffer = new char[4096];
                int read;
                while ((read = process.StandardOutput.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string chunk = new string(buffer, 0, read);
                    onOutput(chunk);
                    tail.Append(chunk);
                    if (tail.Length > 8192) tail.Remove(0, tail.Length - 8192);
                }
                process.WaitForExit();
                string errors = errorTask.Result;
                token.ThrowIfCancellationRequested();

                if (process.ExitCode != 0)
                {
                    string detail = clean(errors.Trim().Length > 0 ? errors : tail.ToString());
                    throw new InvalidOperationException(string.Format("{0} {1} hata koduyla sonlandı ({2}).{3}",
                        name, process.ExitCode, describeExitCode(process.ExitCode), detail.Length > 0 ? " " + detail : ""));
                }
            }
        }

        private static void kill(Process process)
        {
            try
            {
                if (!process.HasExited) process.Kill();
            }
            catch (InvalidOperationException) { }
            catch (Win32Exception) { }
        }

        private static string clean(string output)
        {
            string text = Regex.Replace(output.Replace('\b', ' '), @"\s+", " ").Trim();
            return text.Length > 500 ? "..." + text.Substring(text.Length - 500) : text;
        }

        protected static string quote(string path)
        {
            return "\"" + path.TrimEnd('\\') + "\"";
        }

        // Settings.checkPassword() rejects quotes and a trailing backslash, so plain quoting is safe here.
        protected string passwordSwitch(string prefix)
        {
            return password == null ? "" : " \"" + prefix + password + "\"";
        }

        protected static string readRegistry(string subKey, string valueName)
        {
            foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                using (RegistryKey key = baseKey.OpenSubKey(subKey))
                {
                    string value = key?.GetValue(valueName) as string;
                    if (!string.IsNullOrEmpty(value)) return value;
                }
            }
            return null;
        }

        protected static string firstExisting(params string[] paths)
        {
            foreach (string path in paths)
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) return path;
            return null;
        }
    }
}
