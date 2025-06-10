using System.Collections.Generic;
using TypePrimitives;

namespace InterpretationPrimitives;

/// <summary>
/// A container holding variables and functions. Indexed by name.
/// </summary>
public class Scope
{
    public Scope? parent { get; private set; }
    Dictionary<string, Value> variables;

    /// <summary>
    /// Create a new scope.
    /// </summary>
    /// <param name="parent">Parent scope whose variables can be transitively read by <see cref="GetVar(string)"/>.</param>
    public Scope(Scope? parent = null)
    {
        this.parent = parent;
        variables = new();
        variables.Add("IT", new UntypedValue()); // temporary variable IT
    }

    /// <summary>
    /// Define variable in current scope. Do not rederfine existing variables, for assignment see <see cref="SetVar(string, Value)"/>. Cannot define variables in parent scopes.
    /// </summary>
    public void DefineVar(string name, Value value)
    {
        if (variables.ContainsKey(name))
            throw new RedefiningVarException($"Cannot redefine variable {name}");

        variables.Add(name, value);
    }

    /// <summary>
    /// Set value of variable already defined in current scope. To define a variable, see <see cref="DefineVar(string, Value)"/>. Cannot change variables of parent scopes.
    /// </summary>
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

    /// <summary>
    /// Set variable, or define it if it does not exist in the current scope. Cannot change variables of parent scopes.
    /// </summary>
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

    /// <summary>
    /// Get value of variable. Transitively searches current scope and all parent scopes for variable, then throws an exception if variable is not found.
    /// </summary>
    public Value GetVar(string name)
    {
        if (variables.TryGetValue(name, out Value? variable))
            return variable;
        else if (parent is not null)
            return parent.GetVar(name);

        throw new UninitializedVarExcetion($"Cannot access uninitiazlied varaible {name}");
    }
}
