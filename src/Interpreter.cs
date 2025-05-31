using System;
using ExpressionDefinitions;

namespace Interpretation;

public class Interpreter
{
    public void Interpret(Expr block)
    {
        try
        {
            object value = block.evaluate();
            Console.WriteLine(value);
        }
        catch (RuntimeException e)
        {
            ExceptionReporter.Log(e);
        }
    }
}
