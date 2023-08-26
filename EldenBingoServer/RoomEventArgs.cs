namespace EldenBingoServer
{
    public class RoomEventArgs : EventArgs
    {
        public ServerRoom Room { get; set; }
        public RoomEventArgs(ServerRoom room)
        {
            Room = room;
        }
    }
}
