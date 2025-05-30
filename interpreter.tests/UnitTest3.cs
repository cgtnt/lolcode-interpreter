using System.Collections.Generic;
using ExpressionDefinitions;
using FileUtils;
using Lexing;
using Parsing;
using Tokenization;

namespace Testing;

[TestClass]
public class ParserExpressionTests
{
    private string TEST_DATA_DIR = "data/expressions";

    private void AssertParse(string filepath)
    {
        Lexer lexer = new(Utils.loadSoureCode(filepath));
        List<string> lexemes = lexer.Lex();

        Tokenizer tokenizer = new(lexemes.ToArray());
        List<Token> tokens = tokenizer.Tokenize();

        Parser parser = new(tokens);
        Expr AST = parser.Parse();

        Assert.AreEqual(AST.print(), File.ReadAllText($"{filepath}.parser.out"));
    }

    [DataTestMethod]
    [DataRow("ex1")]
    [DataRow("ex2")]
    [DataRow("ex3")]
    public void TestValidCode(string filename)
    {
        string filepath = $"../../../{TEST_DATA_DIR}/{filename}";
        AssertParse(filepath);
    }
}
