using EldenBingoCommon;

namespace EldenBingo.GameInterop
{
    public class MapCoordinateEventArgs : EventArgs
    {
        public MapCoordinateEventArgs(MapCoordinates? coordinates)
        {
            Coordinates = coordinates;
        }

        public MapCoordinates? Coordinates { get; init; }
    }
}