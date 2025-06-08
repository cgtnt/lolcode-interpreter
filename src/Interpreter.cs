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
        catch (ReturnValue v)
        {
            ExceptionReporter.Log(
                new RuntimeException("Cannot FOUND YR or GTFO from main program block", v.line)
            );
        }
        catch (RuntimeException e)
        {
            ExceptionReporter.Log(e);
        }
    }
}
