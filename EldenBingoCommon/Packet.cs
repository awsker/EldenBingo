namespace EldenBingoCommon
{
    public class Packet
    {
        public const int HeaderSize = 5;

        private byte[]? _dataBytes = null;

        public Packet(byte[] buffer)
        {
            if (buffer.Length < HeaderSize)
                throw new Exception("Invalid packet");
            PacketType = (NetConstants.PacketTypes)buffer[0];
            DataSize = BitConverter.ToInt32(buffer, 1);
            Bytes = buffer;
        }

        public Packet(NetConstants.PacketTypes packetType, byte[] data)
        {
            PacketType = packetType;
            DataSize = data.Length;
            Bytes = createByteBufferWithHeader(packetType, DataSize);
            data.CopyTo(Bytes, HeaderSize);
        }

        public byte[] Bytes { get; }

        public byte[] DataBytes
        {
            get
            {
                if (_dataBytes == null)
                {
                    var buffer = new byte[DataSize];
                    Array.Copy(Bytes, HeaderSize, buffer, 0, DataSize);
                    _dataBytes = buffer;
                }
                return _dataBytes;
            }
        }

        public int DataSize { get; }
        public NetConstants.PacketTypes PacketType { get; }
        public int TotalSize => HeaderSize + DataSize;

        private byte[] createByteBufferWithHeader(NetConstants.PacketTypes packetType, int size)
        {
            var buffer = new byte[HeaderSize + size];
            buffer[0] = (byte)packetType;
            BitConverter.GetBytes(size).CopyTo(buffer, 1);
            return buffer;
        }
    }
}