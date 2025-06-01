using System;
using EvaluationUtils;
using ScopeDefinition;
using Tokenization;
using static EvaluationUtils.EvalUtils;
using static Tokenization.TokenType;

namespace ExpressionDefinitions;

public interface Expr
{
    public object evaluate(Scope s);
    public string ToString();
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

    public object evaluate(Scope scope)
    {
        object firstEval = first.evaluate(scope);
        object secondEval = second.evaluate(scope);

        switch (op.type)
        {
            case PLUS:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x + y,
                    doubleOp: (x, y) => x + y,
                    opType: OperationType.ALGEBRAIC
                );

            case MINUS:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x - y,
                    doubleOp: (x, y) => x - y,
                    opType: OperationType.ALGEBRAIC
                );

            case TIMES:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x * y,
                    doubleOp: (x, y) => x * y,
                    opType: OperationType.ALGEBRAIC
                );

            case QUOTIENT:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x / y,
                    doubleOp: (x, y) => x / y,
                    opType: OperationType.ALGEBRAIC
                );

            case MOD:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x % y,
                    doubleOp: (x, y) => x % y,
                    opType: OperationType.ALGEBRAIC
                );

            case MAX:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => Math.Max(x, y),
                    doubleOp: (x, y) => Math.Max(x, y),
                    opType: OperationType.ALGEBRAIC
                );

            case MIN:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => Math.Min(x, y),
                    doubleOp: (x, y) => Math.Min(x, y),
                    opType: OperationType.ALGEBRAIC
                );

            // boolean operations
            case BOOL_AND_INF:
            case BOOL_AND:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    boolOp: (x, y) => x && y,
                    opType: OperationType.BOOLEAN
                );

            case BOOL_OR_INF:
            case BOOL_OR:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    boolOp: (x, y) => x || y,
                    opType: OperationType.BOOLEAN
                );

            case BOOL_XOR:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    boolOp: (x, y) => x ^ y,
                    opType: OperationType.BOOLEAN
                );

            // string operations
            case CONCAT:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    stringOp: (x, y) => x + y,
                    opType: OperationType.STRING
                );

            // equalities
            case EQUAL:
                return equality(firstEval, secondEval);

            case NOT_EQUAL:
                return !equality(firstEval, secondEval);

            // invalid operator
            default:
                throw new InvalidOpException($"Invalid operator {op.text}", op.line);
        }
    }

    public override string ToString() => $"{op.text} {first} {second}";
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

    public object evaluate(Scope scope)
    {
        object firstEval = first.evaluate(scope);

        switch (op.type)
        {
            case BOOL_NOT:
                return TryExecuteOp(
                    op,
                    firstEval,
                    false,
                    boolOp: (x, y) => !x,
                    opType: OperationType.BOOLEAN
                );

            // invalid operator
            default:
                throw new InvalidOpException($"Invalid operator {op.text}", op.line);
        }
    }

    public override string ToString() => $"{op.text} {first}";
}

public class VariableExpr : Expr
{
    Token name;

    public VariableExpr(Token name)
    {
        this.name = name;
    }

    public object evaluate(Scope scope)
    {
        Scope? s = scope;

        while (s is not null)
        {
            if (s.TryGetVar(name.text, out object value))
                return value;
            else
                s = scope.parent;
        }

        throw new UninitializedVarExcetion(
            $"Accessing unitiliazed variable {name.text}",
            name.line
        );
    }

    public override string ToString() => $"VAR: {name}";
}

public class LiteralExpr : Expr
{
    object literal;

    public LiteralExpr(object literal)
    {
        this.literal = literal;
    }

    public object evaluate(Scope _)
    {
        return literal;
    }

    public override string ToString() => $"{literal}";
}
