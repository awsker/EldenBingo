namespace EldenBingo.Util
{
    public static class Logger
    {
        public static LogMode LogMode { get; set; } = LogMode.ApplicationConsole;

        public static void LogException(Exception ex)
        {
            if(LogMode == LogMode.File || LogMode == LogMode.Both)
            {
                logToFile(ex);
            }
            if(LogMode >= LogMode.ApplicationConsole)
            {
                logToConsole(ex);
            }
        }

        private static void logToFile(Exception ex)
        {
            try
            {
                // Get the path to the user's roaming AppData directory
                string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                // Append your application's name to the AppData path
                string logDirectory = Path.Combine(appDataDir, Application.ProductName);

                // Create the directory if it doesn't exist
                Directory.CreateDirectory(logDirectory);

                var now = DateTime.Now;
                // Generate a timestamp for the log file name
                string timestamp = now.ToString("yyyy-MM-dd");

                // Create the log file path
                string logFilePath = Path.Combine(logDirectory, $"Log_{timestamp}.txt");

                // Write the exception details to the log file
                using (StreamWriter writer = new StreamWriter(logFilePath))
                {
                    writer.WriteLine($"Exception Log - {now.ToString("yyyy-MM-dd-HH-mm-ss")}");
                    writer.WriteLine("-----------------------");
                    writer.WriteLine($"Exception Type: {ex.GetType().FullName}");
                    writer.WriteLine($"Message: {ex.Message}");
                    writer.WriteLine($"Stack Trace:\n{ex.StackTrace}");
                }
            } 
            catch(Exception e)
            {
                if(LogMode >= LogMode.ApplicationConsole)
                {
                    logToConsole(e);
                }
            }
        }

        private static void logToConsole(Exception ex)
        {
            MainForm.Instance?.PrintToConsole(ex.Message, Color.Red, true);
        }
    }

    public enum LogMode
    {
        None,
        File,
        ApplicationConsole,
        Both
    }
}
