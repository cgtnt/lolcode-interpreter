using System.IO;

namespace FileUtils;

public class Utils
{
    public static string loadSoureCode(string filename)
    {
        return File.ReadAllText(filename);
    }
}
