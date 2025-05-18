using System;
using System.Collections.Generic;

namespace lexer;

class Lexer
{
    string s;
    List<string> lexemes = new();

    int start;
    int next;
    int line;

    public Lexer(string sourceCode)
    {
        s = sourceCode;
        line = 1;
    }

    private bool isWhitespace(char c)
    {
        return (c == ' ' || c == '\t');
    }

    private bool isNewline(char c)
    {
        return c == '\n';
    }

    private bool isLexemeTerminator(char c)
    {
        return c switch
        {
            ',' => true,
            '\r' => true,
            '"' => true,
            _ when isWhitespace(c) => true,
            _ when isNewline(c) => true,
            _ => false,
        };
    }

    private string consumeNextLexeme()
    {
        char c;
        string lexeme;

        // skip whitespace
        while (isWhitespace(peekNextChar()))
            consumeNextChar();

        start = next;
        c = consumeNextChar();

        // preserve newline
        if (isNewline(c) || c == ',')
        {
            lexeme = "\n";
        }
        else if (c == '"')
        { // preserve whitespace inside strings
            do
            {
                if (atEOF())
                    throw new System.Exception("Unterminated string");

                c = consumeNextChar();
            } while (c != '"');

            lexeme = s[start..next];
        }
        else
        {
            while (!isLexemeTerminator(peekNextChar()))
            {
                consumeNextChar();
            }

            lexeme = s[start..next];
        }

        return lexeme;
    }

    private char consumeNextChar()
    {
        char c = peekNextChar();
        next++;
        return c;
    }

    private char peekNextChar()
    {
        return atEOF() ? '\0' : s[next];
    }

    private bool atEOF()
    {
        return next >= s.Length;
    }

    public List<string> Lex()
    {
        while (!atEOF())
        {
            start = next;
            lexemes.Add(consumeNextLexeme());
        }

        return lexemes;
    }
}
