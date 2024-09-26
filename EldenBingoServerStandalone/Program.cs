using EldenBingoCommon;
using EldenBingoServer;
using InteractiveReadLine;
using Neto.Shared;

namespace EldenBingoServerStandalone
{
    internal static class Program
    {
        private static bool _stopCalled = false;
        private static Server _server;
        private static Thread _keyboardListenThread;

        private static bool _readInput;

        private const ConsoleColor DefaultColor = ConsoleColor.Gray;
        private const ConsoleColor StatusColor = ConsoleColor.DarkYellow;
        private const ConsoleColor InfoColor = ConsoleColor.Green;
        private const ConsoleColor ErrorColor = ConsoleColor.Red;

        private static IDictionary<char, (string, Action)> _keyboardShortcuts = new Dictionary<char, (string, Action)>()
        {
            {'h', new("List keyboard shortcuts", showShortcuts)},
            {'r', new("List all rooms", printRooms)},
            {'m', new("Maintenance mode", maintenanceMode)}
        };

        private static void log(string text)
        {
            var timestamp = DateTime.Now.ToString();
            File.AppendAllText("log.txt", $"[{timestamp}] {text}{Environment.NewLine}");
        }

        private static void Output(string text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static void Main(string[] args)
        {
            
            int port = BingoConstants.DefaultPort;
            if (args.Length > 0)
            {
                if (!int.TryParse(args[0], out port))
                {
                    Output("Invalid port", ErrorColor);
                }
            }
            _server = new Server(port);
            _server.OnError += server_OnError;
            _server.OnStatus += server_OnStatus;
            _server.Host();
            Output("Press 'h' to view all shortcuts", InfoColor);

            _keyboardListenThread = new Thread(new ThreadStart(listenKeyBoardEvent));
            _keyboardListenThread.Start();
            _readInput = true;
            var waitHandle = new ManualResetEvent(false);
            
            Console.CancelKeyPress += (o, e) =>
            {
                e.Cancel = true;
                Output("Stopping server...", StatusColor);
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
                if(_readInput && Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if(_keyboardShortcuts.TryGetValue(key.KeyChar, out var item))
                    {
                        item.Item2();
                    }
                }
                Thread.Sleep(50);
            }
        }

        private static void showShortcuts()
        {
            Output("---- Keyboard shortcuts ----", InfoColor);
            foreach(var kv in _keyboardShortcuts)
            {
                Output($"{kv.Key}: {kv.Value.Item1}");
            }
        }

        private static void printRooms()
        {
            Output("---- Current Rooms ----", InfoColor);
            foreach (var room in _server.Rooms)
            {
                Output($"{room.Name}: {room.Users.Count} users | Last Activity: {room.LastActivity.ToShortDateString()} {room.LastActivity.ToShortTimeString()}", InfoColor);
                foreach (var client in room.Users)
                {
                    Output($"\t{client.Nick}", DefaultColor);
                }
            }
            Output("-----------------------", InfoColor);
        }

        private static void maintenanceMode()
        {
            try
            {
                _readInput = false;
                Output("Enter a message to send to all connected clients (Escape to cancel):", DefaultColor);
                ConsoleKeyInfo key;
                var config = ReadLineConfig.Basic;
                bool _cancelled = false;
                config.KeyBehaviors.Add(new InteractiveReadLine.KeyBehaviors.KeyId(ConsoleKey.Escape, false, false, false), (kbt) =>
                {
                    _cancelled = true;
                    kbt.Finish();
                });
                string message = ConsoleReadLine.ReadLine(config);
                if(_cancelled)
                {
                    Console.WriteLine();
                    Output("Cancelled maintenance", InfoColor);
                } 
                else
                {
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        message = "Restarting soon due to maintenance";
                    }
                    _server.EnableMaintenanceMode(message);
                }
            }
            finally
            {
                _readInput = true;
            }
        }

        private static void server_OnStatus(object? sender, StringEventArgs e)
        {
            Output(e.Message, StatusColor);
        }

        private static void server_OnError(object? sender, StringEventArgs e)
        {
            var message = $"Error: {e.Message}";
            Output(message, ErrorColor);
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