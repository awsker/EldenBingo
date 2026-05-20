using MessagePack;
using Neto.Shared;
using System.Net;
using System.Net.Sockets;

namespace Neto.Client
{
    public class NetoClient : NetObjectHandler<ClientModel>
    {
        private TcpClient? _tcp;
        private bool _connectedToLegacyServer;
        
        private string _clientUniqueToken;

        // Timer to handle reconnects if no keep alive packets arrived in time
        private System.Timers.Timer _keepAliveTimer;

        /// <summary>
        /// Create a client
        /// </summary>
        /// <param name="clientUniqueToken">This token will be used to identify this client and recover the client identity in case of a disconnect and subsequent reconnect</param>
        public NetoClient(string? clientUniqueToken = null)
        {
            CancellationToken = new CancellationTokenSource();
            _clientUniqueToken = clientUniqueToken ?? string.Empty;
        }

        ~NetoClient()
        {
            CancellationToken.Dispose();
        }

        public event EventHandler? Connected;

        public event EventHandler<StringEventArgs>? Disconnected;

        public event EventHandler<StringEventArgs>? Kicked;

        public virtual string Version => "1";

        public Guid ClientGuid { get; private set; }
        public bool IsConnected => _tcp?.Connected == true;
        protected CancellationTokenSource CancellationToken { get; private set; }
         
        /// <summary>
        /// Resolves <paramref name="address"/> and <paramref name="port"/> to one or more endpoints.
        /// Hostnames may yield multiple addresses (e.g. IPv4 and IPv6); literals yield a single endpoint.
        /// </summary>
        public static IReadOnlyList<IPEndPoint>? EndPointsFromAddress(string address, int port, out string error)
        {
            error = string.Empty;
            if (port < 1 || port > 65535)
            {
                error = "Invalid port";
                return null;
            }

            if (IPAddress.TryParse(address, out var ipAddress))
                return new[] { new IPEndPoint(ipAddress, port) };

            try
            {
                var entry = Dns.GetHostEntry(address);
                var endpoints = new List<IPEndPoint>();
                foreach (var ip in entry.AddressList)
                {
                    endpoints.Add(new IPEndPoint(ip, port));
                }

                if (endpoints.Count == 0)
                {
                    error = $"Unable to resolve hostname {address}";
                    return null;
                }

                return endpoints;
            }
            catch (Exception e)
            {
                error = $"Error resolving hostname {address}: {e.Message}";
                return null;
            }
        }

        public virtual string GetConnectionStatusString()
        {
            if (!IsConnected)
                return "Not connected";
            if (CancellationToken.IsCancellationRequested)
                return "Stopping...";
            else
                return "Connected";
        }

        public async Task<ConnectionResult> Connect(string address, int port)
        {
            if (_tcp != null && _tcp.Connected)
            {
                FireOnError("Already connected");
                return ConnectionResult.Denied;
            }

            var endpoints = EndPointsFromAddress(address, port, out string error);
            if (endpoints == null || endpoints.Count == 0)
            {
                FireOnError(error);
                return ConnectionResult.Denied;
            }

            ConnectionResult lastResult = ConnectionResult.Denied;
            foreach (var endpoint in endpoints)
            {
                lastResult = await Connect(endpoint);
                if (lastResult == ConnectionResult.Connected)
                    return lastResult;
            }
            FireOnError($"Could not connect to {address}:{port}");
            return lastResult;
        }

        public async Task<ConnectionResult> Connect(IPEndPoint ipEndpoint)
        {
            if (_tcp != null && _tcp.Connected)
            {
                FireOnError("Already connected");
                return ConnectionResult.Denied;
            }
            _connectedToLegacyServer = false;
            CancellationToken = new CancellationTokenSource();
            TcpClient? tcp = new TcpClient(ipEndpoint.AddressFamily);
            try
            {
                FireOnStatus($"Connecting to {ipEndpoint}...");
                await tcp.ConnectAsync(ipEndpoint, CancellationToken.Token);

                if (!tcp.Connected)
                {
                    FireOnError($"Could not connect to {ipEndpoint}");
                    CancellationToken.Cancel();
                    tcp.Dispose();
                    return ConnectionResult.Denied;
                }
                _tcp = tcp;
                tcp = null;
                TcpKeepAliveSettings.Apply(_tcp);
                FireOnStatus("Connected to server");
                _ = run();
                return ConnectionResult.Connected;
            }
            catch (Exception e)
            {
                CancellationToken.Cancel();
                FireOnError($"Connect Error: {e.Message}");
                tcp?.Dispose();
                _tcp = null;
                return ConnectionResult.Exception;
            }
        }

