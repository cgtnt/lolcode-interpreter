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

        do
        {
            AST = expression();
            Console.WriteLine(AST.print());
        } while (!atEOF());

        return AST; // FIXME: update this
    }

    // parsing helpers
    Token consumeNextToken() => tokens[next++];

    Token peekNextToken() => tokens[next];

    bool isType(params TokenType[] types) => types.Any(t => peekNextToken().type == t);

    bool atEOF() => tokens[next].type == TokenType.EOF;

    // grammar rule helpers
    Expr expression()
    {
        if (isType(AND))
            consumeNextToken(); // FIXME: Temporary. shouldnt beignored
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
        {
            return new BinaryExpr(consumeNextToken(), expression(), expression());
        }

        // unary expressions
        if (isType(BOOL_NOT, INCREMENT, DECREMENT))
        {
            return new UnaryExpr(consumeNextToken(), expression());
        }

        // literal expressions
        if (isType(TRUE))
            return new LiteralExpr(true);

        if (isType(FALSE))
            return new LiteralExpr(false);

        if (isType(T_INT))
            return new LiteralExpr(int.Parse(consumeNextToken().text));

        if (isType(T_FLOAT))
            return new LiteralExpr(double.Parse(consumeNextToken().text));

        if (isType(T_STRING))
            return new LiteralExpr(consumeNextToken().text);

        consumeNextToken();
        return new InvalidExpr();
    }
}
