using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;

namespace InteractiveReadLine.Tokenizing
{
    /// <summary>
    /// The TokenizedLine class represents a line of text processed by a lexer and split into a sequence of
    /// tokens.
    /// </summary>
    /// <remarks>
    /// These tokens in order contain every character of text in the original line, such that combining
    /// them in sequence would recreate the original string. Additionally, the line's original cursor position
    /// is preserved and contained by one of the tokens. The tokens can be walked in its entirety in either
    /// direction by navigation properties on the tokens themselves, or iterated through in the line.
    /// </remarks>
    public class TokenizedLine : IReadOnlyList<IToken>
    {
        private readonly List<Token> _tokens;
        private int _cursor;

        public TokenizedLine()
        {
            _tokens = new List<Token>();
        }

        /// <summary>
        /// Gets the first token in the sequence, or null if the sequence is empty
        /// </summary>
        public IToken First => _tokens.FirstOrDefault();

        /// <summary>
        /// Gets the last token in the sequence, or null if the sequence is empty
        /// </summary>
        public IToken Last => _tokens.LastOrDefault();

        /// <summary>
        /// Gets the first non-hidden token in the sequence, or null if none exist
        /// </summary>
        public IToken FirstNonHidden => _tokens.FirstOrDefault()?.FirstNonHidden;

        /// <summary>
        /// Gets the combined text of all of the tokens in the sequence, which will match the original line of text
        /// before it was split apart by the lexer
        /// </summary>
        public string Text => Token.BuildText(_tokens);

        /// <summary>
        /// Gets an enumerator which can be used to iterate through the tokens
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IToken> GetEnumerator() => _tokens.GetEnumerator();

        /// <summary>
        /// Gets an enumerator which can be used to iterate through the tokens
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets the number of tokens in the sequence
        /// </summary>
        public int Count => _tokens.Count;

        /// <summary>
        /// Gets the token at the specified index in the sequence
        /// </summary>
        /// <param name="index">Index in the token sequence</param>
        /// <returns></returns>
        public IToken this[int index] => _tokens[index];

        /// <summary>
        /// Gets the token which currently contains the cursor
        /// </summary>
        public IToken CursorToken => _tokens.FirstOrDefault(x => x.Cursor != null);

        /// <summary>
        /// Gets the index number of the token which currently contains the cursor
        /// </summary>
        public int CursorTokenIndex => _tokens.IndexOf(_tokens.FirstOrDefault(x => x.Cursor != null));

        /// <summary>
        /// Gets the overall index of the cursor in the combined text
        /// </summary>
        public int Cursor
        {
            get => _cursor;
            set
            {
                _cursor = value;
                
                int textLen = this.Text.Length;
                if (value > textLen)
                    _cursor = textLen;
            }
        }

        /// <summary>
        /// Add a token to the sequence, making all of the necessary links
        /// </summary>
        /// <param name="text">The text contained by the token</param>
        /// <param name="isHidden">Whether or not this is a hidden token</param>
        /// <param name="cursor">Null if the token does not contain the cursor, otherwise the index of the cursor as an offset from the first character in the token text</param>
        /// <param name="typeCode">An optional type code which can be accessed by a consumer of the token</param>
        public void Add(string text, bool isHidden, int? cursor=null, int typeCode=0)
        {
            var newToken = new Token(text, isHidden, this, typeCode);

            if (_tokens.Any())
            {
                _tokens.Last().Next = newToken;
                newToken.Previous = _tokens.Last();
            }

            _tokens.Add(newToken);

            if (cursor != null)
                newToken.Cursor = cursor;
        }

