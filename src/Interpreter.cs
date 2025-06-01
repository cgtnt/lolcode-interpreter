using System;
using System.Collections.Generic;
using ExpressionDefinitions;
using ScopeDefinition;
using StatementDefinitions;

namespace Interpretation;

#pragma warning disable
public class Interpreter
{
    private Scope programScope = new Scope();

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (Stmt statement in statements)
                statement.evaluate(programScope); // TODO: scope handling
        }
        catch (RuntimeException e)
        {
            ExceptionReporter.Log(e);
        }
    }
}
