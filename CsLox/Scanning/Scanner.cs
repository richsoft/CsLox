using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsLox.Tokens;
using CsLox.ExtensionMethods;

namespace CsLox.Scanning
{
    class Scanner
    {
        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        private static readonly Dictionary<string, TokenType> _keywords =
            new Dictionary<string, TokenType>() {
                {"and", TokenType.AND },
                {"break", TokenType.BREAK },
                {"class", TokenType.CLASS },
                {"else", TokenType.ELSE },
                {"false", TokenType.FALSE },
                {"for", TokenType.FOR },
                {"fun", TokenType.FUN },
                {"if", TokenType.IF },
                {"nil", TokenType.NIL },
                {"or", TokenType.OR },
                {"print", TokenType.PRINT },
                {"return", TokenType.RETURN },
                {"super", TokenType.SUPER },
                {"this", TokenType.THIS },
                {"true", TokenType.TRUE },
                {"var", TokenType.VAR },
                {"while", TokenType.WHILE }
            };

        /// <summary>
        /// Create a new instance of the scanner
        /// </summary>
        /// <param name="source">The source</param>
        public Scanner(string source)
        {
            this._source = source;
        }

        /// <summary>
        /// Scan the source and create the token list
        /// </summary>
        /// <returns>The list of tokens representing the source</returns>
        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are the beginning of the next lexeme
                _start = _current;
                ScanToken();
            }

            // Add the EOF token
            _tokens.Add(new Token(TokenType.EOF, "", null, _line));

            return _tokens;
        }

        /// <summary>
        /// Scan for a new token
        /// </summary>
        private void ScanToken()
        {
            char c = Advance();

            switch (c)
            {
                // 1 char lexemes
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;

                // 2 char lexemes
                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;

                // Slash could be a comment
                case '/':
                    if (Match('/'))
                    {
                        // A comment continues to the end of the line
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;


                // Whitespace (fall-through)
                case ' ':
                case '\r':
                case '\t':
                    // Ignore
                    break;

                // New Line
                case '\n':
                    _line++;
                    break;

                // String
                case '"': String(); break;



                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        CsLox.Error(_line, $"Unexpected character: '{c}'.");
                    }
                    break;
            }


        }

        /// <summary>
        /// Consume an identifier
        /// </summary>
        private void Identifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }

            // Check if this is a keyword
            string text = _source.Slice(_start, _current);
            TokenType type = TokenType.IDENTIFIER;
            if (_keywords.TryGetValue(text, out TokenType kw_type)) {
                type = kw_type;
            }

            AddToken(type);
        }


        /// <summary>
        /// Consume a number literal
        /// </summary>
        private void Number()
        {
            // Loop while we are finding digits
            while (IsDigit(Peek()))
            {
                Advance();
            }

            // Look for a decimal point
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the .
                Advance();

                // Consume the digits
                while (IsDigit(Peek()))
                {
                    Advance();
                }

            }

            // Add the number token
            double value = double.Parse(_source.Slice(_start, _current));
            AddToken(TokenType.NUMBER, value);

        }


        /// <summary>
        /// Consume a string literal
        /// </summary>
        private void String()
        {
            // Look for the end of the string
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    // Strings can span lines, so keep track
                    _line++;
                }
                Advance();
            }

            // Check the string was terminated
            if (IsAtEnd())
            {
                CsLox.Error(_line, "Unterminated string.");
                return;
            }

            // Consume the closing double quote
            Advance();

            // Find the value, cutting off the quotes
            string value = _source.Slice(_start + 1, _current - 1);

            AddToken(TokenType.STRING, value);
        }

        /// <summary>
        /// Check if a character is alpha (a-z, A-Z, or _)
        /// </summary>
        /// <param name="c">The character to test</param>
        /// <returns>True if the character is alpha</returns>
        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        }


        /// <summary>
        /// Check if a character is alphanumeric (0-9, a-z, A-Z, or _)
        /// </summary>
        /// <param name="c">The character to test</param>
        /// <returns>True if the character is alphanumeric</returns>
        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }


        /// <summary>
        /// Check if a character is a digit (0-9)
        /// </summary>
        /// <param name="c">The character to test</param>
        /// <returns>True, if the char is a digit</returns>
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }


        /// <summary>
        /// Check if the current character macthes the expected character (consumes it if it does)
        /// </summary>
        /// <param name="expected">The expected character</param>
        /// <returns>True if the character matches</returns>
        private bool Match(char expected)
        {
            if (IsAtEnd())
            {
                // Run out of chars
                return false;
            }
            else if (_source[_current] != expected)
            {
                // Not the expected char
                return false;
            }

            _current++;
            return true;

        }

        /// <summary>
        /// Consume one character in the source
        /// </summary>
        /// <returns>The next character</returns>
        private char Advance()
        {
            _current++;
            return _source[_current - 1];
        }

        /// <summary>
        /// Lookahead a character, but DO NOT consume it
        /// </summary>
        /// <returns>The character, or \0 if end of the source</returns>
        private char Peek()
        {
            if (IsAtEnd())
            {
                // We have run out of characters
                return '\0';
            }

            return _source[_current];
        }

        /// <summary>
        /// Lookahead a second charactet, but DO NOT consume it
        /// </summary>
        /// <returns>The character, or \0 if end of the source</returns>
        private char PeekNext()
        {
            // Check we are not trying to read off the end of the source
            if (_current + 1 >= _source.Length)
            {
                return '\0';
            }

            return _source[_current + 1];
        }

        /// <summary>
        /// Add a new token
        /// </summary>
        /// <param name="type">The token type</param>
        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        /// <summary>
        /// Add a new token
        /// </summary>
        /// <param name="type">The token type</param>
        /// <param name="literal">The literal value</param>
        private void AddToken(TokenType type, object literal)
        {
            string text = _source.Slice(_start, _current);
            _tokens.Add(new Token(type, text, literal, _line));
        }

        /// <summary>
        /// Check if the end of the source has been reached
        /// </summary>
        /// <returns></returns>
        private bool IsAtEnd()
        {
            // Guard against null source
            if (_source == null)
            {
                return true;
            }

            return _current >= _source.Length;
        }

    }
}
