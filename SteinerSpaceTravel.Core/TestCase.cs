using System.Text;

namespace SteinerSpaceTravel.Core;

public class TestCase
{
    public int N { get; }
    public int M { get; }

    private readonly Point[] _points;
    public ReadOnlySpan<Point> Points => _points;

    private const int MaxN = int.MaxValue / 2;
    private const int MaxM = int.MaxValue / 2;
    private const int MinXy = 0;
    private const int MaxXy = 1000;

    public TestCase(int n, int m, Point[] points)
    {
        if (n <= 0 || MaxN < n)
        {
            throw new ArgumentOutOfRangeException(nameof(n));
        }

        if (m < 0 || MaxM < m)
        {
            throw new ArgumentOutOfRangeException(nameof(m));
        }

        if (points.Any(point => point.X < MinXy || MaxXy < point.X || point.Y < MinXy || MaxXy < point.Y))
        {
            throw new ArgumentOutOfRangeException(nameof(points));
        }

        N = n;
        M = m;
        _points = points;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append($"{N} {M}\n");

        foreach (var (x, y) in Points)
        {
            builder.Append($"{x} {y}\n");
        }
        return builder.ToString();
    }
}
