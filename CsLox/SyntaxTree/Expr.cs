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
            T Visit(Binary expr);
            T Visit(Grouping expr);
            T Visit(Literal expr);
            T Visit(Unary expr);
            T Visit(Variable expr);
            T Visit(Assign expr);
            T Visit(Logical expr);
            T Visit(Call expr);
            T Visit(Get expr);
            T Visit(Set expr);
            T Visit(This expr);
            T Visit(Super expr);
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
                return visitor.Visit(this);
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
                return visitor.Visit(this);
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
                return visitor.Visit(this);
            }
        }

        public class Unary : Expr
        {
            public Token Operator { get; }
            public Expr Right { get; }


            public Unary(Token op, Expr right)
            {
                this.Operator = op;
                this.Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Variable : Expr
        {
            public Token Name { get; }

            public Variable(Token name)
            {
                this.Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Assign : Expr
        {
            public Token Name { get; }
            public Expr value;

            public Assign(Token name, Expr value)
            {
                this.Name = name;
                this.value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Logical : Expr
        {
            public Expr Left { get; }
            public Token Operator { get; }
            public Expr Right { get; }

            public Logical(Expr left, Token op, Expr right)
            {
                this.Left = left;
                this.Operator = op;
                this.Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Call : Expr
        {
            public Expr Callee { get; }
            public Token Paren { get; }
            public IList<Expr> Arguments { get; }

            public Call(Expr callee, Token paren, IList<Expr> arguments)
            {
                this.Callee = callee;
                this.Paren = paren;
                this.Arguments = arguments;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

        }

        public class Get : Expr
        {
            public Expr Object { get; }
            public Token Name { get; }

            public Get(Expr obj, Token name)
            {
                this.Object = obj;
                this.Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Set : Expr
        {
            public Expr Object { get; }
            public Token Name { get; }
            public Expr Value { get; }

            public Set(Expr obj, Token name, Expr value)
            {
                this.Object = obj;
                this.Name = name;
                this.Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class This : Expr
        {
            public Token Keyword { get; }

            public This(Token keyword)
            {
                this.Keyword = keyword;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

        }

        public class Super : Expr
        {
            public Token keyword { get; }
            public Token Method { get; }

            public Super(Token keyword, Token method)
            {
                this.keyword = keyword;
                this.Method = method;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
