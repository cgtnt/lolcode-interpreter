using System;
using System.Numerics;
using Tokenization;

namespace EvaluationUtils;

using static OperationType;
using BoolOp = Func<bool, bool, bool>;
using DoubleOp = Func<double, double, object>;
using IntOp = Func<int, int, object>;
using StringOp = Func<string, string, string>;

public enum OperationType
{
    ALGEBRAIC,
    BOOLEAN,
    STRING,
    UNIVERSAL,
}

// TODO: add untyped vars
public static class Typecaster
{
    public static bool TryCastBool(object value) //FIXME: IMplement casting, this doesnt work
    // https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#types
    {
        if (value is not bool && value is not float && value is not int && value is not string)
            throw new InvalidTypeException();

        return (bool)value;
    }

    public static int TryCastInt(object value)
    {
        if (value is not bool && value is not float && value is not int && value is not string)
            throw new InvalidTypeException();

        return (int)value;
    }

    public static double TryCastDouble(object value)
    {
        if (value is not bool && value is not float && value is not int && value is not string)
            throw new InvalidTypeException();

        return (double)value;
    }

    public static string TryCastString(object value)
    {
        if (value is not bool && value is not float && value is not int && value is not string)
            throw new InvalidTypeException();

        return (string)value;
    }
}

public static class EvalUtils
{
    public static object TryExecuteOp(
        Token oper,
        object first,
        object second,
        OperationType opType,
        IntOp? intOp = null,
        DoubleOp? doubleOp = null,
        BoolOp? boolOp = null,
        StringOp? stringOp = null
    )
    {
        try
        {
            if (opType == ALGEBRAIC && intOp is not null && doubleOp is not null)
                return algebraicOperation(first, second, intOp, doubleOp);
            if (opType == STRING && stringOp is not null)
                return stringOperation(first, second, stringOp);
            if (opType == BOOLEAN && boolOp is not null)
                return booleanOperation(first, second, boolOp);

            throw new InvalidTypeException();
        }
        catch (InvalidTypeException)
        {
            throw new InvalidTypeException(
                $"Cannot {oper.text} types {first.GetType()} and {second.GetType()}",
                oper.line
            );
        }
    }

    public static object algebraicOperation<T, U>(T first, U second, IntOp intOp, DoubleOp doubleOp)
    {
        return (first, second) switch
        {
            (int one, int two) => intOp(one, two),
            (double one, double two) => doubleOp(one, two),
            (int one, double two) => doubleOp(one, two),
            (double one, int two) => doubleOp(one, two),
            _ => doubleOp(Typecaster.TryCastDouble(first), Typecaster.TryCastDouble(second)),
        };
    }

    public static bool booleanOperation<T, U>(T first, U second, BoolOp boolOp)
    {
        return (first, second) switch
        {
            (bool one, bool two) => boolOp(one, two),
            _ => boolOp(Typecaster.TryCastBool(first), Typecaster.TryCastBool(second)),
        };
    }

    public static string stringOperation<T, U>(T first, U second, StringOp stringOp)
    {
        return (first, second) switch
        {
            (string one, string two) => stringOp(one, two),
            _ => stringOp(Typecaster.TryCastString(first), Typecaster.TryCastString(second)),
        };
    }

    public static bool equality<T, U>(T first, U second)
    {
        if (first is null && second is null)
            return true;
        if (first is null || second is null)
            return false;

        return (first.GetType() == second.GetType() && ((object)first).Equals((object)second)); // FIXME: incorrect value
    }
}
