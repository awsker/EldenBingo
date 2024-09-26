using System;
using System.Text;
using InteractiveReadLine.Formatting;

namespace InteractiveReadLine.Abstractions
{
    /// <summary>
    /// A wrapper around the System.Console object. This currently only exists to allow unit testing.
    /// </summary>
    internal class SystemConsoleWrapper : IConsole
    {
        public int CursorLeft
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        public int CursorTop
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }

        public int BufferHeight => Console.BufferHeight;

        public int BufferWidth => Console.BufferWidth;

        public void Write(FormattedText text)
        {
            // Break the text into pieces which have the same foreground and background colors, then write them
            // to the Console object 
            var pieces = text.SplitByFormatting();

            foreach (var piece in pieces)
            {
                if (piece.Length <= 0)
                    continue;
                
                if (piece[0].Foreground == null || piece[0].Background == null)
                    Console.ResetColor();

                if (piece[0].Foreground != null)
                    Console.ForegroundColor = (ConsoleColor) piece[0].Foreground;
                
                if (piece[0].Background != null)
                    Console.BackgroundColor = (ConsoleColor) piece[0].Background;
                
                Console.Write(piece.Text);
            }
        }

        public void WriteLine(FormattedText text)
        {
            this.Write(text);
            this.Write("\n");
        }

        public void Write(FormattedChar c)
        {
            // TODO: Is there a more efficient way of dealing with this?
            Console.ResetColor();

            if (c.Foreground != null)
                Console.ForegroundColor = (ConsoleColor) c.Foreground;

            if (c.Background != null)
                Console.BackgroundColor = (ConsoleColor) c.Background;

            Console.Write(c.Char);
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey(true);
        }
    }
}