using EldenBingoCommon;

namespace EldenBingo.GameInterop
{
    public class MapCoordinateEventArgs : EventArgs
    {
        public MapCoordinates? Coordinates { get; init; }
        public MapCoordinateEventArgs(MapCoordinates? coordinates)
        {
            Coordinates = coordinates;
        }
    }
}
