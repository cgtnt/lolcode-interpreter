using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionDefinitions;
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

    public Expr Parse()
    {
        Expr AST;
        AST = expression();

        return AST; // FIXME: update this
    }

    // parsing helpers
    Token consumeNext()
    {
        Debug.Log($"Consuming: {tokens[next]}");
        return tokens[next++];
    }

    bool consumeNextIf(out Token token, params TokenType[] types)
    {
        if (isType(types))
        {
            token = consumeNext();
            return true;
        }
        else
        {
            token = null;
            return false;
        }
    }

    Token peekNext() => tokens[next];

    bool isType(params TokenType[] types) => types.Any(t => peekNext().type == t);

    bool atEOF() => tokens[next].type == EOF;

    // grammar rule helpers
    Expr expression()
    {
        if (isType(AND))
            consumeNext(); // FIXME: Temporary. shouldnt beignored

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
        if (isType(BOOL_NOT, INCREMENT, DECREMENT))
        {
            return new BinaryExpr(consumeNext(), expression(), expression());
        }

        // n-ary expressions
        if (isType(BOOL_AND_INF, BOOL_OR_INF))
        {
            return naryExpr();
        }

        // literal expressions
        if (isType(TRUE))
        {
            consumeNext();
            return new LiteralExpr(true);
        }

        if (isType(FALSE))
        {
            consumeNext();
            return new LiteralExpr(false);
        }

        if (isType(T_INT))
            return new LiteralExpr(int.Parse(consumeNext().text));

        if (isType(T_FLOAT))
            return new LiteralExpr(double.Parse(consumeNext().text));

        if (isType(T_STRING))
            return new LiteralExpr(consumeNext().text);

        // invalid expression
        throw new ErrorReporter.SyntaxError("Invalid expression", consumeNext().line);
    }

    Expr naryExpr()
    {
        Token op = consumeNext();
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
            throw new ErrorReporter.SyntaxError($"Expected 'MKAY' to terminate {op.text}", op.line);
        }
    }
}
