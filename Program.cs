using System;
using lexer;

class Program
{
    private static string EXECUTABLE_NAME = "lolcode";

    private static void Usage()
    {
        Console.WriteLine($"Usage: {EXECUTABLE_NAME} [file]");
    }

    private static void Process(string sourceCode)
    {
        Lexer lexer = new(sourceCode);
    }

    public static void Main(string[] args)
    {
        if (args.Length == 2)
        {
            Process(args[1]);
        }
        else
        {
            Usage();
        }
    }
}
