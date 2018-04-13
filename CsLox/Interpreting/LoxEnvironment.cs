using CsLox.Collections;
using CsLox.Exceptions;
using CsLox.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.Interpreting
{
    class LoxEnvironment
    {

        private readonly LoxEnvironment _enclosing;
        private readonly HashMap<string, object> _values = new HashMap<string, object>();

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
                _values.Put(name, value);
        }

        /// <summary>
        /// Get a variable value
        /// </summary>
        /// <param name="name">The name token</param>
        /// <returns></returns>
        public object Get(Token name)
        {

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
        /// Get a variable from the given scope depth
        /// </summary>
        /// <param name="distance">The number of scopes to tranverse</param>
        /// <param name="name">The variable name</param>
        /// <returns></returns>
        public object GetAt(int distance, string name)
        {
            return Ancestor(distance)._values.Get(name);
        }

        /// <summary>
        /// Assign a value to a variable
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

        /// <summary>
        /// Assign a value to a variable at a given scope depth
        /// </summary>
        /// <param name="distance">The numbe of scopes to tranverse</param>
        /// <param name="name">The variable name</param>
        /// <param name="value">The value</param>
        public void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance)._values.Put(name.Lexeme, value);
        }

        private LoxEnvironment Ancestor(int distance)
        {
            LoxEnvironment environment = this;
            for (int i =0; i < distance; i++)
            { 
                environment = environment._enclosing;
            }

            return environment;
        }

    }
}
