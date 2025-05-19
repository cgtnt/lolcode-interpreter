using System.Collections.Generic;
using FileUtils;
using Lexing;

namespace Testing;

[TestClass]
public class LexerTests
{
    private string TEST_DATA_DIR = "data";

    private void AssertLex(string filepath)
    {
        Lexer lexer = new(Utils.loadSoureCode(filepath));
        List<string> result = lexer.Lex();

        Assert.AreEqual(string.Join('-', result), File.ReadAllText($"{filepath}.lexer.out"));
    }

    [DataTestMethod]
    [DataRow("ex1")]
    [DataRow("ex4")]
    public void TestValidCode(string filename)
    {
        string filepath = $"../../../{TEST_DATA_DIR}/{filename}";
        AssertLex(filepath);
    }

    [TestMethod]
    public void TestUnterminatedString()
    {
        string filename = $"../../../{TEST_DATA_DIR}/ex2"; //TODO: fix path

        Lexer lexer = new(Utils.loadSoureCode(filename));
        Assert.ThrowsException<Exception>(lexer.Lex);
    }
}
