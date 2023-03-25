namespace EldenBingo.Net.DataContainers
{
    internal class JoinedRoomData
    {
        public Room Room { get; init; }
        public JoinedRoomData(Room room)
        {
            Room = room;
        }
    }
}
