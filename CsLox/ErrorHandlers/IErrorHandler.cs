using CsLox.Exceptions;
using CsLox.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.ErrorHandlers
{
    interface IErrorHandler
    {
        bool SyntaxError { get; }
        bool RuntimeError { get; }

        void Reset();
        void Error(int line, string message);
        void Error(Token token, string message);
        void Error(RuntimeErrorException error);

    }
}
