using System;
using System.Collections.Generic;
using ExpressionDefinitions;
using FileUtils;
using Interpretation;
using Lexing;
using Parsing;
using StatementDefinitions;
using Tokenization;

class Program
{
    static string EXECUTABLE_NAME = "lolcode";

    static void Usage()
    {
        Console.WriteLine($"Usage: {EXECUTABLE_NAME} [file]");
    }

    static void ProcessFile(string file)
    {
        try
        {
            string sourceCode = Utils.loadSoureCode(file);
            Process(sourceCode);
        }
        catch (Exception e)
        {
            Console.WriteLine($"File exception: {e}");
        }
    }

    static void Process(string sourceCode)
    {
        Lexer lexer = new(sourceCode);
        List<string> lexemes = lexer.Lex();

        Tokenizer tokenizer = new(lexemes.ToArray());
        List<Token> tokens = tokenizer.Tokenize();

        //Debug.Log(string.Join('-', tokens));

        Parser parser = new(tokens);
        List<Stmt> statements = parser.Parse();

        Interpreter interpreter = new();
        interpreter.Interpret(statements);
    }

    public static void Main(string[] args)
    {
        if (args.Length == 2)
        {
            ProcessFile(args[1]);
        }
        else
        {
            Usage();
        }
    }
}
