using CsLox.SyntaxTree;
using CsLox.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.Parsing
{

    class Parser
    {
        private readonly List<Token> _tokens;
        private int _current = 0;

        /// <summary>
        /// Create a new Parser instance
        /// </summary>
        /// <param name="tokens">The input tokens</param>
        public Parser(List<Token> tokens)
        {
            this._tokens = tokens;
        }

        /// <summary>
        /// Parse the tokens
        /// </summary>
        /// <returns>The parsed statements</returns>
        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();

            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;

        }

        /// <summary>
        /// Parse a declaration
        /// </summary>
        /// <returns>The statement</returns>
        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.FUN)) return Function("function");
                if (Match(TokenType.VAR)) return VarDeclaration();

                return Statement();
            }
            catch (ParseErrorException)
            {
                // Try and resync if there is an error
                Synchronise();
                return null;
            }
        }

        private Stmt Function(string kind)
        {
            Token name = Consume(TokenType.IDENTIFIER, $"Expect {kind} name.");

            // Parameters
            Consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name.");
            List<Token> parameters = new List<Token>();
            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count() >= 8)
                    {
                        Error(Peek(), "Cannot have more than 8 parameters");
                    }

                    parameters.Add(Consume(TokenType.IDENTIFIER, "Expect parameter name."));
                }
                while (Match(TokenType.COMMA));
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");

            // Body
            Consume(TokenType.LEFT_BRACE, $"Expect '{{' before {kind} body.");
            List<Stmt> body = Block();

            return new Stmt.Function(name, parameters, body);

        }

        /// <summary>
        /// Parse a variable declaration
        /// </summary>
        /// <returns></returns>
        private Stmt VarDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            // If there is a equals, the variable is initalized
            Expr initializer = null;
            if (Match(TokenType.EQUAL))
            {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.VarDeclaration(name, initializer);
        }

        /// <summary>
        /// Parse a statement
        /// </summary>
        /// <returns>The statement</returns>
        private Stmt Statement()
        {

            if (Match(TokenType.FOR)) return ForStatement();
            if (Match(TokenType.IF)) return IfStatement();
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.RETURN)) return ReturnStatement();
            if (Match(TokenType.WHILE)) return WhileStatement();
            if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());


            return ExpressionStatement();
        }


        /// <summary>
        /// Parse a return statement
        /// </summary>
        /// <returns>The statement</returns>
        private Stmt ReturnStatement()
        {
            Token keyword = Previous();
            
            // If not return value is set, make it nil
            Expr value = null;
            if (!Check(TokenType.SEMICOLON))
            {
                value = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after return value.");
            return new Stmt.Return(keyword, value);
        }


        /// <summary>
        /// Parse a for loop
        /// </summary>
        /// <returns>The statement</returns>
        private Stmt ForStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

            // Initializer

            Stmt initializer;
            if (Match(TokenType.SEMICOLON))
            {
                // No initialiser
                initializer = null;
            }
            else if (Match(TokenType.VAR))
            {
                // Its a variable decalration
                initializer = VarDeclaration();
            }
            else
            {
                // Its an expression
                // This must be a _statement_
                initializer = ExpressionStatement();
            }

            // Condition
            Expr condition = null;
            if (!Check(TokenType.SEMICOLON))
            {
                condition = Expression();
            }
            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

            // Increment
            Expr increment = null;
            if (!Check(TokenType.SEMICOLON))
            {
                increment = Expression();
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

            // Body
            Stmt body = Statement();

            // Convert to a while loop
            if (increment != null)
            {
                body = new Stmt.Block(new[] { body, new Stmt.ExpressionStatement(increment) });
            }

            if (condition == null)
            {
                // No condition, so set to true
                condition = new Expr.Literal(true);
            }

            body = new Stmt.While(condition, body);

            if (initializer != null)
            {
                body = new Stmt.Block(new[] { initializer, body });
            }

            return body;

        }


        /// <summary>
        /// Parse while loop
        /// </summary>
        /// <returns>The statement</returns>
        private Stmt WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect '(' after while condition.");
            Stmt body = Statement();

            return new Stmt.While(condition, body);

        }


        /// <summary>
        /// Parse if statement
        /// </summary>
        /// <returns>The statement</returns>
        private Stmt IfStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

            Stmt then_branch = Statement();
            Stmt else_branch = null;

            // Do we have an else?
            if (Match(TokenType.ELSE))
            {
                else_branch = Statement();
            }

            return new Stmt.If(condition, then_branch, else_branch);
        }

        /// <summary>
        /// Parse a block of statementts
        /// </summary>
        /// <returns></returns>
        private List<Stmt> Block()
        {
            List<Stmt> statements = new List<Stmt>();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;

        }


        /// <summary>
        /// Parse a print statement
        /// </summary>
        /// <returns>The statement</returns>
        private Stmt PrintStatement()
        {
            Expr value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value");
            return new Stmt.Print(value);
        }

        /// <summary>
        /// Parse an expression statement
        /// </summary>
        /// <returns>The statement</returns>
        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.ExpressionStatement(expr);
        }


        /// <summary>
        /// Parse an expression
        /// </summary>
        /// <returns>The expression</returns>
        private Expr Expression()
        {
            return Assignment();
        }

        /// <summary>
        /// Parse an assignment
        /// </summary>
        /// <returns></returns>
        private Expr Assignment()
        {
            Expr expr = Or();

            if (Match(TokenType.EQUAL))
            {
                Token equals = Previous();
                Expr value = Assignment();

                // We have found a assignment target
                // Make sure its a variable
                if (expr is Expr.Variable)
                {
                    Token name = ((Expr.Variable)expr).Name;
                    return new Expr.Assign(name, value);
                }

                Error(equals, "invalid assignment target.");
            }

            return expr;

        }

        /// <summary>
        /// Parse an OR expression
        /// </summary>
        /// <returns>The expression</returns>
        private Expr Or()
        {
            Expr expr = And();

            while (Match(TokenType.OR))
            {
                Token op = Previous();
                Expr right = And();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Parse an AND expression
        /// </summary>
        /// <returns>The expression</returns>
        private Expr And()
        {
            Expr expr = Equality();

            while (Match(TokenType.AND))
            {
                Token op = Previous();
                Expr right = Equality();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Parse a equality expression
        /// </summary>
        /// <returns>The expression</returns>
        private Expr Equality()
        {
            Expr expr = Comparison();

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token op = Previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;

        }

        /// <summary>
        /// Parse a comparison expression
        /// </summary>
        /// <returns>The epxression</returns>
        private Expr Comparison()
        {
            Expr expr = Addition();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token op = Previous();
                Expr right = Addition();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Parse an addition/subtraction expression
        /// </summary>
        /// <returns></returns>
        private Expr Addition()
        {
            Expr expr = Multiplication();

            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token op = Previous();
                Expr right = Multiplication();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;

        }

        /// <summary>
        /// Parse a mulitplication/division expression
        /// </summary>
        /// <returns></returns>
        private Expr Multiplication()
        {
            Expr expr = Unary();

            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                Token op = Previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;

        }

        /// <summary>
        /// Parse a unary expression
        /// </summary>
        /// <returns></returns>
        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token op = Previous();
                Expr right = Unary();
                return new Expr.Unary(op, right);
            }

            return Call();

        }

        /// <summary>
        /// Parse a function call
        /// </summary>
        /// <returns>The expression</returns>
        private Expr Call()
        {
            Expr expr = Primary();

            while (true)
            {
                if (Match(TokenType.LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        /// <summary>
        /// Helper for calling functions
        /// </summary>
        /// <param name="callee">The callee</param>
        /// <returns>The expression</returns>
        private Expr FinishCall(Expr callee)
        {
            List<Expr> arguments = new List<Expr>();

            // Find any arguments
            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (arguments.Count() >= 8)
                    {
                        Error(Peek(), "Cannot have more than 8 arguments");
                    }

                    arguments.Add(Expression());
                }
                while (Match(TokenType.COMMA));
            }

            Token paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");

            return new Expr.Call(callee, paren, arguments);

        }

        /// <summary>
        /// Parse the primary/literal expression
        /// </summary>
        /// <returns>The literal expression</returns>
        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Expr.Literal(false);
            if (Match(TokenType.TRUE)) return new Expr.Literal(true);
            if (Match(TokenType.NIL)) return new Expr.Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new Expr.Variable(Previous());
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            // Unknown token
            throw Error(Peek(), "Expect expression.");
        }

        /// <summary>
        /// Consume a token checking it is of the correct type.  Throw an error if not
        /// </summary>
        /// <param name="type">The expected token type</param>
        /// <param name="message">The error message</param>
        /// <returns>The token</returns>
        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }


        /// <summary>
        /// Create a new error exception, and log
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="message">The error message</param>
        private ParseErrorException Error(Token token, string message)
        {
            CsLox.ParseError(token, message);
            return new ParseErrorException();
        }

        /// <summary>
        /// Check if the current token matches one of a set of types, and comsume it if it does
        /// </summary>
        /// <param name="types">The tpyes to match</param>
        /// <returns>True if matched</returns>
        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the current token matches a type
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if the token is the given type</returns>
        private bool Check(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }
            return Peek().Type == type;
        }

        /// <summary>
        /// Consume and return the next token
        /// </summary>
        /// <returns>The current token</returns>
        private Token Advance()
        {
            if (!IsAtEnd())
            {
                _current++;
            }

            return Previous();
        }

        /// <summary>
        /// Check if we have reached the EOF token
        /// </summary>
        /// <returns>True if th next token is EOF</returns>
        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        /// <summary>
        /// Peek the next token without consuming it
        /// </summary>
        /// <returns>The next token</returns>
        private Token Peek()
        {
            return _tokens[_current];
        }

        /// <summary>
        /// The last consumed token
        /// </summary>
        /// <returns>The current token</returns>
        private Token Previous()
        {
            return _tokens[_current - 1];
        }

        /// <summary>
        /// Resyncronise the parser state after a syntax error
        /// </summary>
        private void Synchronise()
        {
            Advance();

            while (!IsAtEnd())
            {

                // We can resync if we are at a semicolon
                if (Previous().Type == TokenType.SEMICOLON) return;

                // Or the next token starts a statement or declaration
                switch (Peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;

                }

                Advance();
            }


        }



        private class ParseErrorException : Exception { }

        private enum FunctionKind
        {
            FUNCTION
        }
    }
}
