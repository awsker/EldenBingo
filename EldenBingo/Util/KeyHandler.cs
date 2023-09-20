using System.Runtime.InteropServices;

namespace EldenBingo.Util
{
    public class KeyHandler
    {
        private const int WM_KEYDOWN = 0x8000;

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys keys);

        private Keys key;
        private bool _running = false;
        private bool _pressed = false;
        private Thread? _thread;

        public event EventHandler? KeyPressed, KeyReleased;

        public KeyHandler(Keys key)
        {
            this.key = key;
            Start();
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
                var status = GetAsyncKeyState(key);
                
                if(!_pressed && (status & WM_KEYDOWN) > 0)
                {
                    _pressed = true;
                    KeyPressed?.Invoke(this, EventArgs.Empty);
                }
                if(_pressed && (status & WM_KEYDOWN) == 0)
                {
                    _pressed = false;
                    KeyReleased?.Invoke(this, EventArgs.Empty);
                }
                Thread.Sleep(20);
            }
        }

        public void Stop()
        {
            _running = false;
        }
    }
}
