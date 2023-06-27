using System.Text;
using B = System.BitConverter;

namespace Neto.Shared
{
    public static class PacketHelper
    {
        public static byte[] ConcatBytes(IEnumerable<byte[]> arrays)
        {
            var totalArraySize = arrays.Sum(a => a.Length);
            byte[] buffer = new byte[totalArraySize];
            int current = 0;
            foreach (var a in arrays)
            {
                a.CopyTo(buffer, current);
                current += a.Length;
            }
            return buffer;
        }

        public static byte[] ConcatBytes(params byte[][] arrays)
        {
            var totalArraySize = arrays.Sum(a => a.Length);
            byte[] buffer = new byte[totalArraySize];
            int current = 0;
            foreach (var a in arrays)
            {
                a.CopyTo(buffer, current);
                current += a.Length;
            }
            return buffer;
        }

        public static byte[] GetStringBytes(string text)
        {
            var textBytes = Encoding.UTF8.GetBytes(text);
            return ConcatBytes(BitConverter.GetBytes(textBytes.Length), textBytes);
        }

        public static bool ReadBoolean(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += 1;
            return B.ToBoolean(buffer, o);
        }

        public static byte ReadByte(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += 1;
            return buffer[o];
        }

        public static char ReadChar(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += sizeof(char);
            return B.ToChar(buffer, o);
        }

        public static short ReadShort(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += sizeof(short);
            return B.ToInt16(buffer, o);
        }

        public static ushort ReadUShort(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += sizeof(ushort);
            return B.ToUInt16(buffer, o);
        }

        public static int ReadInt(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += sizeof(int);
            return B.ToInt32(buffer, o);
        }

        public static uint ReadUInt(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += sizeof(uint);
            return B.ToUInt32(buffer, o);
        }

        public static long ReadLong(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += sizeof(long);
            return B.ToInt64(buffer, o);
        }

        public static ulong ReadULong(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += sizeof(ulong);
            return B.ToUInt64(buffer, o);
        }

        public static float ReadFloat(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += sizeof(float);
            return B.ToSingle(buffer, o);
        }

        public static double ReadDouble(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += sizeof(double);
            return B.ToDouble(buffer, o);
        }

        public static decimal ReadDecimal(byte[] buffer, ref int offset)
        {
            int[] bits = new int[4];
            for (int i = 0; i < 4; ++i)
                bits[i] = ReadInt(buffer, ref offset);
            return new decimal(bits);
        }

        public static Guid ReadGuid(byte[] buffer, ref int offset)
        {
            const int GuidSize = 16;
            var buff = new byte[GuidSize];
            Array.Copy(buffer, offset, buff, 0, GuidSize);
            offset += GuidSize;
            return new Guid(buff);
        }

        public static string ReadString(byte[] buffer, ref int offset)
        {
            var numBytes = B.ToInt32(buffer, offset);
            var o = offset + 4;
            offset += 4 + numBytes;
            return Encoding.UTF8.GetString(buffer, o, numBytes);
        }
    }
}