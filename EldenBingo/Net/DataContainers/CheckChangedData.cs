using EldenBingoCommon;

namespace EldenBingo.Net.DataContainers
{
    internal class CheckChangedData
    {
        public CheckChangedData(Room room, UserInRoom? user, int indexChanged)
        {
            Room = room;
            User = user;
            Index = indexChanged;
        }

        public int Index { get; init; }
        public Room Room { get; init; }
        public UserInRoom? User { get; init; }
    }
}