using EldenBingo.Util;
using EldenBingoCommon;

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
            if(save)
            {
                Properties.Settings.Default.Save();
            }
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(EldenBingo_UnhandledException);
            _mainForm = new MainForm();
            Application.Run(_mainForm);
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