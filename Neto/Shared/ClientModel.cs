using System.Net;
using System.Net.Sockets;

namespace Neto.Shared
{
    public class ClientModel
    {
        public ClientModel(TcpClient client)
        {
            TcpClient = client;
            var ip = client.Client.RemoteEndPoint as IPEndPoint;
            if (ip != null)
                IPAddress = ip.Address;
            ClientGuid = Guid.NewGuid();
            CancellationToken = new CancellationTokenSource();
            LastActivity = DateTime.Now;
            WriteSemaphore = new SemaphoreSlim(1, 1);
        }

        public TcpClient TcpClient { get; init; }
        public IPAddress IPAddress { get; init; }
        public Guid ClientGuid { get; internal set; }
        public CancellationTokenSource CancellationToken { get; init; }
        public bool IsRegistered { get; set; }
        internal int MalformedPackets { get; set; }
        public DateTime LastActivity { get; set; }
        public SemaphoreSlim WriteSemaphore { get; init; }

        public virtual void Stop()
        {
            CancellationToken.Cancel();
            TcpClient.Close();
        }
    }
}