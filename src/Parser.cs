using System.Collections.Generic;
using System.Linq;
using ParsingPrimitives;
using Tokenization;
using TypePrimitives;
using static Tokenization.TokenType;

namespace Parsing;

public class Parser
{
    List<Token> tokens;
    int next;
    bool executable = true;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public bool Parse(out Stmt program)
    {
        while (isType(COMMAND_TERMINATOR))
            ++next;

        if (!isType(BEGIN))
        {
            executable = false;
            ExceptionReporter.Log(new ParsingException("Program must start with HAI", 1));
            synchronize();
        }

        program = statement();
        return executable;
    }

    // parsing helpers
    Token consumeNext() // TODO: change to arrow notation
    {
        Debug.Log($"Consuming: {tokens[next]}");
        return tokens[next++];
    }

    Token peekNext() => tokens[next];

    bool atEOF() => tokens[next].type == EOF;

    bool isType(params TokenType[] types) => types.Any(t => peekNext().type == t);

    bool skipNextType(params TokenType[] types)
    {
        if (isType(types))
        {
            consumeNext();
            return true;
        }

        return false;
    }

    Token expect(params TokenType[] types)
    {
        if (isType(types))
            return consumeNext();
        else
            throw new ParsingException(
                $"Expected tokens: {string.Join(", ", types)}",
                peekNext().line
            );
    }

    void synchronize()
    {
        while (!isType(COMMAND_TERMINATOR))
            consumeNext();

        consumeNext();
    }

    // statement parsing helpers
    VariableDeclareStmt? stictVariableDeclare(Token identifier)
    {
        if (skipNextType(DECLARE_SET_VAR))
        {
            Expr value = expression();
            expect(COMMAND_TERMINATOR, EOF);

            return new VariableDeclareStmt(identifier, value: value);
        }

        if (skipNextType(DECLARE_TYPE_VAR))
        {
            Token type = expect(TI_INT, TI_STRING, TI_BOOL, TI_FLOAT);
            expect(COMMAND_TERMINATOR, EOF);

            return new VariableDeclareStmt(identifier, type: type.type);
        }

        return null;
    }

    BlockStmt blockStatement(TokenType end)
    {
        expect(COMMAND_TERMINATOR);
        List<Stmt> statements = new();

        while (!isType(end, EOF))
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
    Stmt statement()
    {
        if (skipNextType(BEGIN)) // program
        {
            return blockStatement(END);
        }

        if (skipNextType(IF)) // if statement
        {
            expect(COMMAND_TERMINATOR);
            expect(THEN);
            BlockStmt trueBlock = blockStatement(ELSE);
            BlockStmt falseBlock = blockStatement(END_IF);
            expect(COMMAND_TERMINATOR, EOF);

            return new IfStmt(trueBlock, falseBlock);
        }

        if (skipNextType(LOOP_BEGIN)) // loop statement
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
            expect(COMMAND_TERMINATOR);

            return new LoopStmt(block, condition, variable);
        }

        if (skipNextType(DECLARE_VAR)) // variable declaration
        {
            Token identifier = expect(T_IDENTIFIER);

            VariableDeclareStmt? statement = stictVariableDeclare(identifier);

            if (statement is not null)
                return statement;

            expect(COMMAND_TERMINATOR, EOF);
            return new VariableDeclareStmt(identifier);
        }

        if (skipNextType(WRITE_STDOUT)) // printing
        {
            List<Expr> content = new();

            while (!isType(EOF, COMMAND_TERMINATOR, BANG))
            {
                content.Add(expression());
            }

            bool newline = skipNextType(BANG);
            expect(EOF, COMMAND_TERMINATOR);

            return new PrintStmt(newline, content.ToArray());
        }

        if (skipNextType(READ_STDIN)) // reading input
        {
            Token identifier = expect(T_IDENTIFIER);
            expect(EOF, COMMAND_TERMINATOR);
            return new InputStmt(identifier);
        }

        if (isType(T_IDENTIFIER)) // assigning variable values
        {
            Token identifier = consumeNext();

            if (skipNextType(ASSIGN))
            {
                Expr right = expression();
                expect(EOF, COMMAND_TERMINATOR);
                return new VariableAssignStmt(identifier, right);
            }
            else
                --next; // roll back and let expr statement handle it
        }

        Expr expr = expression(); // expression-statements
        expect(EOF, COMMAND_TERMINATOR);
        return new ExpressionStmt(expr);
    }

    // expression parsing helpers
    Expr naryExpr()
    {
        Token op = (consumeNext());
        Expr expr = expression();

        while (!isType(END_INF, EOF, COMMAND_TERMINATOR))
        {
            expr = new BinaryExpr(op, expr, expression());
        }

        if (isType(END_INF))
        {
            consumeNext();
            return expr;
        }
        else
        {
            throw new ParsingException($"Expected 'MKAY' to terminate {op.text}", op.line);
        }
    }

    //  expression parser
    Expr expression()
    {
        skipNextType(AND);

        // binary expressions
        if (
            isType(
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
        if (isType(BOOL_NOT)) // TODO: increment, decremeent?
        {
            return new BinaryExpr(consumeNext(), expression(), expression());
        }

        // n-ary expressions
        if (isType(BOOL_AND_INF, BOOL_OR_INF, CONCAT))
        {
            return naryExpr();
        }

        // variable dereferencing
        if (isType(T_IDENTIFIER))
        {
            return new VariableExpr(consumeNext());
        }

        // literal expressions
        if (skipNextType(TRUE))
        {
            return new LiteralExpr(new BoolValue(true));
        }

        if (skipNextType(FALSE))
        {
            return new LiteralExpr(new BoolValue(false));
        }

        if (isType(T_INT))
            return new LiteralExpr(new IntValue(int.Parse(consumeNext().text)));

        if (isType(T_FLOAT))
            return new LiteralExpr(new FloatValue(float.Parse(consumeNext().text)));

        if (isType(T_STRING))
            return new LiteralExpr(new StringValue(consumeNext().text[1..^1]));

        // invalid expression
        Token invalid = consumeNext();
        throw new ParsingException(
            $"Invalid expression, unexpected token {invalid.text}",
            invalid.line
        );
    }
}
