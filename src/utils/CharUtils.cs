namespace CharChecking;

public class CharChecker
{
    public static bool isWhitespace(char c)
    {
        return (c == ' ' || c == '\t');
    }

    public static bool isNewline(char c)
    {
        return c == '\n';
    }

    public static bool isBang(char c)
    {
        return c == '!';
    }

    public static bool isComma(char c)
    {
        return c == ',';
    }

    public static bool isCommandTerminator(char c)
    {
        return isNewline(c) || isComma(c) || isBang(c);
    }

    public static bool isComma(string s)
    {
        if (s.Length > 1)
            return false;

        return (isComma(char.Parse(s)));
    }

    public static bool isNewline(string s)
    {
        if (s.Length > 1)
            return false;

        return (isNewline(char.Parse(s)));
    }

    public static bool isBang(string s)
    {
        if (s.Length > 1)
            return false;

        return (isBang(char.Parse(s)));
    }

    public static bool isCommandTerminator(string s)
    {
        if (s.Length > 1)
            return false;

        return (isCommandTerminator(char.Parse(s)));
    }
}
