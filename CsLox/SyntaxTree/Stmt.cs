using CsLox.Tokens;
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
            T Visit(VarDeclaration stmt);
            T Visit(Block stmt);
            T Visit(Function stmt);
            T Visit(If stmt);
            T Visit(While stmt);
            T Visit(Return stmt);
            T Visit(Class stmt);
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

        public class VarDeclaration : Stmt
        {
            public Token Name { get; }
            public Expr Initializer { get; }

            public VarDeclaration(Token name, Expr initializer)
            {
                this.Name = name;
                this.Initializer = initializer;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Block : Stmt
        {
            public IEnumerable<Stmt> Statements { get; }

            public Block(IEnumerable<Stmt> statements)
            {
                this.Statements = statements;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class If : Stmt
        {
            public Expr Condition { get; }
            public Stmt ThenBranch { get; }
            public Stmt ElseBranch { get; }

            public If(Expr condtion, Stmt then_branch, Stmt else_branch)
            {
                this.Condition = condtion;
                this.ThenBranch = then_branch;
                this.ElseBranch = else_branch;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class While : Stmt
        {
            public Expr Condition { get; }
            public Stmt Body { get; }

            public While(Expr condition, Stmt body)
            {
                this.Condition = condition;
                this.Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Function : Stmt
        {
            public Token Name { get; }
            public IList<Token> Parameters { get; }
            public IList<Stmt> Body { get; }

            public Function(Token name, IList<Token> parameters, IList<Stmt> body)
            {
                this.Name = name;
                this.Parameters = parameters;
                this.Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Return : Stmt
        {
            public Token Keyword { get; }
            public Expr Value { get; }

            public Return (Token keyword, Expr value)
            {
                this.Keyword = keyword;
                this.Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

        }

        public class Class : Stmt
        {
            public Token Name { get; }
            public Expr.Variable Superclass;
            public IList<Stmt.Function> Methods { get; }

            public Class(Token name, Expr.Variable superclass, IList<Stmt.Function> methods)
            {
                this.Name = name;
                this.Superclass = superclass;
                this.Methods = methods;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

        }

    }
}
