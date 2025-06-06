using Lexing;
using Parsing;
using ParsingPrimitives;
using Tokenization;

namespace Testing;

[TestClass]
public class ParserExpressionTests
{
    private string TEST_DATA_DIR = "data/expressions";

    private void AssertParse(string filepath)
    {
        Lexer lexer = new(PreprocessingUtils.Utils.loadSoureCode(filepath));
        List<string> lexemes = lexer.Lex();

        Tokenizer tokenizer = new(lexemes.ToArray());
        List<Token> tokens = tokenizer.Tokenize();

        Parser parser = new(tokens);
        Expr AST = parser.Parse() as Expr; // TODO: update to handle null AST (failed parsing)

        Assert.AreEqual(AST.ToString(), File.ReadAllText($"{filepath}.parser.out"));
    }

    [DataTestMethod]
    [DataRow("ex1")]
    [DataRow("ex2")]
    [DataRow("ex3")]
    [DataRow("ex4")]
    public void TestValidCode(string filename)
    {
        string filepath = $"../../../{TEST_DATA_DIR}/{filename}";
        AssertParse(filepath);
    }
}
