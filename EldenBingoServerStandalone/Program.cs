using EldenBingoCommon;
using EldenBingoServer;
using Neto.Shared;

namespace EldenBingoServerStandalone
{
    internal static class Program
    {
        private static bool _stopCalled = false;
        private static Server _server;

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
            _server = new Server(port);
            _server.OnError += server_OnError;
            _server.OnStatus += server_OnStatus;
            _server.Host();

            var keyboardListenThread = new Thread(new ThreadStart(listenKeyBoardEvent));
            keyboardListenThread.Start();
            var waitHandle = new ManualResetEvent(false);
            
            Console.CancelKeyPress += (o, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("Stopping server...");
                _stopCalled = true;
                waitHandle.Set();
            };
            waitHandle.WaitOne();
            _server.Stop();
        }

        private static void listenKeyBoardEvent()
        {
            while(!_stopCalled)
            {
                if(Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if(key.KeyChar == 'r')
                    {
                        Console.WriteLine("---- Current Rooms ----");
                        foreach (var room in _server.Rooms)
                        {
                            Console.WriteLine($"{room.Name}: {room.Users.Count} users | Last Activity: {room.LastActivity.ToShortDateString()} {room.LastActivity.ToShortTimeString()}");
                            foreach(var client in room.Users)
                            {
                                Console.WriteLine($"\t{client.Nick}");
                            }
                        }
                        Console.WriteLine("-----------------------");
                    }
                }
                Thread.Sleep(50);
            }
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