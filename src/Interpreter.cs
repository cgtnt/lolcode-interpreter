using ASTPrimitives;
using InterpretationPrimitives;

namespace Interpretation;

public class Interpreter
{
    public void Interpret(Stmt statement)
    {
        Scope programScope = new Scope();

        try
        {
            statement.evaluate(programScope);
        }
        catch (RuntimeException e)
        {
            ExceptionReporter.Log(e);
        }
    }
}
