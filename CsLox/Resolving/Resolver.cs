using CsLox.Collections;
using CsLox.Interpreting;
using CsLox.SyntaxTree;
using CsLox.Tokens;
using CsLox.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.Resolving
{
    class Resolver : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        private readonly Interpreter _interpeter;
        private readonly StackList<HashMap<string, bool>> _scopes = new StackList<HashMap<string, bool>>();

        private FunctionType _current_function = FunctionType.NONE;

        public Resolver(Interpreter interpreter)
        {
            _interpeter = interpreter;
        }

        /// <summary>
        /// Resolve the scope for a block
        /// </summary>
        /// <param name="stmt">The block statement</param>
        public object Visit(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();
            return null;
        }

        /// <summary>
        /// Resolve a variable declaration
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object Visit(Stmt.VarDeclaration stmt)
        {
            Declare(stmt.Name);
            if (stmt.Initializer != null)
            {
                Resolve(stmt.Initializer);
            }
            Define(stmt.Name);
            return null;

        }

        /// <summary>
        /// Resolve a variable expression
        /// </summary>
        /// <param name="expr">The expression</param>
        public object Visit(Expr.Variable expr)
        {
            if (_scopes.IsEmpty() && _scopes.Peek().Get(expr.Name.Lexeme) == false)
            {
                CsLox.Error(expr.Name, "Cannot read local variable in its own initializer.");
            }
            ResolveLocal(expr, expr.Name);

            return null;

        }


        /// <summary>
        /// Resolve a assignement
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns></returns>
        public object Visit(Expr.Assign expr)
        {
            Resolve(expr.value);
            ResolveLocal(expr, expr.Name);
            return null;

        }

        /// <summary>
        /// Bind and resolve a function declaration
        /// </summary>
        /// <param name="stmt">The statement</param>
        public object Visit(Stmt.Function stmt)
        {
            // Declare and define the function name
            Declare(stmt.Name);
            Define(stmt.Name);

            ResolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }

        /// <summary>
        /// Resolve a statement expression
        /// </summary>
        /// <param name="stmt">The statement</param>
        /// <returns></returns>
        public object Visit(Stmt.ExpressionStatement stmt)
        {
            Resolve(stmt.Expression);
            return null;
        }

        /// <summary>
        /// Resolve an if statement
        /// </summary>
        /// <param name="stmt">The statement</param>
        /// <returns></returns>
        public object Visit(Stmt.If stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null)
            {
                Resolve(stmt.ElseBranch);
            }

            return null;
        }

        /// <summary>
        /// Resolve a print statement
        /// </summary>
        /// <param name="stmt">The statement</param>
        /// <returns></returns>
        public object Visit(Stmt.Print stmt)
        {
            Resolve(stmt.Expression);

            return null;
        }

        /// <summary>
        /// Resolve a return statement
        /// </summary>
        /// <param name="stmt">The statement</param>
        /// <returns></returns>
        public object Visit(Stmt.Return stmt)
        {
            // Make sure we are in a function
            if (_current_function == FunctionType.NONE)
            {
                CsLox.Error(stmt.Keyword, "Cannot return from top-level code.");
            }


            if (stmt.Value != null)
            {
                Resolve(stmt.Value);
            }
            return null;
        }

        /// <summary>
        /// Resolve a while statement
        /// </summary>
        /// <param name="stmt">The statement</param>
        /// <returns></returns>
        public object Visit(Stmt.While stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);

            return null;
        }

        /// <summary>
        /// Resolve a binary expression
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns></returns>
        public object Visit(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);

            return null;
        }

        /// <summary>
        /// Resolve a call expression
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns></returns>
        public object Visit(Expr.Call expr)
        {
            Resolve(expr.Callee);

            foreach (Expr arg in expr.Arguments)
            {
                Resolve(arg);
            }

            return null;
        }

        /// <summary>
        /// Resolve a grouping expression
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns></returns>
        public object Visit(Expr.Grouping expr)
        {
            Resolve(expr.Expression);

            return null;
        }

        /// <summary>
        /// Resolve a literal expression
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns></returns>
        public object Visit(Expr.Literal expr)
        {
            return null;
        }

        /// <summary>
        /// Resolve a logical expression
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns></returns>
        public object Visit(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);

            return null;
        }

        /// <summary>
        /// Resolve an unary expression
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns></returns>
        public object Visit(Expr.Unary expr)
        {
            Resolve(expr.Right);

            return null;
        }


        /// <summary>
        /// Resolve scope for a list of statements
        /// </summary>
        /// <param name="statements">The statements</param>
        public void Resolve(IEnumerable<Stmt> statements)
        {
            foreach (Stmt statement in statements)
            {
                Resolve(statement);
            }
        }

        /// <summary>
        /// Resolve the scope for a statement
        /// </summary>
        /// <param name="stmt">The statement</param>
        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        /// <summary>
        /// Resolve a expression
        /// </summary>
        /// <param name="expr">The expression</param>
        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        /// <summary>
        /// Resolve a local variable
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <param name="name">The name token</param>
        private void ResolveLocal(Expr expr, Token name)
        {
            // Look down the stack
            for (int i = _scopes.Count() - 1; i >= 0; i--)
            {
                if (_scopes[i].ContainsKey(name.Lexeme))
                {
                    _interpeter.Resolve(expr, _scopes.Count() - 1 - i);
                }
            }

            // Global?
        }


        /// <summary>
        /// Resolve a function, creating a scope and binding its parameters
        /// </summary>
        /// <param name="function">The function</param>
        private void ResolveFunction(Stmt.Function function, FunctionType type)
        {
            // Keep track of functions
            FunctionType enclosing_function = _current_function;
            _current_function = type;

            BeginScope();
            foreach (Token param in function.Parameters)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.Body);
            EndScope();

            _current_function = enclosing_function;

        }


        /// <summary>
        /// Begin a new scope
        /// </summary>
        private void BeginScope()
        {
            _scopes.Push(new HashMap<string, bool>());
        }

        /// <summary>
        /// End a scope
        /// </summary>
        private void EndScope()
        {
            _scopes.Pop();
        }

        /// <summary>
        /// Declare a variable into the current scope
        /// </summary>
        /// <param name="name">The variable name token</param>
        private void Declare(Token name)
        {
            // Make sure there is a active scope
            if (!_scopes.Any()) return;

            HashMap<string, bool> scope = _scopes.Peek();

            // Make sure we hav't already declared this variable
            if (scope.ContainsKey(name.Lexeme))
            {
                CsLox.Error(name, "Variable with this name already in this scope.");
            }


            scope.Put(name.Lexeme, false);
        }

        /// <summary>
        /// Define a varibale into the current scope
        /// </summary>
        /// <param name="name">The variable name token</param>
        private void Define(Token name)
        {
            if (!_scopes.Any()) return;

            _scopes.Peek().Put(name.Lexeme, true);

        }


        private enum FunctionType
        {
            NONE,
            FUNCTION
        }


    }
}
