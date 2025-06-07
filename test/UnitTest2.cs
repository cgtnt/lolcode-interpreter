using System;
using System.Collections.Generic;
using System.IO;
using ASTPrimitives;
using Interpretation;
using Lexing;
using Parsing;
using Tokenization;
using TokenizationPrimitives;

namespace Testing;

[TestClass]
public class InterpreterUnitTests
{
    private void AssertInterpret(string filepath)
    {
        Lexer lexer = new(PreprocessingUtils.Utils.loadSoureCode(filepath));
        List<string> lexemes = lexer.Lex();

        Tokenizer tokenizer = new(lexemes.ToArray());
        List<Token> tokens = tokenizer.Tokenize();

        Parser parser = new(tokens);
        bool execute = parser.Parse(out Stmt program);

        StringWriter consoleRedirection = new();
        Console.SetOut(consoleRedirection);

        Interpreter interpreter = new();
        interpreter.Interpret(program);
        string result = consoleRedirection.ToString();

        Assert.AreEqual(
            result,
            PreprocessingUtils.Utils.loadSoureCode($"{filepath}.interpreter.out")
        );
    }

    [TestMethod]
    [DynamicData(
        nameof(TestDataLoader.CorrectCode),
        typeof(TestDataLoader),
        DynamicDataSourceType.Method
    )]
    public void TestValidCode(string filepath)
    {
        AssertInterpret(filepath);
    }
}
