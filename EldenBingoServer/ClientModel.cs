using EldenBingoCommon;
using System.Net.Sockets;

namespace EldenBingoServer
{
    internal class ClientModel : INetSerializable
    {
        public bool IsRegistered { get; set; }
        public TcpClient TcpClient { get; init; }
        public Guid UserGuid { get; init; }

        public ServerRoom? Room { get; set; }

        public CancellationTokenSource CancellationToken { get; init; }

        public ClientModel(TcpClient client)
        {
            TcpClient = client;
            UserGuid = Guid.NewGuid();
            CancellationToken = new CancellationTokenSource();
        }

        public byte[] GetBytes()
        {
            return PacketHelper.ConcatBytes(UserGuid.ToByteArray());
        }

        public void Stop()
        {
            CancellationToken.Cancel();
            TcpClient.Close();
        }

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
    }
}
