using System.Runtime.InteropServices;

namespace EldenBingo.GameInterop
{
    internal class WinAPI
    {
        internal const uint PROCESS_ALL_ACCESS = 0x001F0FFF;
        internal const int PROCESS_WM_READ = 0x0010;

        public enum DeviceCap : int
        {
            /// <summary>
            /// Horizontal width in pixels
            /// </summary>
            HORZRES = 8,

            /// <summary>
            /// Vertical height in pixels
            /// </summary>
            VERTRES = 10,

            /// <summary>
            /// Current vertical refresh rate of the display device (for displays only) in Hz
            /// </summary>
            VREFRESH = 116
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceCaps(IntPtr hdc, DeviceCap nIndex);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr OpenProcess(
            uint dwDesiredAccess,
            bool bInheritHandle,
            uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadProcessMemory(
            IntPtr hProcess,
            long lpBaseAddress,
            [Out] byte[] lpBuffer,
            ulong dwSize,
            out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteProcessMemory(
            IntPtr hProcess,
            long lpBaseAddress,
            [In, Out] byte[] lpBuffer,
            ulong dwSize,
            out IntPtr lpNumberOfBytesWritten);
    }
}