namespace EldenBingo.Util
{
    public static class CrashLogger
    {
        public static string LogException(Exception ex)
        {
            // Get the path to the user's roaming AppData directory
            string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // Append your application's name to the AppData path
            string logDirectory = Path.Combine(appDataDir, Application.ProductName);

            // Create the directory if it doesn't exist
            Directory.CreateDirectory(logDirectory);

            // Generate a timestamp for the log file name
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            // Create the log file path
            string logFilePath = Path.Combine(logDirectory, $"CrashLog_{timestamp}.txt");

            // Write the exception details to the log file
            using (StreamWriter writer = new StreamWriter(logFilePath))
            {
                writer.WriteLine($"Crash Log - {timestamp}");
                writer.WriteLine("-----------------------");
                writer.WriteLine($"Exception Type: {ex.GetType().FullName}");
                writer.WriteLine($"Message: {ex.Message}");
                writer.WriteLine($"Stack Trace:\n{ex.StackTrace}");
            }
            return logFilePath;
        }
    }
}
