namespace TypeDefinitions;

public abstract record Value
{
    public abstract object? RawValue { get; }
}

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

// implicit type casting
public static class TypeCaster
{
    // https://github.com/justinmeza/lolcode-spec/blob/master/v1.3/lolcode-spec-v1.3.md#types
    public static BoolValue TryCastBool(Value value)
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
                throw new InvalidTypeException();
        }
    }

    public static IntValue TryCastInt(Value value)
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
                throw new InvalidTypeException();
        }
    }

    public static FloatValue TryCastFloat(Value value)
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
                throw new InvalidTypeException();
        }
    }

    public static StringValue TryCastString(Value value)
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
                throw new InvalidTypeException();
        }
    }
}
