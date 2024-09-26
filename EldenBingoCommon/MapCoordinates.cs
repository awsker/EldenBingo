namespace EldenBingoCommon
{
    public enum MapInstance
    {
        MainMap,
        DLC,
    }

    public struct MapCoordinates : IEquatable<MapCoordinates>
    {
        public bool IsUnderground;
        public MapInstance MapInstance;
        public float X, Y, Angle;

        public MapCoordinates(float x, float y, bool underground, float angle, MapInstance map)
        {
            X = x;
            Y = y;
            IsUnderground = underground;
            Angle = angle;
            MapInstance = map;
        }

        public bool Equals(MapCoordinates other)
        {
            return X == other.X && Y == other.Y && Angle == other.Angle && IsUnderground == other.IsUnderground && MapInstance == other.MapInstance;
        }

        public override string ToString()
        {
            return $"{X}, {Y} Angle: {Angle}";
        }
    }
}