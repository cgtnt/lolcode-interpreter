using System;
using TypePrimitives;

namespace InterpretationPrimitives;

public class ReturnValue : Exception
{
    public Value value { get; private set; }

    public ReturnValue(Value value)
        : base()
    {
        this.value = value;
    }
}
