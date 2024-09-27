using EldenBingoCommon;
using EldenBingoServer;
using InteractiveReadLine;
using Neto.Shared;

namespace EldenBingoServerStandalone
{
    internal static class Program
    {
        private const ConsoleColor DefaultColor = ConsoleColor.Gray;
        private const ConsoleColor StatusColor = ConsoleColor.DarkYellow;
        private const ConsoleColor InfoColor = ConsoleColor.Green;
        private const ConsoleColor ErrorColor = ConsoleColor.Red;
        private static bool _stopCalled = false;
        private static Server _server;
        private static Thread _keyboardListenThread;

        private static string _jsonFile;

        private static bool _readInput;

        private static IDictionary<char, (string, Action)> _keyboardShortcuts = new Dictionary<char, (string, Action)>()
        {
            {'k', new("List keyboard commands", showShortcuts)},
            {'r', new("List all rooms", printRooms)},
            {'j', new("Print path to server data json", showJsonPath)},
            {'m', new("Enable Maintenance mode", maintenanceMode)},
        };

        public static void Main(string[] args)
        {
            int port = BingoConstants.DefaultPort;
            if (args.Length > 0)
            {
                if (!int.TryParse(args[0], out port))
                {
                    output("Invalid port", ErrorColor);
                }
            }
            if (args.Length > 1)
            {
                _jsonFile = args[1];
            }
            else
            {
                string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appSpecificFolder = Path.Combine(appDataFolder, "EldenBingo");

                if (!Directory.Exists(appSpecificFolder))
                {
                    Directory.CreateDirectory(appSpecificFolder);
                }
                _jsonFile = Path.Combine(appSpecificFolder, "serverData.json");
            }
            _server = new Server(port, _jsonFile);
            _server.OnError += server_OnError;
            _server.OnStatus += server_OnStatus;
            _server.Host();
            output("Press 'k' to list all keyboard commands", InfoColor);

            _keyboardListenThread = new Thread(new ThreadStart(listenKeyBoardEvent));
            _keyboardListenThread.Start();
            _readInput = true;
            var waitHandle = new ManualResetEvent(false);

            Console.CancelKeyPress += async (o, e) =>
            {
                e.Cancel = true;
                _stopCalled = true;
                output("Stopping server...", StatusColor);
                await _server.Stop();
                waitHandle.Set();
            };
            waitHandle.WaitOne();
        }

        private static void log(string text)
        {
            var timestamp = DateTime.Now.ToString();
            File.AppendAllText("log.txt", $"[{timestamp}] {text}{Environment.NewLine}");
        }

        private static void output(string text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static void listenKeyBoardEvent()
        {
            while (!_stopCalled)
            {
                if (_readInput && Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (_keyboardShortcuts.TryGetValue(key.KeyChar, out var item))
                    {
                        item.Item2();
                    }
                }
                Thread.Sleep(50);
            }
        }

        private static void showShortcuts()
        {
            output("---- Keyboard Commands ----", InfoColor);
            foreach (var kv in _keyboardShortcuts)
            {
                if (kv.Key == 'k')
                    continue;
                output($"{kv.Key}: {kv.Value.Item1}");
            }
            output("---------------------------", InfoColor);
        }

        private static void printRooms()
        {
            output("---- Current Rooms ----", InfoColor);
            foreach (var room in _server.Rooms)
            {
                output($"{room.Name}: {room.Users.Count} users | Last Activity: {room.LastActivity.ToShortDateString()} {room.LastActivity.ToShortTimeString()}", InfoColor);
                foreach (var client in room.Users)
                {
                    output($"\t{client.Nick}", DefaultColor);
                }
            }
            output("-----------------------", InfoColor);
        }

        private static void maintenanceMode()
        {
            try
            {
                output("Maintenance Mode", InfoColor);
                _readInput = false;
                output("Enter a message to send to all connected clients (Escape to cancel):", DefaultColor);
                ConsoleKeyInfo key;
                var config = ReadLineConfig.Basic;
                bool _cancelled = false;
                config.KeyBehaviors.Add(new InteractiveReadLine.KeyBehaviors.KeyId(ConsoleKey.Escape, false, false, false), (kbt) =>
                {
                    _cancelled = true;
                    kbt.Finish();
                });
                string message = ConsoleReadLine.ReadLine(config);
                if (_cancelled)
                {
                    Console.WriteLine();
                    output("Cancelled maintenance", InfoColor);
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

        private static void showJsonPath()
        {
            output("Json Path", InfoColor);
            var text = _jsonFile;
            try
            {
                if (File.Exists(_jsonFile))
                {
                    var info = new FileInfo(_jsonFile);
                    string[] sizes = { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB" };
                    long len = info.Length;
                    int order = 0;

                    while (len >= 1024 && order < sizes.Length - 1)
                    {
                        order++;
                        len /= 1024;
                    }
                    text += $" ({len:0.##} {sizes[order]})";
                }
            } catch(Exception) {}
            output(text, DefaultColor);
        }

        private static void server_OnStatus(object? sender, StringEventArgs e)
        {
            output(e.Message, StatusColor);
        }

        private static void server_OnError(object? sender, StringEventArgs e)
        {
            var message = $"Error: {e.Message}";
            output(message, ErrorColor);
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