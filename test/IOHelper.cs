using System.Collections.Generic;
using System.IO;

public static class TestDataLoader
{
    private static string BASE_PATH = "../../../../test";
    private static string TEST_DATA_DIR = "data";

    public static IEnumerable<object[]> CorrectCode()
    {
        return getFiles($"{BASE_PATH}/{TEST_DATA_DIR}/correct");
    }

    public static IEnumerable<object[]> IncorrectCode()
    {
        return getFiles($"{BASE_PATH}/{TEST_DATA_DIR}/incorrect");
    }

    static IEnumerable<object[]> getFiles(string path)
    {
        List<object[]> output = new();
        string[] directories = Directory.GetDirectories(path);

        foreach (string dir in directories)
        {
            foreach (string file in Directory.GetFiles(dir))
            {
                if (file.Split('/')[^1].Split('.').Length == 1)
                    output.Add([file]);
            }
        }

        return output;
    }
}
