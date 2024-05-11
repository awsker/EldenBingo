using EldenBingo.Util;

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

            if (Properties.Settings.Default.IsFirstRun)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.IsFirstRun = false;
                Properties.Settings.Default.Save();
            }
            const int idTokenLength = 10;
            if (Properties.Settings.Default.IdentityToken.Length != idTokenLength)
            {
                Properties.Settings.Default.IdentityToken = generateIdentityToken(idTokenLength);
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

        private static string generateIdentityToken(int length)
        {
            var r = new Random();
            string token = string.Empty;
            for (int i = 0; i < length; ++i)
            {
                var t = r.Next(0, 3);
                token += t switch
                {
                    0 => (char)r.Next(48, 58),
                    1 => (char)r.Next(65, 91),
                    _ => (char)r.Next(97, 123),
                };
            }
            return token;
        }
    }
}