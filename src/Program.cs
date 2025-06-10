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
        try
        {
            string sourceCode = PreprocessingUtils.Utils.loadSoureCode(file);
            Process(sourceCode);
        }
        catch (Exception e) // any exception that wasn't caught below this level is a critical error
        {
            throw new CriticalError(e);
        }
    }

    /// <summary>
    /// Process given string as LOLCODE source code.
    /// </summary>
    /// <param name="sourceCode">LOLCODE source code.</param>
    public static void Process(string sourceCode)
    {
        try
        {
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
            }
        }
        catch (Exception e)
        {
            throw new CriticalError(e);
        }
    }

    public static void Main(string[] args)
    {
        if (args.Length == 2)
        {
            try
            {
                ProcessFile(args[1]);
            }
            catch (CriticalError e)
            {
                ExceptionReporter.Log(e);
            }
        }
        else
        {
            Usage();
        }
    }
}
