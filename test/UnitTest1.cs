using System.Collections.Generic;
using Lexing;
using Tokenization;
using TokenizationPrimitives;

namespace Testing;

[TestClass]
public class TokenizerTests
{
    private void AssertTokenize(string filepath)
    {
        Lexer lexer = new(PreprocessingUtils.Utils.loadSoureCode(filepath));
        List<string> lexemes = lexer.Lex();

        Tokenizer tokenizer = new(lexemes.ToArray());
        List<Token> result = tokenizer.Tokenize();

        Assert.AreEqual(
            string.Join('-', result) + '\n',
            PreprocessingUtils.Utils.loadSoureCode($"{filepath}.tokenizer.out")
        );
    }

    private void AssertTokenizeIncorrect(string filepath)
    {
        string result;
        try
        {
            Lexer lexer = new(PreprocessingUtils.Utils.loadSoureCode(filepath));
            List<string> lexemes = lexer.Lex();

            Tokenizer tokenizer = new(lexemes.ToArray());
            List<Token> tokens = tokenizer.Tokenize();

            result = string.Join('-', tokens);
        }
        catch (SyntaxException e)
        {
            result = e.Message;
        }

        Assert.AreEqual(
            result + '\n',
            PreprocessingUtils.Utils.loadSoureCode($"{filepath}.tokenizer.out")
        );
    }

    [TestMethod]
    [DynamicData(
        nameof(TestDataLoader.CorrectCode),
        typeof(TestDataLoader),
        DynamicDataSourceType.Method
    )]
    public void TestCode(string filepath)
    {
        AssertTokenize(filepath);
    }

    [TestMethod]
    [DynamicData(
        nameof(TestDataLoader.IncorrectCode),
        typeof(TestDataLoader),
        DynamicDataSourceType.Method
    )]
    public void TestIncorrectCode(string filepath)
    {
        AssertTokenizeIncorrect(filepath);
    }
}
