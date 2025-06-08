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

    bool atEOF() => next >= src.Length;

    bool isInteger(string lexeme)
    {
        foreach (char c in lexeme)
        {
            if (!char.IsNumber(c))
                return false;
        }

        return true;
    }

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

    bool isString(string lexeme) => (lexeme.Length >= 2 && lexeme[0] == '"' && lexeme[^1] == '"');

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

        string[]? lexemeBuffer = new string[LONGEST_KEYWORD_LEN];
        int bufferNext = 0;

        TokenType type = INVALID;
        string match = "";
        int longestMatchLength = -1;

        while (!atEOF() && bufferNext < LONGEST_KEYWORD_LEN)
        {
            lexemeBuffer[bufferNext++] = src[next++];

            string candidate = string.Join(" ", lexemeBuffer.Take(bufferNext));

            if (keywords.TryGetValue(candidate, out TokenType potential_type))
            {
                if (bufferNext > longestMatchLength)
                {
                    longestMatchLength = bufferNext;
                    match = candidate;
                    type = potential_type;
                }
            }
        }

        if (longestMatchLength == -1)
        {
            string lexeme = lexemeBuffer[0];
            match = lexeme;
            // definitely not keyword - only 1 word long and failed to match

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
            next = start + longestMatchLength; // consume matched # of tokens
        }

        return new Token(type, match, line);
    }

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

    public static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
    {
        { "HAI", BEGIN },
        { "KTHXBYE", END },
        { "IT", TEMP },
        { "I HAS A", DECLARE_VAR },
        { "ITZ", DECLARE_SET_VAR },
        { "ITZ A", DECLARE_TYPE_VAR },
        { "YARN", TI_STRING },
        { "TROOF", TI_BOOL },
        { "NUMBR", TI_INT },
        { "NUMBAR", TI_FLOAT },
        { "NOOB", TI_UNTYPED },
        { "R", ASSIGN },
        { "GIMMEH", READ_STDIN },
        { "VISIBLE", WRITE_STDOUT },
        { "O RLY?", IF },
        { "YA RLY", THEN },
        { "NO WAI", ELSE },
        { "OIC", END_IF },
        { "YR", ARG },
        { "IM IN YR", LOOP_BEGIN },
        { "IM OUTTA YR", LOOP_END },
        { "TILL", UNTIL },
        { "WILE", WHILE },
        { "GTFO", RETURN_NULL },
        { "HOW IZ I", FUNC_BEGIN },
        { "IF U SAY SO", FUNC_END },
        { "FOUND YR", RETURN_VAL },
        { "I IZ", FUNC_CALL },
        { "SMOOSH", CONCAT },
        { "AN", AND },
        { "MKAY", END_INF },
        { "BOTH OF", BOOL_AND },
        { "EITHER OF", BOOL_OR },
        { "WON OF", BOOL_XOR },
        { "NOT", BOOL_NOT },
        { "ALL OF", BOOL_AND_INF },
        { "ANY OF", BOOL_OR_INF },
        { "BOTH SAEM", EQUAL },
        { "DIFFRINT", NOT_EQUAL },
        { "SUM OF", PLUS },
        { "DIFF OF", MINUS },
        { "PRODUKT OF", TIMES },
        { "QUOSHUNT OF", QUOTIENT },
        { "MOD OF", MOD },
        { "BIGGR OF", MAX },
        { "SMALLR OF", MIN },
        { "UPPIN", INCREMENT },
        { "NERFIN", DECREMENT },
        { "WIN", TRUE },
        { "FAIL", FALSE },
        { "!", BANG },
    };
}
