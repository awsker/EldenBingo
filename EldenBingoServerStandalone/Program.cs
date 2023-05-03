using EldenBingoCommon;
using EldenBingoServer;

namespace EldenBingoServerStandalone
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            int port = NetConstants.DefaultPort; 
            if(args.Length > 0)
            {
                if(!int.TryParse(args[0], out port))
                {
                    Console.WriteLine("Invalid port");
                }
            }
            var server = new Server(port);
            server.OnError += server_onError;
            server.Host();
            Console.WriteLine($"Hosting Elden Bingo server on port {port}");
            Thread.Sleep(Timeout.Infinite);
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
        
        private static void log(string text)
        {
            var timestamp = DateTime.Now.ToString();
            File.AppendAllText("log.txt", $"[{timestamp}] {text}{Environment.NewLine}");
        }
    }
}