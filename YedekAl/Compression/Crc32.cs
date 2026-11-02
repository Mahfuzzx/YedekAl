namespace YedekAl
{
    internal sealed class Crc32
    {
        private static readonly uint[] table = createTable();
        private uint crc = 0xFFFFFFFF;

        public uint value { get { return ~crc; } }

        public void update(byte[] buffer, int offset, int count)
        {
            uint c = crc;
            for (int i = offset; i < offset + count; i++)
                c = table[(c ^ buffer[i]) & 0xFF] ^ (c >> 8);
            crc = c;
        }

        private static uint[] createTable()
        {
            uint[] result = new uint[256];
            for (uint n = 0; n < 256; n++)
            {
                uint c = n;
                for (int k = 0; k < 8; k++)
                    c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
                result[n] = c;
            }
            return result;
        }
    }
}
