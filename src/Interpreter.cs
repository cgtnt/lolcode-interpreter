using ParsingPrimitives;
using ScopeDefinition;

namespace Interpretation;

public class Interpreter
{
    private Scope programScope = new Scope();

    public void Interpret(Stmt statement)
    {
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
