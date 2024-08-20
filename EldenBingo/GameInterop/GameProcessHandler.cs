using EldenBingoCommon;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace EldenBingo.GameInterop
{
    public enum GameRunningStatus
    {
        NotRunning,
        RunningWithEAC,
        RunningWithoutEAC
    }

    public class GameProcessHandler : IDisposable
    {
        private static readonly Color ErrorColor = Color.Red;
        private static readonly Color IdleColor = Color.DarkGray;
        private static readonly Color SuccessColor = Color.LightGreen;
        private static readonly Color WorkingColor = Color.Orange;

        /// <summary>
        /// Determines if game is currently being started up.
        /// </summary>
        private static bool _startup = false;

        private readonly object _processLock = new object();
        private long _csMenuManAddress = -1L;
        private long _eventManAddress = -1L;
        private long _setEventFlagAddress = -1L;
        private bool _disposed;
        private IntPtr _gameAccessHwnd = IntPtr.Zero;
        private Process? _gameProc = null;
        private MapCoordinates? _lastCoordinates;
        private Thread? _scanGameThread;
        private string? _steam_appid_path;
        private bool _readingProcess;

        public event EventHandler<MapCoordinateEventArgs>? CoordinatesChanged;

        public event EventHandler<StatusEventArgs>? StatusChanged;

        public event EventHandler? ProcessReadingChanged;

        public bool ReadingProcess
        {
            get { return _readingProcess; }
            set
            {
                if (value != _readingProcess)
                {
                    _readingProcess = value;
                    ProcessReadingChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public MapCoordinates? LastCoordinates => _lastCoordinates;

        /// <summary>
        /// Gets the install path of an application.
        /// </summary>
        /// <param name="p_name">The full name of the application from control panel.</param>
        /// <returns>The folder the application is installed into.</returns>
        public static string? GetApplicationPath(string p_name)
        {
            string? getInstallDir(RegistryKey host, string regKey)
            {
                RegistryKey? key = host.OpenSubKey(regKey);
                if (key != null)
                {
                    foreach (string keyName in key.GetSubKeyNames())
                    {
                        RegistryKey? subkey = key.OpenSubKey(keyName);
                        string? displayName = subkey?.GetValue("DisplayName") as string;
                        if (subkey != null && displayName != null)
                        {
                            if (InstallDirCheck(displayName, p_name, subkey, out string? installDir))
                                return installDir;
                        }
                    }
                }
                return null;
            }

            string? key;
            if ((key = getInstallDir(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")) != null)
                return key;

            if ((key = getInstallDir(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")) != null)
                return key;

            if ((key = getInstallDir(Registry.LocalMachine, @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall")) != null)
                return key;

            // NOT FOUND
            return null;
        }

        public void Dispose()
        {
            _disposed = true;
            try
            {
                if (_steam_appid_path != null && File.Exists(_steam_appid_path))
                {
                    File.Delete(_steam_appid_path);
                }
            }
            catch { }
        }

        /// <summary>
        /// Check if game is running
        /// </summary>
        /// <returns>True if the game is responding and running.<returns>
        public async Task<GameRunningStatus> GetGameRunningStatus()
        {
            // check for game
            var process = GetGameProcess();
            if (process == null)
                return GameRunningStatus.NotRunning;

            // make sure game isn't in half-dead state
            int timeout = 0;
            while (!process.Responding && timeout < 10000)
            {
                UpdateStatus("Game not responding...", WorkingColor);
                process.Refresh();
                await Task.Delay(500);
                timeout += 500;
            }
            //Force kill the application after 10 seconds if it's still not responding
            if (!process.Responding)
            {
                process.Kill();
                return GameRunningStatus.NotRunning;
            }

            if (process.HasExited)
            {
                await Task.Delay(500);
                return GameRunningStatus.NotRunning;
            }
            return IsEACRunning() ? GameRunningStatus.RunningWithEAC : GameRunningStatus.RunningWithoutEAC;
        }

        public void KillGameAndEAC()
        {
            var process = GetGameProcess();
            try
            {
                foreach (var sc in GetEACServices())
                {
                    try
                    {
                        if (sc != null && sc.Status != ServiceControllerStatus.Stopped && sc.Status != ServiceControllerStatus.StopPending)
                            sc.Stop();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Error stopping EAC: {e.Message}", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception)
            {
                //Ignore any errors when trying to get EAC service
            }
            if (process != null)
            {
                process.Kill();
                process.WaitForExit(3000);
                process.Close();
            }
        }

        /// <summary>
        /// Starts the game in offline mode without EAC.
        /// </summary>
        public async Task SafeStartGame(string? path = null)
        {
            _startup = true;
            UpdateStatus("Starting game...", WorkingColor);

            // get game path
            string gameExePath = Properties.Settings.Default.GamePath;
            string? gamePath;

            if (!File.Exists(gameExePath))
            {
                gamePath = path ?? GetApplicationPath("ELDEN RING");
                if (gamePath == null || !File.Exists(Path.Combine(gamePath, $"{Properties.Settings.Default.GameName}.exe")) && !File.Exists(Path.Combine(gamePath, "GAME", $"{Properties.Settings.Default.GameName}.exe")))
                    gameExePath = PromptForGamePath();
                else
                {
                    if (File.Exists(Path.Combine(gamePath, "GAME", $"{Properties.Settings.Default.GameName}.exe")))
                        gameExePath = Path.Combine(gamePath, "GAME", $"{Properties.Settings.Default.GameName}.exe");
                    else if (File.Exists(Path.Combine(gamePath, $"{Properties.Settings.Default.GameName}.exe")))
                        gameExePath = Path.Combine(gamePath, $"{Properties.Settings.Default.GameName}.exe");
                    else
                        gameExePath = PromptForGamePath();
                }
            }
            else
            {
                var fileInfo = FileVersionInfo.GetVersionInfo(gameExePath);
                if (fileInfo?.FileDescription == null || !fileInfo.FileDescription.ToLower().Contains(GameData.PROCESS_DESCRIPTION))
                {
                    gameExePath = PromptForGamePath();
                }
            }
            Properties.Settings.Default.GamePath = gameExePath;
            gamePath = Path.GetDirectoryName(gameExePath);

            if (gamePath == null)
                return;

            try
            {
                _steam_appid_path = Path.Combine(gamePath, "steam_appid.txt");
                File.WriteAllText(_steam_appid_path, "1245620");
            }
            catch
            {
                MessageBox.Show("Couldn't write steam id file!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(0);
            }

            ProcessStartInfo siGame = new ProcessStartInfo(Path.Combine(gamePath, $"{Properties.Settings.Default.GameName}.exe"))
            {
                Arguments = "noeac",
                WorkingDirectory = gamePath,
                UseShellExecute = true
            };
            Process procGameStarter = new Process
            {
                StartInfo = siGame
            };
            procGameStarter.Start();
            bool startedSuccessfully = await WaitForProgram(Properties.Settings.Default.GameName, 10000);
            _startup = false;
        }

        public void StartScan()
        {
            _scanGameThread = new Thread(gameProcessScan);
            _scanGameThread.Start();
        }

        /*
        private void initGameMemoryWorker()
        {
            if (_gameProc != null)
            {
                GameMemoryWorker = new GameMemoryWorker(_gameProc, _gameAccessHwnd);
                GameMemoryWorker.Start();
                GameMemoryWorkerChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void killGameMemoryWorker()
        {
            if (GameMemoryWorker != null)
            {
                GameMemoryWorker.Stop();
                GameMemoryWorker = null;
                GameMemoryWorkerChanged?.Invoke(this, EventArgs.Empty);
            }
        }*/

        /// <summary>
        /// Logs messages to log file.
        /// </summary>
        /// <param name="msg">The message to write to file.</param>
        internal static void LogToFile(string msg)
        {
            /*
            string timedMsg = $"[{DateTime.Now:HH:mm:ss}] {msg}";
            Debug.WriteLine(timedMsg);
            try
            {
                using (StreamWriter writer = new StreamWriter(_path_logs, true))
                {
                    writer.WriteLine(timedMsg);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Writing to log file failed: {ex.Message}", Application.ProductName);
            }
            */
        }

        private static Process? GetGameProcess()
        {
            var procList = Process.GetProcessesByName(Properties.Settings.Default.GameName);
            return procList.Length != 1 ? null : procList[0];
        }

        /// <summary>
        /// Check if installDir of an application is valid.
        /// </summary>
        /// <param name="displayName">The application install dir.</param>
        /// <param name="p_name">The apps name.</param>
        /// <param name="subkey">The registry key.</param>
        /// <param name="installDir"></param>
        /// <returns>True if valid.</returns>
        private static bool InstallDirCheck(string displayName, string p_name, RegistryKey subkey, out string? installDir)
        {
            const string RegexNotASCII = @"[^\x00-\x80]+";
            installDir = string.Empty;

            // Check for non-English characters in displayName (CN, KR, ...)
            if (Regex.IsMatch(displayName, RegexNotASCII))
            {
                // check if InstallLocation path contains ELDEN RING sind displayName contains non-standard characters
                installDir = subkey.GetValue("InstallLocation") as string;
                if (installDir != null && installDir.Contains(p_name))
                {
                    // Not needed but just an additional check to see if eldenring.exe is in the InstallLocation path
                    if (File.Exists(Path.Combine(installDir, @"\Game\eldenring.exe")))
                        return true;
                }
            }
            else if (p_name.Equals(displayName, StringComparison.OrdinalIgnoreCase))
            {
                installDir = subkey.GetValue("InstallLocation") as string;
                if (!string.IsNullOrEmpty(installDir))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the user has called this application as administrator.
        /// </summary>
        /// <returns>True if application is running as administrator.</returns>
        private static bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Opens file dialog.
        /// </summary>
        /// <param name="title">The title to sho in the file selection window.</param>
        /// <param name="defaultDir">The default directory to start up to.</param>
        /// <param name="defaultExt">A list of default extensions in ".extension" format.</param>
        /// <param name="filter">A list of names of a file with this extension ("Extension File").</param>
        /// <returns>The path to the selected file.</returns>
        private static string? OpenSelectFileDialog(string title, string defaultDir, string[] defaultExt, string[] filter, bool explicitFilter = false)
        {
            if (defaultExt.Length != filter.Length)
                throw new ArgumentOutOfRangeException("defaultExt must be the same length as filter!");
            string fullFilter = "";
            if (explicitFilter)
            {
                fullFilter = filter[0] + "|" + defaultExt[0];
            }
            else
            {
                for (int i = 0; i < defaultExt.Length; i++)
                {
                    if (i > 0)
                        fullFilter += "|";
                    fullFilter += filter[i] + " (*" + defaultExt[i] + ")|*" + defaultExt[i];
                }
            }

            OpenFileDialog dlg = new OpenFileDialog
            {
                Title = title,
                InitialDirectory = defaultDir,
                //DefaultExt = defaultExt,
                Filter = fullFilter,
                FilterIndex = 0,
            };
            var result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return null;
            return File.Exists(dlg.FileName) ? dlg.FileName : null;
        }

        /// <summary>
        /// Open a prompt to let user choose game installation path.
        /// </summary>
        /// <returns>The choosen file location.<returns>
        private static string PromptForGamePath()
        {
            MessageBox.Show("Couldn't find game installation path!\n\n" +
                            "Please specify the installation path yourself...", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            string? gameExePath = OpenSelectFileDialog("Select eldenring.exe", "C:\\", new[] { "*.exe" }, new[] { "Elden Ring Executable" }, true);
            if (string.IsNullOrEmpty(gameExePath) || !File.Exists(gameExePath))
                Environment.Exit(0);
            var fileInfo = FileVersionInfo.GetVersionInfo(gameExePath);
            if (fileInfo?.FileDescription == null || !fileInfo.FileDescription.ToLower().Contains(GameData.PROCESS_DESCRIPTION))
            {
                MessageBox.Show("Invalid game file!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(0);
            }
            Properties.Settings.Default.GameName = Path.GetFileNameWithoutExtension(gameExePath);
            return gameExePath;
        }

        private ServiceController[] GetEACServices()
        {
            return ServiceController.GetServices().Where(service => service.ServiceName.Contains("EasyAntiCheat")).ToArray();
        }

        private bool IsEACRunning()
        {
            try
            {
                foreach (var sc in GetEACServices())
                {
                    bool eacRunning = sc.Status == ServiceControllerStatus.Running ||
                                 sc.Status == ServiceControllerStatus.ContinuePending ||
                                 sc.Status == ServiceControllerStatus.StartPending;
                    if (eacRunning)
                    {
                        return true;
                    }
                }
            }
            catch(Win32Exception)
            {
                //Exception reading services, let the app continue
                return false;
            }
            return false;
        }

        /// <summary>
        /// Write a status to the status bar.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="color">The color to use.</param>
        private void UpdateStatus(string text, Color color)
        {
            StatusChanged?.Invoke(this, new StatusEventArgs(text, color));
        }

        /// <summary>
        /// Waits a set timeout for a process to appear.
        /// </summary>
        /// <param name="appName">The process to look for.</param>
        /// <param name="timeout">The maximum amount of time in milliseconds to wait for.</param>
        /// <returns>True if process showed up before timeout passed.</returns>
        private async Task<bool> WaitForProgram(string appName, int timeout = 5000)
        {
            int timePassed = 0;
            while (true)
            {
                Process[] procList = Process.GetProcessesByName(appName);
                foreach (Process proc in procList)
                {
                    if (proc.ProcessName == appName && proc.Responding)
                        return true;
                }

                await Task.Delay(500);
                timePassed += 500;
                if (timePassed > timeout)
                    return false;
            }
        }

        #region Game process scanning

        /// <summary>
        /// Checks if an address is valid.
        /// </summary>
        /// <param name="address">The address (the pointer points to).</param>
        /// <returns>True if (pointer points to a) valid address.</returns>
        private static bool IsValidAddress(long address)
        {
            return address >= 0x10000 && address < 0x000F000000000000;
        }

        private void establishCSMenuManAddress()
        {
            try
            {
                _csMenuManAddress = resolveAddressFromAssembly(GameData.PATTERN_CSMENUMAN);
            }
            catch (Exception)
            {
                _csMenuManAddress = -1;
            }
        }
        
        private void establishEventManagerAddresses()
        {
            try
            {
                _eventManAddress = resolveAddressFromAssembly(GameData.PATTERN_CSFD4VIRTUALMEMORYFLAG);
            }
            catch (Exception)
            {
                _eventManAddress = -1;
            }
            
            try
            {
                _setEventFlagAddress = resolveAddressFromAssembly(GameData.PATTERN_SETEVENTFLAGFUNC);
            }
            catch (Exception)
            {
                _setEventFlagAddress = -1;
            }
        }

        /// <summary>
        /// Initialize PatternScanner and read all memory from process.
        /// </summary>
        /// <param name="hProcess">Handle to the process in whose memory pattern will be searched for.</param>
        /// <param name="pModule">Module which will be searched for the pattern.</param>
        private long findPatternInProcess(IntPtr hProcess, ProcessModule pModule, byte[] pattern, string mask)
        {
            long dwStart = processBaseAddress(pModule);
            int nSize = pModule.ModuleMemorySize;

            var bData = new byte[nSize];

            if (!WinAPI.ReadProcessMemory(hProcess, dwStart, bData, (ulong)nSize, out IntPtr lpNumberOfBytesRead))
            {
                throw new Exception("Could not read memory in PatternScan()!");
            }
            if (lpNumberOfBytesRead.ToInt64() != nSize || bData == null || bData.Length == 0)
            {
                throw new Exception("ReadProcessMemory error in PatternScan()!");
            }
            return PatternScanLazySIMD.FindPattern(bData, pattern, mask);
        }

        private long followPointers(long startAddress, long[] offsets)
        {
            long currentAddr = startAddress;
            foreach (var offset in offsets)
            {
                currentAddr += offset;
                if (!IsValidAddress(currentAddr))
                    return -1;
                currentAddr = readPointer(currentAddr); //Follow reference pointer
            }
            return currentAddr;
        }

        private void gameProcessScan()
        {
            UpdateStatus("Waiting for game...", IdleColor);
            //Keep count of how many coordinate polls returned the same coordinates as last time. After 10, send coordinates anyway
            int pollsSinceSend = 0;
            while (!_disposed)
            {
                if (!_startup)
                {
                    Process? process = null;
                    try
                    {
                        process = GetGameProcess();
                    } 
                    catch (Win32Exception)
                    {
                        //This seems to happen sometimes when the game is just starting. Ignore it and wait for next loop
                    }
                    if (process == null) //No process found
                    {
                        //If we already had a reference to the process, the game has exited
                        if (_gameProc != null)
                        {
                            lock (_processLock)
                            {
                                _gameProc = null;
                            }
                            UpdateStatus("Waiting for game...", IdleColor);
                        }
                    }
                    else //Process found
                    {
                        if (IsEACRunning())
                        {
                            UpdateStatus("EAC is running...", ErrorColor);
                            continue;
                        }
                        //New process found (different from previous)
                        if (_gameProc == null || _gameProc.Id != process.Id)
                        {
                            _gameAccessHwnd = IntPtr.Zero;
                            _csMenuManAddress = -1;
                            resetCoordinates();
                            OpenGame();
                        }
                        //If process was found
                        if (_gameProc != null && !_gameProc.HasExited && _gameAccessHwnd != IntPtr.Zero && _gameProc.MainModule?.BaseAddress != IntPtr.Zero)
                        {
                            ReadingProcess = true;
                            var coordinates = readPlayerCoordinates();
                            //Coordinates changed or 10 polls since last send
                            if (_lastCoordinates.HasValue != coordinates.HasValue ||
                                _lastCoordinates.HasValue && coordinates.HasValue && !_lastCoordinates.Equals(coordinates.Value) ||
                                pollsSinceSend >= 10)
                            {
                                CoordinatesChanged?.Invoke(this, new MapCoordinateEventArgs(coordinates));
                                pollsSinceSend = 0;
                            }
                            else
                            {
                                ++pollsSinceSend;
                            }
                            _lastCoordinates = coordinates;
                            //Do this 10 times per second
                            Thread.Sleep(100);
                            //Jump to next loop
                            continue;
                        }
                        else
                        {
                            ReadingProcess = false;
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Opens the game for full memory access.
        /// </summary>
        /// <param name="retry">Indicates if a single retry for access should be executed.</param>
        /// <returns>True if access has been granted.</returns>
        private bool OpenGame(bool retry = true)
        {
            UpdateStatus("Accessing game...", WorkingColor);
            var process = GetGameProcess();
            if (process == null)
            {
                UpdateStatus("Game not running...", ErrorColor);
                return false;
            }
            lock (_processLock)
            {
                _gameProc = process;
                // open game
                _gameAccessHwnd = WinAPI.OpenProcess(WinAPI.PROCESS_WM_READ, false, (uint)_gameProc.Id);
            }
            try
            {
                if (_gameAccessHwnd == IntPtr.Zero || _gameProc.MainModule.BaseAddress == IntPtr.Zero)
                {
                    LogToFile($"Process: {_gameProc.ProcessName} - {_gameProc.MainWindowTitle}");
                    LogToFile($"Access hWnd: {_gameAccessHwnd}:X");
                    LogToFile($"BaseAddress: {_gameProc.MainModule.BaseAddress:X}");
                    LogToFile($"Responding: {_gameProc.Responding}");
                    if (retry)
                    {
                        Thread.Sleep(5000);
                        return OpenGame(false);
                    }
                    else
                    {
                        UpdateStatus("No access to game process...", ErrorColor);
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                UpdateStatus("No access to game process...", ErrorColor);
                return false;
            }
            UpdateStatus("Monitoring game...", SuccessColor);
            return true;
        }

        private long processBaseAddress(ProcessModule pModule)
        {
            long dwStart = 0;
            if (IntPtr.Size == 4)
                dwStart = (uint)pModule.BaseAddress;
            else if (IntPtr.Size == 8)
                dwStart = (long)pModule.BaseAddress;
            return dwStart;
        }

        private MapCoordinates? readPlayerCoordinates()
        {
            if (_gameProc != null && _gameProc.Responding)
            {
                if (_csMenuManAddress <= 0)
                {
                    establishCSMenuManAddress();
                }
                if (IsValidAddress(_csMenuManAddress))
                {
                    var ptrAddr = followPointers(_csMenuManAddress, new long[] { 0x80L, 0x250L });

                    if (IsValidAddress(ptrAddr))
                    {
                        byte[] buf = new byte[20];
                        if (WinAPI.ReadProcessMemory(_gameAccessHwnd, ptrAddr + 0x24L, buf, (ulong)buf.Length, out _)) //Read values from offset 0x24L
                        {
                            var map = BitConverter.ToInt32(buf, 0);
                            if(map != 0)
                            {
                                //Player not in overworld (so probably in loading screen or DLC area)
                                return null;
                            }
                            var x = BitConverter.ToSingle(buf, 4);
                            var y = BitConverter.ToSingle(buf, 8);
                            var underground = BitConverter.ToBoolean(buf, 12);
                            var rad = BitConverter.ToSingle(buf, 16);
                            if (x > 0 && y > 0)
                            {
                                return new MapCoordinates(x, y, underground, rad);
                            }
                            if (x == 0 || y == 0)
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        _csMenuManAddress = -1;
                    }
                }
                else
                {
                    _csMenuManAddress = -1;
                }
            }
            return null;
        }

        private long readPointer(long address)
        {
            byte[] buf = new byte[IntPtr.Size];

            if (WinAPI.ReadProcessMemory(_gameAccessHwnd, address, buf, (ulong)IntPtr.Size, out _))
            {
                return IntPtr.Size == 4 ? BitConverter.ToInt32(buf) : BitConverter.ToInt64(buf);
            }
            return -1;
        }

        private void resetCoordinates()
        {
            if (_lastCoordinates != null)
            {
                CoordinatesChanged?.Invoke(this, new MapCoordinateEventArgs(null));
            }
            _lastCoordinates = null;
        }

        private long resolveAddressFromAssembly(string pattern, uint addressLength = 4, int offset = 0)
        {
            var processOffset = processBaseAddress(_gameProc.MainModule);

            var patternData = stringToByteArray(pattern);

            byte[] assm = new byte[addressLength];

            long address = processOffset + findPatternInProcess(_gameAccessHwnd, _gameProc.MainModule, patternData.Item1, patternData.Item2) + offset;
            WinAPI.ReadProcessMemory(_gameAccessHwnd, address + patternData.Item3, assm, addressLength, out _); //Only fetch the 4 address bytes
            var offsetFromAsm = BitConverter.ToUInt32(assm);
            return readPointer(address + offsetFromAsm + patternData.Item3 + addressLength);
        }

        private (byte[], string, int) stringToByteArray(string szPattern)
        {
            string[] saPattern = szPattern.Split(' ');
            string szMask = "";
            int firstMaskIndex = -1;
            for (int i = 0; i < saPattern.Length; i++)
            {
                if (saPattern[i] == "??")
                {
                    szMask += "?";
                    saPattern[i] = "0";
                    if (firstMaskIndex == -1)
                        firstMaskIndex = i;
                }
                else szMask += "x";
            }
            byte[] cbPattern = new byte[saPattern.Length];
            for (int i = 0; i < saPattern.Length; i++)
                cbPattern[i] = Convert.ToByte(saPattern[i], 0x10);

            if (cbPattern == null || cbPattern.Length == 0)
                throw new Exception("Pattern's length is zero!");
            if (cbPattern.Length != szMask.Length)
                throw new Exception("Pattern's bytes and szMask must be of the same size!");
            return (cbPattern, szMask, firstMaskIndex);
        }

        #endregion Game process scanning

        public void getEventManPointers() {
            if (_eventManAddress <= 0 || _setEventFlagAddress <= 0) {
                establishEventManagerAddresses();
            }
        }
        
        public IntPtr GetEventManPtr() {
            getEventManPointers();
            if (IsValidAddress(_eventManAddress)) {
                byte[] pointer = new byte[sizeof(Int64)];
                WinAPI.ReadProcessMemory(_gameAccessHwnd, _eventManAddress, pointer, (ulong)pointer.Length, out _);
                return new IntPtr(BitConverter.ToInt64(pointer));
            }
            
            return IntPtr.Zero;
        }
        
        public IntPtr GetSetEventFlagPtr() {
            getEventManPointers();
            if (IsValidAddress(_setEventFlagAddress)) {
                byte[] pointer = new byte[sizeof(Int64)];
                WinAPI.ReadProcessMemory(_gameAccessHwnd, _setEventFlagAddress, pointer, (ulong)pointer.Length, out _);
                return new IntPtr(BitConverter.ToInt64(pointer));
            }
            return IntPtr.Zero;
        }

        public IntPtr GetPrefferedIntPtr(int size, IntPtr? basePtr = null, uint flProtect = WinAPI.PAGE_READWRITE)
        {
            long baseAddress = _gameProc!.MainModule.BaseAddress.ToInt64();
            if (basePtr != null)
                baseAddress = basePtr.Value.ToInt64();

            var ptr = IntPtr.Zero;
            var i = 1;
            while (ptr == IntPtr.Zero)
            {
                var distance = baseAddress - (WinAPI.SystemInfo.dwAllocationGranularity * i);
                ptr = WinAPI.VirtualAllocEx(_gameAccessHwnd, (IntPtr)distance, (IntPtr)size, WinAPI.MEM_RESERVE | WinAPI.MEM_COMMIT, flProtect);
                i++;
            }

            return ptr;
        }
        
        public void ExecuteAsm(byte[] asm) {
            IntPtr insertPtr = GetPrefferedIntPtr(asm.Length,
                flProtect:WinAPI.PAGE_EXECUTE_READWRITE);

            WinAPI.WriteProcessMemory(_gameAccessHwnd, insertPtr.ToInt64(), asm, (ulong)asm.Length, out _);
            Execute(insertPtr);
            Free(insertPtr);
        }
        
        /// <summary>
        /// Starts a thread at the given address and waits for it to complete. Returns execution result.
        /// </summary>
        public uint Execute(IntPtr address, uint timeout = 0xFFFFFFFF)
        {
            IntPtr thread = WinAPI.CreateRemoteThread(_gameAccessHwnd, IntPtr.Zero, 0, address, IntPtr.Zero, 0, IntPtr.Zero);
            uint result = WinAPI.WaitForSingleObject(thread, timeout);
            WinAPI.CloseHandle(thread);
            return result;
        }
        
        /// <summary>
        /// Frees a memory region at the given address. Returns true if successful.
        /// </summary>
        public bool Free(IntPtr address)
        {
            return WinAPI.VirtualFreeEx(_gameAccessHwnd, address, IntPtr.Zero, WinAPI.MEM_RELEASE);
        }
    }
}