using System.Collections.Generic;
using System.Linq;
using LexicalUtils;
using TokenizationPrimitives;
using static TokenizationPrimitives.TokenType;

namespace Tokenization;

public class Tokenizer
{
    int LONGEST_KEYWORD_LEN = 4;

    string[] src;
    List<Token> tokens = new();

    int start = 0;
    int next = 0;
    int line = 1;

    public Tokenizer(string[] src)
    {
        this.src = src;
    }

    // tokenization helpers
    /// <summary>
    /// Return true if next pointer is at end of file.
    /// </summary>
    bool atEOF() => next >= src.Length;

    // type identification helpers
    /// <summary>
    /// Return true if given lexeme is a valid NUMBR in LOLCODE.
    /// </summary>
    bool isInteger(string lexeme)
    {
        foreach (char c in lexeme)
        {
            if (!char.IsNumber(c))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Return true if given lexeme is a valid NUMBAR in LOLCODE.
    /// </summary>
    bool isFloat(string lexeme)
    {
        bool decimalDotReached = false;
        foreach (char c in lexeme)
        {
            if (!char.IsNumber(c))
                if (!decimalDotReached && c == '.')
                    decimalDotReached = true;
                else
                    return false;
        }

        return true;
    }

    /// <summary>
    /// Return true if given lexeme is a valid YARN in LOLCODE.
    /// </summary>
    bool isString(string lexeme) => (lexeme.Length >= 2 && lexeme[0] == '"' && lexeme[^1] == '"');

    /// <summary>
    /// Return true if given lexeme is a valid identtifier in LOLCODE.
    /// </summary>
    bool isIdentifier(string lexeme)
    {
        if (lexeme.Length < 1 || !char.IsLetter(lexeme[0]))
            return false;

        foreach (char c in lexeme)
        {
            if (c != '_' && !char.IsLetter(c))
                return false;
        }

        return true;
    }

    // tokenization
    /// <summary>
    /// Return next Token and advance next pointer.
    /// </summary>
    Token nextToken()
    {
        if (CharChecker.isCommandTerminator(src[next]))
        {
            while (!atEOF() && CharChecker.isCommandTerminator(src[next]))
            {
                if (CharChecker.isBang(src[next]))
                {
                    return new Token(BANG, src[next++], line);
                }

                if (CharChecker.isNewline(src[next]))
                    line++;

                next++;
            }

            return new Token(COMMAND_TERMINATOR, src[next - 1], line);
        }

        TokenType type = INVALID;
        string match = "";
        int longestMatchLength = -1;

        string[]? lexemeBuffer = new string[LONGEST_KEYWORD_LEN]; // handle multi-word keywords
        int bufferNext = 0;

        while (!atEOF() && bufferNext < LONGEST_KEYWORD_LEN)
        {
            lexemeBuffer[bufferNext++] = src[next++];

            string candidate = string.Join(" ", lexemeBuffer.Take(bufferNext));

            if (
                TokenTranslation.keywordToToken.TryGetValue(candidate, out TokenType potential_type)
            ) // successful match of keyword
            {
                if (bufferNext > longestMatchLength)
                {
                    longestMatchLength = bufferNext;
                    match = candidate;
                    type = potential_type;
                }
            }
        }

        if (longestMatchLength == -1) // didn't match keyword
        {
            string lexeme = lexemeBuffer[0];
            match = lexeme;

            // try matching type literals, identifiers
            if (isIdentifier(lexeme))
            {
                type = T_IDENTIFIER;
            }
            else if (isInteger(lexeme))
            {
                type = T_INT;
            }
            else if (isFloat(lexeme))
            {
                type = T_FLOAT;
            }
            else if (isString(lexeme))
            {
                type = T_STRING;
            }
            else
            {
                type = INVALID;
            }

            next = start + 1; // consume 1 token
        }
        else
        {
            next = start + longestMatchLength; // consume matched # of tokens (matched by keyword)
        }

        return new Token(type, match, line);
    }

    /// <summary>
    /// Tokenize <see cref="Lexing.Lexer"/> output.
    /// </summary>
    /// <returns>List of tokens to be passed to <see cref="Parsing.Parser"/></returns>
    public List<Token> Tokenize()
    {
        while (!atEOF())
        {
            start = next;
            tokens.Add(nextToken());
        }

        tokens.Add(new Token(EOF, "", line));

        return tokens;
    }
}
