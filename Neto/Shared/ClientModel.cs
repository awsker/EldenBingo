using System.Net.Sockets;

namespace Neto.Shared
{
    public class ClientModel
    {
        public ClientModel(TcpClient client)
        {
            TcpClient = client;
            ClientGuid = Guid.NewGuid();
            CancellationToken = new CancellationTokenSource();
        }

        public TcpClient TcpClient { get; init; }
        public Guid ClientGuid { get; internal set; }
        public CancellationTokenSource CancellationToken { get; init; }
        public bool IsRegistered { get; set; }
        internal int MalformedPackets { get; set; }

        public virtual void Stop()
        {
            CancellationToken.Cancel();
            TcpClient.Close();
        }
    }
}