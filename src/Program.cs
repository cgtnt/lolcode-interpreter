using System;
using System.Collections.Generic;
using ASTPrimitives;
using Interpretation;
using Lexing;
using Parsing;
using Tokenization;
using TokenizationPrimitives;

public static class Program
{
    static string EXECUTABLE_NAME = "[executable]";

    /// <summary>
    /// Print CLI usage instructions.
    /// </summary>
    public static void Usage()
    {
        Console.WriteLine($"Usage: {EXECUTABLE_NAME} <source file>");
    }

    /// <summary>
    /// Process contents of given file as LOLCODE source code.
    /// </summary>
    /// <param name="file">Path to source file.</param>
    public static void ProcessFile(string file)
    {
        string sourceCode = PreprocessingUtils.Utils.loadSoureCode(file);
        Process(sourceCode);
    }

    /// <summary>
    /// Process contents of standard input as LOLCODE source code.
    /// </summary>
    public static void ProcessStdin()
    {
        string? sourceCode = Console.In.ReadToEnd();

        if (sourceCode is null)
            throw new Exception("Source code must contain code (shocking)");

        Process(sourceCode as string);
    }

    /// <summary>
    /// Process given string as LOLCODE source code.
    /// </summary>
    /// <param name="sourceCode">LOLCODE source code string.</param>
    public static void Process(string sourceCode)
    {
        try {
            Lexer lexer = new(sourceCode);
            List<string> lexemes = lexer.Lex();

            Tokenizer tokenizer = new(lexemes.ToArray());
            List<Token> tokens = tokenizer.Tokenize();

            Debugger.Log(string.Join('-', tokens));

            Parser parser = new(tokens);
            bool execute = parser.Parse(out Stmt? program);

            if (execute && program is not null)
            {
                Interpreter interpreter = new();
                interpreter.Interpret(program);
            } else {
                Environment.ExitCode = 1;
            }
        } catch (Exception e) { // any exception not caught before this point is a critial error
            throw new CriticalError(e);
        }
    }

    public static void Main(string[] args)
    {
        try
        {
            if (args.Length == 1)
              ProcessFile(args[0]);
            else if (args.Length == 0)
              ProcessStdin();
            else
              Usage();
        }
        catch (CriticalError e) 
        {
            ExceptionReporter.Log(e);
            Environment.Exit(1);
        }
    }
}
