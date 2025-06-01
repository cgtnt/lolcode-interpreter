using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionDefinitions;
using StatementDefinitions;
using Tokenization;
using static Tokenization.TokenType;

namespace Parsing;

public class Parser
{
    List<Token> tokens;
    int next;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public List<Stmt> Parse()
    {
        List<Stmt> statements = new();

        while (!atEOF())
        {
            try
            {
                statements.Add(statement());
            }
            catch (ParsingException e)
            {
                ExceptionReporter.Log(e);
                synchronize();
            }
        }

        return statements;
    }

    // parsing helpers
    Token consumeNext()
    {
        Debug.Log($"Consuming: {tokens[next]}");
        return tokens[next++];
    }

    Token expect(params TokenType[] types)
    {
        if (isType(types))
            return consumeNext();
        else
            throw new ParsingException(
                $"Expected tokens: {string.Join("", types)}",
                peekNext().line
            );
    }

    bool skipNextType(params TokenType[] types)
    {
        if (isType(types))
        {
            consumeNext();
            return true;
        }

        return false;
    }

    Token peekNext() => tokens[next];

    bool isType(params TokenType[] types) => types.Any(t => peekNext().type == t);

    bool atEOF() => tokens[next].type == EOF;

    void synchronize()
    {
        while (!isType(COMMAND_TERMINATOR))
            consumeNext();

        consumeNext();
    }

    // statement parser
    Stmt statement()
    {
        if (skipNextType(DECLARE_VAR))
        {
            Token identifier = expect(T_IDENTIFIER);
            Token type;
            Expr value;

            if (skipNextType(DECLARE_SET_VAR))
            {
                value = expression();
                expect(COMMAND_TERMINATOR, EOF);

                return new VariableDeclareStmt(identifier, value: value);
            }

            if (skipNextType(DECLARE_TYPE_VAR))
            {
                type = expect(TI_INT, TI_STRING, TI_BOOL, TI_FLOAT, TI_UNTYPED);
                expect(COMMAND_TERMINATOR, EOF);

                return new VariableDeclareStmt(identifier, type: type.type);
            }

            expect(COMMAND_TERMINATOR, EOF);
            return new VariableDeclareStmt(identifier);
        }

        if (skipNextType(WRITE_STDOUT))
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

        Expr expr = expression();
        expect(EOF, COMMAND_TERMINATOR);
        return new ExpressionStmt(expr);
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
            return new LiteralExpr(true);
        }

        if (skipNextType(FALSE))
        {
            return new LiteralExpr(false);
        }

        if (isType(T_INT))
            return new LiteralExpr(int.Parse(consumeNext().text));

        if (isType(T_FLOAT))
            return new LiteralExpr(double.Parse(consumeNext().text));

        if (isType(T_STRING))
            return new LiteralExpr(consumeNext().text[1..^1]);

        // invalid expression
        Token invalid = consumeNext();
        throw new ParsingException(
            $"Invalid expression, unexpected token {invalid.text}",
            invalid.line
        );
    }

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
}
