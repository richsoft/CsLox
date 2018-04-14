using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsLox.Scanning;
using CsLox.Tokens;
using CsLox.SyntaxTree;
using CsLox.Parsing;
using CsLox.Exceptions;
using CsLox.Runtime;
using CsLox.ErrorHandlers;
using System.Threading;

namespace CsLox
{
    class CsLox
    {
        private static readonly Interpreter _interpreter;
        private static readonly IErrorHandler _error_handler;

        static CsLox()
        {
            _error_handler = new ConsoleErrorHandler();
            _interpreter = new Interpreter(_error_handler);
        }


        static void Main(string[] args)
        {

            if (args.Length > 1)
            {
                Console.WriteLine("Usage CsLox [script]");
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }

        }

        /// <summary>
        /// Load a source file to run
        /// </summary>
        /// <param name="path">The path to the file</param>
        private static void RunFile(string path)
        {
            string source = File.ReadAllText(path);
            Run(source);

            Console.ReadLine();

            // If there was an error, exit with a error code
            if (_error_handler.SyntaxError) Environment.Exit(64);
            if (_error_handler.RuntimeError) Environment.Exit(70);
        }

        /// <summary>
        /// Set up a REPL prompt loop
        /// </summary>
        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");

                string code = Console.ReadLine();

                if (code == null) break;

                Run(code);

                // Reset the error flag
                _error_handler.Reset();
            }
        }

        /// <summary>
        /// Core run function
        /// </summary>
        /// <param name="source">The source to run</param>
        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source, _error_handler);
            List<Token> tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens, _error_handler);
            List<Stmt> statements = parser.Parse();

            if (_error_handler.SyntaxError) return;

            Resolver resolver = new Resolver(_interpreter, _error_handler);
            resolver.Resolve(statements);

            if (_error_handler.SyntaxError) return;

            _interpreter.Interpret(statements);

        }

    }
}
