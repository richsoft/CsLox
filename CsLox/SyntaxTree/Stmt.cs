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
            T Visit(Block stmt);
            T Visit(Break stmt);
            T Visit(Class stmt);
            T Visit(Continue stmt);
            T Visit(ExpressionStatement stmt);
            T Visit(Function stmt);
            T Visit(If stmt);
            T Visit(Print stmt);
            T Visit(Return stmt);
            T Visit(VarDeclaration stmt);
            T Visit(While stmt);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class Block : Stmt
        {
            public Block(IEnumerable<Stmt> statements)
            {
                this.Statements = statements;
            }

            public IEnumerable<Stmt> Statements { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Break : Stmt
        {

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Class : Stmt
        {
            public Expr.Variable Superclass;
            public Class(Token name, Expr.Variable superclass, IList<Stmt.Function> methods)
            {
                this.Name = name;
                this.Superclass = superclass;
                this.Methods = methods;
            }

            public IList<Stmt.Function> Methods { get; }
            public Token Name { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

        }

        public class Continue : Stmt
        {

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class ExpressionStatement : Stmt
        {
            public ExpressionStatement(Expr expression)
            {
                this.Expression = expression;
            }

            public Expr Expression { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Function : Stmt
        {
            public Function(Token name, IList<Token> parameters, IList<Stmt> body)
            {
                this.Name = name;
                this.Parameters = parameters;
                this.Body = body;
            }

            public IList<Stmt> Body { get; }
            public Token Name { get; }
            public IList<Token> Parameters { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class If : Stmt
        {
            public If(Expr condtion, Stmt then_branch, Stmt else_branch)
            {
                this.Condition = condtion;
                this.ThenBranch = then_branch;
                this.ElseBranch = else_branch;
            }

            public Expr Condition { get; }
            public Stmt ElseBranch { get; }
            public Stmt ThenBranch { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Print : Stmt
        {
            public Print(Expr expression)
            {
                this.Expression = expression;
            }

            public Expr Expression { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Return : Stmt
        {
            public Return(Token keyword, Expr value)
            {
                this.Keyword = keyword;
                this.Value = value;
            }

            public Token Keyword { get; }
            public Expr Value { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

        }

        public class VarDeclaration : Stmt
        {
            public VarDeclaration(Token name, Expr initializer)
            {
                this.Name = name;
                this.Initializer = initializer;
            }

            public Expr Initializer { get; }
            public Token Name { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        public class While : Stmt
        {
            public While(Expr condition, Stmt body)
            {
                this.Condition = condition;
                this.Body = body;
            }

            public Stmt Body { get; }
            public Expr Condition { get; }
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
