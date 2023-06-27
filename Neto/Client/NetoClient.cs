using MessagePack;
using Neto.Shared;
using System.Net;
using System.Net.Sockets;

namespace Neto.Client
{
    public class NetoClient : NetObjectHandler<ClientModel>
    {
        protected CancellationTokenSource CancelToken { get; private set; }
        private TcpClient? _tcp;

        public NetoClient()
        {
            CancelToken = new CancellationTokenSource();
        }

        public event EventHandler? Connected;

        public event EventHandler<StringEventArgs>? Disconnected;

        public Guid ClientGuid { get; private set; }

        public bool IsConnected => _tcp?.Connected == true;

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
            if (CancelToken.IsCancellationRequested)
                return "Stopping...";
            else
                return "Connected";
        }

        public async Task<bool> Connect(IPEndPoint ipEndpoint)
        {
            if (_tcp != null && _tcp.Connected)
            {
                FireOnError("Already connected");
                return false;
            }
            CancelToken = new CancellationTokenSource();
            _tcp = new TcpClient(ipEndpoint.AddressFamily);
            try
            {
                FireOnStatus($"Connecting to {ipEndpoint}...");
                await _tcp.ConnectAsync(ipEndpoint, CancelToken.Token);

                if (_tcp.Connected)
                {
                    FireOnStatus("Connected to server");
                    _ = Task.Run(run);
                }
                else
                {
                    FireOnError($"Could not connect to {ipEndpoint.Address}:{ipEndpoint.Port}");
                    CancelToken.Cancel();
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
            await SendPacketToServer(new Packet(NetConstants.PacketTypes.ClientDisconnect));
            CancelToken.Cancel();
        }

        public async Task SendPacketToServer(Packet p)
        {
            if (CancelToken.IsCancellationRequested || _tcp == null || !_tcp.Connected)
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
                var stream = _tcp.GetStream();
                await stream.WriteAsync(data, CancelToken.Token);
            }
            catch (OperationCanceledException)
            { }
            catch (Exception e)
            {
                CancelToken.Cancel();
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

        private async Task handleIncomingPacket(Packet packet)
        {
            switch (packet.PacketType)
            {
                case NetConstants.PacketTypes.ServerRegisterAccepted:
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

                case NetConstants.PacketTypes.ServerClientDropped:
                    CancelToken.Cancel();
                    FireOnDisconnect("Kicked from server");
                    break;

                case NetConstants.PacketTypes.ServerShutdown:
                    CancelToken.Cancel();
                    FireOnDisconnect("Server shutting down");
                    break;

                case NetConstants.PacketTypes.ObjectData:
                    DispatchObjectsInPacket(null, packet);
                    break;
            }
        }

        private async Task run()
        {
            try
            {
                var registerPacket = new Packet(NetConstants.PacketTypes.ClientRegister, new ClientRegister(NetConstants.ClientRegisterString));
                await SendPacketToServer(registerPacket);
                while (!CancelToken.IsCancellationRequested)
                {
                    await waitForPacketAsync();
                }
            }
            catch (Exception e)
            {
                CancelToken.Cancel();
                FireOnStatus(e.Message);
            }
            FireOnDisconnect("Disconnected");
            FireOnStatus("Disconnected");
            _tcp = null;
        }

        private async Task waitForPacketAsync()
        {
            if (_tcp == null)
                return;

            var stream = _tcp.GetStream();
            var size = _tcp.ReceiveBufferSize;
            try
            {
                MemoryStream ms = new MemoryStream(size);
                var dataChunks = new List<byte>();
                do
                {
                    ms.Seek(0, SeekOrigin.End);
                    byte[] buffer = new byte[size];
                    var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, size), CancelToken.Token);
                    if (CancelToken.IsCancellationRequested)
                        return;
                    ms.Write(buffer, 0, bytesRead);
                } while (!IsMessageTerminated(ms));

                var packet = ReadPacket(ms.ToArray());
                if (packet != null)
                    await handleIncomingPacket(packet);
            }
            catch (Exception e)
            {
                //Stream was closed, most likely due to the server shutting down
                //but could also be because server sent malformed packet
                FireOnError(e.Message);
            }
        }
    }
}