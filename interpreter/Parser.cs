using System;
using System.Collections.Generic;
using ExpressionDefinitions;
using Tokenization;

namespace parser;

public class Parser
{
    private List<Token> tokens;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public Expr Parse() { }
}
