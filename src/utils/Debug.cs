using System;

public static class Debug
{
    static bool enabled = true;

    public static void Log(string message)
    {
        if (enabled)
            Console.WriteLine(message);
    }
}
