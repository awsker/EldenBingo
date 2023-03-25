using EldenBingoCommon;

namespace EldenBingo.Net.DataContainers
{
    internal class UserJoinedLeftRoomData
    {
        public Room Room { get; init; }
        public UserInRoom User { get; init; }
        public bool Joined { get; init; }

        public UserJoinedLeftRoomData(Room room, UserInRoom user, bool joined)
        {
            Room = room;
            User = user;
            Joined = joined;
        }
    }
}
