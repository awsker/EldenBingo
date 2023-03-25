using static EldenBingoCommon.NetConstants;

namespace EldenBingo
{
    public class ObjectEventArgs : EventArgs
    {
        public PacketTypes PacketType { get; init; }
        public object Object { get; init; }

        public ObjectEventArgs(PacketTypes packetType, object o)
        {
            PacketType = packetType;
            Object = o;
        }
    }
}
