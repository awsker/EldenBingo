namespace EldenBingo.Net.DataContainers
{
    internal class JoinedRoomData
    {
        public JoinedRoomData(Room room)
        {
            Room = room;
        }

        public Room Room { get; init; }
    }
}