using System;

namespace ErrorReporter;

class ErrorReporter
{
    public static void Print(string error)
    {
        Console.WriteLine(error);
    }
}

public class SyntaxError : System.Exception
{
    public SyntaxError() { }

    public SyntaxError(string message, int line)
        : base($"Line {line}: {message}") { }

    public SyntaxError(string message, int line, Exception inner)
        : base($"Line {line}: {message}", inner) { }
}
