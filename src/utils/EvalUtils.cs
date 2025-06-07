using System;
using TokenizationPrimitives;
using TypePrimitives;

namespace EvaluationUtils;

using static OperationType;
using BoolOp = Func<bool, bool, bool>;
using FloatOp = Func<float, float, float>;
using IntOp = Func<int, int, int>;
using StringOp = Func<string, string, string>;

public enum OperationType
{
    ALGEBRAIC,
    BOOLEAN,
    STRING,
    UNIVERSAL,
}

public static class EvalUtils
{
    public static Value TryExecuteOp(
        Token oper,
        Value first,
        Value second,
        OperationType opType,
        IntOp? intOp = null,
        FloatOp? floatOp = null,
        BoolOp? boolOp = null,
        StringOp? stringOp = null
    )
    {
        try
        {
            if (opType == ALGEBRAIC && intOp is not null && floatOp is not null)
                return algebraicOperation(first, second, intOp, floatOp);
            if (opType == STRING && stringOp is not null)
                return stringOperation(first, second, stringOp);
            if (opType == BOOLEAN && boolOp is not null)
                return booleanOperation(first, second, boolOp);

            throw new InvalidOpException("Invalid operation", oper.line);
        }
        catch (InvalidTypeException)
        {
            throw new InvalidTypeException(
                $"Cannot {oper.text} types {first.GetType()} and {second.GetType()}",
                oper.line
            );
        }
    }

    public static Value algebraicOperation(Value first, Value second, IntOp intOp, FloatOp floatOp)
    {
        return (first, second) switch
        {
            (IntValue one, IntValue two) => new IntValue(intOp(one.Value, two.Value)),
            _ => new FloatValue(
                floatOp(TypeCaster.CastFloat(first).Value, TypeCaster.CastFloat(second).Value)
            ),
        };
    }

    public static BoolValue booleanOperation(Value first, Value second, BoolOp boolOp)
    {
        return new BoolValue(
            (first, second) switch
            {
                (BoolValue one, BoolValue two) => boolOp(one.Value, two.Value),
                _ => boolOp(TypeCaster.CastBool(first).Value, TypeCaster.CastBool(second).Value),
            }
        );
    }

    public static StringValue stringOperation(Value first, Value second, StringOp stringOp)
    {
        return new StringValue(
            (first, second) switch
            {
                (StringValue one, StringValue two) => stringOp(one.Value, two.Value),
                _ => stringOp(
                    TypeCaster.CastString(first).Value,
                    TypeCaster.CastString(second).Value
                ),
            }
        );
    }

    public static BoolValue equality(Value first, Value second)
    {
        return new BoolValue(
            (first, second) switch
            {
                (BoolValue one, BoolValue two) => one.Value == two.Value,
                (StringValue one, StringValue two) => one.Value == two.Value,
                (IntValue one, IntValue two) => one.Value == two.Value,
                (FloatValue one, FloatValue two) => one.Value == two.Value,
                _ => false,
            }
        );
    }
}
