namespace interpreter.tests;

using System.Collections.Generic;
using interpreter;
using lexer;

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
    private string TEST_DATA_DIR = "examples";

    [TestMethod]
    public void TestExample1()
    {
        string filename = $"../../../{TEST_DATA_DIR}/ex1"; //TODO: fix path

        Lexer lexer = new(Utils.loadFile(filename));
        List<string> result = lexer.Lex();

        Assert.AreEqual(string.Join('_', result), Utils.loadFile($"{filename}.out"));
    }
}

