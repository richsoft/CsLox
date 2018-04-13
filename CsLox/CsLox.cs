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
using CsLox.Interpreting;

namespace CsLox
{
    class CsLox
    {
        private static readonly Interpreter _interpreter = new Interpreter();

        static bool _had_error = false;
        static bool _had_runtime_error = false;

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
            if (_had_error) Environment.Exit(64);
            if (_had_runtime_error) Environment.Exit(70);
        }

        /// <summary>
        /// Set up a REPL prompt loop
        /// </summary>
        private static void RunPrompt()
        {
            bool stop = false;

            Console.CancelKeyPress += (sender, e) =>
            {
                stop = true;
                e.Cancel = true;
            };

            while (!stop)
            {
                Console.Write("> ");
                Run(Console.ReadLine());

                // Reset the error flag
                _had_error = false;
                _had_runtime_error = false;
            }
        }

        /// <summary>
        /// Core run function
        /// </summary>
        /// <param name="source">The source to run</param>
        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.Parse();

            if (_had_error) return;

            _interpreter.Interpret(statements);

        }

        /// <summary>
        /// Log a scanning error
        /// </summary>
        /// <param name="line">The line number</param>
        /// <param name="message">The error message</param>
        public static void Error(int line, string message)
        {

            Report(line, "", message);
        }

        /// <summary>
        /// Log a parsing error
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="message">The error message</param>
        public static void ParseError(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $" at '{token.Lexeme}'", message);
            }

            _had_error = true;
        }

        public static void RuntimeError(RuntimeErrorException error)
        {
            Console.Error.WriteLine(error.Message);
            Console.Error.WriteLine($"[line {error.Token.Line}]");

            _had_runtime_error = true;

        }

        /// <summary>
        /// Print an error message
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="where">Location</param>
        /// <param name="message">The message</param>
        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine("[line {0}] Error {1}: {2}", line, where, message);
        }
    }
}
