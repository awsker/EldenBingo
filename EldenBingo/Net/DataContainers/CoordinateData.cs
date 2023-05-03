using EldenBingoCommon;

namespace EldenBingo.Net.DataContainers
{
    internal class CoordinateData
    {
        public CoordinateData(UserInRoom user, MapCoordinates coordinates)
        {
            User = user;
            Coordinates = coordinates;
        }

        public MapCoordinates Coordinates { get; init; }
        public UserInRoom User { get; init; }
    }
}