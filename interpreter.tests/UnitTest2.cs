using System.Collections.Generic;
using lexer;
using tokenizer;

namespace tests;

[TestClass]
public class TokenizerTests
{
    private string TEST_DATA_DIR = "data";

    [TestMethod]
    public void TestValidCode()
    {
        string filename = $"../../../{TEST_DATA_DIR}/ex3"; //TODO: fix path

        Lexer lexer = new(Utils.loadFile(filename));
        List<string> lexemes = lexer.Lex();

        Tokenizer tokenizer = new(lexemes.ToArray());
        List<Token> result = tokenizer.Tokenize();

        Assert.AreEqual(string.Join('-', result), Utils.loadFile($"{filename}.tokenizer.out"));
    }

    [TestMethod]
    public void TestVariableDeclaration()
    {
        string filename = $"../../../{TEST_DATA_DIR}/ex4"; //TODO: fix path

        Lexer lexer = new(Utils.loadFile(filename));
        List<string> lexemes = lexer.Lex();

        Tokenizer tokenizer = new(lexemes.ToArray());
        List<Token> result = tokenizer.Tokenize();

        Assert.AreEqual(string.Join('-', result), Utils.loadFile($"{filename}.tokenizer.out"));
    }
}
