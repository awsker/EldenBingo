using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tokenizing;

namespace InteractiveReadLine
{
    /// <summary>
    /// This class handles getting a single line of input from the underlying IReadLineProvider. It reads keys from the
    /// provider, determines what the current line of text being edited should be and where the cursor should be positioned.
    /// It pushes out the text to the provider, which also serves as the view.
    /// </summary>
    public class ReadLineHandler : IKeyBehaviorTarget
    {
        private readonly IReadLineProvider _provider;
        private readonly ReadLineConfig _config;
        private int _cursorPos;
        private int _autoCompleteIndex;
        private TokenizedLine _autoCompleteTokens;
        private bool _autoCompleteCalled = false;
        private string[] _autoCompleteSuggestions;

        private int _historyIndex;
        private LineState _preHistoryState;

        private bool _finishTrigger = false;

        public ReadLineHandler(IReadLineProvider provider, ReadLineConfig config=null)
        {
            _config = config ?? ReadLineConfig.Basic;
            _provider = provider;
            TextBuffer = new StringBuilder();
            _cursorPos = 0;

            _autoCompleteIndex = int.MinValue;
            _autoCompleteSuggestions = null;

            // The history index should start one element past the length of the current history
            _historyIndex = config?.History?.Any() == true ? config.History.Count : 0;
        }

        /// <summary>
        /// Gets the current LineState representation of the text and the cursor position
        /// </summary>
        public LineState LineState => new LineState(TextBuffer.ToString(), _cursorPos);

        /// <inheritdoc />
        public StringBuilder TextBuffer { get; }

        /// <inheritdoc />
        public int CursorPosition
        {
            get => _cursorPos;
            set
            {
                _cursorPos = value;
                if (_cursorPos > TextBuffer.Length)
                    _cursorPos = TextBuffer.Length;
                if (_cursorPos < 0)
                    _cursorPos = 0;
            }
        }

        /// <inheritdoc />
        public ConsoleKeyInfo ReceivedKey { get; private set; }

        /// <inheritdoc />
        public void AutoCompleteNext()
        {
            if (_autoCompleteIndex >= 0)
            {
                // Next index
                _autoCompleteIndex++;
                if (_autoCompleteIndex >= _autoCompleteSuggestions.Length)
                    _autoCompleteIndex = 0;

                this.SetAutoCompleteText();
            }
            else
                this.StartAutoComplete();
        }

        /// <inheritdoc />
        public void AutoCompletePrevious()
        {
            if (_autoCompleteIndex >= 0)
            {
                // Previous index
                _autoCompleteIndex--;
                if (_autoCompleteIndex < 0)
                    _autoCompleteIndex = _autoCompleteSuggestions.Length - 1;

                this.SetAutoCompleteText();
            }
            else 
                this.StartAutoComplete();

        }

        /// <inheritdoc />
        public void InsertText(FormattedText text)
        {
            _provider.InsertText(text);
        }

        /// <inheritdoc />
        public TokenizedLine GetTextTokens()
        {
            return _config.Lexer?.Invoke(this.LineState);
        }

        public void HistoryNext()
        {
            // If there is no history, we don't need to do anything
            if (_config.History?.Any() != true)
                return;

            // If we're at the end of the history (including the entered text) we do nothing
            if (_historyIndex == _config.History.Count)
                return; 

            // Otherwise we increment the history index and set the current text buffer based 
            // on whether or not we still have another history element
            _historyIndex++;
            this.TextBuffer.Clear();
            if (_historyIndex == _config.History.Count)
            {
                this.TextBuffer.Append(_preHistoryState.Text);
                this.CursorPosition = _preHistoryState.Cursor;
            }
            else
            {
                this.TextBuffer.Append(_config.History[_historyIndex]);
                this.CursorPosition = this.TextBuffer.Length;
            }
        }

        public void HistoryPrevious()
        {
            // If there is no history, we don't need to do anything
            if (_config.History?.Any() != true)
                return;

            if (_historyIndex == 0)
                return;

            // Check if we're about to leave entered text to go backwards in the history. If so
            // we want to store it first.
            if (_historyIndex == _config.History.Count)
            {
                _preHistoryState = new LineState(this.LineState.Text, this.LineState.Cursor);
            }

            _historyIndex--;
            this.TextBuffer.Clear();
            this.TextBuffer.Append(_config.History[_historyIndex]);
            this.CursorPosition = this.TextBuffer.Length;
        }

