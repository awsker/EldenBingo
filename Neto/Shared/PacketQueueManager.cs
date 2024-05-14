using MessagePack;

namespace Neto.Shared
{
    internal class PacketQueueManager<CM> where CM : ClientModel
    {
        private struct QueuedPacket
        {
            public CM? Client;
            public Stream Stream;
            public Packet Packet;
            public QueuedPacket(CM? client, Stream stream, Packet packet)
            {
                Client = client;
                Stream = stream;
                Packet = packet;
            }
        }

        private Queue<QueuedPacket> _packets;
        private object _monitor = new object();
        private Thread _thread;
        private bool _running;
        MessagePackSerializerOptions? _messagePackOptions;

        public event EventHandler<PacketSendErrorEventArgs<CM>> OnException;

        public PacketQueueManager(MessagePackSerializerOptions? options)
        {
            _messagePackOptions = options;
            _packets = new Queue<QueuedPacket>();
            _running = true;
            _thread = new Thread(run);
            _thread.Start();
        }

        public void SendPacket(CM client, Packet p)
        {
            lock (_monitor)
            {
                var stream = client.TcpClient.GetStream();
                if (stream != null)
                {
                    _packets.Enqueue(new QueuedPacket(client, stream, p));
                    Monitor.Pulse(_monitor);
                }
            }
        }

        public void SendPacket(Stream stream, Packet p)
        {
            lock (_monitor)
            {
                _packets.Enqueue(new QueuedPacket(null, stream, p));
                Monitor.Pulse(_monitor);
            }
        }

        public void Stop()
        {
            _running = false;
            lock (_monitor)
            {
                Monitor.Pulse(_monitor);
            }
        }

        public void Clear()
        {
            lock (_monitor)
            {
                _packets.Clear();
            }
        }

        private void run()
        {
            while (_running)
            {
                lock (_monitor)
                {
                    Monitor.Wait(_monitor);
                    while (_packets.TryDequeue(out var p))
                    {
                        try
                        {
                            byte[] data = MessagePackSerializer.Serialize(p.Packet, _messagePackOptions);
                            data = PacketHelper.ConcatBytes(data, NetConstants.EndOfMessage);
                            p.Stream.Write(data, 0, data.Length);
                        }
                        catch (Exception e)
                        {
                            OnException.Invoke(this, new PacketSendErrorEventArgs<CM>(p.Client, e));
                            return;
                        }
                    }
                }
            }
        }
    }

    internal class PacketSendErrorEventArgs<CM> : EventArgs where CM : ClientModel
    {
        public CM? Client { get; init; }
        public Exception Exception { get; init; }
        public PacketSendErrorEventArgs(CM? client, Exception exception)
        {
            Client = client;
            Exception = exception;
        }
    }
}
