using System;
using System.Collections.Generic;
using System.IO;

namespace YedekAl
{
    internal sealed class SourceFile
    {
        public readonly string fullPath;
        public readonly string relativePath;
        public readonly long length;

        public SourceFile(string fullPath, string relativePath, long length)
        {
            this.fullPath = fullPath;
            this.relativePath = relativePath;
            this.length = length;
        }
    }

    // Snapshot of the database folder (or single file). Relative paths start with the root name, e.g. "db\sub\file.dat".
    internal sealed class SourceTree
    {
        public string rootPath { get; private set; }
        public string rootName { get; private set; }
        public List<SourceFile> files { get; } = new List<SourceFile>();
        public List<string> directories { get; } = new List<string>();
        public long totalBytes { get; private set; }

        public static SourceTree scan(string path)
        {
            string fullPath = normalize(path);
            SourceTree tree = new SourceTree { rootPath = fullPath, rootName = Path.GetFileName(fullPath) };
            if (File.Exists(fullPath))
            {
                tree.add(new FileInfo(fullPath), tree.rootName);
                return tree;
            }

            DirectoryInfo root = new DirectoryInfo(fullPath);
            if (!root.Exists) throw new DirectoryNotFoundException("Veritabanı konumu bulunamadı: " + fullPath);
            tree.directories.Add(tree.rootName);
            foreach (DirectoryInfo dir in root.EnumerateDirectories("*", SearchOption.AllDirectories))
                tree.directories.Add(tree.relative(dir.FullName));
            foreach (FileInfo file in root.EnumerateFiles("*", SearchOption.AllDirectories))
                tree.add(file, tree.relative(file.FullName));
            return tree;
        }

        public static string normalize(string path)
        {
            string full = Path.GetFullPath(path.Trim());
            string root = Path.GetPathRoot(full);
            return full.Length > root.Length ? full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) : full;
        }

        public static bool isRoot(string path)
        {
            string full = normalize(path);
            return string.Equals(full.TrimEnd('\\'), Path.GetPathRoot(full).TrimEnd('\\'), StringComparison.OrdinalIgnoreCase);
        }

        public static bool isSameOrInside(string path, string parent)
        {
            return string.Equals(path, parent, StringComparison.OrdinalIgnoreCase)
                || path.StartsWith(parent.TrimEnd('\\') + "\\", StringComparison.OrdinalIgnoreCase);
        }

        private string relative(string fullName)
        {
            return Path.Combine(rootName, fullName.Substring(rootPath.Length + 1));
        }

        private void add(FileInfo file, string relativePath)
        {
            files.Add(new SourceFile(file.FullName, relativePath, file.Length));
            totalBytes += file.Length;
        }
    }
}