        /// <summary>
        /// Interactively manage the user input of a line of text at the console, returning the contents
        /// of the text when finished.
        /// </summary>
        public string ReadLine()
        {
            // The display must be updated at the beginning if any prompts or other prefix/suffix text
            // is to be displayed 
            this.UpdateDisplay();

            // The main processing loop of the handler, this loop will block until it receives a single key from the
            // console. It will then attempt to look up a key behavior for that key, and if it finds one it will 
            // invoke it, otherwise it will invoke the default behavior if there is one. After that it will check
            // if the condition to finish the input has been set, and if not it will update the display and wait
            // for the next key.
            while (true)
            {
                this.ReceivedKey = _provider.ReadKey();

                // We will need to check if the line state (text & cursor position) is altered by the
                // key behavior which will be run, so we store the current state now
                var previousState = this.LineState;
                _autoCompleteCalled = false;
                
                // See if there's a specific behavior which should be mapped to this key,
                // and if so, run it instead of checking the insert/enter behaviors
                var behavior = this.GetKeyAction(ReceivedKey);
                if (behavior != null)
                {
                    behavior.Invoke(this);
                }
                else
                {
                    _config.DefaultKeyBehavior?.Invoke(this);
                }

                // Check if the Finish behavior was called, indicating that we can exit this method
                // and return the contents of the text buffer to the caller
                if (_finishTrigger)
                    break;

                // If the text contents or the cursor have changed at all, and we weren't currently
                // doing autocomplete, we need to invalidate the auto-completion information
                if ((!previousState.Equals(this.LineState)) && !_autoCompleteCalled)
                    this.InvalidateAutoComplete();

                this.UpdateDisplay();
            }

            // If there is a delegate to update the history, invoke it now
            _config.UpdateHistory?.Invoke(TextBuffer.ToString());

            return TextBuffer.ToString();
        }

        /// <summary>
        /// Updates the display on the underlying provider. This is where any formatter is called, immediately
        /// prior to the display being set.
        /// </summary>
        private void UpdateDisplay()
        {
            // Finally, if we have an available formatter, we can get a display format from here
            var display = new LineDisplayState(string.Empty, TextBuffer.ToString(), string.Empty, _cursorPos);

            if (_config.FormatterFromLine != null)
                display = _config.FormatterFromLine.Invoke(LineState);
            else if (_config.FormatterFromTokens != null && _config.Lexer != null)
                display = _config.FormatterFromTokens(GetTextTokens());

            _provider.SetDisplay(display);
        }

        /// <summary>
        /// Check a ConsoleKeyInfo to see if the configuration object has a behavior registered
        /// for that key. The character is checked first, and if that fails, the ConsoleKey and the modifier keys
        /// are checked. If that fails, null is returned
        /// </summary>
        private Action<IKeyBehaviorTarget> GetKeyAction(ConsoleKeyInfo info)
        {
                var charKey = new KeyId(info.KeyChar);
            if (_config.KeyBehaviors.ContainsKey(charKey))
                return _config.KeyBehaviors[charKey];

            var key = new KeyId(info.Key, (info.Modifiers & ConsoleModifiers.Control) != 0,
                (info.Modifiers & ConsoleModifiers.Alt) != 0, (info.Modifiers & ConsoleModifiers.Shift) != 0);
            if (_config.KeyBehaviors.ContainsKey(key))
                return _config.KeyBehaviors[key];

            return null;
        }

        /// <summary>
        /// Initializes and begins the AutoComplete functionality. AutoComplete is its own special state of interaction,
        /// and must be initialized before the next/previous functions will work. It then can continue until an action
        /// other than next/previous occurs, after which it will have to be reinitialized to be used again. The
        /// initialization process involves fetching the valid suggestions for the currently entered text and cursor
        /// position.
        /// </summary>
        private void StartAutoComplete()
        {
            if (!_config.CanAutoComplete)
                return;

            _autoCompleteTokens = _config.Lexer(new LineState(TextBuffer.ToString(), _cursorPos));
            if (_autoCompleteTokens.CursorToken == null)
                return;

            _autoCompleteSuggestions = _config.AutoCompletion(_autoCompleteTokens) ?? Array.Empty<string>();

            if (_autoCompleteTokens.Text != TextBuffer.ToString())
            {
                TextBuffer.Clear();
                TextBuffer.Append(_autoCompleteTokens.Text);
                CursorPosition = _autoCompleteTokens.Cursor;
            }

            if (_autoCompleteSuggestions.Any())
            {
                _autoCompleteIndex = 0;
                SetAutoCompleteText();
            }

        }

        /// <summary>
        /// If AutoComplete is currently active, the only allowable actions are next/previous. If any other
        /// modification is made to the line, the current AutoComplete suggestions are invalidated and any attempt
        /// to use the next/previous functionality will require a reinitialization of the AutoComplete mechanism.
        /// </summary>
        private void InvalidateAutoComplete()
        {
            _autoCompleteIndex = Int32.MinValue;
            _autoCompleteTokens = null;
            _autoCompleteSuggestions = null;
        }

        /// <summary>
        /// Inserts the text from the currently selected auto complete suggestion into the token under the cursor. Only
        /// works if the system is currently in autocomplete mode.
        /// </summary>
        private void SetAutoCompleteText()
        {
            if (!_config.CanAutoComplete || _autoCompleteTokens == null || _autoCompleteIndex < 0)
                return;

            _autoCompleteCalled = true;
            _autoCompleteTokens.CursorToken.Text = _autoCompleteSuggestions[_autoCompleteIndex];
            _autoCompleteTokens.CursorToken.Cursor = _autoCompleteTokens.CursorToken.Text.Length;

            TextBuffer.Clear();
            TextBuffer.Append(_autoCompleteTokens.Text);
            CursorPosition = _autoCompleteTokens.Cursor;

        }

        /// <summary>
        /// Causes the ReadLine handler to finish, returning the contents of the text buffer
        /// </summary>
        public void Finish() => _finishTrigger = true;
    }
}