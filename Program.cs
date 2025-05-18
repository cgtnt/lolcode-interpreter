using System;
using System.Collections.Generic;
using System.IO;
using ErrorReporter;
using lexer;

class Program
{
    private static string EXECUTABLE_NAME = "lolcode";

    private static void Usage()
    {
        Console.WriteLine($"Usage: {EXECUTABLE_NAME} [file]");
    }

    private static void ProcessFile(string file)
    {
        try
        {
            string sourceCode = File.ReadAllText(file);
            Process(sourceCode);
        }
        catch (Exception e)
        {
            Console.WriteLine($"File exception: {e}");
        }
    }

    private static void Process(string sourceCode)
    {
        Lexer lexer = new(sourceCode);
        List<string> result = lexer.Lex();

        Console.WriteLine(string.Join('_', result));
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
