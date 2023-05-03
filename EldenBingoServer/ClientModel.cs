using EldenBingoCommon;
using System.Net.Sockets;

namespace EldenBingoServer
{
    public class ClientModel : INetSerializable
    {
        public ClientModel(TcpClient client)
        {
            TcpClient = client;
            UserGuid = Guid.NewGuid();
            CancellationToken = new CancellationTokenSource();
        }

        public CancellationTokenSource CancellationToken { get; init; }

        public bool IsAdmin
        {
            get
            {
                if (Room == null)
                    return false;
                var clientInRoom = Room.GetClient(UserGuid);
                return clientInRoom == null ? false : clientInRoom.IsAdmin;
            }
        }

        public bool IsRegistered { get; set; }

        public bool IsSpectator
        {
            get
            {
                if (Room == null)
                    return false;
                var clientInRoom = Room.GetClient(UserGuid);
                return clientInRoom == null ? false : clientInRoom.IsSpectator;
            }
        }

        public ServerRoom? Room { get; set; }
        public TcpClient TcpClient { get; init; }
        public Guid UserGuid { get; init; }

        public byte[] GetBytes()
        {
            return PacketHelper.ConcatBytes(UserGuid.ToByteArray());
        }

        public void Stop()
        {
            CancellationToken.Cancel();
            TcpClient.Close();
        }
    }
}