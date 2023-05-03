using EldenBingoCommon;

namespace EldenBingo.Net.DataContainers
{
    internal class UserJoinedLeftRoomData
    {
        public UserJoinedLeftRoomData(Room room, UserInRoom user, bool joined)
        {
            Room = room;
            User = user;
            Joined = joined;
        }

        public bool Joined { get; init; }
        public Room Room { get; init; }
        public UserInRoom User { get; init; }
    }
}