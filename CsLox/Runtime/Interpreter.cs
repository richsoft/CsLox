using CsLox.Exceptions;
using CsLox.SyntaxTree;
using CsLox.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsLox.Collections;

namespace CsLox.Runtime
{
    class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {

        private readonly LoxEnvironment _globals = new LoxEnvironment();
        private readonly HashMap<Expr, int?> _locals = new HashMap<Expr, int?>();

        private LoxEnvironment _environment;

        public Interpreter()
        {
            // We need to set this here in C#
            _environment = _globals;

            // Add some native functions
            _globals.Define("clock", new NativeFunctions.Clock());
        }


        /// <summary>
        /// Interpret statements
        /// </summary>
        /// <param name="statements">The statements</param>
        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeErrorException ex)
            {
                CsLox.RuntimeError(ex);
            }
        }


        public object Visit(Expr.Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);

                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);

                case TokenType.GREATER:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;

                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;

                case TokenType.LESS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;

                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;

                case TokenType.MINUS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;

                case TokenType.PLUS:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }

                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }

                    // Can't add numbers and strings
                    throw new RuntimeErrorException(expr.Operator, "Operands must be tow numbers or two strings.");

                case TokenType.SLASH:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;

                case TokenType.STAR:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
            }

            // Unreachable?
            return null;


        }

        public object Visit(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object Visit(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object Visit(Expr.Unary expr)
        {
            object right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.BANG_EQUAL:
                    return IsTruthy(right);

                case TokenType.MINUS:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double)right;
            }

            // Unreachable?
            return null;


        }

        public object Visit(Expr.Variable expr)
        {
            return LookUpVariable(expr.Name, expr);
        }

        public object Visit(Expr.Assign expr)
        {
            object value = Evaluate(expr.value);

            int? distance = _locals.Get(expr);
            if (distance != null)
            {
                _environment.AssignAt(distance.Value, expr.Name, value);
            }
            else
            {
                _globals.Assign(expr.Name, value);
            }


            return value;

        }

        public object Visit(Expr.Logical expr)
        {
            object left = Evaluate(expr.Left);

            // Short-Curcuit logic
            if (expr.Operator.Type == TokenType.OR)
            {
                // OR - if left side is true, the expression is true
                if (IsTruthy(left)) return left;
            }
            else
            {
                // AND - if left side is false, the expression is false
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.Right);
        }

        public object Visit(Expr.Call expr)
        {
            object callee = Evaluate(expr.Callee);

            List<object> arguments = new List<object>();
            foreach (Expr arg in expr.Arguments)
            {
                arguments.Add(Evaluate(arg));
            }

            // Make sure we the callee is actually callable
            if (!(callee is ILoxCallable))
            {
                throw new RuntimeErrorException(expr.Paren, "Can only call functions and classes.");
            }

            ILoxCallable function = (ILoxCallable)callee;

            // Make sure we are passing the correct number of arguments
            if (arguments.Count() != function.Arity)
            {
                throw new RuntimeErrorException(expr.Paren, $"Expected {function.Arity} argumuments, but got {arguments.Count()}.");
            }

            return function.Call(this, arguments);
        }

        public object Visit(Expr.Get expr)
        {
            object obj = Evaluate(expr.Object);
            if (obj is LoxInstance)
            {
                return ((LoxInstance)obj).Get(expr.Name);
            }

            throw new RuntimeErrorException(expr.Name, "Only instances has properties.");


        }

        public object Visit(Expr.Set expr)
        {
            object obj = Evaluate(expr.Object);

            if (!(obj is LoxInstance))
            {
                throw new RuntimeErrorException(expr.Name, "only instances have fields.");
            }

            object value = Evaluate(expr.Value);
            ((LoxInstance)obj).Set(expr.Name, value);
            return value;
        }

        public object Visit(Expr.Super expr)
        {
            // Look up the superclass
            int? distance = _locals.Get(expr);
            LoxClass superclass = (LoxClass)_environment.GetAt(distance.Value, "super");

            // "this" is always one level nearer than "super"
            LoxInstance obj = (LoxInstance)_environment.GetAt(distance.Value - 1, "this");

            // Lookup the method
            LoxFunction method = superclass.FindMethod(obj, expr.Method.Lexeme);

            if (method == null)
            {
                throw new RuntimeErrorException(expr.Method, $"Undefined property '{expr.Method.Lexeme}'.");
            }

            return method;
        }

        public object Visit(Expr.This expr)
        {
            return LookUpVariable(expr.Keyword, expr);
        }


        /// <summary>
        /// Execute a statement
        /// </summary>
        /// <param name="stmt">The statement</param>
        private void Execute(Stmt stmt)
        {
            if (stmt != null)
            {
                stmt.Accept(this);
            }
        }

        /// <summary>
        /// Recursively evaluate an expression
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns>The expression result</returns>
        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        /// <summary>
        /// Check if an object is 'truthy'
        /// </summary>
        /// <param name="obj">The obj to check</param>
        /// <returns>True, if the object is truthy</returns>
        private bool IsTruthy(object obj)
        {
            // NULL is false
            // Booleans are as epected
            // Everything else is true

            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;

        }

        /// <summary>
        /// Check if two objects are equals
        /// </summary>
        /// <param name="a">Object a</param>
        /// <param name="b">Object b</param>
        /// <returns>True if the objects are equal</returns>
        private bool IsEqual(object a, object b)
        {
            // Nil is only ever equal is nil
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        /// <summary>
        /// Check if an operand is a number, thow if not
        /// </summary>
        /// <param name="op">The operator token</param>
        /// <param name="operand">The operand to check</param>
        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double) return;

            throw new RuntimeErrorException(op, "Operand must be a number.");
        }


        /// <summary>
        /// Check the left and right operand are numbers, thow if not
        /// </summary>
        /// <param name="op">The operator token</param>
        /// <param name="left">The left operand to check</param>
        /// <param name="right">The right operand to check</param>
        private void CheckNumberOperands(Token op, object left, object right)
        {
            if (left is double && right is double) return;

            throw new RuntimeErrorException(op, "Operands must be numbers.");
        }

        /// <summary>
        /// Convert an object to a string
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The string value</returns>
        private string Stringify(object obj)
        {
            // NULL/nil
            if (obj == null) return "nil";

            // Doubles
            // If this in an integer, don't show the decimal
            if (obj is double)
            {
                if ((double)obj % 1 == 0)
                {
                    return ((double)obj).ToString("0");
                }
            }

            return obj.ToString();


        }

        /// <summary>
        /// Look up a variable, using the correct scope
        /// </summary>
        /// <param name="name">The token name</param>
        /// <param name="expr">The expression</param>
        /// <returns></returns>
        public object LookUpVariable(Token name, Expr expr)
        {
            int? distance = _locals.Get(expr);
            if (distance != null)
            {
                return _environment.GetAt(distance.Value, name.Lexeme);
            }
            else
            {
                // Its global
                return _globals.Get(name);
            }

        }

        public object Visit(Stmt.ExpressionStatement stmt)
        {
            Evaluate(stmt.Expression);
            return null;
        }

        public object Visit(Stmt.Print stmt)
        {
            object value = Evaluate(stmt.Expression);
            Console.WriteLine(Stringify(value));
            return null;

        }

        public object Visit(Stmt.Return stmt)
        {
            // Set to null if nothing returned
            object value = null;
            if (stmt.Value != null)
            {
                value = Evaluate(stmt.Value);
            }

            throw new ReturnException(value);
        }

        public object Visit(Stmt.VarDeclaration stmt)
        {
            object value = null;

            // Evaluate the initializer if one is set
            if (stmt.Initializer != null)
            {
                value = Evaluate(stmt.Initializer);
            }

            _environment.Define(stmt.Name.Lexeme, value);

            return null;
        }

        public object Visit(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new LoxEnvironment(_environment));
            return null;
        }

        public object Visit(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            }
            else
            {
                Execute(stmt.ElseBranch);
            }

            return null;
        }

        public object Visit(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.Body);
            }

            return null;
        }

        public object Visit(Stmt.Function stmt)
        {
            LoxFunction function = new LoxFunction(stmt, _environment, false);
            _environment.Define(stmt.Name.Lexeme, function);
            return null;
        }

        public object Visit(Stmt.Class stmt)
        {
            _environment.Define(stmt.Name.Lexeme, null);

            // Superclass
            object superclass = null;
            if (stmt.Superclass != null)
            {
                superclass = Evaluate(stmt.Superclass);
                if (!(superclass is LoxClass))
                {
                    throw new RuntimeErrorException(stmt.Superclass.Name, "Superclass must be a class.");
                }

                _environment = new LoxEnvironment(_environment);
                _environment.Define("super", superclass);
            }


            // Methods
            HashMap<string, LoxFunction> methods = new HashMap<string, LoxFunction>();
            foreach (Stmt.Function method in stmt.Methods)
            {
                LoxFunction function = new LoxFunction(method, _environment, method.Name.Lexeme.Equals("init"));
                methods.Put(method.Name.Lexeme, function);
            }

            LoxClass @class = new LoxClass(stmt.Name.Lexeme, (LoxClass)superclass, methods);

            if (superclass != null)
            {
                _environment = _environment.Enclosing;
            }

            _environment.Assign(stmt.Name, @class);
            return null;
        }

        /// <summary>
        /// Ececute a block of statements
        /// </summary>
        /// <param name="statements">The statements</param>
        /// <param name="environment">The environment</param>
        public void ExecuteBlock(IEnumerable<Stmt> statements, LoxEnvironment environment)
        {
            // Save the current environment
            LoxEnvironment previous = this._environment;

            try
            {
                // Set the new environment
                this._environment = environment;

                foreach (Stmt statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                // Restore the old environment
                this._environment = previous;
            }


        }

        /// <summary>
        /// Resolve a variable
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <param name="depth">The scope depth</param>
        public void Resolve(Expr expr, int depth)
        {
            _locals.Put(expr, depth);
        }
    }
}
