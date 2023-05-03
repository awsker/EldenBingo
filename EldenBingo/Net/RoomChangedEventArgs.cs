namespace EldenBingo.Net
{
    internal class RoomChangedEventArgs : EventArgs
    {
        public RoomChangedEventArgs(Room? previousRoom, Room? newRoom)
        {
            PreviousRoom = previousRoom;
            NewRoom = newRoom;
        }

        public Room? NewRoom { get; init; }
        public Room? PreviousRoom { get; init; }
    }
}