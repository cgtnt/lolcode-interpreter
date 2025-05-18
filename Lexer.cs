using System.Collections.Generic;

namespace lexer;

class Lexer
{
    string s;
    List<string> lexemes = new();

    int start;
    int current;
    int line;

    public Lexer(string sourceCode)
    {
        s = sourceCode;
        line = 1;
    }

    private string consumeNextLexeme()
    {
        char c = consumeNextChar();

        switch (c)
        {
            case ',':
                break;
            case ' ':
                break;
            case '\r':
                break;
            case '\n':
                line++;
                start++;
                break;
        }

        string lexeme = s[start..current];

        return lexeme;
    }

    private char consumeNextChar()
    {
        char c = peekNextChar();
        current++;
        return c;
    }

    private char peekNextChar()
    {
        return atEOF() ? '\0' : s[current];
    }

    private bool atEOF()
    {
        return current >= s.Length;
    }

    public List<string> lex()
    {
        while (!atEOF())
        {
            start = current;
            lexemes.Add(consumeNextLexeme());
        }

        return lexemes;
    }
}
