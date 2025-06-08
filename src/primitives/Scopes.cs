using System.Collections.Generic;
using TypePrimitives;

namespace InterpretationPrimitives;

public class Scope
{
    public Scope? parent { get; private set; }
    Dictionary<string, Value> variables;

    public Scope(Scope? parent = null)
    {
        this.parent = parent;
        variables = new();
        variables.Add("IT", new UntypedValue()); // temporary variable IT
    }

    public void DefineVar(string name, Value value)
    {
        if (variables.ContainsKey(name))
            throw new RedefiningVarException($"Cannot redefine variable {name}");

        variables.Add(name, value);
    }

    public void SetVar(string name, Value value)
    {
        if (!variables.TryGetValue(name, out Value? current))
            throw new UninitializedVarExcetion(
                $"Cannot assign {value.RawValue}({TypeCaster.GetValueType(value)}) to uninitiazlied variable {name}"
            );

        if (
            TypeChecker.Equals(current.GetType(), value.GetType())
            || current is UntypedValue
            || name == "IT"
        )
            variables[name] = value;
        else
            throw new TypeCastingException(
                $"Cannot assign {value.RawValue}({TypeCaster.GetValueType(value)}) to {name}({TypeCaster.GetValueType(current)})"
            );
    }

    public void SetOrDefineVar(string name, Value value)
    {
        try
        {
            SetVar(name, value);
        }
        catch (UninitializedVarExcetion)
        {
            DefineVar(name, value);
        }
    }

    public Value GetVar(string name)
    {
        if (variables.TryGetValue(name, out Value? variable))
            return variable;
        else if (parent is not null)
            return parent.GetVar(name);

        throw new UninitializedVarExcetion($"Cannot access uninitiazlied varaible {name}");
    }
}
