using System;
using System.Numerics;
using EvaluationUtils;
using Tokenization;
using static EvaluationUtils.EvalUtils;
using static Tokenization.TokenType;

namespace ExpressionDefinitions;

public interface Expr
{
    public object evaluate();
    public string ToString();
}

#pragma warning disable
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

    public object evaluate()
    {
        object firstEval = first.evaluate();
        object secondEval = second.evaluate();

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
                break;

            case MINUS:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x - y,
                    doubleOp: (x, y) => x - y,
                    opType: OperationType.ALGEBRAIC
                );
                break;

            case TIMES:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x * y,
                    doubleOp: (x, y) => x * y,
                    opType: OperationType.ALGEBRAIC
                );
                break;

            case QUOTIENT:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x / y,
                    doubleOp: (x, y) => x / y,
                    opType: OperationType.ALGEBRAIC
                );
                break;

            case MOD:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x % y,
                    doubleOp: (x, y) => x % y,
                    opType: OperationType.ALGEBRAIC
                );
                break;

            case MAX:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => Math.Max(x, y),
                    doubleOp: (x, y) => Math.Max(x, y),
                    opType: OperationType.ALGEBRAIC
                );
                break;

            case MIN:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => Math.Min(x, y),
                    doubleOp: (x, y) => Math.Min(x, y),
                    opType: OperationType.ALGEBRAIC
                );
                break;

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
                break;

            case BOOL_OR_INF:
            case BOOL_OR:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    boolOp: (x, y) => x || y,
                    opType: OperationType.BOOLEAN
                );
                break;

            case BOOL_XOR:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    boolOp: (x, y) => x ^ y,
                    opType: OperationType.BOOLEAN
                );
                break;

            // equalities
            case EQUAL:
                return equality(firstEval, secondEval);
                break;

            case NOT_EQUAL:
                return !equality(firstEval, secondEval);
                break;

            // invalid operator
            default:
                throw new InvalidOpException($"Invalid operator {op.text}", op.line);
                break;
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

    public object evaluate()
    {
        object firstEval = first.evaluate();

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
                break;

            // invalid operator
            default:
                throw new InvalidOpException($"Invalid operator {op.text}", op.line);
                break;
        }
    }

    public override string ToString() => $"{op.text} {first}";
}

public class LiteralExpr : Expr
{
    object literal;

    public LiteralExpr(object literal)
    {
        this.literal = literal;
    }

    public object evaluate()
    {
        return literal;
    }

    public override string ToString() => literal.ToString();
}
