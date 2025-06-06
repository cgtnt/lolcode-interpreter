using System.Collections.Generic;
using Lexing;
using PreprocessingUtils;
using Tokenization;

namespace Testing;

[TestClass]
public class TokenizerTests
{
    private string TEST_DATA_DIR = "data";

    private void AssertTokenize(string filepath)
    {
        Lexer lexer = new(PreprocessingUtils.Utils.loadSoureCode(filepath));
        List<string> lexemes = lexer.Lex();

        Tokenizer tokenizer = new(lexemes.ToArray());
        List<Token> result = tokenizer.Tokenize();

        Assert.AreEqual(
            string.Join('-', result),
            PreprocessingUtils.Utils.loadSoureCode($"{filepath}.tokenizer.out")
        );
    }

    [DataTestMethod]
    [DataRow("ex3")]
    [DataRow("ex4")]
    public void TestValidCode(string filename)
    {
        string filepath = $"../../../{TEST_DATA_DIR}/{filename}";
        AssertTokenize(filepath);
    }
}
