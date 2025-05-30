using System;
using Tokenization;

namespace ExpressionDefinitions;

public interface Expr
{
    public Object evaluate();
}

public class BinaryExpr : Expr
{
    Token op;
    Expr first;
    Expr second;

    public BinaryExpr(Token op, Expr first, Expr second)
    {
        this.op = op;
        this.first = first;
        this.second = second;
    }

    public Object evaluate()
    {
        return null;
    }
}

public class UnaryExpr : Expr
{
    Token op;
    Expr first;

    public UnaryExpr(Token op, Expr first)
    {
        this.op = op;
        this.first = first;
    }

    public Object evaluate()
    {
        return null;
    }
}

public class LiteralExpr : Expr
{
    Object literal;

    public LiteralExpr(Object literal)
    {
        this.literal = literal;
    }

    public Object evaluate()
    {
        return literal;
    }
}
