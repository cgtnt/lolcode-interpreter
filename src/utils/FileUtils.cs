using System.IO;

namespace PreprocessingUtils;

public class Utils
{
    public static string loadSoureCode(string filename)
    {
        return File.ReadAllText(filename);
    }
}
