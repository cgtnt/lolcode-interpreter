using System;
using System.Collections.Generic;
using System.Linq;
using CharChecking;

namespace Tokenization;

public class Token
{
    public TokenType type;
    public string text;
    public int line;

    public Token(TokenType type, string text, int line)
    {
        this.type = type;
        this.text = text;
        this.line = line;
    }

    public override string ToString() => $"@{line}:T_{type}:{text}";
}

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

    bool isFloat(string lexeme) //TODO: maybe update to disallow num beginning or ending with a dot
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
                if (CharChecker.isNewline(src[next]))
                    line++;

                next++;
            }

            return new Token(TokenType.COMMAND_TERMINATOR, src[next - 1], line);
        }

        string[]? lexemeBuffer = new string[LONGEST_KEYWORD_LEN];
        int bufferNext = 0;

        TokenType type = TokenType.INVALID;
        string match = "";
        int longestMatchLength = -1;

        while (!atEOF() && bufferNext < LONGEST_KEYWORD_LEN)
        {
            lexemeBuffer[bufferNext++] = src[next++];

            string candidate = string.Join(" ", lexemeBuffer.Take(bufferNext));

            if (keywords.TryGetValue(candidate, out TokenType potential_type))
            { // TODO: Change this and keywords to have list of lexems instad of string?
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
                type = TokenType.T_IDENTIFIER;
            }
            else if (isInteger(lexeme))
            {
                type = TokenType.T_INT;
            }
            else if (isFloat(lexeme))
            {
                type = TokenType.T_FLOAT;
            }
            else if (isString(lexeme))
            {
                type = TokenType.T_STRING;
            }
            else
            {
                type = TokenType.INVALID;
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

        tokens.Add(new Token(TokenType.EOF, "", line));

        return tokens;
    }

    public static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
    {
        { "HAI", TokenType.BEGIN },
        { "KTHX", TokenType.END },
        { "IT", TokenType.TEMP },
        { "I HAS A", TokenType.DECLARE_VAR },
        { "ITZ", TokenType.DECLARE_SET_VAR },
        { "ITZ A", TokenType.DECLARE_TYPE_VAR },
        { "YARN", TokenType.T_STRING },
        { "TROOF", TokenType.T_BOOL },
        { "NUMBR", TokenType.T_INT },
        { "NUMBAR", TokenType.T_FLOAT },
        { "NOOB", TokenType.T_UNTYPED },
        { "R", TokenType.ASSIGN },
        { "GIMMEH", TokenType.READ_STDIN },
        { "VISIBLE", TokenType.WRITE_STDIN },
        { "O RLY?", TokenType.IF },
        { "YA RLY", TokenType.THEN },
        { "NO WAI", TokenType.ELSE },
        { "OIC", TokenType.END_IF },
        { "YR", TokenType.ARG },
        { "IM IN YR", TokenType.LOOP_BEGIN },
        { "IM OUTTA YR", TokenType.LOOP_END },
        { "TILL", TokenType.UNTIL },
        { "WILE", TokenType.WHILE },
        { "GTFO", TokenType.RETURN_NULL },
        { "HOW IZ I", TokenType.FUNC_BEGIN },
        { "IF U SAY SO", TokenType.FUNC_END },
        { "FOUND YR", TokenType.RETURN_VAL },
        { "I IZ", TokenType.FUNC_CALL },
        { "SMOOSH", TokenType.CONCAT },
        { "AN", TokenType.AND },
        { "MKAY", TokenType.END_INF },
        { "BOTH OF", TokenType.BOOL_AND },
        { "EITHER OF", TokenType.BOOL_OR },
        { "WON OF", TokenType.BOOL_XOR },
        { "NOT", TokenType.BOOL_NOT },
        { "ALL OF", TokenType.BOOL_AND_INF },
        { "ANY OF", TokenType.BOOL_OR_INF },
        { "BOTH SAEM", TokenType.EQUAL },
        { "DIFFRINT", TokenType.NOT_EQUAL },
        { "SUM OF", TokenType.PLUS },
        { "DIFF OF", TokenType.MINUS },
        { "PRODUKT OF", TokenType.TIMES },
        { "QUOSHUNT OF", TokenType.QUOTIENT },
        { "MOD OF", TokenType.MOD },
        { "BIGGR OF", TokenType.MAX },
        { "SMALLR OF", TokenType.MIN },
        { "UPPIN", TokenType.INCREMENT },
        { "NERFIN", TokenType.DECREMENT },
        { "WIN", TokenType.TRUE },
        { "FAIL", TokenType.FALSE },
    };
}

public enum TokenType
{
    // lexer control tokens
    INVALID,
    COMMAND_TERMINATOR,
    EOF,

    // program
    BEGIN, // hai
    END, // kthx

    // automatic
    TEMP, // it

    // declaration
    DECLARE_VAR, //i has a
    DECLARE_SET_VAR, // itz
    DECLARE_TYPE_VAR, // itz a

    // types
    T_STRING, // yarn
    T_BOOL, // troof
    T_INT, // numbr
    T_FLOAT, // numbar
    T_UNTYPED, // noob
    T_IDENTIFIER,

    // values
    TRUE, // win
    FALSE, // fail

    // assignment
    ASSIGN, // r

    // I/O
    READ_STDIN, // gimmeh
    WRITE_STDIN, // visible

    // control flow - if
    IF, // o rly?
    THEN, // ya rly
    ELSE, // no wai
    END_IF, // oic

    //arguments
    ARG, // yr

    //control flow - while
    LOOP_BEGIN, // im in yr
    LOOP_END, // im outta yr
    UNTIL, // till
    WHILE, // wile

    RETURN_NULL, // gtfo

    // functions
    FUNC_BEGIN, // how iz i
    FUNC_END, // if u say so
    RETURN_VAL, // found yr
    FUNC_CALL, // i iz

    // concatenation
    CONCAT, // smoosh

    // separators/chain operators
    AND, // an
    END_INF, // mkay

    // boolean operations
    BOOL_AND, // both of
    BOOL_OR, // either of
    BOOL_XOR, // won of
    BOOL_NOT, // NOT
    BOOL_AND_INF, // all of
    BOOL_OR_INF, // any of

    // comparisons
    EQUAL, // both saem
    NOT_EQUAL, // diffrint

    //  arithmetic
    PLUS, // sum of
    MINUS, // diff of
    TIMES, // produkt of
    QUOTIENT, // quoshunt of
    MOD, // mod of
    MAX, // biggr of
    MIN, // smallr of
    INCREMENT, // uppin
    DECREMENT, // nerfin
}
