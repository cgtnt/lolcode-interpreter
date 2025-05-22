using System;
using Tokenization;

namespace ExpressionDefinitions;

public interface Expr
{
    public Object evaluate();
}

public class BinaryExpr : Expr
{
    Expr left;
    Expr right;
    Token op;

    public Object evaluate()
    {
        return null;
    }
}

public class UnaryExpr : Expr
{
    Expr expr;
    Token op;

    public Object evaluate()
    {
        return null;
    }
}

public class LiteralExpr : Expr
{
    Object literal;

    public Object evaluate()
    {
        return literal;
    }
}
