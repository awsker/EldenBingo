using System;
using System.Collections.Generic;
using InteractiveReadLine.Abstractions;
using InteractiveReadLine.Formatting;

namespace InteractiveReadLine
{
    /// <summary>
    /// Exposes a IReadLineProvider that's wrapping a IConsole object, which by default is a wrapper around the System.Console.
    /// This class is the standard implementation of a ReadLine provider, intended for use with console applications.
    /// </summary>
    public class ConsoleReadLine : IReadLineProvider
    {
        private readonly IConsole _console;
        private FormattedText _lastWrittenText;
        private int _lastWrittenCursor;
        private int _startingRow;

        public ConsoleReadLine(IConsole console=null)
        {
            _console = console ?? new SystemConsoleWrapper();
            this.Start();
        }

        /// <summary>
        /// Reads a console key from the underlying provider. This method blocks until a key is received.
        /// </summary>
        /// <returns></returns>
        public ConsoleKeyInfo ReadKey()
        {
            return _console.ReadKey();
        }

        /// <summary>
        /// Sets the display state on the underlying console, consisting of a prefix, body, suffix, and cursor
        /// position.
        /// </summary>
        /// <param name="state"></param>
        public void SetDisplay(LineDisplayState state)
        {
            var totalText = state.Prefix + state.LineBody + state.Suffix;
            var cursor = state.Prefix.Length + state.Cursor;
            this.SetText(totalText, cursor);
        }

        /// <summary>
        /// Sets the read line to contain the specified text and cursor position.
        /// </summary>
        /// <param name="totalText"></param>
        /// <param name="cursorPos"></param>
        private void SetText(FormattedText totalText, int cursorPos)
        {
            // The process of setting the input text requires us to find the difference between 
            // the last written text and the new text, then perform the minimum amount of character
            // writes necessary to make the two identical

            // First, we should determine what the new line needs to look like. If the new line is 
            // longer than the old line, we will write the new line exactly.  If it's shorter, we'll 
            // need to pad it out with empty characters 
            var writeText = (totalText.Length >= _lastWrittenText.Length)
                ? totalText
                : totalText + new string(' ', _lastWrittenText.Length - totalText.Length);

            // Sweep through each character in the text to write and determine if an edit needs to be 
            // made (or does the FormattedChar match what was last written at this position?)
            // Cluster the edits into contiguous FormattedText objects, each with a cursor start position.
            var edits = new List<Tuple<int, FormattedText>>();

            Tuple<int, FormattedText> edit = null;
            
            for (int i = 0; i < writeText.Length; i++)
            {
                if (i < _lastWrittenText.Length && writeText[i].Equals(_lastWrittenText[i]))
                {
                    if (edit != null)
                    {
                        edits.Add(edit);
                        edit = null;
                    }

                    continue;
                }

                if (edit == null)
                {
                    edit = new Tuple<int, FormattedText>(i, writeText[i]);
                }
                else
                {
                    edit = new Tuple<int, FormattedText>(edit.Item1, edit.Item2 + writeText[i]);
                }

                /*
                int left = this.ColOffset(i);
                int top = this.RowOffset(i) + _startingRow;

                if (left != _console.CursorLeft)
                    _console.CursorLeft = left;
                if (top != _console.CursorTop)
                    _console.CursorTop = top;

                _console.Write(writeText[i]);
            */
            }
            if (edit != null)
                edits.Add(edit);

            foreach (var e in edits)
            {
                
                int left = this.ColOffset(e.Item1);
                int top = this.RowOffset(e.Item1) + _startingRow;

                if (left != _console.CursorLeft)
                    _console.CursorLeft = left;
                if (top != _console.CursorTop)
                    _console.CursorTop = top;

                _console.Write(e.Item2);
            }
            
            _lastWrittenText = totalText;

            // Check if we shifted down the buffer. In certain cases, if we reach the end of the buffer
            // height and we skip a line, the System.Console shifts everything up, and our starting row
            // will effectively be less than where we started.  It will never move down.
            var writtenRowOffset = this.RowOffset(_lastWrittenText.Length);
            if (writtenRowOffset + _startingRow >= _console.BufferHeight)
            {
                _startingRow = _console.BufferHeight - writtenRowOffset - 1;
            }

            _console.CursorTop = _startingRow + this.RowOffset(cursorPos);
            _console.CursorLeft = this.ColOffset(cursorPos);
            _lastWrittenCursor = cursorPos;
        }

        /// <summary>
        /// Writes a message out to the console out in the spot where the current read line input is, then
        /// immediately re-displays the line input on the next row.
        /// </summary>
        /// <param name="text">The text to write to the console, a newline char will be added automatically</param>
        public void InsertText(FormattedText text)
        {
            var currentText = _lastWrittenText;
            var currentCursor = _lastWrittenCursor;

            SetText(text, text.Length);
            _console.WriteLine("");

            _startingRow = _console.CursorTop;
            _lastWrittenText = "";
            _lastWrittenCursor = 0;
            SetText(currentText, currentCursor);

        }
        
        public void Dispose()
        {
            this.Finish();
        }

        private void Start()
        {
            // _console.WriteLine(string.Empty);
            _startingRow = _console.CursorTop;
            _console.CursorLeft = 0;
            _lastWrittenText = string.Empty;
        }

        private void Finish()
        {
            _console.WriteLine(string.Empty);
        }

        private int ColOffset(int length) => length % _console.BufferWidth;

        private int RowOffset(int length) => (length - this.ColOffset(length)) / _console.BufferWidth;

        /// <summary>
        /// Provides a convenient static method of calling the ReadLine method on the System.Console
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string ReadLine(ReadLineConfig config=null)
        {
            var provider = new ConsoleReadLine();
            return provider.ReadLine(config ?? ReadLineConfig.Basic);
        }
    }
}