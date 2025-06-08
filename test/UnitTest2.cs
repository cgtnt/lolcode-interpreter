using System;
using System.IO;

namespace Testing;

[TestClass]
public class InterpreterUnitTests
{
    private void AssertInterpret(string filepath)
    {
        StringWriter consoleRedirection = new();
        Console.SetOut(consoleRedirection);
        Console.SetError(consoleRedirection);

        try
        {
            Program.ProcessFile(filepath);
        }
        catch (CriticalError e)
        {
            ExceptionReporter.Log(e);
        }

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

    [TestMethod]
    [DynamicData(
        nameof(TestDataLoader.IncorrectCode),
        typeof(TestDataLoader),
        DynamicDataSourceType.Method
    )]
    public void TestIncorrectCode(string filepath)
    {
        AssertInterpret(filepath);
    }
}
