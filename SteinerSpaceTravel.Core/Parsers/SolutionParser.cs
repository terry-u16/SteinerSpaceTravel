using SteinerSpaceTravel.Core.Checkers;

namespace SteinerSpaceTravel.Core.Parsers;

public static class SolutionParser
{
    public static Solution Parse(TestCase testCase, ReadOnlySpan<string> input)
    {
        if (input.Length < testCase.M + 1)
        {
            throw new ParseFailedException(NotEnoughLineMessage);
        }

        var stations = ParseStations(input[..testCase.M]);
        var visits = ParseVisits(testCase, input[testCase.M..]);

        return new Solution(testCase, stations, visits);
    }

    private static Point[] ParseStations(ReadOnlySpan<string> input)
    {
        var points = new Point[input.Length];

        for (var i = 0; i < input.Length; i++)
        {
            var line = input[i].SplitAndTrim();

            if (line.Length != 2 || !int.TryParse(line[0], out var x) || !int.TryParse(line[1], out var y) ||
                !SolutionConstraintChecker.IsXInRange(x) || !SolutionConstraintChecker.IsYInRange(y))
            {
                throw new ParseFailedException(GetInvalidInputMessage(i));
            }

            points[i] = new Point(x, y);
        }

        return points;
    }

    private static Visit[] ParseVisits(TestCase testCase, ReadOnlySpan<string> input)
    {
        if (!int.TryParse(input[0].Trim(), out var lineCount))
        {
            throw new ParseFailedException(GetInvalidInputMessage(testCase.M));
        }

        if (lineCount <= 0)
        {
            throw new ParseFailedException(InvalidVisitLengthMessage);
        }

        input = input[1..];
        var lineOffset = testCase.M + 1;

        if (input.Length < lineCount)
        {
            throw new ParseFailedException(NotEnoughLineMessage);
        }

        input = input[..lineCount];
        var visits = ParseVisitsInner(testCase, input, lineOffset);

        CheckValidVisits(testCase, visits);

        return visits;
    }

    private static Visit[] ParseVisitsInner(TestCase testCase, ReadOnlySpan<string> input, int lineOffset)
    {
        var visits = new Visit[input.Length];

        for (var i = 0; i < input.Length; i++)
        {
            var line = input[i].SplitAndTrim();

            if (line.Length != 2 || !int.TryParse(line[1], out var index))
            {
                throw new ParseFailedException(GetInvalidInputMessage(i + lineOffset));
            }

            var type = ParseAstronomicalType(line[0], i + lineOffset);
            index--;
            if (!SolutionConstraintChecker.IsVisitIndexInRange(testCase, type, index))
            {
                throw new ParseFailedException(GetInvalidInputMessage(i + lineOffset));
            }

            visits[i] = new Visit(type, index);
        }

        return visits;
    }

    private static void CheckValidVisits(TestCase testCase, Visit[] visits)
    {
        if (!SolutionConstraintChecker.IsValidStart(visits))
        {
            throw new ParseFailedException(InvalidStartMessage);
        }

        if (!SolutionConstraintChecker.IsValidGoal(visits))
        {
            throw new ParseFailedException(InvalidGoalMessage);
        }

        if (!SolutionConstraintChecker.HasVisitedAll(testCase, visits))
        {
            throw new ParseFailedException(NotVisitedAllMessage);
        }
    }

    private const string NotEnoughLineMessage = "出力の長さが不足しています。";
    private const string InvalidVisitLengthMessage = "経路の長さは正の整数でなければなりません。";
    private const string NotVisitedAllMessage = "未訪問の星が存在します。";
    private const string InvalidStartMessage = "経路は星1から開始しなければなりません。";
    private const string InvalidGoalMessage = "経路は星1で終了しなければなりません。";

    private static string GetInvalidInputMessage(int lineNumber) => $"{lineNumber + 1}行目の出力が不正です。";

    private static AstronomicalType ParseAstronomicalType(string type, int lineNumber) => type switch
    {
        "1" => AstronomicalType.Planet,
        "2" => AstronomicalType.Station,
        _ => throw new ParseFailedException(GetInvalidInputMessage(lineNumber))
    };
}