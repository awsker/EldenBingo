namespace Neto.Shared
{
    public class NetConstants
    {
        public const string ServerRegisterString = "neto server";
        public const string ClientRegisterString = "hello";
        public static readonly byte[] EndOfMessage = new byte[] { 0xFB, 0xFC, 0xFD, 0xFE };

        public enum PacketTypes
        {
            ServerRegisterAccepted, //string, Guid
            ServerClientDropped, //(none)
            ServerShutdown, //(none)

            ClientRegister, //string
            ClientDisconnect, //(none)

            ObjectData,
        }
    }
}