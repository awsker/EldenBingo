using EldenBingoCommon;
using EldenBingoServer;
using Neto.Shared;

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
            int port = BingoConstants.DefaultPort;
            if (args.Length > 0)
            {
                if (!int.TryParse(args[0], out port))
                {
                    Console.WriteLine("Invalid port");
                }
            }
            var server = new Server(port);
            server.OnError += server_OnError;
            server.OnStatus += server_OnStatus;
            server.Host();

            Thread.Sleep(Timeout.Infinite);
        }

        private static void server_OnStatus(object? sender, StringEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void server_OnError(object? sender, StringEventArgs e)
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