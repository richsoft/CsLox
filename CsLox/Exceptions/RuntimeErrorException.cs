using CsLox.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.Exceptions
{
    class RuntimeErrorException : Exception
    {
        public Token Token { get; }

        public RuntimeErrorException(Token token, string message): base(message)
        {
            this.Token = token;
        }

    }
}
