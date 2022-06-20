namespace SteinerSpaceTravel.Core.Checkers;

internal static class TestCaseConstraintChecker
{
    private const int MinCoordinate = 0;
    private const int MaxCoordinate = 1000;

    public static bool IsNInRange(int n) => n > 0;

    public static bool IsMInRange(int m) => m >= 0;

    public static bool IsXInRange(int x) => x is >= MinCoordinate and <= MaxCoordinate;

    public static bool IsYInRange(int y) => y is >= MinCoordinate and <= MaxCoordinate;
}