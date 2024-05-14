using MessagePack;
using Neto.Shared;
using System.Net;
using System.Net.Sockets;

namespace Neto.Client
{
    public class NetoClient : NetObjectHandler<ClientModel>
    {
        private TcpClient? _tcp;
        private string _clientUniqueToken;

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

        public static IPEndPoint? EndPointFromAddress(string address, int port, out string error)
        {
            error = string.Empty;
            if (port < 1 || port > 65535)
            {
                error = "Invalid port";
                return null;
            }
            if (IPAddress.TryParse(address, out var ipAddress))
            {
                var endpoint = new IPEndPoint(ipAddress, port);
                return endpoint;
            }
            else
            {
                try
                {
                    IPAddress[] addresses = Dns.GetHostAddresses(address);
                    foreach (var ip in addresses)
                    {
                        if (ip.ToString() == "::1")
                            continue;
                        var endpoint = new IPEndPoint(ip, port);
                        return endpoint;
                    }
                    error = $"Unable to resolve hostname {address}";
                    return null;
                }
                catch (Exception e)
                {
                    error = $"Unable to resolve hostname {address}: {e.Message}";
                    return null;
                }
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
            var ipEndpoint = EndPointFromAddress(address, port, out string error);
            if (ipEndpoint == null)
            {
                FireOnError(error);
                return ConnectionResult.Denied;
            }
            if (_tcp != null && _tcp.Connected)
            {
                FireOnError("Already connected");
                return ConnectionResult.Denied;
            }
            CancellationToken = new CancellationTokenSource();
            _tcp = new TcpClient(ipEndpoint.AddressFamily);
            try
            {
                FireOnStatus($"Connecting to {address}:{port}...");
                await _tcp.ConnectAsync(ipEndpoint, CancellationToken.Token);

                if (_tcp.Connected)
                {
                    FireOnStatus("Connected to server");
                    _ = run();
                }
                else
                {
                    FireOnError($"Could not connect to {address}:{port}");
                    CancellationToken.Cancel();
                }
            }
            catch (Exception e)
            {
                FireOnError($"Connect Error: {e.Message}");
                return ConnectionResult.Exception;
            }
            return _tcp != null && _tcp.Connected ? ConnectionResult.Connected : ConnectionResult.Denied;
        }

        public async Task<bool> Connect(IPEndPoint ipEndpoint)
        {
            if (_tcp != null && _tcp.Connected)
            {
                FireOnError("Already connected");
                return false;
            }
            CancellationToken = new CancellationTokenSource();
            _tcp = new TcpClient(ipEndpoint.AddressFamily);
            try
            {
                FireOnStatus($"Connecting to {ipEndpoint}...");
                await _tcp.ConnectAsync(ipEndpoint, CancellationToken.Token);

                if (_tcp.Connected)
                {
                    FireOnStatus("Connected to server");
                    _ = Task.Run(run);
                }
                else
                {
                    FireOnError($"Could not connect to {ipEndpoint.Address}:{ipEndpoint.Port}");
                    CancellationToken.Cancel();
                }
            }
            catch (Exception e)
            {
                FireOnError($"Connect Error: {e.Message}");
            }
            return _tcp.Connected;
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
                using (var cts = new CancellationTokenSource(5000))
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

        private void FireOnDisconnect(string message)
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
                        ClientGuid = objData.ClientGuid;
                        FireOnConnected();
                    }
                    else
                    {
                        FireOnDisconnect("Invalid server response");
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
                    FireOnDisconnect("Server shutting down");
                    break;

                case PacketTypes.ObjectData:
                    DispatchObjectsInPacket(null, packet);
                    break;

                case PacketTypes.KeepAlive:
                    await SendPacketToServer(new Packet(PacketTypes.KeepAlive, new KeepAlive()));
                    break;
            }
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
            FireOnDisconnect("Disconnected");
        }

        private async Task waitForPacketAsync()
        {
            if (_tcp == null)
                return;

            try
            {
                var packets = await ReadPackets(_tcp, CancellationToken);
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