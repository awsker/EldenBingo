namespace EldenBingo.Net
{
    internal class RoomChangedEventArgs : EventArgs
    {
        public Room? PreviousRoom { get; init; }
        public Room? NewRoom { get; init; }
        public RoomChangedEventArgs(Room? previousRoom, Room? newRoom)
        {
            PreviousRoom = previousRoom;
            NewRoom = newRoom;
        }
    }
}
