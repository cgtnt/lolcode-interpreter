using System;
using TypePrimitives;

namespace InterpretationPrimitives;

public class ReturnValue : Exception
{
    public Value value { get; private set; }
    public int line { get; private set; }

    public ReturnValue(Value value, int line)
        : base()
    {
        this.value = value;
        this.line = line;
    }
}
