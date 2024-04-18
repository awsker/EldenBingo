using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace EldenBingo.Util
{
    public class KeyHandler
    {
        private const int WM_KEYDOWN = 0x8000;

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys keys);

        private IDictionary<Keys, bool> _keys;
        private bool _running = false;
        private Thread? _thread;

        public event EventHandler<KeyPressedEventArgs>? KeyPressed, KeyReleased;

        public KeyHandler()
        {
            _keys = new ConcurrentDictionary<Keys, bool>();
            Start();
        }

        public void ClearKeys()
        {
            _keys.Clear();
        }

        public void AddKey(Keys key)
        {
            _keys[key] = false;
        }

        public void RemoveKey(Keys key)
        {
            _keys.Remove(key);
        }

        public void ReplaceKey(Keys oldKey, Keys newKey)
        {
            RemoveKey(oldKey);
            AddKey(newKey);
        }

        public void Start()
        {
            if (!_running)
            {
                _running = true;
                _thread = new Thread(poll);
                _thread.Start();
            }
        }

        private void poll()
        {
            while(_running)
            {
                var keys = new List<Keys>(_keys.Keys);
                foreach (var key in keys)
                {
                    if (!_keys.TryGetValue(key, out var pressed))
                        continue;
                    var status = GetAsyncKeyState(key);
                    if (!pressed && (status & WM_KEYDOWN) > 0)
                    {
                        _keys[key] = true;
                        KeyPressed?.Invoke(this, new KeyPressedEventArgs(key));
                    }
                    if (pressed && (status & WM_KEYDOWN) == 0)
                    {
                        _keys[key] = false;
                        KeyReleased?.Invoke(this, new KeyPressedEventArgs(key));
                    }
                }
                Thread.Sleep(20);
            }
        }

        public void Stop()
        {
            _running = false;
        }
    }

    public class KeyPressedEventArgs : EventArgs
    {
        public Keys Key;
        public KeyPressedEventArgs(Keys key)
        {
            Key = key;
        }
    }
}
