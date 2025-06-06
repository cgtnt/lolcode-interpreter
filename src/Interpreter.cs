using ScopeDefinition;
using StatementDefinitions;

namespace Interpretation;

public class Interpreter
{
    private Scope programScope = new Scope();

    public void Interpret(Stmt statement)
    {
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
