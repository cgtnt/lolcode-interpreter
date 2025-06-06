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

    public void SetVar(string name, Value value)
    {
        try
        {
            Value current = GetVar(name);

            if (TypeChecker.Equals(current.GetType(), value.GetType()) || current is UntypedValue)
                variables[name] = value;
            else
                throw new TypeCastingException(
                    $"Cannot assign {value.RawValue}({value.GetType()}) to {name}({current.GetType()})"
                );
        }
        catch (UninitializedVarExcetion)
        {
            throw new UninitializedVarExcetion(
                $"Cannot assign {value.RawValue}({value.GetType()}) to uninitiazlied variable {name}"
            );
        }
    }

    public Value GetVar(string name)
    {
        if (variables.TryGetValue(name, out Value? variable))
            return variable;

        throw new UninitializedVarExcetion($"Cannot access uninitiazlied varaible {name}");
    }
}
