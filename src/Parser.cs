using System.Collections.Generic;
using System.Linq;
using ASTPrimitives;
using TokenizationPrimitives;
using TypePrimitives;
using static TokenizationPrimitives.TokenType;

namespace Parsing;

public class Parser
{
    List<Token> tokens;
    int next;
    bool executable = true;

    /// <summary>
    /// Create Parser.
    /// </summary>
    /// <param name="tokens">List of LOLCODE tokens. See <see cref="Token"/></param>
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    /// <summary>
    /// Parse list of tokens stored in Parser.
    /// </summary>
    public bool Parse(out Stmt? program) // entry point
    {
        program = null;

        while (nextIsType(COMMAND_TERMINATOR)) // skip leading empty lines
            ++next;

        if (!nextIsType(BEGIN))
        {
            executable = false;
            ExceptionReporter.Log(new ParsingException("Program must start with HAI", 1));
            synchronize();
        }

        try
        {
            program = statement(); // program is wrapped in block statement
        }
        catch (ParsingException e)
        {
            executable = false;
            ExceptionReporter.Log(e);
        }

        return executable;
    }

    // parsing helpers
    /// <summary>
    /// Return next Token and advance next pointer.
    /// </summary>
    Token consumeNext()
    {
        if (!atEOF())
            return tokens[next++];
        else
        {
            return tokens[^1];
        }
    }

    /// <summary>
    /// Return next Token without advancing next pointer.
    /// </summary>
    Token peekNext() => tokens[next];

    /// <summary>
    /// Returns true if next pointer is at end of file.
    /// </summary>
    bool atEOF() => peekNext().type == EOF;

    /// <summary>
    /// Return true if next Token is of specified type. See <see cref="TokenType"/>
    /// </summary>
    bool nextIsType(params TokenType[] types) => types.Any(t => peekNext().type == t);

