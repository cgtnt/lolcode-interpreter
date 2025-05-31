using System;

class ExceptionReporter
{
    public static void Log(Exception e)
    {
        Console.WriteLine(e.Message);
    }
}
