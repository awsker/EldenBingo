namespace EldenBingoCommon
{
    public struct MapCoordinates : INetSerializable, IEquatable<MapCoordinates>
    {
        public float X, Y, Angle;
        public bool IsUnderground;

        public MapCoordinates(float x, float y, bool underground, float angle)
        {
            X = x;
            Y = y;
            IsUnderground = underground;
            Angle = angle;
        }

        public MapCoordinates(byte[] bytes, ref int offset)
        {
            X = PacketHelper.ReadFloat(bytes, ref offset);
            Y = PacketHelper.ReadFloat(bytes, ref offset);
            IsUnderground = PacketHelper.ReadBoolean(bytes, ref offset);
            Angle = PacketHelper.ReadFloat(bytes, ref offset);
        }

        public byte[] GetBytes()
        {
            return PacketHelper.ConcatBytes(
                BitConverter.GetBytes(X),
                BitConverter.GetBytes(Y),
                BitConverter.GetBytes(IsUnderground),
                BitConverter.GetBytes(Angle)
            );
        }

        public bool Equals(MapCoordinates other)
        {
            return X == other.X && Y == other.Y && Angle == other.Angle && IsUnderground == other.IsUnderground;
        }

        public override string ToString()
        {
            return $"{X}, {Y} Angle: {Angle}";
        }
    }
}
