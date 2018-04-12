using CsLox.Exceptions;
using CsLox.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.Environments
{
    class LoxEnvironment
    {

        private readonly LoxEnvironment _enclosing;
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public LoxEnvironment()
        {
            _enclosing = null;
        }

        public LoxEnvironment(LoxEnvironment enclosing)
        {
            _enclosing = enclosing;
        }


        /// <summary>
        /// Define a variable
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="value">The value</param>
        public void Define(string name, object value)
        {
            // Replace an existing value
            if (_values.ContainsKey(name))
            {
                _values[name] = value; 
            }
            else
            {
                _values.Add(name, value);
            }
        }

        /// <summary>
        /// Get a variable value
        /// </summary>
        /// <param name="name">The name token</param>
        /// <returns></returns>
        public object Get(Token name)
        {
            // Return null if not in the dictionary

            if (_values.TryGetValue(name.Lexeme, out object value)) {
                return value;
            }

            // Check the parent environment
            if (_enclosing != null)
            {
                return _enclosing.Get(name);
            }

            throw new RuntimeErrorException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        /// <summary>
        /// Assign a new vakue to a variable
        /// </summary>
        /// <param name="name">The name token</param>
        /// <param name="value">The value to assign</param>
        public void Assign(Token name, object value)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return;
            }

            // Check the parent
            if (_enclosing != null)
            {
                _enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeErrorException(name, $"Undefined variable '{name.Lexeme}'.");
        }


    }
}
