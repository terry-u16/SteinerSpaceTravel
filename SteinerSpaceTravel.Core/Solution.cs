namespace SteinerSpaceTravel.Core;

public class Solution
{
    public TestCase TestCase { get; }

    private readonly Point[] _stations;
    public ReadOnlySpan<Point> Stations => _stations;

    private readonly Visit[] _visits;
    public ReadOnlySpan<Visit> Visits => _visits;

    public Solution(TestCase testCase, Point[] stations, Visit[] visits)
    {
        TestCase = testCase;
        _stations = stations;
        _visits = visits;
    }

    public Point GetPointAt(int index)
    {
        if ((uint)index >= (uint)Visits.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var (type, i) = Visits[index];
        var point = type switch
        {
            AstronomicalType.Planet => TestCase.Points[i],
            AstronomicalType.Station => Stations[i],
            _ => throw new InvalidOperationException()
        };

        return point;
    }

    public AstronomicalType GetTypeAt(int index)
    {
        if ((uint)index >= (uint)Visits.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return Visits[index].Type;
    }
}