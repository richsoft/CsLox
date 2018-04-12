using CsLox.Exceptions;
using CsLox.SyntaxTree;
using CsLox.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsLox.Environments;

namespace CsLox
{
    class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {

        private Environments.Environment _environment = new Environments.Environment();

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
            return _environment.Get(expr.Name);
        }

        public object Visit(Expr.Assign expr)
        {
            object value = Evaluate(expr.value);

            _environment.Assign(expr.Name, value);
            return value;

        }


        /// <summary>
        /// Execute a statement
        /// </summary>
        /// <param name="stmt">The statement</param>
        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
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
            if (obj is double) {
                if ((double)obj % 1 == 0)
                {
                    return ((double)obj).ToString("0");
                }
            }

            return obj.ToString();


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
    }
}
