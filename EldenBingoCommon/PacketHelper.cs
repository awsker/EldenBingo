using System.Runtime.InteropServices;
using System.Text;
using B = System.BitConverter;

namespace EldenBingoCommon
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
        /*
        public static byte[] ConcatPackets(IEnumerable<INetSerializable> stuff)
        {
            return ConcatPackets(stuff.ToList());
        }

        public static byte[] ConcatPackets(IList<INetSerializable> stuff)
        {
            byte[][] data = new byte[stuff.Count + 1][];
            data[0] = B.GetBytes(stuff.Count);
            for (int i = 0; i < stuff.Count; ++i)
            {
                data[i + 1] = stuff[i].GetBytes();
            }
            return ConcatBytes(data);
        }
        */
        public static byte[] GetStringBytes(string text)
        {
            var textBytes = Encoding.UTF8.GetBytes(text);
            return ConcatBytes(B.GetBytes(textBytes.Length), textBytes);
        }

        public static string ReadString(byte[] buffer, ref int offset)
        {
            var numBytes = B.ToInt32(buffer, offset);
            var o = offset + 4;
            offset += 4 + numBytes;
            return Encoding.UTF8.GetString(buffer, o, numBytes);
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


        public static Guid ReadGuid(byte[] buffer, ref int offset)
        {
            const int GuidSize = 16;
            var buff = new byte[GuidSize];
            Array.Copy(buffer, offset, buff, 0, GuidSize);
            offset += GuidSize;
            return new Guid(buff);
        }

        public static bool ReadBoolean(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += 1;
            return B.ToBoolean(buffer, o);
        }

        public static float ReadFloat(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += sizeof(float);
            return B.ToSingle(buffer, o);
        }

        public static byte ReadByte(byte[] buffer, ref int offset)
        {
            var o = offset;
            offset += 1;
            return buffer[o];
        }

        public static UserInRoom ReadUserInRoom(byte[] buffer, ref int offset)
        {
            return new UserInRoom(buffer, ref offset);
        }

        public static MapCoordinates ReadMapCoordinates(byte[] buffer, ref int offset)
        {
            return new MapCoordinates(buffer, ref offset);
        }

        public static Packet CreateUserRegistrationPacket()
        {
            return new Packet(NetConstants.PacketTypes.ClientRegister, GetStringBytes(NetConstants.UserRegisterString));
        }

        public static Packet CreateCreateRoomPacket(string room, string adminPass, string nick, int color, int team, bool spectator)
        {
            var roomBytes = GetStringBytes(room);
            var adminPassBytes = GetStringBytes(adminPass);
            var nickBytes = GetStringBytes(nick);
            var colorBytes = B.GetBytes(color);
            var teamBytes = B.GetBytes(team);
            var spectatorByte = B.GetBytes(spectator);
            return new Packet(NetConstants.PacketTypes.ClientRequestCreateRoom, 
                ConcatBytes(roomBytes, adminPassBytes,
                nickBytes, colorBytes, teamBytes, spectatorByte));
        }

        public static Packet CreateJoinRoomPacket(string room, string adminPass, string nick, int color, int team, bool spectator)
        {
            var roomBytes = GetStringBytes(room);
            var adminPassBytes = GetStringBytes(adminPass);
            var nickBytes = GetStringBytes(nick);
            var colorBytes = B.GetBytes(color);
            var teamBytes = B.GetBytes(team);
            var spectatorByte = B.GetBytes(spectator);
            return new Packet(NetConstants.PacketTypes.ClientRequestJoinRoom,
                ConcatBytes(roomBytes, adminPassBytes,
                nickBytes, colorBytes, teamBytes, spectatorByte));
        }

        public static Packet CreateUserToServerChatMessage(string text)
        {
            var data = GetStringBytes(text);
            return new Packet(NetConstants.PacketTypes.ClientChat, data);
        }

        public static Packet CreateCoordinatesPacket(MapCoordinates? coordinates)
        {
            var coords = coordinates ?? default;
            return new Packet(NetConstants.PacketTypes.ClientCoordinates, coords.GetBytes());
        }
    }
}
