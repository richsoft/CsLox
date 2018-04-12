using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsLox.Scanning;
using CsLox.Tokens;


namespace CsLox
{
    class CsLox
    {
        static Boolean _had_error = false;

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

            // If there was an error, exit with a error code
            if (_had_error)
            {
                Environment.Exit(64);
            }
        }

        /// <summary>
        /// Set up a REPL prompt loop
        /// </summary>
        private static void RunPrompt()
        {
            while(true)
            {
                Console.Write("> ");
                Run(Console.ReadLine());

                // Reset the error flag
                _had_error = false;
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

            // Print the tokens for now
            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }

        }

        /// <summary>
        /// Raise an error message
        /// </summary>
        /// <param name="line">The line number</param>
        /// <param name="message">The error message</param>
        public static void Error(int line, string message)
        {
            Report(line, "", message);
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
