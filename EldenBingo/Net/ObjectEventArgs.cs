using static EldenBingoCommon.NetConstants;

namespace EldenBingo
{
    public class ObjectEventArgs : EventArgs
    {
        public ObjectEventArgs(PacketTypes packetType, object o)
        {
            PacketType = packetType;
            Object = o;
        }

        public object Object { get; init; }
        public PacketTypes PacketType { get; init; }
    }
}