    /// <summary>
    /// Consume next Token if it is of specified type. See <see cref="TokenType"/>
    /// </summary>
    bool skipIfNextIsType(params TokenType[] types)
    {
        if (nextIsType(types))
        {
            consumeNext();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Consume next token if it is of specified type, or throw exception. See <see cref="TokenType"/>
    /// </summary>
    Token expect(params TokenType[] types)
    {
        if (nextIsType(types))
            return consumeNext();
        else
            throw new ParsingException(
                $"Expected tokens: {string.Join(", ", TokenTranslation.TokensToKeywords(types))}",
                peekNext().line
            );
    }

    Token expectStmtTerm()
    {
        return expect(COMMAND_TERMINATOR, EOF);
    }

    /// <summary>
    /// Skip to beginning of next statement.
    /// </summary>
    void synchronize() // if stmt invalid, skip to beginning of next statement
    {
        while (!nextIsType(COMMAND_TERMINATOR, EOF))
            consumeNext();

        consumeNext();
    }

    // statement parsing helpers
    /// <summary>
    /// Parse variable declaration statement with immediate value or type.
    /// </summary>
    VariableDeclareStmt? stictVariableDeclare(Token identifier) // declare variable with type or value
    {
        if (skipIfNextIsType(DECLARE_SET_VAR))
        {
            Expr value = expression();

            expectStmtTerm();
            return new VariableDeclareStmt(identifier, value: value);
        }

        if (skipIfNextIsType(DECLARE_TYPE_VAR))
        {
            Token type = expect(TI_INT, TI_STRING, TI_BOOL, TI_FLOAT);

            expectStmtTerm();
            return new VariableDeclareStmt(identifier, type: type.type);
        }

        return null;
    }

    // block statements - body of loops, function, ifs
    /// <summary>
    /// Parse block of statements.
    /// </summary>
    BlockStmt blockStatement(TokenType end)
    {
        expect(COMMAND_TERMINATOR);
        List<Stmt> statements = new();

        while (!nextIsType(end, EOF))
        {
            try
            {
                statements.Add(statement());
            }
            catch (ParsingException e)
            {
                ExceptionReporter.Log(e);
                executable = false;
                synchronize();
            }
        }
        expect(end);

        return new BlockStmt(statements.ToArray());
    }

    // statement parser
    /// <summary>
    /// Parse statement. See <see cref="Stmt"/>
    /// </summary>
    Stmt statement()
    {
        if (skipIfNextIsType(BEGIN)) // program
        {
            return blockStatement(END);
        }

        if (skipIfNextIsType(FUNC_BEGIN)) // function declaration
        {
            Token identifier = expect(T_IDENTIFIER);

            // parse arguemnts - AS EXPRESSION
            Token[] param = parameters();

            BlockStmt block = blockStatement(FUNC_END);

            expectStmtTerm();
            return new FunctionDeclareStmt(identifier, block, param);
        }

        if (nextIsType(RETURN_VAL, RETURN_NULL)) // function return
        {
            Token keyword = consumeNext();

            if (skipIfNextIsType(COMMAND_TERMINATOR, EOF)) // return NOOB
                return new ReturnStmt(keyword.line);

            Expr returnVal = expression(); // return value

            expectStmtTerm();
            return new ReturnStmt(keyword.line, returnVal);
        }

        if (skipIfNextIsType(IF)) // if statement
        {
            expect(COMMAND_TERMINATOR);
            expect(THEN);
            BlockStmt trueBlock = blockStatement(ELSE);
            BlockStmt falseBlock = blockStatement(END_IF);

            expectStmtTerm();
            return new IfStmt(trueBlock, falseBlock);
        }

        if (skipIfNextIsType(LOOP_BEGIN)) // loop statement
        {
            expect(DECLARE_VAR);
            Token identifier = expect(T_IDENTIFIER);
            VariableDeclareStmt? variable = stictVariableDeclare(identifier);

            if (variable is null)
                throw new SyntaxException(
                    "Expected variable declaration statement",
                    identifier.line
                );

            expect(WHILE);
            Expr condition = expression();
            BlockStmt block = blockStatement(LOOP_END);

            expectStmtTerm();
            return new LoopStmt(block, condition, variable);
        }

        if (skipIfNextIsType(DECLARE_VAR)) // variable declaration
        {
            Token identifier = expect(T_IDENTIFIER);

            VariableDeclareStmt? statement = stictVariableDeclare(identifier);

            if (statement is not null)
                return statement;

            expectStmtTerm();
            return new VariableDeclareStmt(identifier);
        }

        if (nextIsType(WRITE_STDOUT)) // printing
        {
            int line = consumeNext().line;
            List<Expr> content = new();

            while (!nextIsType(EOF, COMMAND_TERMINATOR, BANG))
            {
                content.Add(expression());
            }

            bool newline = skipIfNextIsType(BANG);

            expectStmtTerm();
            return new PrintStmt(newline, line, content.ToArray());
        }

        if (skipIfNextIsType(READ_STDIN)) // reading input
        {
            Token identifier = expect(T_IDENTIFIER);

            expectStmtTerm();
            return new InputStmt(identifier);
        }

        if (nextIsType(T_IDENTIFIER)) // assigning variable values
        {
            Token identifier = consumeNext();

            if (skipIfNextIsType(ASSIGN))
            {
                Expr right = expression();

                expectStmtTerm();
                return new VariableAssignStmt(identifier, right);
            }
            else
                --next; // roll back and let expr statement handle it
        }

        Expr expr = expression(); // expression-statements
        Token terminator = expectStmtTerm();
        return new ExpressionStmt(expr, terminator.line);
    }

    // expression parsing helpers
    /// <summary>
    /// Parse expression of unknown arity.
    /// </summary>
    /// <returns>Nested binary expression</returns>
    Expr naryExpr()
    {
        Token op = (consumeNext());
        Expr expr = expression();

        while (!nextIsType(END_INF, EOF, COMMAND_TERMINATOR))
        {
            expr = new BinaryExpr(op, expr, expression());
        }
        expect(END_INF);

        return expr;
    }

    /// <summary>
    /// Parse function declaration parameters list.
    /// </summary>
    /// <returns>List of <see cref="Token"/> to be used as parameter identifies inside function body</returns>
    Token[] parameters() // func declaration parameters
    {
        List<Token> parameters = new();

        while (!nextIsType(COMMAND_TERMINATOR, EOF))
        {
            expect(ARG);
            parameters.Add(expect(T_IDENTIFIER));
            skipIfNextIsType(AND);
        }

        return parameters.ToArray();
    }

    /// <summary>
    /// Parse function call arguments list.
    /// </summary>
    /// <returns>List of <see cref="Expr"/> to be evaluated and passed to function as arguments</returns>
    Expr[] arguments() // func call arguments
    {
        List<Expr> args = new();

        while (!nextIsType(END_INF, COMMAND_TERMINATOR, EOF))
        {
            expect(ARG);
            args.Add(expression());
            skipIfNextIsType(AND);
        }
        expect(END_INF);

        return args.ToArray();
    }

    //  expression parser
    /// <summary>
    /// Parse expression. See <see cref="Expr"/>
    /// </summary>
    Expr expression()
    {
        // skip optional AN separators
        skipIfNextIsType(AND);

        // function call
        if (skipIfNextIsType(FUNC_CALL))
        {
            Token name = expect(T_IDENTIFIER);
            Expr[] args = arguments();

            return new FunctionCallExpr(name, args);
        }

        // binary expressions
        if (
            nextIsType(
                PLUS,
                MINUS,
                TIMES,
                QUOTIENT,
                MOD,
                MAX,
                MIN,
                EQUAL,
                NOT_EQUAL,
                BOOL_AND,
                BOOL_OR,
                BOOL_XOR
            )
        )
            return new BinaryExpr(consumeNext(), expression(), expression());

        // unary expressions
        if (nextIsType(BOOL_NOT))
        {
            return new UnaryExpr(consumeNext(), expression());
        }

        // n-ary expressions
        if (nextIsType(BOOL_AND_INF, BOOL_OR_INF, CONCAT))
        {
            return naryExpr();
        }

        // variable dereferencing
        if (nextIsType(T_IDENTIFIER))
        {
            return new VariableExpr(consumeNext());
        }

        // literal expressions
        if (skipIfNextIsType(TRUE))
        {
            return new LiteralExpr(new BoolValue(true));
        }

        if (skipIfNextIsType(FALSE))
        {
            return new LiteralExpr(new BoolValue(false));
        }

        if (nextIsType(T_INT))
            return new LiteralExpr(new IntValue(int.Parse(consumeNext().text)));

        if (nextIsType(T_FLOAT))
            return new LiteralExpr(new FloatValue(float.Parse(consumeNext().text)));

        if (nextIsType(T_STRING))
            return new LiteralExpr(new StringValue(consumeNext().text[1..^1]));

        // invalid expression
        Token invalid = consumeNext();
        throw new ParsingException(
            $"Invalid expression, unexpected token {invalid.text}",
            invalid.line
        );
    }
}
