namespace lexer;

class Lexer
{
    string s;

    public Lexer(string sourceCode)
    {
        s = sourceCode;
    }
}

enum TokenType
{
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
    STRING, // yarn
    BOOL, // troof
    INT, // numbr
    FLOAT, // numbar
    UNTYPED, // noob

    // assignment
    ASSIGN, // r

    // I/O
    READ, // gimmeh
    PRINT, // visible

    // control flow - if
    IF, // o rly?
    THEN, // ya rly
    ELSE, // no wai
    FI, // oic

    //arguments
    ARG, // yr

    //control flow - while
    LOOP_START, // im in yr
    LOOP_END, // im outta yr
    UNTIL, // till
    WHILE, // wile
    BREAK, // gtfo

    // functions
    FUNC_BEGIN, // how iz i
    FUNC_END, // if u say so
    RETURN, // found yr
    FUNC_CALL, // i iz

    // concatenation
    CONCAT, // smoosh

    // separators/chain operators
    AND, // an
    END_INF, // mkay
    COMMA, // , -- line terminator

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
