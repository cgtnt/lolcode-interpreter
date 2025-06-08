using System.Collections.Generic;
using LexicalUtils;

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

    bool isLexemeTerminator(char c) // lexeme terminators, should be treated as separate tokens
    {
        return c switch
        {
            ',' => true,
            '\r' => true,
            '"' => true,
            '!' => true,
            _ when CharChecker.isWhitespace(c) => true,
            _ when CharChecker.isNewline(c) => true,
            _ => false,
        };
    }

    string consumeNextLexeme()
    {
        char c;
        string lexeme;

        // skip whitespace
        while (CharChecker.isWhitespace(peekNextChar()))
            consumeNextChar();

        start = next;
        c = consumeNextChar();

        if (CharChecker.isCommandTerminator(c)) // consume newline, comma and !
        {
            lexeme = $"{c}";

            if (CharChecker.isNewline(c))
            {
                line++;
            }
        }
        else if (c == '"') // consume string
        { // preserve whitespace inside strings
            do
            {
                if (atEOF())
                    throw new SyntaxException("Unterminated string", line);

                c = consumeNextChar();
            } while (c != '"');

            lexeme = s[start..next];
        }
        else // consume anything else
        {
            while (!atEOF() && !isLexemeTerminator(peekNextChar()))
            {
                consumeNextChar();
            }

            lexeme = s[start..next];
        }

        return lexeme;
    }

    char consumeNextChar() => s[next++];

    char peekNextChar() => s[next];

    bool atEOF() => next >= s.Length;

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
