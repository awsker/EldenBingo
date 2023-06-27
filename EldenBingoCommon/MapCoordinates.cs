namespace EldenBingoCommon
{
    public struct MapCoordinates : IEquatable<MapCoordinates>
    {
        public bool IsUnderground;
        public float X, Y, Angle;

        public MapCoordinates(float x, float y, bool underground, float angle)
        {
            X = x;
            Y = y;
            IsUnderground = underground;
            Angle = angle;
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