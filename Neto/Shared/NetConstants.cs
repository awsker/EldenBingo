namespace Neto.Shared
{
    public class NetConstants
    {
        public const string ServerRegisterString = "neto server";
        public const string ClientRegisterString = "hello";
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