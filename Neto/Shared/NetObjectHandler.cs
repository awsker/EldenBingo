using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using System.Net.Sockets;
using System.Reflection;

namespace Neto.Shared
{
    public class NetObjectHandler<CM> where CM : ClientModel
    {
        private ISet<Assembly> _assemblies;
        private IDictionary<string, TypeContainer<CM>> _eventDispatchers;

        private PacketResolver _packetResolver;
        private MessagePackSerializerOptions _cachedOptions;

        public NetObjectHandler()
        {
            _assemblies = new HashSet<Assembly>();
            _eventDispatchers = new Dictionary<string, TypeContainer<CM>>();
            RegisterType(typeof(ServerRegisterAccepted));
            RegisterType(typeof(ServerRegisterDenied));
            RegisterType(typeof(ClientRegister));
            RegisterType(typeof(ServerKicked));

            _packetResolver = new PacketResolver((s) => getOrRegisterDispatcher(s)?.Type);
            _cachedOptions = new MessagePackSerializerOptions(_packetResolver).WithCompression(MessagePackCompression.Lz4BlockArray);
        }

        public event EventHandler<StringEventArgs>? OnError;

        public event EventHandler<StringEventArgs>? OnStatus;

        public static string NameFromType(Type type)
        {
            return type?.FullName ?? throw new ArgumentNullException(nameof(type));
        }

        public void RegisterAssembly(Assembly a)
        {
            _assemblies.Add(a);
        }

        public void RegisterType(Type type)
        {
            getOrRegisterDispatcher(type);
        }

        public void AddListener<T>(Action<CM?, T> func)
        {
            var typeName = NameFromType(typeof(T));
            if (!_eventDispatchers.TryGetValue(typeName, out TypeContainer<CM>? dispatcher))
            {
                _eventDispatchers.Add(typeName, dispatcher = new TypeContainer<CM, T>());
            }
            if (dispatcher is TypeContainer<CM, T> tct)
            {
                tct.OnDispatch += func;
            }
        }

        public void RemoveListener<T>(Action<CM?, T> func)
        {
            var typeName = NameFromType(typeof(T));
            if (_eventDispatchers.TryGetValue(typeName, out TypeContainer<CM>? dispatcher) && dispatcher is TypeContainer<CM, T> tct)
            {
                tct.OnDispatch -= func;
            }
        }

        protected async Task<Packet?[]> ReadPackets(TcpClient client, CancellationTokenSource cancelToken)
        {
            var stream = client.GetStream();
            var size = client.ReceiveBufferSize;
            try
            {
                MemoryStream ms = new MemoryStream(size);
                do
                {
                    byte[] buffer = new byte[size];
                    var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, size), cancelToken.Token);
                    //0 bytes read when connection closed on the other end
                    if (bytesRead == 0)
                        cancelToken.Cancel();
                    if (cancelToken.IsCancellationRequested || client?.Connected != true)
                        return Array.Empty<Packet?>();
                    ms.Write(buffer, 0, bytesRead);
                } while (!IsMessageTerminated(ms));

                return readPackets(ms.ToArray());
            }
            catch (OperationCanceledException)
            {
                //Do nothing, disconnect requested
                return Array.Empty<Packet?>();
            }
        }

        private Packet?[] readPackets(byte[] bytes)
        {
            var packets = new List<Packet?>();
            try
            {
                var messagePackReader = new MessagePackReader(bytes);
                while (!messagePackReader.End)
                {
                    var p = MessagePackSerializer.Deserialize<Packet>(ref messagePackReader, _cachedOptions);
                    packets.Add(p);
                    //Skip end message sequence
                    messagePackReader.ReadRaw(NetConstants.EndOfMessage.Length);
                }
            }
            catch (MessagePackSerializationException)
            {
                packets.Add(null);
            }
            return packets.ToArray();
        }

        protected void DispatchObjectsInPacket(CM? sender, Packet packet)
        {
            foreach (var o in packet.Objects)
            {
                getOrRegisterDispatcher(NameFromType(o.GetType()))?.Dispatch(sender, o);
            }
        }

        protected MessagePackSerializerOptions GetMessagePackOptions()
        {
            return _cachedOptions;
        }

        protected bool IsMessageTerminated(MemoryStream stream)
        {
            var eomLength = NetConstants.EndOfMessage.Length;
            if (stream.Position < eomLength)
                return false;
            var lastBytes = new byte[eomLength];
            stream.Seek(-4, SeekOrigin.End);
            stream.Read(lastBytes, 0, eomLength);
            for (int i = 0; i < eomLength; ++i)
            {
                if (lastBytes[i] != NetConstants.EndOfMessage[i])
                    return false;
            }
            return true;
        }

        protected void FireOnStatus(string message)
        {
            OnStatus?.Invoke(this, new StringEventArgs(message));
        }

        protected void FireOnError(string message)
        {
            OnError?.Invoke(this, new StringEventArgs(message));
        }

        private TypeContainer<CM>? getOrRegisterDispatcher(Type type)
        {
            var typeName = NameFromType(type);
            if (!_eventDispatchers.TryGetValue(typeName, out var dispatcher))
            {
                var typeContainer = typeof(TypeContainer<,>);
                var constructedListType = typeContainer.MakeGenericType(typeof(CM), type);
                try
                {
                    var d = Activator.CreateInstance(constructedListType);
                    if (d != null)
                    {
                        dispatcher = (TypeContainer<CM>)d;
                        _eventDispatchers.Add(typeName, dispatcher);
                    }
                }
                catch (Exception)
                {
#if DEBUG
                    throw;
#endif
                } //Suppress all exceptions when creating instance, and ignore this object, because I'm lazy
            }
            return dispatcher;
        }

        private TypeContainer<CM>? getOrRegisterDispatcher(string typeName)
        {
            if (_eventDispatchers.TryGetValue(typeName, out var dispatcher))
            {
                return dispatcher;
            }
            var type = resolveType(typeName);
            if (type != null)
            {
                return getOrRegisterDispatcher(type);
            }
            throw new ApplicationException($"Could not resolve type {typeName}");
        }

        private Type? resolveType(string typeName)
        {
            foreach (var a in _assemblies)
            {
                var t = a.GetType(typeName);
                if (t != null)
                    return t;
            }
            return null;
        }

        private class PacketResolver : IFormatterResolver
        {
            private Func<string, Type?> _typeFromStringFunc;
            private IMessagePackFormatter<Packet>? _cachedFormatter;

            public PacketResolver(Func<string, Type?> getTypeFromStringFunc)
            {
                _typeFromStringFunc = getTypeFromStringFunc;
            }

            public IMessagePackFormatter<T>? GetFormatter<T>()
            {
                if (typeof(T) == typeof(Packet))
                {
                    if (_cachedFormatter == null)
                    {
                        _cachedFormatter = new PacketFormatter(_typeFromStringFunc);
                    }
                    return (IMessagePackFormatter<T>)_cachedFormatter;
                }
                return ContractlessStandardResolver.Instance.GetFormatter<T>();
            }
        }
    }
}