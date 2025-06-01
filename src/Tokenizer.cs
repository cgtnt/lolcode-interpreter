using System;
using System.Collections.Generic;
using System.Linq;
using CharChecking;
using static Tokenization.TokenType;

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
        { "KTHX", END },
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

    TI_STRING, // yarn
    TI_BOOL, // troof
    TI_INT, // numbr
    TI_FLOAT, // numbar
    TI_UNTYPED, // noob

    // values
    TRUE, // win
    FALSE, // fail

    // assignment
    ASSIGN, // r

    // I/O
    READ_STDIN, // gimmeh
    WRITE_STDOUT, // visible

    // control flow - if
    IF, // o rly?
    THEN, // ya rly
    ELSE, // no wai
    END_IF, // oic

    //arguments
    ARG, // yr
    BANG, // printstmt ending (for newline)

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
