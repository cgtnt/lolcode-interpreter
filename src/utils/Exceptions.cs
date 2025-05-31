using System;

public class LineException : Exception
{
    public LineException() { }

    public LineException(string message, int line)
        : base($"Line {line}: {message}") { }

    public LineException(string message, int line, Exception inner)
        : base($"Line {line}: {message}", inner) { }
}

public class RuntimeException : LineException
{
    public RuntimeException() { }

    public RuntimeException(string message, int line)
        : base(message, line) { }
}

public class SyntaxException : LineException
{
    public SyntaxException(string message, int line)
        : base(message, line) { }
}

public class ParsingException : LineException
{
    public ParsingException(string message, int line)
        : base(message, line) { }
}

public class InvalidTypeException : RuntimeException
{
    public InvalidTypeException() { }

    public InvalidTypeException(string message, int line)
        : base(message, line) { }
}

public class InvalidOpException : RuntimeException
{
    public InvalidOpException(string message, int line)
        : base(message, line) { }
}
