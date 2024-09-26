using System.Runtime.CompilerServices;

namespace InteractiveReadLine
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Interactively read a line of text from the provider, using a specified configuration if one is
        /// given as an optional argument.
        /// </summary>
        /// <param name="provider">the IReadLineProvider provider which will perform the interaction with the user (for
        /// example, the ConsoleReadLine object which wraps System.Console)</param>
        /// <param name="config">The configuration to use for this specific interaction</param>
        /// <returns>the text read from the user</returns>
        public static string ReadLine(this IReadLineProvider provider, ReadLineConfig config=null)
        {
            using (provider)
            {
                var handler = new ReadLineHandler(provider, config);
                return handler.ReadLine();
            }
        }

    }
}