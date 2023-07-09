using MessagePack;
using Neto.Shared;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Neto.Server
{
    public class NetoServer<CM> : NetObjectHandler<CM> where CM : ClientModel
    {
        private readonly ConcurrentBag<CM> _clients;
        private readonly ConcurrentBag<TcpListener> _tcpListeners;
        private readonly ConstructorInfo _clientModelConstructor;
        private CancellationTokenSource _cancelToken;

        public NetoServer(int port) : base()
        {
            Port = port;
            IPAddresses = getIpAddresses();

            _tcpListeners = new ConcurrentBag<TcpListener>();
            _clients = new ConcurrentBag<CM>();
            _cancelToken = new CancellationTokenSource();

            var clientModelConstructor = typeof(CM).GetConstructor(new[] { typeof(TcpClient) });
            if (clientModelConstructor == null)
                throw new ApplicationException("No constructor with TcpClient as argument was found");
            else
                _clientModelConstructor = clientModelConstructor;
        }

        public event EventHandler<ClientEventArgs<CM>>? OnClientConnected;

        public event EventHandler<ClientEventArgs<CM>>? OnClientDisconnected;

        public IPAddress[] IPAddresses { get; init; }
        public int Port { get; init; }
        protected bool Hosting { get; private set; }

        public static string? GetClientIp(ClientModel client)
        {
            if (client.TcpClient.Client?.RemoteEndPoint is IPEndPoint ip)
                return ip.Address.ToString();
            return null;
        }

        public void Host()
        {
            if (Hosting)
                throw new Exception("Already hosting");

            _tcpListeners.Clear();

            _cancelToken = new CancellationTokenSource();
            foreach (var ip in getIpAddresses())
            {
                var localIp = ip;
                var thread = new Thread(() => runTcpListener(localIp));
                thread.Start();
            }
            Hosting = true;
            FireOnStatus($"Hosting server on port {Port}");
        }

        public async void Stop()
        {
            if (!Hosting)
                throw new Exception("Not hosting");

            Hosting = false;

            await sendShutdownToAll();
            foreach (var tcp in _tcpListeners)
            {
                tcp.Stop();
            }
            _cancelToken.Cancel();
            FireOnStatus($"Stopped server");
        }

        public async Task SendPacketToClient(Packet p, CM client)
        {
            byte[] data;
            try
            {
                data = MessagePackSerializer.Serialize(p, GetMessagePackOptions());
                data = PacketHelper.ConcatBytes(data, NetConstants.EndOfMessage);
            }
            catch (Exception e)
            {
                FireOnError(e.Message);
                return;
            }
            try
            {
                var stream = client.TcpClient.GetStream();
                await stream.WriteAsync(data, client.CancellationToken.Token);
            }
            catch
            {
                await DropClient(client);
            }
        }

        protected virtual async Task DropClient(CM client)
        {
            if (client.IsRegistered)
            {
                fireOnClientDisconnected(client);
                client.IsRegistered = false;
            }
            client.Stop();
        }

        protected async Task KickClient(CM client)
        {
            var packet = new Packet(NetConstants.PacketTypes.ServerClientDropped);
            await SendPacketToClient(packet, client);
            await DropClient(client);
        }

        protected async Task SendPacketToAllClients(Packet p, bool onlyRegistered = false)
        {
            var clientsToInclude = onlyRegistered ? _clients.Where(c => c.IsRegistered) : _clients;
            await SendPacketToClients(p, clientsToInclude);
        }

        protected async Task SendPacketToClients(Packet p, IEnumerable<CM> clients)
        {
            byte[] data;
            try
            {
                data = MessagePackSerializer.Serialize(p, GetMessagePackOptions());
                data = PacketHelper.ConcatBytes(data, NetConstants.EndOfMessage);
            }
            catch (Exception e)
            {
                FireOnError(e.Message);
                return;
            }
            foreach (var client in clients)
            {
                try
                {
                    var stream = client.TcpClient.GetStream();
                    await stream.WriteAsync(data, client.CancellationToken.Token);
                }
                catch
                {
                    await DropClient(client);
                }
            }
        }

        protected async Task SendPacketToAllClientsExcept(Packet p, Guid except, bool onlyRegistered = false)
        {
            var clientsToInclude = onlyRegistered ? _clients.Where(c => c.IsRegistered && c.ClientGuid != except) : _clients.Where(c => c.ClientGuid != except);
            await SendPacketToClients(p, clientsToInclude);
        }

        private static IPAddress[] getIpAddresses()
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            var local = IPAddress.Parse("127.0.0.1");
            if (!addresses.Any(a => a.Equals(local)))
            {
                return addresses.Concat(new IPAddress[] { local }).ToArray();
            }
            return addresses;
        }

        private void fireOnClientConnected(CM client)
        {
            OnClientConnected?.Invoke(this, new ClientEventArgs<CM>(client));
        }

        private void fireOnClientDisconnected(CM client)
        {
            OnClientDisconnected?.Invoke(this, new ClientEventArgs<CM>(client));
        }

        private async Task acceptIncomingConnections(TcpListener tcp)
        {
            try
            {
                var tcpClient = await tcp.AcceptTcpClientAsync(_cancelToken.Token);
                var client = (CM)_clientModelConstructor.Invoke(new[] { tcpClient });
                _clients.Add(client);
                FireOnStatus($"Client connected ({GetClientIp(client)})");
                _ = Task.Run(() => clientTcpListenerTask(client));
            }
            catch (SocketException)
            {
                _cancelToken.Cancel();
            }
        }

        private async Task clientTcpListenerTask(CM client)
        {
            var ip = GetClientIp(client);
            try
            {
                while (!client.CancellationToken.IsCancellationRequested)
                {
                    await waitForPacketAsync(client);
                }
            }
            catch (Exception)
            {
                await DropClient(client);
            }
            FireOnStatus($"Client disconnected ({ip})");
        }

        private async Task handleIncomingPacket(CM client, Packet packet)
        {
            //A non-registered client's first packet better be ClientRegister, otherwise kicked and socket closed
            if (!client.IsRegistered && packet.PacketType != NetConstants.PacketTypes.ClientRegister)
            {
                await DropClient(client);
                return;
            }
            switch (packet.PacketType)
            {
                case NetConstants.PacketTypes.ClientRegister:
                    ClientRegister? objData = packet.GetObjectData<ClientRegister>();
                    if (objData?.Message != NetConstants.ClientRegisterString)
                    {
                        await DropClient(client);
                        return;
                    }
                    if (!client.IsRegistered)
                    {
                        client.IsRegistered = true;
                        var acceptPacket = new Packet(NetConstants.PacketTypes.ServerRegisterAccepted, new ServerRegisterAccepted(NetConstants.ServerRegisterString, client.ClientGuid));
                        await SendPacketToClient(acceptPacket, client);
                        fireOnClientConnected(client);
                    }
                    break;

                case NetConstants.PacketTypes.ClientDisconnect:
                    await DropClient(client);
                    break;

                case NetConstants.PacketTypes.ObjectData:
                    DispatchObjectsInPacket(client, packet);
                    break;
            }
        }

        private async void runTcpListener(IPAddress ip)
        {
            try
            {
                var tcp = new TcpListener(ip, Port);
                tcp.Start();
                _tcpListeners.Add(tcp);
                while (!_cancelToken.IsCancellationRequested)
                {
                    await acceptIncomingConnections(tcp);
                }
            }
            catch (Exception e)
            {
                _cancelToken.Cancel();
                FireOnError(e.Message);
            }
            try
            {
                await sendShutdownToAll();
            }
            catch (Exception e)
            {
                _cancelToken.Cancel();
                FireOnError(e.Message);
            }
        }

        private async Task sendShutdownToAll()
        {
            var packet = new Packet(NetConstants.PacketTypes.ServerShutdown);
            await SendPacketToAllClients(packet);
            foreach (var c in _clients)
            {
                c.Stop();
            }
        }

        private async Task waitForPacketAsync(CM client)
        {
            var stream = client.TcpClient.GetStream();
            var size = client.TcpClient.ReceiveBufferSize;
            try
            {
                MemoryStream ms = new MemoryStream(size);
                var dataChunks = new List<byte>();
                do
                {
                    ms.Seek(0, SeekOrigin.End);
                    byte[] buffer = new byte[size];
                    var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, size), client.CancellationToken.Token);
                    if (client.CancellationToken.IsCancellationRequested)
                        return;
                    ms.Write(buffer, 0, bytesRead);
                } while (!IsMessageTerminated(ms));

                var packet = ReadPacket(ms.ToArray());
                if (packet == null)
                {
                    //Drop client after 3 malformed packets
                    if(++client.MalformedPackets == 3)
                    {
                        await KickClient(client);
                        FireOnStatus($"Kicked client connected from {GetClientIp(client)}");
                    }
                }
                else
                {
                    client.MalformedPackets = Math.Max(0, client.MalformedPackets - 1);
                    await handleIncomingPacket(client, packet);
                }
            }
            catch (Exception e)
            {
                //Stream was closed, most likely due to the server shutting down
                //but could also be because client sent malformed packet
                await DropClient(client);
                FireOnError(e.Message);
            }
        }
    }
}