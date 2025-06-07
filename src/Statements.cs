using System;
using System.Linq;
using ScopeDefinition;
using Tokenization;
using TypePrimitives;
using static Tokenization.TokenType;

namespace ParsingPrimitives;

public interface Stmt
{
    public void evaluate(Scope scope);
}

// variable handling statements
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

// I/O statements
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
        string result = string.Join(
            "",
            content.Select(e => TypeCaster.TryCastString(e.evaluate(scope)))
        );

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
        scope.SetOrDefineVar(identifier.text, value);
    }
}

// control flow statements
public class IfStmt : Stmt
{
    BlockStmt trueBlock;
    BlockStmt falseBlock;

    public IfStmt(BlockStmt trueBlock, BlockStmt falseBlock)
    {
        this.trueBlock = trueBlock;
        this.falseBlock = falseBlock;
    }

    public void evaluate(Scope scope)
    { // if-else operates on the implict variable IT
        Value condition = scope.GetVar("IT");
        bool outcome = TypeCaster.TryCastBool(condition).Value;

        if (outcome)
            trueBlock.evaluate(new Scope(scope));
        else
            falseBlock.evaluate(new Scope(scope));
    }
}

public class LoopStmt : Stmt
{
    BlockStmt block;
    Expr condition;
    Token variable;

    public LoopStmt(BlockStmt block, Expr condition, Token variable)
    {
        this.block = block;
        this.condition = condition;
        this.variable = variable;
    }

    public void evaluate(Scope scope)
    {
        Scope localScope = new Scope(scope);

        localScope.DefineVar(variable.text, new UntypedValue()); // FIXME: count type is unintialized, fails on expr comparison beow

        while (TypeCaster.TryCastBool(condition.evaluate(localScope)).Value)
        {
            block.evaluate(localScope);
        }
    }
}

// function statements

// auxiliary statements
public class BlockStmt : Stmt
{
    Stmt[] statements;

    public BlockStmt(Stmt[] statements)
    {
        this.statements = statements;
    }

    public void evaluate(Scope scope)
    {
        foreach (Stmt s in statements)
            s.evaluate(scope);
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
