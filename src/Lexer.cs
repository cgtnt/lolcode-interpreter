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

    /// <summary>
    /// Creates a new Lexer.
    /// </summary>
    /// <param name="sourceCode">LOLCODE source code.</param>
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

    /// <summary>
    /// Consumes next lexeme.
    /// </summary>
    string consumeNextLexeme()
    {
        char c;
        string lexeme;

        // skip whitespace
        while (CharChecker.isWhitespace(peekNextChar()) || peekNextChar() == '\r')
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

    /// <summary>
    /// Return next char and advance next pointer.
    /// </summary>
    char consumeNextChar() => s[next++];

    /// <summary>
    /// Return next char and without advancing next pointer.
    /// </summary>
    char peekNextChar() => s[next];

    /// <summary>
    /// Returns true if next pointer is at end of file.
    /// </summary>
    bool atEOF() => next >= s.Length;

    /// <summary>
    /// Lex LOLCODE source code.
    /// </summary>
    /// <returns>List of lexemes to be passed to <see cref="Tokenization.Tokenizer"/></returns>
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