        public async Task Disconnect()
        {
            await SendPacketToServer(new Packet(PacketTypes.ClientDisconnect));
            CancellationToken.Cancel();
        }

        public async Task SendPacketToServer(Packet p)
        {
            if (CancellationToken.IsCancellationRequested || _tcp == null || !_tcp.Connected)
            {
                FireOnError($"Error sending message to server: Not connected");
                return;
            }
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
                using (var cts = new CancellationTokenSource(15000))
                {
                    var stream = _tcp.GetStream();
                    await stream.WriteAsync(data, cts.Token).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                CancellationToken.Cancel();
                FireOnError($"Error sending message to server: {e.Message}");
            }
        }

        private void FireOnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        private void FireOnDisconnected(string message)
        {
            Disconnected?.Invoke(this, new StringEventArgs(message));
        }

        private void FireOnKicked(string message)
        {
            Kicked?.Invoke(this, new StringEventArgs(message));
        }

        private async Task handleIncomingPacket(Packet packet)
        {
            switch (packet.PacketType)
            {
                case PacketTypes.ServerRegisterAccepted:
                    ServerRegisterAccepted? objData = packet.GetObjectData<ServerRegisterAccepted>();
                    if (objData?.Message == NetConstants.ServerRegisterString)
                    {
                        _connectedToLegacyServer = false;
                        startKeepAlive();
                        ClientGuid = objData.ClientGuid;
                        FireOnConnected();
                    }
                    else if (objData?.Message == NetConstants.ServerRegisterStringLegacy)
                    {
                        _connectedToLegacyServer = true;
                        ClientGuid = objData.ClientGuid;
                        FireOnConnected();
                    }
                    else
                    {
                        FireOnDisconnected("Invalid server response");
                        await Disconnect();
                    }
                    break;

                case PacketTypes.ServerRegisterDenied:
                    ServerRegisterDenied? deniedData = packet.GetObjectData<ServerRegisterDenied>();
                    FireOnKicked($"Registration denied: {deniedData?.Message ?? "Unknown reason"}");
                    await Disconnect();
                    break;

                case PacketTypes.ServerClientDropped:
                    CancellationToken.Cancel();
                    ServerKicked? kickedData = packet.GetObjectData<ServerKicked>();
                    FireOnKicked($"Kicked from server: {kickedData?.Reason ?? "Unknown reason"}");
                    break;

                case PacketTypes.ServerShutdown:
                    CancellationToken.Cancel();
                    FireOnDisconnected("Server shutting down");
                    break;

                case PacketTypes.ObjectData:
                    DispatchObjects(null, packet.Objects);
                    break;

                case PacketTypes.KeepAlive:
                    startKeepAlive();
                    break;
            }
        }

        private void startKeepAlive()
        {
            if (_keepAliveTimer != null)
            {
                _keepAliveTimer.Stop();
                _keepAliveTimer.Dispose();
            }
            //On a legacy server, we can only expect one keepalive packet every 25 seconds. On new servers, that time is 5 seconds. Delay disconnect accordingly
            _keepAliveTimer = new System.Timers.Timer(_connectedToLegacyServer ? 60000 : 15000);
            _keepAliveTimer.Elapsed += async (sender, e) =>
            {
                try
                {
                    FireOnError("Expected keepalive packets but did not receive any from server for 15 seconds");
                    CancellationToken.Cancel();
                }
                catch
                {
                    // Ignore errors for now
                }
            };
            _keepAliveTimer.AutoReset = false;
            _keepAliveTimer.Enabled = true;
        }

        private async Task run()
        {
            try
            {
                var registerPacket = new Packet(PacketTypes.ClientRegister, new ClientRegister(NetConstants.ClientRegisterString, Version, _clientUniqueToken));
                await SendPacketToServer(registerPacket);
                while (_tcp?.Connected == true && !CancellationToken.IsCancellationRequested)
                {
                    await waitForPacketAsync();
                }
            }
            catch (Exception e)
            {
                CancellationToken.Cancel();
                FireOnStatus(e.Message);
            }
            if (_tcp?.Connected == true)
            {
                _tcp.Close();
            }
            _tcp = null;
            FireOnDisconnected("Disconnected");
        }

        private async Task waitForPacketAsync()
        {
            if (_tcp == null)
                return;

            try
            {
                var packets = await ReadPackets(_tcp.GetStream(), CancellationToken);
                foreach (var packet in packets)
                {
                    if (packet != null)
                    {
                        await handleIncomingPacket(packet);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                //Do nothing, disconnect requested
            }
            catch (Exception e)
            {
                //Stream was closed, most likely due to the server shutting down
                //but could also be because server sent malformed packet
                FireOnError(e.Message);
                CancellationToken.Cancel();
            }
        }
    }
}