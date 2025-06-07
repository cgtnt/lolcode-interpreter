using System;
using System.Collections.Generic;
using ASTPrimitives;
using Interpretation;
using Lexing;
using Parsing;
using Tokenization;
using TokenizationPrimitives;

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
            string sourceCode = PreprocessingUtils.Utils.loadSoureCode(file);
            Process(sourceCode);
        }
        catch (Exception e)
        {
            throw new CriticalError(e);
        }
    }

    static void Process(string sourceCode)
    {
        try
        {
            Lexer lexer = new(sourceCode);
            List<string> lexemes = lexer.Lex();

            Tokenizer tokenizer = new(lexemes.ToArray());
            List<Token> tokens = tokenizer.Tokenize();

            //Debug.Log(string.Join('-', tokens));

            Parser parser = new(tokens);
            bool execute = parser.Parse(out Stmt program);

            if (execute)
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
