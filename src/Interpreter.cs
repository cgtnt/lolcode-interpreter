using InterpretationPrimitives;
using ParsingPrimitives;

namespace Interpretation;

public class Interpreter
{
    public void Interpret(Stmt statement)
    {
        Scope programScope = new Scope();

        try
        {
            statement.evaluate(programScope); // TODO: make sure line of runtime exceptions is printed
        }
        catch (RuntimeException e)
        {
            ExceptionReporter.Log(e);
        }
    }
}
