using CsLox.Exceptions;
using CsLox.SyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.Interpreting
{
    class LoxFunction : ILoxCallable
    {
        private readonly Stmt.Function _declaration;
        private readonly LoxEnvironment _closure;

        public int Arity => _declaration.Parameters.Count();

        public LoxFunction (Stmt.Function declaration, LoxEnvironment closure)
        {
            _declaration = declaration;
            _closure = closure;

        }

        public object Call(Interpreter interpreter, IList<object> arguments)
        {
            // Environment
            LoxEnvironment environment = new LoxEnvironment(_closure);

            // Arguments
            for (int i = 0; i < _declaration.Parameters.Count(); i++)
            {
                environment.Define(_declaration.Parameters[i].Lexeme, arguments[i]);
            }

            // Execute
            try
            {
                interpreter.ExecuteBlock(_declaration.Body, environment);
            }
            catch (ReturnException return_value)
            {
                // We hit a return statement
                return return_value.Value;
            }
            
            return null;
        }

        public override string ToString()
        {
            return $"<fn {_declaration.Name.Lexeme}>";
        }

    }
}
