using EldenBingo.Util;
using EldenBingoCommon;
using System.Configuration;
using System.Reflection;

namespace EldenBingo
{
    internal static class Program
    {
        private static MainForm? _mainForm;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            handleSettingsChanges();

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(EldenBingo_UnhandledException);
            _mainForm = new MainForm();
            Application.Run(_mainForm);
        }

        private static void handleSettingsChanges()
        {
            bool changed = false;
            const int idTokenLength = 10;

            if (Properties.Settings.Default.IsFirstRun)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.IsFirstRun = false;
                changed = true;
            }
            if (Properties.Settings.Default.IdentityToken.Length != idTokenLength)
            {
                Properties.Settings.Default.IdentityToken = IdentityToken.GenerateIdentityToken(idTokenLength);
                changed = true;
            }
            //Use previous binding for Click hotkey if set
            if (Properties.Settings.Default.ClickHotkey > 0)
            {
                Properties.Settings.Default.Hotkey_Check = Properties.Settings.Default.ClickHotkey;
                Properties.Settings.Default.ClickHotkey = 0;
                changed = true;
            }
            if (Properties.Settings.Default.NumpadNavigation)
            {
                Properties.Settings.Default.Hotkey_Up = (int)Keys.NumPad8;
                Properties.Settings.Default.Hotkey_Down = (int)Keys.NumPad2;
                Properties.Settings.Default.Hotkey_Left = (int)Keys.NumPad4;
                Properties.Settings.Default.Hotkey_Right = (int)Keys.NumPad6;
                Properties.Settings.Default.Hotkey_UpLeft = (int)Keys.NumPad7;
                Properties.Settings.Default.Hotkey_UpRight = (int)Keys.NumPad9;
                Properties.Settings.Default.Hotkey_DownLeft = (int)Keys.NumPad1;
                Properties.Settings.Default.Hotkey_DownRight = (int)Keys.NumPad3;
                Properties.Settings.Default.Hotkey_Star = (int)Keys.Multiply;
                Properties.Settings.Default.Hotkey_CountIncrease = (int)Keys.Add;
                Properties.Settings.Default.Hotkey_CountDecrease = (int)Keys.Subtract;
                Properties.Settings.Default.NumpadNavigation = false;
                changed = true;
            }
            else if (Properties.Settings.Default.ArrowNavigation)
            {
                Properties.Settings.Default.Hotkey_Up = (int)Keys.Up;
                Properties.Settings.Default.Hotkey_Down = (int)Keys.Down;
                Properties.Settings.Default.Hotkey_Left = (int)Keys.Left;
                Properties.Settings.Default.Hotkey_Right = (int)Keys.Right;
                Properties.Settings.Default.ArrowNavigation = false;
                changed = true;
            }
            // Save settings if the address was changed from the old default value
            changed |= rewriteOldAddress();
            if (changed)
            {
                Properties.Settings.Default.Save();
            }
        }

        private static bool rewriteOldAddress()
        {
            var serverAddress = Properties.Settings.Default.ServerAddress;
            var oldServerAddresses = new HashSet<string>(Properties.Settings.Default.OldServerAddresses.Split('|'), StringComparer.InvariantCultureIgnoreCase);
            if (oldServerAddresses.Contains(serverAddress)) 
            {
                var prop = typeof(Properties.Settings).GetProperty("ServerAddress");
                if (prop != null)
                {
                    var defValue = prop.GetCustomAttribute<DefaultSettingValueAttribute>();
                    if (defValue != null)
                    {
                        Properties.Settings.Default.ServerAddress = defValue.Value;
                        return true;
                    }
                }
            }
            return false;
        }

        private static void EldenBingo_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            void showError()
            {
                var message = string.Empty;
                var path = string.Empty;
                if (e.ExceptionObject is Exception ex) 
                {
                    message = ex.Message;
                    path = CrashLogger.LogException(ex);
                    message += $"{Environment.NewLine}{Environment.NewLine}{"Log written to:"}{Environment.NewLine}{path}";
                }
                MessageBox.Show(_mainForm,
                    $"Unexpected application exception.{Environment.NewLine}{message}",
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Application.Exit();
            }
            if (_mainForm != null)
            {
                _mainForm.Invoke(showError);
            }
        }
    }
}