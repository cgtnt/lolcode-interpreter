using System;
using System.Linq;
using InterpretationPrimitives;
using TokenizationPrimitives;
using TypePrimitives;
using static TokenizationPrimitives.TokenType;

namespace ParsingPrimitives;

public interface Stmt
{
    public void evaluate(Scope scope);
}

// variable handling statements
public class VariableDeclareStmt : Stmt
{
    string identifier;
    int line;
    Expr? value;
    TokenType? type;

    public VariableDeclareStmt(Token identifier, TokenType? type = null, Expr? value = null)
    {
        this.identifier = identifier.text;
        this.line = identifier.line;
        this.value = value;
        this.type = type;
    }

    public void evaluate(Scope scope)
    {
        if (type is null && value is not null)
            scope.DefineVar(identifier, value.evaluate(scope));
        else if (type is not null && value is null)
            scope.DefineVar(
                identifier,
                type switch
                {
                    TI_STRING => new StringValue(),
                    TI_BOOL => new BoolValue(),
                    TI_FLOAT => new FloatValue(),
                    TI_INT => new IntValue(),
                    TI_UNTYPED => new UntypedValue(),
                    _ => throw new InvalidTypeException(
                        $"Cannot declare variable of type {identifier}",
                        line
                    ),
                }
            );
        else if (type is null && value is null)
            scope.DefineVar(identifier, new UntypedValue());
    }
}

public class VariableAssignStmt : Stmt
{
    string identifier;
    Expr value;

    public VariableAssignStmt(Token identifier, Expr value)
    {
        this.identifier = identifier.text;
        this.value = value;
    }

    public void evaluate(Scope scope)
    {
        scope.SetVar(identifier, value.evaluate(scope));
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
    string identifier;

    public InputStmt(Token identifier)
    {
        this.identifier = identifier.text;
    }

    public void evaluate(Scope scope)
    {
        string? input = Console.ReadLine();
        StringValue value = new StringValue(input is not null ? input : "");
        scope.SetOrDefineVar(identifier, value);
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
    VariableDeclareStmt variable;

    public LoopStmt(BlockStmt block, Expr condition, VariableDeclareStmt variable)
    {
        this.block = block;
        this.condition = condition;
        this.variable = variable;
    }

    public void evaluate(Scope scope)
    {
        Scope localScope = new Scope(scope);

        variable.evaluate(localScope);

        while (TypeCaster.TryCastBool(condition.evaluate(localScope)).Value)
        {
            block.evaluate(localScope);
        }
    }
}

// function statements
public class FunctionDeclareStmt : Stmt
{
    string name;
    BlockStmt block;
    Token[] parameters;

    public FunctionDeclareStmt(Token name, BlockStmt block, Token[] parameters)
    {
        this.name = name.text;
        this.block = block;
        this.parameters = parameters;
    }

    public void evaluate(Scope scope)
    {
        FunctionValue function = new(block, parameters.Select(a => a.text).ToArray());

        scope.DefineVar(name, function);
    }
}

public class ReturnStmt : Stmt
{
    Expr? value;

    public ReturnStmt(Expr? value = null)
    {
        this.value = value;
    }

    public void evaluate(Scope scope)
    {
        throw new ReturnValue(value is not null ? value.evaluate(scope) : new UntypedValue());
    }
}

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
