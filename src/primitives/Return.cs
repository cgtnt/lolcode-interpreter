using System;
using TypePrimitives;

namespace InterpretationPrimitives;

public class ReturnValue : Exception
{
    public Value value { get; private set; }
    public int line { get; private set; }

    /// <summary>
    /// Used to unwind callstack when evaluating a LOLCODE function and return a value. Do not use outside of function body.
    /// </summary>
    public ReturnValue(Value value, int line)
        : base()
    {
        this.value = value;
        this.line = line;
    }
}
