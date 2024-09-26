namespace InteractiveReadLine.Tokenizing
{
    /// <summary>
    /// A contract for a single token produced from a line of text by a lexer. 
    /// </summary>
    /// 
    /// <remarks>
    /// A lexer takes a string and splits it into a sequence of tokens. In order for the autocomplete
    /// mechanism to work, there are additional constraints placed on lexers and tokens in the InteractiveReadLine
    /// codebase that might not be necessary for other applications.
    ///
    /// Any implementation of IToken must make sure to properly implement the four different Previous/Next
    /// links, meaning that the lexer itself must connect the token objects such that the entire sequence
    /// of tokens can be walked by following these links from beginning to end and back again.
    ///
    /// Additionally, the Cursor property setter must ensure that setting the cursor on one token
    /// will remove it from the previous token in the sequence on which it resided. 
    /// 
    /// </remarks>
    public interface IToken
    {
        /// <summary>
        /// Gets or sets the text of this particular token. If the cursor is currently on this token, the cursor
        /// will be adjusted if necessary if the length of the text changes. 
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the cursor position as an offset from the first character in the the Text property. Setting
        /// the cursor on this token will remove it from any other token in the sequence.
        /// </summary>
        int? Cursor { get; set; }

        /// <summary>
        /// Gets the next token in the sequence, regardless of whether the next token is hidden or not. Returns null
        /// if this is the last token in the sequence.
        /// </summary>
        IToken Next { get; }

        /// <summary>
        /// Gets the previous token in the sequence, regardless of whether the previous token is hidden or not. Returns
        /// null if this is the first token in the sequence.
        /// </summary>
        IToken Previous { get; }

        /// <summary>
        /// Gets the next token in the sequence, ignoring hidden tokens. Returns null if this is the last non-hidden
        /// token in the sequence.
        /// </summary>
        IToken NextNotHidden { get; }

        /// <summary>
        /// Gets the previous token in the sequence, ignoring hidden tokens. Returns null if this is the first non-hidden
        /// token in the sequence.
        /// </summary>
        IToken PreviousNotHidden { get; }

        /// <summary>
        /// Gets a value indicating whether or not this token is considered a hidden (grammatically meaningless) token. An
        /// example of a typical hidden token in most grammars is whitespace.
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Determines the distance between two tokens as the number of links that must be followed to get from this token
        /// to the other.  Adjacent tokens, for example, have a distance of 1. Returns null if the two tokens are not in
        /// the same sequence.
        /// </summary>
        /// <param name="other">Another token in the same sequence to measure the distance to</param>
        /// <param name="ignoreHidden">Specifies whether or not hidden tokens count as part of the distance</param>
        /// <returns>The distance between tokens, or null if the distance cannot be found</returns>
        int? DistanceTo(IToken other, bool ignoreHidden=false);

        /// <summary>
        /// Gets the integer type code associated with this token. The lexer has the option of assigning an integer type code
        /// to tokens, typically with some semantic meaning known by the lexer as it processed the text. If so, that value
        /// it assigned is available here.
        /// </summary>
        int TypeCode { get; }
    }
}