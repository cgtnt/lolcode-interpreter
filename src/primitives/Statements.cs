using System;
using System.Linq;
using InterpretationPrimitives;
using TokenizationPrimitives;
using TypePrimitives;
using static TokenizationPrimitives.TokenType;

namespace ASTPrimitives;

public interface Stmt
{
    public void evaluate(Scope scope);
}

// variable handling statements
public class VariableDeclareStmt : Stmt
{
    string name;
    int line;
    Expr? value;
    TokenType? type;

    public VariableDeclareStmt(Token identifier, TokenType? type = null, Expr? value = null)
    {
        name = identifier.text;
        line = identifier.line;
        this.value = value;
        this.type = type;
    }

    public void evaluate(Scope scope)
    {
        try
        {
            if (type is null && value is not null)
                scope.DefineVar(name, value.evaluate(scope));
            else if (type is not null && value is null)
                scope.DefineVar(
                    name,
                    type switch
                    {
                        TI_STRING => new StringValue(),
                        TI_BOOL => new BoolValue(),
                        TI_FLOAT => new FloatValue(),
                        TI_INT => new IntValue(),
                        TI_UNTYPED => new UntypedValue(),
                        _ => throw new InvalidTypeException(
                            $"Cannot declare variable of type {name}"
                        ),
                    }
                );
            else if (type is null && value is null)
                scope.DefineVar(name, new UntypedValue());
        }
        catch (RuntimeException e)
        {
            throw new RuntimeException(e.Message, line);
        }
    }
}

public class VariableAssignStmt : Stmt
{
    string name;
    int line;
    Expr value;

    public VariableAssignStmt(Token identifier, Expr value)
    {
        name = identifier.text;
        line = identifier.line;
        this.value = value;
    }

    public void evaluate(Scope scope)
    {
        try
        {
            scope.SetVar(name, value.evaluate(scope));
        }
        catch (RuntimeException e)
        {
            throw new RuntimeException(e.Message, line);
        }
    }
}

// I/O statements
public class PrintStmt : Stmt
{
    Expr[] content;
    bool newline;
    int line;

    public PrintStmt(bool newline, int line, params Expr[] content)
    {
        this.newline = newline;
        this.content = content;
        this.line = line;
    }

    public void evaluate(Scope scope)
    {
        try
        {
            string[] resultLiterals = content
                .Select(e => TypeCaster.CastString(e.evaluate(scope)).Value)
                .ToArray();

            string result = string.Join("", resultLiterals);

            if (newline)
                Console.WriteLine(result);
            else
                Console.Write(result);
        }
        catch (RuntimeException e)
        {
            throw new RuntimeException(e.Message, line);
        }
    }
}

public class InputStmt : Stmt
{
    string name;
    int line;

    public InputStmt(Token identifier)
    {
        name = identifier.text;
        line = identifier.line;
    }

    public void evaluate(Scope scope)
    {
        try
        {
            string? input = Console.ReadLine();
            StringValue value = new StringValue(input is not null ? input : "");
            scope.SetOrDefineVar(name, value);
        }
        catch (RuntimeException e)
        {
            throw new RuntimeException(e.Message, line);
        }
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
        bool outcome = TypeCaster.CastBool(condition).Value;

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

        while (TypeCaster.CastBool(condition.evaluate(localScope)).Value)
        {
            block.evaluate(localScope);
        }
    }
}

// function statements
public class FunctionDeclareStmt : Stmt
{
    string name;
    int line;
    BlockStmt block;
    Token[] parameters;

    public FunctionDeclareStmt(Token identifier, BlockStmt block, Token[] parameters)
    {
        name = identifier.text;
        line = identifier.line;
        this.block = block;
        this.parameters = parameters;
    }

    public void evaluate(Scope scope)
    {
        FunctionValue function = new(block, parameters.Select(a => a.text).ToArray());

        try
        {
            scope.DefineVar(name, function);
        }
        catch (RuntimeException e)
        {
            throw new RuntimeException(e.Message, line);
        }
    }
}

public class ReturnStmt : Stmt
{
    Expr? value;
    int line;

    public ReturnStmt(int line, Expr? value = null)
    {
        this.value = value;
        this.line = line;
    }

    public void evaluate(Scope scope)
    {
        throw new ReturnValue(value is not null ? value.evaluate(scope) : new UntypedValue(), line);
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
    int line;

    public ExpressionStmt(Expr expr, int line)
    {
        expression = expr;
        this.line = line;
    }

    public void evaluate(Scope scope)
    {
        try
        {
            scope.SetVar("IT", expression.evaluate(scope));
        }
        catch (RuntimeException e)
        {
            throw new RuntimeException(e.Message, line);
        }
    }
}
