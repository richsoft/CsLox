using CsLox.SyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox
{
    class AstPainter : Expr.IVisitor<string>
    {
        /// <summary>
        /// Create a string represnetation of the AST nodes
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns>The string</returns>
        public string Print(Expr expr)
        {
            if (expr != null)
            {
                return expr.Accept(this);
            }

            return "";

        }

        public string Visit(Expr.Binary expr)
        {
            return Parenthesise(expr.Operator.Lexeme, expr.Left, expr.Right);
 
        }

        public string Visit(Expr.Grouping expr)
        {
            return Parenthesise("group", expr.Expression);
        }

        public string Visit(Expr.Literal expr)
        {
            if (expr.Value == null)
            {
                return "nil";
            }

            return expr.Value.ToString();
        }

        public string Visit(Expr.Unary expr)
        {
            return Parenthesise(expr.Operator.Lexeme, expr.Right);
        }

        public string Visit(Expr.Variable expr)
        {
            throw new NotImplementedException();
        }

        public string Visit(Expr.Assign expr)
        {
            throw new NotImplementedException();
        }

        public string Visit(Expr.Logical expr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Build a bracketed string for the expressions
        /// </summary>
        /// <param name="name">The operator name</param>
        /// <param name="exprs">The sub expressions</param>
        /// <returns>The bracketed string</returns>
        private string Parenthesise(string name, params Expr[] exprs)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(");
            sb.Append(name);
            foreach (Expr expr in exprs)
            {
                sb.Append(" ");
                sb.Append(expr.Accept(this));
            }
            sb.Append(")");

            return sb.ToString();
        }


    }
}
