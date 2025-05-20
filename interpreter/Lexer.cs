using System;
using System.Collections.Generic;
using CharChecking;

namespace Lexing;

public class Lexer
{
    string s;
    List<string> lexemes = new();

    int start;
    int next;
    int line = 1;

    public Lexer(string sourceCode)
    {
        s = sourceCode;
    }

    private bool isLexemeTerminator(char c)
    {
        return c switch
        {
            ',' => true,
            '\r' => true,
            '"' => true,
            _ when CharChecker.isWhitespace(c) => true,
            _ when CharChecker.isNewline(c) => true,
            _ => false,
        };
    }

    private string consumeNextLexeme()
    {
        char c;
        string lexeme;

        // skip whitespace
        while (CharChecker.isWhitespace(peekNextChar()))
            consumeNextChar();

        start = next;
        c = consumeNextChar();

        // preserve newline and comma
        if (CharChecker.isNewline(c) || c == ',')
        {
            lexeme = $"{c}";

            if (CharChecker.isNewline(c))
            {
                line++;
            }
        }
        else if (c == '"')
        { // preserve whitespace inside strings
            do
            {
                if (atEOF())
                    throw new ErrorReporter.SyntaxError("Unterminated string", line);

                c = consumeNextChar();
            } while (c != '"');

            lexeme = s[start..next];
        }
        else
        {
            while (!atEOF() && !isLexemeTerminator(peekNextChar()))
            {
                consumeNextChar();
            }

            lexeme = s[start..next];
        }

        return lexeme;
    }

    private char consumeNextChar()
    {
        return s[next++];
    }

    private char peekNextChar()
    {
        return s[next];
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
