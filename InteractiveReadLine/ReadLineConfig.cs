using System;
using System.Collections.Generic;
using System.Text;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tokenizing;

namespace InteractiveReadLine
{
    /// <summary>
    /// The main configuration object for the ReadLineHandler, instances of this class carry all of the objects
    /// which customize and control every aspect of the ReadLineHandler's behavior. Many standard behaviors are
    /// provided through the CommonFormatters/CommonKeyBehaviors/CommonLexers tools, but custom behaviors can
    /// be attached to this object instead.
    /// </summary>
    public class ReadLineConfig
    {
        private ReadLineConfig()
        {
            this.KeyBehaviors = new Dictionary<KeyId, Action<IKeyBehaviorTarget>>();
        }

        /// <summary>
        /// Gets an Action which can be used to update the history. This is automatically set when the
        /// 
        /// </summary>
        public Action<string> UpdateHistory { get; private set; }

        /// <summary>
        /// Gets a list which contains the history of entered text, used for any behaviors which interact
        /// with the entered history.
        /// </summary>
        public IReadOnlyList<string> History { get; private set; }

        /// <summary>
        /// Gets a providing function used to format the line to display based on a tokenization of the
        /// readline content just before display. Requires a Lexer to work.
        /// </summary>
        public Func<TokenizedLine, LineDisplayState> FormatterFromTokens { get; private set; }

        /// <summary>
        /// Gets a format providing method which should format the line based on the raw LineState
        /// </summary>
        public Func<LineState, LineDisplayState> FormatterFromLine { get; private set; }

        /// <summary>
        /// Gets a dictionary which maps key press information to key behavior methods
        /// </summary>
        public Dictionary<KeyId, Action<IKeyBehaviorTarget>> KeyBehaviors { get; }

        /// <summary>
        /// Gets the default key behavior, which is applied if no other key behavior is first located by
        /// the KeyBehaviors dictionary
        /// </summary>
        public Action<IKeyBehaviorTarget> DefaultKeyBehavior { get; private set; }
        
        /// <summary>
        /// Gets the lexer for the readline handler to use, which tokenizes a LineState object.
        /// A non-null lexer is a critical component of auto-completion and certain token-based key behaviors 
        /// </summary>
        public Func<LineState, TokenizedLine> Lexer { get; private set; }

        /// <summary>
        /// Gets the auto-completion provider, which is a method that takes a TokenizedLine object
        /// and returns a list of suggestions for the token under the cursor. The TokenizedLine can
        /// also be modified by the auto-completion method.
        /// </summary>
        public Func<TokenizedLine, string[]> AutoCompletion { get; private set; }

        /// <summary>
        /// Gets whether the configuration is capable of auto-completion, which requires both a Lexer
        /// and an auto-completion handler.
        /// </summary>
        public bool CanAutoComplete => this.Lexer != null && this.AutoCompletion != null;

        /// <summary>
        /// Sets the auto-completion handler for the configuration
        /// </summary>
        public ReadLineConfig SetAutoCompletion(Func<TokenizedLine, string[]> handler)
        {
            this.AutoCompletion = handler;
            return this;
        }

        /// <summary>
        /// Sets the lexer for the configuration
        /// </summary>
        public ReadLineConfig SetLexer(Func<LineState, TokenizedLine> lexer)
        {
            this.Lexer = lexer;
            return this;
        }

        /// <summary>
        /// Sets the formatter for the configuration using a formatter based on a tokenized line. This will
        /// require that a lexer be provided for the configuration.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public ReadLineConfig SetFormatter(Func<TokenizedLine, LineDisplayState> formatter)
        {
            this.FormatterFromLine = null;
            this.FormatterFromTokens = formatter;
            return this;
        }

        /// <summary>
        /// Sets the formatter for the configuration using a formatter which only needs a simple LineState.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public ReadLineConfig SetFormatter(Func<LineState, LineDisplayState> formatter)
        {
            this.FormatterFromLine = formatter;
            this.FormatterFromTokens = null;
            return this;
        }

        /// <summary>
        /// Sets the collection of previously entered lines to be used as the command history. Use this setter
        /// if you do not want the history to be automatically updated. Also, make sure to set some key behavior
        /// which will make use of the history.
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public ReadLineConfig SetHistorySource(IReadOnlyList<string> history)
        {
            this.History = history;
            return this;
        }

        /// <summary>
        /// Sets the collection of previously entered lines to be used as the command history. This setter will
        /// cause the history list to be automatically updated. Also, make sure to set some key behavior which
        /// will make use of the history.
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public ReadLineConfig SetUpdatingHistorySource(List<string> history)
        {
            this.SetHistoryUpdateAction(history.Add);
            return this.SetHistorySource(history);
        }

        /// <summary>
        /// Sets the action delegate which will get call when the readline is finalized and the history would otherwise
        /// update itself.  Use this to provide a custom update action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ReadLineConfig SetHistoryUpdateAction(Action<string> action)
        {
            this.UpdateHistory = action;
            return this;
        }

        /// <summary>
        /// Set the default KeyBehavior which is called when no other special action for a key has been registered
        /// in the configuration. Typically this will be used to insert the key character into the input line.
        /// </summary>
        /// <param name="defaultBehavior"></param>
        /// <returns></returns>
        public ReadLineConfig SetDefaultKeyBehavior(Action<IKeyBehaviorTarget> defaultBehavior)
        {
            this.DefaultKeyBehavior = defaultBehavior;
            return this;
        }


        /// <summary>
        /// Gets an empty configuration with absolutely nothing set
        /// </summary>
        public static ReadLineConfig Empty => new ReadLineConfig();

        /// <summary>
        /// Gets a configuration that has the standard key behaviors added to it 
        /// </summary>
        public static ReadLineConfig Basic => new ReadLineConfig().AddStandardKeys();

    }
}