        /// <summary>
        /// Internal implementation of the token
        /// </summary>
        /// 
        /// <remarks>
        /// Tokens and the TokenizedLine require a lot of internal plumbing to work correctly, and don't necessarily
        /// have a meaning or purpose except in conjunction with each other.  To make sure that the TokenizedLine.Add
        /// method is the obvious way of producing tokens and connecting them, and that tokens are not created
        /// independent from a TokenizedLine, this class is hidden from consumers and exposed only through the
        /// IToken interface.  
        /// </remarks>
        private class Token : IToken
        {
            private readonly TokenizedLine _parent;
            private string _text;

            public Token(string text, bool isHidden, TokenizedLine parent, int typeCode)
            {
                _parent = parent;
                TypeCode = typeCode;
                _text = text;
                this.IsHidden = isHidden;
            }

            public int TypeCode { get; }

            public string Text
            {
                get => _text;
                set
                {
                    int? cursorMove = null;

                    // Adjust the cursor if the cursor lies in this token
                    if (Cursor != null && Cursor > value.Length)
                    {
                        Cursor = value.Length;
                    }
                    else
                    {
                        // Check if the cursor is after this token, and if so adjust it accordingly
                        var beforeLen = TextBefore().Length;
                        if (beforeLen + _text.Length - 1 < _parent.Cursor)
                        {
                            var delta = value.Length - _text.Length;
                            cursorMove = _parent.Cursor + delta;
                        }
                    }

                    _text = value;
                    if (cursorMove != null)
                        _parent.Cursor = (int) cursorMove;
                }
            }

            public int? Cursor
            {
                get
                {
                    int lengthBefore = this.TextBefore().Length;
                    if (_parent.Cursor < lengthBefore)
                        return null;

                    int offset = _parent.Cursor - lengthBefore;

                    if (offset == Text.Length && Next?.IsHidden == false)
                    {
                        return null;
                    }

                    if (offset <= Text.Length)
                        return offset;
                    return null;
                }
                set
                {
                    if (value == null || value < 0 || value > Text.Length)
                        return;

                    _parent.Cursor = this.TextBefore().Length + (int) value;
                }
            }

            public Token Next { get; set; }
            public Token Previous { get; set; }
            
            public IToken PreviousNotHidden => this.Previous?.ThisOrPrevIfHidden();
            public IToken NextNotHidden => this.Next?.ThisOrNextIfHidden();

            IToken IToken.Next => this.Next;

            IToken IToken.Previous => this.Previous;

            public Token First => this.Previous == null ? this : this.Previous.First;

            public Token FirstNonHidden => this.First.ThisOrNextIfHidden();

            public bool IsHidden { get; set; }

            public int? DistanceTo(IToken other, bool ignoreHidden = false)
            {
                var token = other as Token;
                if (token == null)
                    return null;

                var gap = Next?.ForwardTo(token, new List<Token>());
                if (ignoreHidden)
                    return gap?.Where(x => !x.IsHidden).Count();
                else 
                    return gap?.Count;
            }

            private List<Token> ForwardTo(Token other, List<Token> gap)
            {
                gap.Add(this);

                if (other == this)
                    return gap;
                else
                {
                    return this.Next?.ForwardTo(other, gap);
                }
            }

            private Token[] TokensBefore()
            {
                var tokens = new List<Token>();
                var first = this.First;

                var pointer = first;
                while (pointer != this)
                {
                    tokens.Add(pointer);
                    pointer = pointer.Next;
                }

                return tokens.ToArray();
            }

            private string TextBefore()
            {
                return BuildText(this.TokensBefore());
            }

            private Token ThisOrNextIfHidden()
            {
                if (this.IsHidden)
                    return this.Next?.ThisOrNextIfHidden();
                else
                    return this;
            }

            private Token ThisOrPrevIfHidden()
            {
                if (this.IsHidden)
                    return this.Previous?.ThisOrPrevIfHidden();
                else
                    return this;
            }

            public static string BuildText(IEnumerable<Token> tokens)
            {
                var builder = new StringBuilder();
                foreach (var token in tokens)
                {
                    builder.Append(token.Text);
                }

                return builder.ToString();
            }
        }
    }
}