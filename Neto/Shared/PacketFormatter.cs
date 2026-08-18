using MessagePack;
using MessagePack.Formatters;

namespace Neto.Shared
{
    public class PacketFormatter : IMessagePackFormatter<Packet>
    {
        private Func<string, Type?> _typeFromStringFunc;

        public PacketFormatter(Func<string, Type?> getTypeFromStringFunc)
        {
            _typeFromStringFunc = getTypeFromStringFunc;
        }

        public Packet Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var noCompressionOptions = options.WithCompression(MessagePackCompression.None);
            options.Security.DepthStep(ref reader);
            try
            {
                var arrayCount = reader.ReadArrayHeader();

                if (arrayCount < 2 || (arrayCount - 2) % 2 != 0)
                    throw new MessagePackSerializationException("Invalid Packet format.");

                var packetType = (PacketTypes)reader.ReadByte();
                var objectCount = reader.ReadInt32();
                var objects = new List<object>();
                for (int i = 0; i < objectCount; ++i)
                {
                    var dataType = reader.ReadString();
                    if (dataType == null)
                        throw new ApplicationException("Could not read type in packet");
                    var type = _typeFromStringFunc(dataType);
                    if (type == null)
                        throw new ApplicationException($"Incoming type {dataType} was not registered");
                    var o = MessagePackSerializer.Deserialize(type, ref reader, noCompressionOptions);
                    if (o != null)
                        objects.Add(o);
                }
                return new Packet(packetType, objects.ToArray());
            } 
            finally
            {
                reader.Depth--;
            }
        }

        public void Serialize(ref MessagePackWriter writer, Packet packet, MessagePackSerializerOptions options)
        {
            var objectsToSend = packet.Objects;
            if (objectsToSend == null)
                throw new ArgumentNullException("Objects are null");

            writer.WriteArrayHeader(2 + objectsToSend.Count * 2);
            writer.Write((byte)packet.PacketType);
            writer.Write(objectsToSend.Count);
            var noCompressionOptions = options.WithCompression(MessagePackCompression.None);
            foreach (var o in objectsToSend)
            {
                var dataType = NetObjectHandler<ClientModel>.NameFromType(o.GetType());
                writer.Write(dataType);
                MessagePackSerializer.Serialize(o.GetType(), ref writer, o, noCompressionOptions);
            }
            //writer.Flush();
        }
    }
}