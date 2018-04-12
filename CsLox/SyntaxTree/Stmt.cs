using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.SyntaxTree
{
    abstract class Stmt
    {
        public interface IVisitor<T>
        {
            T Visit(ExpressionStatement stmt);
            T Visit(Print stmt);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class ExpressionStatement: Stmt
        {
            public Expr Expression { get; }

            public ExpressionStatement(Expr expression)
            {
                this.Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Print : Stmt
        {
            public Expr Expression { get; }

            public Print(Expr expression)
            {
                this.Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

    }
}
