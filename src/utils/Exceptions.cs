using System;

public class LineException : Exception
{
    public LineException() { }

    public LineException(string message, int line)
        : base($"Line {line}: {message}") { }

    public LineException(string message, int line, Exception inner)
        : base($"Line {line}: {message}", inner) { }

    public LineException(string message)
        : base(message) { }
}

public class RuntimeException : LineException
{
    public RuntimeException() { }

    public RuntimeException(string message, int line)
        : base(message, line) { }

    public RuntimeException(string message)
        : base(message) { }
}

public class SyntaxException : LineException
{
    public SyntaxException(string message, int line)
        : base(message, line) { }
}

public class ParsingException : LineException
{
    public ParsingException() { }

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

public class UninitializedVarExcetion : RuntimeException
{
    public UninitializedVarExcetion() { }

    public UninitializedVarExcetion(string message, int line)
        : base(message, line) { }

    public UninitializedVarExcetion(string message)
        : base(message) { }
}

public class RedefiningVarException : RuntimeException
{
    public RedefiningVarException() { }

    public RedefiningVarException(string message, int line)
        : base(message, line) { }
}

public class TypeCastingException : RuntimeException
{
    public TypeCastingException() { }

    public TypeCastingException(string message, int line)
        : base(message, line) { }

    public TypeCastingException(string message)
        : base(message) { }
}

public class CriticalError : Exception
{
    public CriticalError(Exception e)
        : base(e.Message) { }
}
