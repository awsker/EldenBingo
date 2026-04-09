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

            bool save = false;
            if (Properties.Settings.Default.IsFirstRun)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.IsFirstRun = false;
                save = true;
            }
            const int idTokenLength = 10;
            
            if (Properties.Settings.Default.IdentityToken.Length != idTokenLength)
            {
                Properties.Settings.Default.IdentityToken = IdentityToken.GenerateIdentityToken(idTokenLength);
                save = true;
            }
            // Save settings if the address was changed from the old default value
            save |= rewriteOldAddress();
            if (save)
            {
                Properties.Settings.Default.Save();
            }
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(EldenBingo_UnhandledException);
            _mainForm = new MainForm();
            Application.Run(_mainForm);
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