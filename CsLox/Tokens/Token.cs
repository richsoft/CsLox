namespace CsLox.Tokens
{
    internal class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public object Literal { get; }
        public int Line { get; }

        /// <summary>
        /// Create a new token instance
        /// </summary>
        /// <param name="type">The token type</param>
        /// <param name="lexeme">The lexeme</param>
        /// <param name="literal">The literal value</param>
        /// <param name="line">The line in the source</param>
        public Token(TokenType type, string lexeme, object literal, int line)
        {
            this.Type = type;
            this.Lexeme = lexeme;
            this.Literal = literal;
            this.Line = line;
        }

        public override string ToString()
        {
            return $"{this.Type} {this.Lexeme} {this.Literal}";
        }
    }
}