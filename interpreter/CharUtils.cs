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

    public static bool isComma(char c)
    {
        return c == ',';
    }

    public static bool isNewline(string s)
    {
        if (s.Length > 1)
            return false;

        return (isNewline(char.Parse(s)));
    }
}
