using EldenBingoCommon;
using EldenBingoServer;

namespace EldenBingoServerStandalone
{
    internal static class Program
    {
        private static void log(string text)
        {
            var timestamp = DateTime.Now.ToString();
            File.AppendAllText("log.txt", $"[{timestamp}] {text}{Environment.NewLine}");
        }

        private static void Main(string[] args)
        {
            int port = NetConstants.DefaultPort;
            if (args.Length > 0)
            {
                if (!int.TryParse(args[0], out port))
                {
                    Console.WriteLine("Invalid port");
                }
            }
            var server = new Server(port);
            server.OnError += server_onError;
            server.StatusChanged += server_StatusChanged;
            server.Host();
            
            Thread.Sleep(Timeout.Infinite);
        }

        private static void server_StatusChanged(object? sender, StatusEventArgs e)
        {
            Console.WriteLine(e.Status);
        }

        private static void server_onError(object? sender, StringEventArgs e)
        {
            var message = $"Error: {e.Message}";
            Console.WriteLine(message);
            try
            {
                log(message);
            }
            finally
            {
                Environment.Exit(0);
            }
        }
    }
}