namespace EldenBingo.UI
{
    public class HotkeyEventArgs : EventArgs
    {
        public int HotkeyHash { get; init; }
        public HotkeyEventArgs(int hotkeyHash)
        {
            HotkeyHash = hotkeyHash;
        }
    }
}
