using System;
using EvaluationUtils;
using InterpretationPrimitives;
using TokenizationPrimitives;
using TypePrimitives;
using static EvaluationUtils.EvalUtils;
using static TokenizationPrimitives.TokenType;

namespace ASTPrimitives;

public interface Expr
{
    public Value evaluate(Scope s);
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

    public Value evaluate(Scope scope)
    {
        Value firstEval = first.evaluate(scope);
        Value secondEval = second.evaluate(scope);

        switch (op.type)
        {
            case PLUS:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x + y,
                    floatOp: (x, y) => x + y,
                    opType: OperationType.ALGEBRAIC
                );

            case MINUS:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x - y,
                    floatOp: (x, y) => x - y,
                    opType: OperationType.ALGEBRAIC
                );

            case TIMES:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x * y,
                    floatOp: (x, y) => x * y,
                    opType: OperationType.ALGEBRAIC
                );

            case QUOTIENT:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x / y,
                    floatOp: (x, y) => x / y,
                    opType: OperationType.ALGEBRAIC
                );

            case MOD:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => x % y,
                    floatOp: (x, y) => x % y,
                    opType: OperationType.ALGEBRAIC
                );

            case MAX:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => Math.Max(x, y),
                    floatOp: (x, y) => Math.Max(x, y),
                    opType: OperationType.ALGEBRAIC
                );

            case MIN:
                return TryExecuteOp(
                    op,
                    firstEval,
                    secondEval,
                    intOp: (x, y) => Math.Min(x, y),
                    floatOp: (x, y) => Math.Min(x, y),
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
                return new BoolValue(!equality(firstEval, secondEval).Value);

            // invalid operator
            default:
                throw new InvalidOpException($"Invalid operator {op.text}", op.line);
        }
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

    public Value evaluate(Scope scope)
    {
        Value firstEval = first.evaluate(scope);

        switch (op.type)
        {
            case BOOL_NOT:
                return TryExecuteOp(
                    op,
                    firstEval,
                    new BoolValue(false),
                    boolOp: (x, y) => !x,
                    opType: OperationType.BOOLEAN
                );

            // invalid operator
            default:
                throw new InvalidOpException($"Invalid operator {op.text}", op.line);
        }
    }
}

public class VariableExpr : Expr
{
    string name;
    int line;

    public VariableExpr(Token identifier)
    {
        name = identifier.text;
        line = identifier.line;
    }

    public Value evaluate(Scope scope)
    {
        return scope.GetVar(name);
    }
}

public class FunctionCallExpr : Expr
{
    string name;
    int line;
    Expr[] arguments;

    public FunctionCallExpr(Token identifier, Expr[] arguments)
    {
        name = identifier.text;
        line = identifier.line;
        this.arguments = arguments;
    }

    public Value evaluate(Scope scope)
    {
        FunctionValue? function = scope.GetVar(name) as FunctionValue;

        if (function is null)
            throw new InvalidTypeException($"Cannot call {name}, not a function", line);

        Scope localScope = new Scope(scope);

        if (function.ParametersCount != arguments.Length)
            throw new SyntaxException($"Invalid number of arguments provided to {name}", line);

        for (int i = 0; i < function.ParametersCount; ++i)
            localScope.DefineVar(function.parameters[i], arguments[i].evaluate(scope));

        try
        {
            function.block.evaluate(localScope);
            throw new ReturnValue(localScope.GetVar("IT")); // if the block doesn't return anything, return implicit IT
        }
        catch (ReturnValue v)
        {
            return v.value;
        }
    }
}

public class LiteralExpr : Expr
{
    Value literal;

    public LiteralExpr(Value val)
    {
        this.literal = val;
    }

    public Value evaluate(Scope _)
    {
        return literal;
    }
}
