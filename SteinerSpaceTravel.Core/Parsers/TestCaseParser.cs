using SteinerSpaceTravel.Core.Checkers;

namespace SteinerSpaceTravel.Core.Parsers;

public static class TestCaseParser
{
    public static TestCase Parse(ReadOnlySpan<string> input)
    {
        if (input.Length == 0)
        {
            throw new ParseFailedException(NotEnoughLineMessage);
        }

        var (n, m) = ParseNAndM(input[0]);
        var points = ParsePoints(input[1..], n, 1);

        return new TestCase(n, m, points);
    }

    private static (int n, int m) ParseNAndM(string input)
    {
        var line = input.SplitAndTrim();
        if (line.Length != 2)
        {
            throw new ParseFailedException(GetInvalidInputMessage(0));
        }

        if (!int.TryParse(line[0], out var n) || !int.TryParse(line[1], out var m)
            || !TestCaseConstraintChecker.IsNInRange(n) || !TestCaseConstraintChecker.IsMInRange(m))
        {
            throw new ParseFailedException(GetInvalidInputMessage(0));
        }

        return (n, m);
    }

    private static Point[] ParsePoints(ReadOnlySpan<string> input, int n, int lineOffset)
    {
        if (input.Length < n)
        {
            throw new ParseFailedException(NotEnoughLineMessage);
        }

        var points = new Point[n];

        for (int i = 0; i < points.Length; i++)
        {
            var line = input[i].SplitAndTrim();
            if (line.Length != 2)
            {
                throw new ParseFailedException(GetInvalidInputMessage(i + lineOffset));
            }

            if (!int.TryParse(line[0], out var x) || !int.TryParse(line[1], out var y)
                || !TestCaseConstraintChecker.IsXInRange(x) || !TestCaseConstraintChecker.IsYInRange(y))
            {
                throw new ParseFailedException(GetInvalidInputMessage(i + lineOffset));
            }

            points[i] = new Point(x, y);
        }

        return points;
    }

    private const string NotEnoughLineMessage = "入力の長さが不足しています。";

    private static string GetInvalidInputMessage(int lineNumber) => $"{lineNumber + 1}行目の入力が不正です。";
}