using System.Collections.Generic;
using lexer;

namespace tests;

public class Utils
{
    public static string loadFile(string filename)
    {
        return File.ReadAllText(filename);
    }
}

[TestClass]
public class LexerTests
{
    private string TEST_DATA_DIR = "data";

    [TestMethod]
    public void TestValidCode()
    {
        string filename = $"../../../{TEST_DATA_DIR}/ex1"; //TODO: fix path

        Lexer lexer = new(Utils.loadFile(filename));
        List<string> result = lexer.Lex();

        Assert.AreEqual(string.Join('-', result), Utils.loadFile($"{filename}.lexer.out"));
    }

    [TestMethod]
    public void TestVariableDeclaration()
    {
        string filename = $"../../../{TEST_DATA_DIR}/ex4"; //TODO: fix path

        Lexer lexer = new(Utils.loadFile(filename));
        List<string> result = lexer.Lex();

        Assert.AreEqual(string.Join('-', result), Utils.loadFile($"{filename}.lexer.out"));
    }

    [TestMethod]
    public void TestUnterminatedString()
    {
        string filename = $"../../../{TEST_DATA_DIR}/ex2"; //TODO: fix path

        Lexer lexer = new(Utils.loadFile(filename));

        Assert.ThrowsException<Exception>(lexer.Lex);
    }
}
