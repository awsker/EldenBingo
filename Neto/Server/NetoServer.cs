﻿using MessagePack;
using Neto.Shared;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Neto.Server
{
    public class NetoServer<CM> : NetObjectHandler<CM> where CM : ClientModel
    {
        private const float KeepAliveTime = 25f;
        private readonly ConcurrentDictionary<Guid, CM> _clients;
        private readonly ConcurrentBag<TcpListener> _tcpListeners;
        private readonly ConstructorInfo _clientModelConstructor;
        private CancellationTokenSource _cancelToken;

        private System.Timers.Timer _keepAliveTimer;

        public NetoServer(int port) : base()
        {
            Port = port;
            IPAddresses = getIpAddresses();

            _tcpListeners = new ConcurrentBag<TcpListener>();
            _clients = new ConcurrentDictionary<Guid, CM>();
            _cancelToken = new CancellationTokenSource();
            var clientModelConstructor = typeof(CM).GetConstructor(new[] { typeof(TcpClient) });
            if (clientModelConstructor == null)
                throw new ApplicationException("No constructor with TcpClient as argument was found");
            else
                _clientModelConstructor = clientModelConstructor;

            CachedIdentities = new ConcurrentDictionary<string, ClientIdentity>();

            _keepAliveTimer = new System.Timers.Timer(5000f);
            _keepAliveTimer.Elapsed += keepAlive;
            _keepAliveTimer.Start();
        }

        ~NetoServer()
        {
            _cancelToken.Dispose();
        }

        public event EventHandler<ClientEventArgs<CM>>? OnClientConnected;

        public event EventHandler<ClientEventArgs<CM>>? OnClientDisconnected;

        public virtual string Version => "1";

        public IPAddress[] IPAddresses { get; init; }
        public int Port { get; init; }
        protected bool Hosting { get; private set; }
        protected ConcurrentDictionary<string, ClientIdentity> CachedIdentities { get; set; }

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

        public virtual async Task Stop()
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
            await sendBytesToClient(data, client);
        }

        protected virtual async Task DropClient(CM client)
        {
            if (client.IsRegistered)
            {
                fireOnClientDisconnected(client);
                client.IsRegistered = false;
            }
            client.Stop();
            _clients.Remove(client.ClientGuid, out _);
        }

        protected async Task KickClient(CM client, string reason)
        {
            var packet = new Packet(PacketTypes.ServerClientDropped, new ServerKicked(reason));
            await SendPacketToClient(packet, client);
            await DropClient(client);
        }

        protected async Task SendPacketToAllClients(Packet p, bool onlyRegistered = false)
        {
            var clientsToInclude = onlyRegistered ? _clients.Values.Where(c => c.IsRegistered) : _clients.Values;
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
            var tasks = new List<Task>();

            foreach (var client in clients)
            {
                var t = sendBytesToClient(data, client);
                tasks.Add(t);
            }
            await Task.WhenAll(tasks);
        }

        protected async Task SendPacketToAllClientsExcept(Packet p, Guid except, bool onlyRegistered = false)
        {
            var clientsToInclude = onlyRegistered ? _clients.Values.Where(c => c.IsRegistered && c.ClientGuid != except) : _clients.Values.Where(c => c.ClientGuid != except);
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

        private async Task sendBytesToClient(byte[] bytes, CM client)
        {
            try
            {
                if (!client.TcpClient.Connected)
                {
                    await DropClient(client);
                    return;
                }
                var stream = client.TcpClient.GetStream();
                using (var cts = new CancellationTokenSource(5000))
                {
                    await stream.WriteAsync(bytes, cts.Token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                var ip = GetClientIp(client);
                FireOnStatus($"Sending data to client timed out. Dropping client. ({ip})");
                await DropClient(client);
            }
            catch
            {
                await DropClient(client);
            }
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
                tcpClient.GetStream().WriteTimeout = 10000;
                var client = (CM)_clientModelConstructor.Invoke(new[] { tcpClient });
                _clients[client.ClientGuid] = client;
                FireOnStatus($"Client connected ({GetClientIp(client)})");
                _ = Task.Run(() => clientTcpListenerTask(client));
            }
            catch (OperationCanceledException)
            { }
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
                while (client.TcpClient.Connected && !client.CancellationToken.IsCancellationRequested)
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
            if (!client.IsRegistered && packet.PacketType != PacketTypes.ClientRegister)
            {
                await DropClient(client);
                return;
            }
            switch (packet.PacketType)
            {
                case PacketTypes.ClientRegister:
                    ClientRegister? objData = packet.GetObjectData<ClientRegister>();
                    if (objData?.Message != NetConstants.ClientRegisterString)
                    {
                        await DropClient(client);
                        return;
                    }
                    if (objData?.Version != Version)
                    {
                        var deniedPacket = new Packet(PacketTypes.ServerRegisterDenied, new ServerRegisterDenied($"Incorrect version {objData?.Version}. Server is running version {Version}"));
                        await SendPacketToClient(deniedPacket, client);
                        await DropClient(client);
                        return;
                    }
                    if (!client.IsRegistered)
                    {
                        client.IsRegistered = true;
                        if (!string.IsNullOrEmpty(objData.IdentityToken))
                        {
                            var ipToken = clientToken(client.TcpClient, objData.IdentityToken);
                            if (!string.IsNullOrEmpty(ipToken))
                            {
                                lock (CachedIdentities)
                                {
                                    if (CachedIdentities.TryGetValue(ipToken, out var identity))
                                    {
                                        //We have a cached identity for this ip token,
                                        //check if another client is already connected with this guid

                                        //If a client with the same IP and identity token was already connected,
                                        //they might have been disconnected (or already connected from the same machine)
                                        //so immediately try sending them a KeepAlive. This will kick them from the
                                        //server if it's not responded to within 5 seconds
                                        if (_clients.TryGetValue(identity.ClientGuid, out var alreadyConnectedClient))
                                        {
                                            _ = SendPacketToClient(new Packet(new KeepAlive()), alreadyConnectedClient);
                                        }
                                        else
                                        {
                                            //No other client connected, so switch out the guid of the client,
                                            //and replace the client guid in the dictionary
                                            _clients.Remove(client.ClientGuid, out _);
                                            client.ClientGuid = identity.ClientGuid;
                                            _clients[client.ClientGuid] = client;
                                        }
                                    }
                                    else
                                    {
                                        //Client not registered, register client with its currently assigned guid
                                        CachedIdentities[ipToken] = new ClientIdentity(ipToken, client.ClientGuid);
                                    }
                                }
                            }
                        }
                        var acceptPacket = new Packet(PacketTypes.ServerRegisterAccepted, new ServerRegisterAccepted(NetConstants.ServerRegisterString, client.ClientGuid));
                        await SendPacketToClient(acceptPacket, client);
                        fireOnClientConnected(client);
                    }
                    break;

                case PacketTypes.ClientDisconnect:
                    await DropClient(client);
                    break;

                case PacketTypes.ObjectData:
                    DispatchObjects(client, packet.Objects);
                    break;
            }
        }

        private bool clientAlreadyExists(Guid potentialGuid)
        {
            return _clients.TryGetValue(potentialGuid, out var _);
        }

        private string clientToken(TcpClient client, string token)
        {
            if (client.Client?.RemoteEndPoint is IPEndPoint ip)
            {
                return ip.Address.ToString() + ":" + token;
            }
            return string.Empty;
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
            var packet = new Packet(PacketTypes.ServerShutdown);
            await SendPacketToAllClients(packet);
            var clients = new List<CM>(_clients.Values);
            foreach (var c in clients)
            {
                c.Stop();
            }
        }

        private async Task waitForPacketAsync(CM client)
        {
            try
            {
                var packets = await ReadPackets(client.TcpClient, client.CancellationToken);
                foreach (var packet in packets)
                {
                    if (packet == null)
                    {
                        //Drop client after 3 malformed packets
                        if (++client.MalformedPackets >= 3)
                        {
                            var ip = GetClientIp(client);
                            await KickClient(client, "Sent too many malformed packets");
                            FireOnStatus($"Client ({ip}) kicked for sending too many malformed packets");
                            break;
                        }
                    }
                    else
                    {
                        client.MalformedPackets = Math.Max(0, client.MalformedPackets - 1);
                        client.LastActivity = DateTime.Now;
                        await handleIncomingPacket(client, packet);
                    }
                }
            }
            catch (Exception)
            {
                //Stream was closed, most likely due to the client shutting down
                //but could also be because client sent malformed packet
                await DropClient(client);
            }
        }

        private void keepAlive(object? sender, EventArgs e)
        {
            var now = DateTime.Now;
            var clients = new List<CM>(_clients.Values);
            foreach (var client in clients)
            {
                if (!client.TcpClient.Connected)
                {
                    _clients.Remove(client.ClientGuid, out _);
                }
                else if ((now - client.LastActivity).TotalSeconds > KeepAliveTime)
                {
                    _ = SendPacketToClient(new Packet(PacketTypes.KeepAlive, new KeepAlive()), client);
                    client.LastActivity = DateTime.Now;
                }
            }
        }
    }
}