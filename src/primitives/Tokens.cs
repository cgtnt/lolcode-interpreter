using System.Collections.Generic;
using System.Linq;
using static TokenizationPrimitives.TokenType;

namespace TokenizationPrimitives;

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

/// <summary>
/// Type of LOLCODE token. See <see href="https://github.com/cgtnt/lolcode-interpreter?tab=readme-ov-file#language-features">language implementation specification</see>.
/// </summary>
public enum TokenType
{
    // lexer control tokens
    INVALID,
    EOF,
    COMMAND_TERMINATOR, // \n, ','

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
}

public class TokenTranslation
{
    public static Dictionary<string, TokenType> keywordToToken = new Dictionary<string, TokenType>
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
        { "WIN", TRUE },
        { "FAIL", FALSE },
        { "!", BANG },
    };

    public static Dictionary<TokenType, string> tokenToKeyword = constructTokenToKeyword();

    /// <summary>
    /// Translates given TokenTypes to associated keywords in the LOLCODE langauge. See <see cref="TokenType"/>.
    /// </summary>
    public static string[] TokensToKeywords(params TokenType[] types) =>
        types.Select(t => tokenToKeyword[t]).ToArray();

    static Dictionary<V, K> swap<K, V>(Dictionary<K, V> original)
        where K : notnull // K,V need to be notnull -> implements GetHashCode() otherwise compiler warns
        where V : notnull
    {
        Dictionary<V, K> result = new();

        foreach ((K key, V value) in original)
            result.Add(value, key);

        return result;
    }

    static Dictionary<TokenType, string> constructTokenToKeyword()
    {
        Dictionary<TokenType, string> res = swap(keywordToToken);
        res.Add(COMMAND_TERMINATOR, "',' , newline");
        res.Add(EOF, "end of file");
        return res;
    }
}
