using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YedekAl
{
    // Reads the CRC-32 of every entry from a ZIP file's central directory (ZIP64 aware).
    // .NET Framework's ZipArchive neither checks nor exposes these, so verify() needs them from here.
    internal static class ZipCentralDirectory
    {
        private const uint endOfCentralDirectorySignature = 0x06054b50;
        private const uint zip64LocatorSignature = 0x07064b50;
        private const uint zip64EndSignature = 0x06064b50;
        private const uint centralHeaderSignature = 0x02014b50;
        private const int endRecordSize = 22;
        private const int maxCommentSize = 0xFFFF;
        private const int utf8NameFlag = 1 << 11;

        public static Dictionary<string, uint> readChecksums(string archivePath)
        {
            using (FileStream stream = File.OpenRead(archivePath))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                long endRecord = findEndRecord(stream, reader);
                stream.Position = endRecord + 10;
                long entryCount = reader.ReadUInt16();
                stream.Position = endRecord + 16;
                long directoryOffset = reader.ReadUInt32();

                if (entryCount == 0xFFFF || directoryOffset == 0xFFFFFFFF)
                    readZip64Values(stream, reader, endRecord, ref entryCount, ref directoryOffset);

                Encoding legacyNames = Encoding.GetEncoding(437);
                Dictionary<string, uint> checksums = new Dictionary<string, uint>(StringComparer.Ordinal);
                stream.Position = directoryOffset;
                for (long i = 0; i < entryCount; i++)
                {
                    if (reader.ReadUInt32() != centralHeaderSignature) throw new InvalidDataException("ZIP merkezi dizini bozuk.");
                    stream.Position += 4;                      // version made by, version needed
                    int flags = reader.ReadUInt16();
                    stream.Position += 6;                      // method, time, date
                    uint crc = reader.ReadUInt32();
                    stream.Position += 8;                      // compressed and uncompressed size
                    int nameLength = reader.ReadUInt16();
                    int extraLength = reader.ReadUInt16();
                    int commentLength = reader.ReadUInt16();
                    stream.Position += 12;                     // disk, attributes, local header offset
                    byte[] name = reader.ReadBytes(nameLength);
                    stream.Position += extraLength + commentLength;
                    Encoding encoding = (flags & utf8NameFlag) != 0 ? Encoding.UTF8 : legacyNames;
                    checksums[encoding.GetString(name)] = crc;
                }
                return checksums;
            }
        }

        private static long findEndRecord(FileStream stream, BinaryReader reader)
        {
            long start = Math.Max(0, stream.Length - endRecordSize - maxCommentSize);
            for (long position = stream.Length - endRecordSize; position >= start; position--)
            {
                stream.Position = position;
                if (reader.ReadUInt32() == endOfCentralDirectorySignature) return position;
            }
            throw new InvalidDataException("ZIP dosyasının sonu bulunamadı; arşiv eksik ya da bozuk.");
        }

        private static void readZip64Values(FileStream stream, BinaryReader reader, long endRecord, ref long entryCount, ref long directoryOffset)
        {
            stream.Position = endRecord - 20;
            if (reader.ReadUInt32() != zip64LocatorSignature) throw new InvalidDataException("ZIP64 bilgisi bulunamadı.");
            stream.Position += 4;
            long zip64End = (long)reader.ReadUInt64();
            stream.Position = zip64End;
            if (reader.ReadUInt32() != zip64EndSignature) throw new InvalidDataException("ZIP64 kaydı bozuk.");
            stream.Position = zip64End + 32;
            entryCount = (long)reader.ReadUInt64();
            stream.Position = zip64End + 48;
            directoryOffset = (long)reader.ReadUInt64();
        }
    }
}
