## Operator Precedence and Associatity


Name            | Operators | Associates 
----------------|-----------|-------
Unary           | ! -       | Right 
Multiplication  | / *       | Left 
Addition        | - +       | Left 
Comparison      | > >= < <= | Left 
Equality        | == !=     | Left 


expression     → equality ;
equality       → comparison ( ( "!=" | "==" ) comparison )* ;
comparison     → addition ( ( ">" | ">=" | "<" | "<=" ) addition )* ;
addition       → multiplication ( ( "-" | "+" ) multiplication )* ;
multiplication → unary ( ( "/" | "*" ) unary )* ;
unary          → ( "!" | "-" ) unary
               | primary ;
primary        → NUMBER | STRING | "false" | "true" | "nil"
               | "(" expression ")" ;