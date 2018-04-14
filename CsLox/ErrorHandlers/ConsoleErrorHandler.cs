using CsLox.Exceptions;
using CsLox.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.ErrorHandlers
{
    class ConsoleErrorHandler : IErrorHandler
    {
        public bool SyntaxError { get; private set; }
        public bool RuntimeError { get; private set; }

        /// <summary>
        /// Log a scanning error
        /// </summary>
        /// <param name="line">The line number</param>
        /// <param name="message">The error message</param>
        public void Error(int line, string message)
        {

            Report(line, "", message);
        }

        /// <summary>
        /// Log a parsing error
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="message">The error message</param>
        public void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, "at end", message);
            }
            else
            {
                Report(token.Line, $"at '{token.Lexeme}'", message);
            }

            this.SyntaxError = true;
        }

        public void Error(RuntimeErrorException error)
        {
            
            if (error.Token.Type == TokenType.EOF)
            {
                Report(error.Token.Line, "at end", error.Message);
            }
            else
            {
                Report(error.Token.Line, $"at '{error.Token.Lexeme}'", error.Message);
            }

            this.RuntimeError = true;

        }

        /// <summary>
        /// Reset the error flags
        /// </summary>
        public void Reset()
        {
            this.SyntaxError = false;
            this.RuntimeError = false;
        }

        /// <summary>
        /// Print an error message
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="where">Location</param>
        /// <param name="message">The message</param>
        private void Report(int line, string where, string message)
        {
            Console.Error.WriteLine("[line {0}] Error {1}: {2}", line, where, message);
        }
    }

}
