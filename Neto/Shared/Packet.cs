namespace Neto.Shared
{
    public class Packet
    {
        private List<object> _objects;

        public Packet(params object[] objectsToSend) : this(NetConstants.PacketTypes.ObjectData, objectsToSend)
        { }

        internal Packet(NetConstants.PacketTypes packetType, params object[] objectsToSend)
        {
            PacketType = packetType;
            _objects = new List<object>(objectsToSend ?? Array.Empty<object>());
        }

        public NetConstants.PacketTypes PacketType { get; }

        public int NumObjects => Objects.Count;

        public IReadOnlyList<object> Objects => _objects;

        public void AddObject(object o)
        {
            _objects.Add(o);
        }

        public T? GetObjectData<T>()
        {
            return _objects.OfType<T>().FirstOrDefault();
        }
    }
}