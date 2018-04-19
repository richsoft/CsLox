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
            T Visit(Assign expr);
            T Visit(Binary expr);
            T Visit(Call expr);
            T Visit(Get expr);
            T Visit(Grouping expr);
            T Visit(Literal expr);
            T Visit(Logical expr);
            T Visit(Set expr);
            T Visit(Super expr);
            T Visit(This expr);
            T Visit(Unary expr);
            T Visit(Variable expr);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class Assign : Expr
        {
            public Expr value;
            public Assign(Token name, Expr value)
            {
                this.Name = name;
                this.value = value;
            }

            public Token Name { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Binary : Expr
        {
            public Binary(Expr left, Token op, Expr right)
            {
                this.Left = left;
                this.Operator = op;
                this.Right = right;
            }

            public Expr Left { get; }
            public Token Operator { get; }
            public Expr Right { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Call : Expr
        {
            public Call(Expr callee, Token paren, IList<Expr> arguments)
            {
                this.Callee = callee;
                this.Paren = paren;
                this.Arguments = arguments;
            }

            public IList<Expr> Arguments { get; }
            public Expr Callee { get; }
            public Token Paren { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

        }

        public class Get : Expr
        {
            public Get(Expr obj, Token name)
            {
                this.Object = obj;
                this.Name = name;
            }

            public Token Name { get; }
            public Expr Object { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Grouping : Expr
        {
            public Grouping(Expr expression)
            {
                this.Expression = expression;
            }

            public Expr Expression { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Literal : Expr
        {
            public Literal(object value)
            {
                this.Value = value;
            }

            public object Value { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Logical : Expr
        {
            public Logical(Expr left, Token op, Expr right)
            {
                this.Left = left;
                this.Operator = op;
                this.Right = right;
            }

            public Expr Left { get; }
            public Token Operator { get; }
            public Expr Right { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Set : Expr
        {
            public Set(Expr obj, Token name, Expr value)
            {
                this.Object = obj;
                this.Name = name;
                this.Value = value;
            }

            public Token Name { get; }
            public Expr Object { get; }
            public Expr Value { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Super : Expr
        {
            public Super(Token keyword, Token method)
            {
                this.keyword = keyword;
                this.Method = method;
            }

            public Token keyword { get; }
            public Token Method { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class This : Expr
        {
            public This(Token keyword)
            {
                this.Keyword = keyword;
            }

            public Token Keyword { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

        }

        public class Unary : Expr
        {
            public Unary(Token op, Expr right)
            {
                this.Operator = op;
                this.Right = right;
            }

            public Token Operator { get; }
            public Expr Right { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Variable : Expr
        {
            public Variable(Token name)
            {
                this.Name = name;
            }

            public Token Name { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
