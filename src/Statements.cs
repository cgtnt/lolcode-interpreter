using System;
using ExpressionDefinitions;
using ScopeDefinition;
using Tokenization;
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

    public VariableDeclareStmt(Token identifier, Expr? value = null, TokenType? type = TI_UNTYPED)
    {
        this.identifier = identifier;
        this.value = value;
        this.type = type;
    }

    public void evaluate(Scope scope)
    {
        scope.DefineVar(identifier.text, value); // TODO: type handling
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
        string result = "";

        foreach (Expr e in content)
            result += e.evaluate(scope);

        if (newline)
            Console.WriteLine(result);
        else
            Console.Write(result);
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
        scope.SetValue("IT", expression.evaluate(scope));
    }
}
