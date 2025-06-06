using System;
using System.Linq;
using ExpressionDefinitions;
using ScopeDefinition;
using Tokenization;
using TypeDefinitions;
using static Tokenization.TokenType;

namespace StatementDefinitions;

public interface Stmt
{
    public void evaluate(Scope scope);
}

public class VariableDeclareStmt : Stmt
{
    Token identifier;
    Expr? value;
    TokenType? type;

    public VariableDeclareStmt(Token identifier, TokenType? type = null, Expr? value = null)
    {
        this.identifier = identifier;
        this.value = value;
        this.type = type;
    }

    public void evaluate(Scope scope)
    {
        if (type is null && value is not null)
            scope.DefineVar(identifier.text, value.evaluate(scope));
        else if (type is not null && value is null)
            scope.DefineVar(
                identifier.text,
                type switch
                {
                    TI_STRING => new StringValue(),
                    TI_BOOL => new BoolValue(),
                    TI_FLOAT => new FloatValue(),
                    TI_INT => new IntValue(),
                    TI_UNTYPED => new UntypedValue(),
                    _ => throw new InvalidTypeException(
                        $"Cannot declare variable of type {identifier.text}",
                        identifier.line
                    ),
                }
            );
        else if (type is null && value is null)
            scope.DefineVar(identifier.text, new UntypedValue());
    }
}

public class VariableAssignStmt : Stmt
{
    Token identifier;
    Expr value;

    public VariableAssignStmt(Token identifier, Expr value)
    {
        this.identifier = identifier;
        this.value = value;
    }

    public void evaluate(Scope scope)
    {
        scope.SetVar(identifier.text, value.evaluate(scope));
    }
}

public class PrintStmt : Stmt
{
    Expr[] content;
    bool newline;

    public PrintStmt(bool newline, params Expr[] content)
    {
        this.newline = newline;
        this.content = content;
    }

    public void evaluate(Scope scope)
    {
        string result = string.Join("", content.Select(e => e.evaluate(scope)));

        if (newline)
            Console.WriteLine(result);
        else
            Console.Write(result);
    }
}

public class InputStmt : Stmt
{
    Token identifier;

    public InputStmt(Token identifier)
    {
        this.identifier = identifier;
    }

    public void evaluate(Scope scope)
    {
        string? input = Console.ReadLine();
        StringValue value = new StringValue(input is not null ? input : "");
        scope.SetVar(identifier.text, value);
    }
}

public class ExpressionStmt : Stmt
{
    Expr expression;

    public ExpressionStmt(Expr expr)
    {
        expression = expr;
    }

    public void evaluate(Scope scope)
    {
        scope.SetVar("IT", expression.evaluate(scope));
    }
}
