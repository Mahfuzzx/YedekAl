using System;
using System.IO;
using System.Linq;

namespace YedekAl.Tests
{
    // A temporary folder that is deleted again, with a small sample "database" inside when asked for.
    internal sealed class TestFolder : IDisposable
    {
        public TestFolder()
        {
            path = Path.Combine(Path.GetTempPath(), "yedekal_test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
        }

        public string path { get; }

        public string sub(params string[] parts)
        {
            return Path.Combine(new[] { path }.Concat(parts).ToArray());
        }

        // Folder "Veri Tabanı" with Turkish names, nesting, an empty folder and a 3 MB incompressible file.
        public string createDatabase(string name = "Veri Tabanı")
        {
            string db = sub(name);
            Directory.CreateDirectory(Path.Combine(db, @"alt\derin"));
            Directory.CreateDirectory(Path.Combine(db, "boş klasör"));
            File.WriteAllText(Path.Combine(db, "a.txt"), "merhaba dünya");
            byte[] big = new byte[3 << 20];
            new Random(1).NextBytes(big);
            File.WriteAllBytes(Path.Combine(db, @"alt\buyuk.bin"), big);
            File.WriteAllText(Path.Combine(db, @"alt\derin\ğüşiöç.txt"), "türkçe");
            return db;
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        // Same files, folders and contents.
        public static bool sameTree(string a, string b)
        {
            string[] filesA = relative(a, Directory.GetFiles(a, "*", SearchOption.AllDirectories));
            string[] filesB = relative(b, Directory.GetFiles(b, "*", SearchOption.AllDirectories));
            string[] dirsA = relative(a, Directory.GetDirectories(a, "*", SearchOption.AllDirectories));
            string[] dirsB = relative(b, Directory.GetDirectories(b, "*", SearchOption.AllDirectories));
            return filesA.SequenceEqual(filesB) && dirsA.SequenceEqual(dirsB)
                && filesA.All(f => File.ReadAllBytes(a + f).SequenceEqual(File.ReadAllBytes(b + f)));
        }

        private static string[] relative(string root, string[] paths)
        {
            return paths.Select(p => p.Substring(root.Length)).OrderBy(p => p, StringComparer.Ordinal).ToArray();
        }
    }
}
