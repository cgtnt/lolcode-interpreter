using System;
using System.Collections.Generic;
using ExpressionDefinitions;
using Tokenization;

namespace parser;

public class Parser
{
    List<Token> tokens;
    int next;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public Expr Parse()
    {
        return null; // FIXME: update this
    }

    // parsing helpers


    // grammar rule helpers
    Expr Expression() { }
}
