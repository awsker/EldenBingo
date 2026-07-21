using System.Diagnostics;

namespace EldenBingo.UI
{

    public partial class KeybindingControl : UserControl
    {
        public class RebindControlEventArgs : EventArgs
        {
            public KeybindingControl BindingControl { get; init; }
            public RebindControlEventArgs(KeybindingControl control)
            {
                BindingControl = control;
            }
        }

        public class TabClosedEventArgs : EventArgs
        {
            public bool Save { get; init; }
            public TabClosedEventArgs(bool save)
            {
                Save = save;
            }
        }

        public static event EventHandler<RebindControlEventArgs> RebindStarted;
        public static event EventHandler<RebindControlEventArgs> RebindFinished;
        public static event EventHandler<TabClosedEventArgs> TabClosed;

        public static void OnTabClosed(bool save)
        {
            TabClosed?.Invoke(null, new TabClosedEventArgs(save));
        }

        private Keys _key;
        public Keys Key
        {
            get
            {
                return _key;
            }
            set
            {
                if (_key != value)
                {
                    _key = value;
                }
                updateRebindText();
            }
        }
        private bool _rebindingKey = false;

        public string DisplayName
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        private string? _valueName;
        public string? ValueName
        {
            get
            {
                return _valueName;
            }
            set
            {
                _valueName = value;
                if (_valueName != null)
                {
                    var propVal = Properties.Settings.Default[value];
                    if (propVal is int)
                    {
                        Key = (Keys)propVal;
                    }
                }
            }
        }

        public KeybindingControl()
        {
            InitializeComponent();
            RebindStarted += KeybindingControl_RebindStarted;
            RebindFinished += KeybindingControl_RebindFinished;
            TabClosed += KeybindingControl_TabClosed;
            Disposed += OnDisposed;
        }

        private void OnDisposed(object? sender, EventArgs e)
        {
            RebindStarted -= KeybindingControl_RebindStarted;
            RebindFinished -= KeybindingControl_RebindFinished;
            TabClosed -= KeybindingControl_TabClosed;
            Disposed -= OnDisposed;
        }

        private void KeybindingControl_RebindStarted(object? sender, RebindControlEventArgs e)
        {
            // If another rebinding was triggered while this was rebinding, cancel this rebind
            if (_rebindingKey && e.BindingControl != this)
            {
                stopRebind();
            }
        }

        private void KeybindingControl_RebindFinished(object? sender, RebindControlEventArgs e)
        {
            // If another control was bound to the same key as this control
            if (e.BindingControl != this && Key != Keys.None && e.BindingControl.Key == Key)
            {
                Key = Keys.None;
            }
        }

        private void KeybindingControl_TabClosed(object? sender, TabClosedEventArgs e)
        {
            if (_rebindingKey)
            {
                stopRebind();
            }
            if (e.Save && _valueName != null)
            {
                Properties.Settings.Default[_valueName] = (int)_key;
            }
        }

        private void _rebindTextBox_Enter(object sender, EventArgs e)
        {
            startRebind();
        }

        private void _rebindTextBox_Leave(object sender, EventArgs e)
        {
            if (_rebindingKey)
            {
                stopRebind();
                RebindFinished?.Invoke(this, new RebindControlEventArgs(this));
                _label.Focus();
            }
        }

        private void _rebindTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (_rebindingKey)
            {
                Key = e.KeyCode == Keys.Escape ? Keys.None : e.KeyCode;
                stopRebind();
                RebindFinished?.Invoke(this, new RebindControlEventArgs(this));
                _label.Focus();
            }
        }

        private void startRebind()
        {
            _rebindingKey = true;
            RebindStarted?.Invoke(this, new RebindControlEventArgs(this));
            updateRebindText();
        }

        private void stopRebind()
        {
            _rebindingKey = false;
            updateRebindText();
        }

        private void updateRebindText()
        {
            void update()
            {
                if (_rebindingKey)
                {
                    _rebindTextBox.Text = "Press a key...";
                }
                else
                {
                    _rebindTextBox.Text = _key.ToString().ToUpper();
                }
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }

        private void _clearBindingButton_Click(object sender, EventArgs e)
        {
            if (_rebindingKey)
                stopRebind();
            Key = Keys.None;
        }
    }
}
