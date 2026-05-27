using System.Net.Sockets;

namespace Neto.Shared
{
    public static class TcpKeepAliveSettings
    {
        /// <summary>Seconds of idle time before the first keepalive probe is sent.</summary>
        public const int IdleSeconds = 15;

        /// <summary>Seconds between probe retransmissions if the peer does not respond.</summary>
        public const int IntervalSeconds = 5;

        /// <summary>Number of failed probes before the connection is aborted.</summary>
        public const int RetryCount = 5;

        public static void Apply(TcpClient? tcpClient)
        {
            if (tcpClient?.Client is not { } socket)
                return;

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            try
            {
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, IdleSeconds);
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, IntervalSeconds);
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, RetryCount);
            }
            catch (Exception)
            {
                // Fine-grained TCP keepalive tuning is unavailable on some hosts; socket-level KeepAlive remains enabled.
            }
        }
    }
}
