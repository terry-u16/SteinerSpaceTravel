namespace SteinerSpaceTravel.Core;

public class TestCaseGenerator
{
    private const int N = 100;
    private const int M = 8;
    private const int PivotCount = 15;
    private const int ScatterRange = 100;
    private const int MapSize = 1000;

    public static TestCase Generate(ulong seed)
    {
        var random = new Xoshiro256(seed);
        var pivots = new Point[PivotCount];

        for (int i = 0; i < pivots.Length; i++)
        {
            while (true)
            {
                var x = random.Next(ScatterRange, MapSize - ScatterRange + 1);
                var y = random.Next(ScatterRange, MapSize - ScatterRange + 1);
                var pivot = new Point(x, y); 
                var ok = pivots.Take(i).All(p => pivot.CalculateSquaredDistance(p) > ScatterRange * ScatterRange);

                if (ok)
                {
                    pivots[i] = pivot;
                    break;
                }
            }
        }

        var planetSet = new HashSet<Point>();
        var planets = new Point[N];

        foreach (ref var planet in planets.AsSpan())
        {
            while (true)
            {
                var pivotIndex = random.Next(pivots.Length);
                var (u, v) = pivots[pivotIndex];
                var dx = random.Next(-ScatterRange, ScatterRange + 1);
                var dy = random.Next(-ScatterRange, ScatterRange + 1);
                var x = u + dx;
                var y = v + dy;
                planet = new Point(x, y);

                if (planetSet.Add(planet))
                {
                    break;
                }
            }
        }

        var testCase = new TestCase(N, M, planets);
        return testCase;
    }
}
