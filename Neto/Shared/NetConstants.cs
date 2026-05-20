namespace Neto.Shared
{
    public class NetConstants
    {
        public const string ServerRegisterStringLegacy = "neto server";
        public const string ServerRegisterString = "neto server keepalive";
        public const string ClientRegisterStringLegacy = "hello";
        public const string ClientRegisterString = "hello keepalive";
        public static readonly byte[] EndOfMessage = new byte[] { 0xFB, 0xFC, 0xFD, 0xFE };      
    }

    public enum ConnectionResult
    {
        Connected,
        Denied,
        Exception,
    }

    public enum PacketTypes
    {
        ServerRegisterAccepted,
        ServerRegisterDenied,
        ServerClientDropped,
        ServerShutdown,

        ClientRegister,
        ClientDisconnect,

        ObjectData,

        KeepAlive,
    }
}