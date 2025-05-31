using System;

public class SyntaxException : Exception
{
    public SyntaxException() { }

    public SyntaxException(string message, int line)
        : base($"Line {line}: {message}") { }

    public SyntaxException(string message, int line, Exception inner)
        : base($"Line {line}: {message}", inner) { }
}

public class ParsingException : Exception
{
    public ParsingException() { }

    public ParsingException(string message, int line)
        : base($"Line {line}: {message}") { }

    public ParsingException(string message, int line, Exception inner)
        : base($"Line {line}: {message}", inner) { }
}
