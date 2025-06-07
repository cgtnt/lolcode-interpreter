using System;

public static class Debugger
{
    static bool enabled = false;

    public static void Log(string message)
    {
        if (enabled)
            Console.WriteLine(message);
    }
}
