using System;
using ExpressionDefinitions;

namespace Interpretation;

#pragma warning disable
public class Interpreter
{
    public object Interpret(Expr block)
    {
        try
        {
            return block.evaluate();
        }
        catch (RuntimeException e)
        {
            ExceptionReporter.Log(e);
            return null;
        }
    }
}
