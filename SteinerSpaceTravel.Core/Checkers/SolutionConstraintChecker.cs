namespace SteinerSpaceTravel.Core.Checkers;

internal static class SolutionConstraintChecker
{
    private const int MinCoordinate = 0;
    private const int MaxCoordinate = 1000;
    private static readonly Visit StartPoint = new(AstronomicalType.Planet, 0);

    public static bool IsXInRange(int x) => x is >= MinCoordinate and <= MaxCoordinate;

    public static bool IsYInRange(int y) => y is >= MinCoordinate and <= MaxCoordinate;

    public static bool IsVisitIndexInRange(TestCase testCase, AstronomicalType type, int index)
    {
        var length = type switch
        {
            AstronomicalType.Planet => testCase.N,
            AstronomicalType.Station => testCase.M,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        return (uint)index < (uint)length;
    }

    public static bool HasVisitedAll(TestCase testCase, IEnumerable<Visit> visits)
    {
        var visited = new bool[testCase.N];

        foreach (var (type, index) in visits)
        {
            if (type == AstronomicalType.Planet)
            {
                visited[index] = true;
            }
        }

        return visited.All(v => v);
    }

    public static bool IsValidStart(ReadOnlySpan<Visit> visits) => visits.Length > 0 && visits[0] == StartPoint;

    public static bool IsValidGoal(ReadOnlySpan<Visit> visits) => visits.Length > 0 && visits[^1] == StartPoint;
}