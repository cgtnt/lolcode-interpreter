using System.Collections.Generic;

namespace ScopeDefinition;

public class Scope
{
    public Scope? parent { get; private set; }
    Dictionary<string, object> variables;

    public Scope()
    {
        variables = new();
    }

    public void DefineVar(string name, object value)
    {
        if (variables.ContainsKey(name))
            throw new RedefiningVarException();

        variables.Add(name, value);
    }

    public void SetValue(string name, object value)
    {
        if (TryGetVar(name, out object _))
            variables[name] = value;
    }

    public bool TryGetVar(string name, out object value)
    {
        if (variables.TryGetValue(name, out value))
            return true;
        else
            return false;
    }
}
