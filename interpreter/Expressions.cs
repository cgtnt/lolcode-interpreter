using System;
using Tokenization;

namespace ExpressionDefinitions;

public interface Expr
{
    public Object evaluate();
    public string print();
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

    public string print() => $"{op.text} {first.print()} {second.print()}";
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

    public string print() => $"{op.text} {first.print()}";
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

    public string print() => literal.ToString();
}

public class InvalidExpr : Expr
{
    public Object evaluate()
    {
        Console.WriteLine(print());
        return null;
    }

    public string print() => "INVALID_EXPR";
}
