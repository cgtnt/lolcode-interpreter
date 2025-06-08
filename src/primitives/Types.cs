using System;
using ASTPrimitives;
using TokenizationPrimitives;
using static TokenizationPrimitives.TokenType;

namespace TypePrimitives;

public abstract record Value
{
    public abstract object? RawValue { get; }
}

// types
public record IntValue(int Value = 0) : Value
{
    public override object RawValue => Value;

    public override string ToString() => Value.ToString();
}

public record FloatValue(float Value = 0) : Value
{
    public override object RawValue => Value;

    public override string ToString() => Value.ToString();
}

public record StringValue(string Value = "") : Value
{
    public override object RawValue => Value;

    public override string ToString() => Value.ToString();
}

public record BoolValue(bool Value = false) : Value
{
    public override object RawValue => Value;

    public override string ToString() => Value.ToString();
}

public record UntypedValue() : Value
{
    public override object? RawValue => null;

    public override string ToString() => "NOOB";
}

// function type
public record FunctionValue(BlockStmt block, string[] parameters) : Value
{
    public override object RawValue => $"function({string.Join(", ", parameters)})";

    public int ParametersCount => parameters.Length;
}

// implicit type casting
public static class TypeCaster
{
    public static string GetValueType(Value value)
    {
        switch (value)
        {
            case BoolValue v:
                return TokenTranslation.TokensToKeywords(TI_BOOL)[0];
            case UntypedValue v:
                return TokenTranslation.TokensToKeywords(TI_UNTYPED)[0];
            case StringValue v:
                return TokenTranslation.TokensToKeywords(TI_STRING)[0];
            case IntValue v:
                return TokenTranslation.TokensToKeywords(TI_INT)[0];
            case FloatValue v:
                return TokenTranslation.TokensToKeywords(TI_FLOAT)[0];
            case FunctionValue v:
                return "FUNCTION";
            default:
                throw new CriticalError(
                    new TypeCastingException($"Unknown type {value.GetType()}")
                );
        }
    }

    public static BoolValue CastBool(Value value)
    {
        switch (value)
        {
            case BoolValue v:
                return v;
            case UntypedValue v:
                return new BoolValue(false);
            case StringValue v:
                return new BoolValue(v.Value != "");
            case IntValue v:
                return new BoolValue(v.Value != 0);
            case FloatValue v:
                return new BoolValue(v.Value != 0);
            default:
                throw new TypeCastingException($"Cannot cast {GetValueType(value)} to TROOF");
        }
    }

    public static IntValue CastInt(Value value)
    {
        switch (value)
        {
            case IntValue v:
                return v;
            case StringValue v:
                if (int.TryParse(v.Value, out int val))
                    return new IntValue(val);
                else
                    throw new TypeCastingException($"Cannot cast {v.Value} to NUMBR");
            case FloatValue v:
                return new IntValue((int)v.Value);
            case BoolValue v:
                return new IntValue(v.Value ? 1 : 0);
            default:
                throw new TypeCastingException($"Cannot cast {GetValueType(value)} to NUMBR");
        }
    }

    public static FloatValue CastFloat(Value value)
    {
        switch (value)
        {
            case FloatValue v:
                return v;
            case StringValue v:
                if (float.TryParse(v.Value, out float val))
                    return new FloatValue(val);
                else
                    throw new TypeCastingException($"Cannot cast {v.Value} to NUMBAR");
            case IntValue v:
                return new FloatValue((float)v.Value);
            case BoolValue v:
                return new FloatValue(v.Value ? 1 : 0);
            default:
                throw new TypeCastingException($"Cannot cast {GetValueType(value)} to NUMBAR");
        }
    }

    public static StringValue CastString(Value value)
    {
        switch (value)
        {
            case StringValue v:
                return v;
            case IntValue v:
                return new StringValue(v.Value.ToString());
            case FloatValue v:
                return new StringValue(v.Value.ToString("F2"));
            case BoolValue v:
                return new StringValue(v.Value ? "WIN" : "FAIL");
            default:
                throw new TypeCastingException($"Cannot cast {GetValueType(value)} to YARN");
        }
    }
}

public class TypeChecker
{
    public static bool Equals(Type one, Type two)
    {
        return one == two;
    }

    public static bool Matches(Type one, Type[] targets)
    {
        foreach (Type t in targets)
            if (one == t)
                return true;

        return false;
    }
}
