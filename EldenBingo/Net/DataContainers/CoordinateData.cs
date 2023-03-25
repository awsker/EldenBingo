using EldenBingoCommon;

namespace EldenBingo.Net.DataContainers
{
    internal class CoordinateData
    {
        public UserInRoom User { get; init; }
        public MapCoordinates Coordinates { get; init; }

        public CoordinateData(UserInRoom user, MapCoordinates coordinates)
        {
            User = user;
            Coordinates = coordinates;
        }
    }
}
