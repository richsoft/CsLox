using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsLox.Tokens;

namespace CsLox.SyntaxTree
{
    abstract class Expr
    {

        public interface IVisitor<T>
        {
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class Binary : Expr
        {
            public Expr Left { get; }
            public Token Operator { get; }
            public Expr Right { get; }

            public Binary(Expr left, Token op, Expr right)
            {
                this.Left = left;
                this.Operator = op;
                this.Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        public class Grouping : Expr
        {
            public Expr Expression { get; }

            public Grouping(Expr expression)
            {
                this.Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        public class Literal : Expr
        {
            public object Value { get; }

            public Literal(object value)
            {
                this.Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        public class Unary : Expr
        {
            public Token Operator { get; }
            public Expr right;


            public Unary(Token op, Expr right)
            {
                this.Operator = op;
                this.right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }

    }
}
