using System.Collections.Generic;
using EvaluationUtils;
using TypeDefinitions;

namespace ScopeDefinition;

public class Scope
{
    public Scope? parent { get; private set; }
    Dictionary<string, Value> variables;

    public Scope()
    {
        variables = new();
    }

    public void DefineVar(string name, Value value)
    {
        if (variables.ContainsKey(name))
            throw new RedefiningVarException();

        variables.Add(name, value);
    }

    public void SetValue(string name, Value value)
    {
        if (
            TryGetVar(name, out Value current)
            && TypeChecker.Equals(current.GetType(), value.GetType())
        )
            variables[name] = value;
    }

    public bool TryGetVar(string name, out Value value)
    {
        if (variables.TryGetValue(name, out value))
            return true;
        else
            return false;
    }
}
