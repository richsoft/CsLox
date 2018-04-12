using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.Tokens
{
    class Token
    {
        private readonly TokenType _type;
        private readonly string _lexeme;
        private readonly object _literal;
        private readonly int _line;

        // Add readonly properties
        public TokenType Type => _type;
        public string Lexeme => _lexeme;
        public object Literal => _literal;
        public int Line => _line;

        /// <summary>
        /// Create a new token instance
        /// </summary>
        /// <param name="type">The token type</param>
        /// <param name="lexeme">The lexeme</param>
        /// <param name="literal">The literal value</param>
        /// <param name="line">The line in the source</param>
        public Token(TokenType type, string lexeme, object literal, int line)
        {
            this._type = type;
            this._lexeme = lexeme;
            this._literal = literal;
            this._line = line;
        }

        public override string ToString()
        {
            return $"{_type} {_lexeme} {_literal}";
        }
    }
}
