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
