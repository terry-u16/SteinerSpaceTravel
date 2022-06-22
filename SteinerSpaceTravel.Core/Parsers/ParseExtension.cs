namespace SteinerSpaceTravel.Core.Parsers;

internal static class ParseExtension
{
    public static string[] SplitAndTrim(this string s)
        => s.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
}