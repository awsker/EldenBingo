using MessagePack;
using Neto.Shared;
using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;

namespace Neto.Server
{
    public class NetoServer<CM> : NetObjectHandler<CM> where CM : ClientModel
    {
        private readonly ConcurrentDictionary<Guid, CM> _clients;
        private readonly ConcurrentBag<TcpListener> _tcpListeners;
        private readonly ConstructorInfo _clientModelConstructor;
        private CancellationTokenSource _cancelToken;

        // Timer to handle sending KeepAlive packets regularly
        private System.Timers.Timer? _keepAliveTimer;

        // Message type or code for KeepAlive, customize as needed
        private static readonly Packet KeepAlivePacket = new Packet(PacketTypes.KeepAlive, new KeepAlive());

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
            _ = runTcpListenerAsync(IPAddress.Any);
            _ = runTcpListenerAsync(IPAddress.IPv6Any);
            startKeepAlive();
            Hosting = true;
            FireOnStatus($"Hosting server on port {Port}");
        }

        public virtual async Task Stop()
        {
            if (!Hosting)
                throw new Exception("Not hosting");

            stopKeepAlive();
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
            // Only remove this client from the dictionary if it hasn't already been replaced by a new connection. 
            // Compare TcpClients to ensure the client is still the same
            if (_clients.TryGetValue(client.ClientGuid, out var knownClient) && client.TcpClient == knownClient.TcpClient)
            {
                _clients.Remove(client.ClientGuid, out _);
            }
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

        // Local unicast addresses (for display); listeners bind to IPAddress.Any and IPv6Any instead.
        private static IPAddress[] getIpAddresses()
        {
            var addresses = new List<IPAddress>();

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up)
                    continue;

                var ipProps = ni.GetIPProperties();
                foreach (var unicast in ipProps.UnicastAddresses)
                {
                    if (unicast.Address.AddressFamily == AddressFamily.InterNetwork || unicast.Address.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        addresses.Add(unicast.Address);
                    }
                }
            }

            // Always include loopback
            if (!addresses.Any(a => IPAddress.IsLoopback(a)))
            {
                addresses.Add(IPAddress.Loopback);
            }

            return addresses.Distinct().ToArray();
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
                using (var cts = new CancellationTokenSource(15000))
                {
                    // Write length of data before sending actual data
                    await stream.WriteAsync(BitConverter.GetBytes(bytes.Length), cts.Token).ConfigureAwait(false);
                    // Send actual data
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
                TcpKeepAliveSettings.Apply(tcpClient);
                tcpClient.GetStream().WriteTimeout = 10000;
                var client = (CM)_clientModelConstructor.Invoke(new[] { tcpClient });
                _clients[client.ClientGuid] = client;
                FireOnStatus($"Client connected ({GetClientIp(client)})");
                _ = clientTcpListenerTask(client);
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
            // Give the client 5 seconds to register
            var checkRegistryTimer = new System.Timers.Timer(5000);
            checkRegistryTimer.Elapsed += (sender, e) =>
            {
                // If the client didn't register in 5 seconds then we assume it 
                // was probably because the registration packet was formatted according
                // to the old way (data + EndOfMessage) and therefore could not be
                // parsed
                if (!client.IsRegistered)
                {
                    kickLegacyClient(client);
                }
                checkRegistryTimer.Dispose();
                checkRegistryTimer = null;
            };
            checkRegistryTimer.AutoReset = false;
            checkRegistryTimer.Enabled = true;
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
                        var deniedPacket = createRegistryDeniedPacket(objData?.Version);
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
                                        //Switch out the guid of the client,
                                        //and replace the client guid in the dictionary
                                        _clients.Remove(client.ClientGuid, out _);
                                        client.ClientGuid = identity.ClientGuid;
                                        _clients[client.ClientGuid] = client;
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

        private string clientToken(TcpClient client, string token)
        {
            if (client.Client?.RemoteEndPoint is IPEndPoint ip)
            {
                return ip.Address.ToString() + ":" + token;
            }
            return string.Empty;
        }

        private async Task runTcpListenerAsync(IPAddress bindAddress)
        {
            TcpListener? tcp = null;
            try
            {
                tcp = new TcpListener(bindAddress, Port);
                tcp.Start();
                _tcpListeners.Add(tcp);
                while (!_cancelToken.IsCancellationRequested)
                {
                    await acceptIncomingConnections(tcp);
                }
            }
            catch (SocketException e) when (bindAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                FireOnError($"IPv6 listen unavailable on port {Port}: {e.Message}");
            }
            catch (Exception e) when (!_cancelToken.IsCancellationRequested)
            {
                _cancelToken.Cancel();
                FireOnError(e.Message);
                try
                {
                    await sendShutdownToAll();
                }
                catch (Exception shutdownError)
                {
                    FireOnError(shutdownError.Message);
                }
            }
            finally
            {
                if (tcp != null)
                {
                    try { tcp.Stop(); } catch { /* listener may already be stopped */ }
                }
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
                var packets = await ReadPackets(client.TcpClient.GetStream(), client.CancellationToken);
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

        private void startKeepAlive()
        {
            _keepAliveTimer = new System.Timers.Timer(5000);
            _keepAliveTimer.Elapsed += (sender, e) =>
            {
                try
                {
                    _ = SendPacketToAllClients(KeepAlivePacket, true);
                }
                catch
                {
                    // Ignore errors for now
                }
            };
            _keepAliveTimer.AutoReset = true;
            _keepAliveTimer.Enabled = true;
        }

        private void stopKeepAlive()
        {
            if (_keepAliveTimer != null)
            {
                _keepAliveTimer.Stop();
                _keepAliveTimer.Dispose();
                _keepAliveTimer = null;
            }
        }

        private Packet createRegistryDeniedPacket(string? clientVersion = null)
        {
            return new Packet(PacketTypes.ServerRegisterDenied, new ServerRegisterDenied($"Incorrect version {clientVersion ?? ""}. Server is running version {Version}"));
        }

        private async void kickLegacyClient(CM client)
        {
            try
            {
                var stream = client.TcpClient.GetStream();
                var packet = createRegistryDeniedPacket();
                // Send a packet formatted according to the old way (terminated with EndOfMessage)
                // so that legacy clients become aware that they need to update
                var data = MessagePackSerializer.Serialize(packet, GetMessagePackOptions());
                data = PacketHelper.ConcatBytes(data, NetConstants.EndOfMessageLegacy);
                await stream.WriteAsync(data);
                await DropClient(client);
            }
            catch (Exception)
            {
                // Ignore any errors in this rudimentary kick function
            }
        }
    }
}