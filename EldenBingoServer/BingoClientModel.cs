using Neto.Shared;
using System.Net.Sockets;

namespace EldenBingoServer
{
    public class BingoClientModel : ClientModel
    {
        public BingoClientModel(TcpClient client) : base(client)
        {
        }

        public bool IsAdmin
        {
            get
            {
                if (Room == null)
                    return false;
                var clientInRoom = Room.GetUser(ClientGuid);
                return clientInRoom == null ? false : clientInRoom.IsAdmin;
            }
        }

        public bool IsSpectator
        {
            get
            {
                if (Room == null)
                    return false;
                var clientInRoom = Room.GetUser(ClientGuid);
                return clientInRoom == null ? false : clientInRoom.IsSpectator;
            }
        }

        public ServerRoom? Room { get; set; }
    }
}