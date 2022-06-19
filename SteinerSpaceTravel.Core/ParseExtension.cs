namespace SteinerSpaceTravel.Core;

public static class ParseExtension
{
    public static string[] SplitAndTrim(this string s) 
        => s.Split(' ').Select(t => t.Trim()).Where(t => t.Length > 0).ToArray();
}