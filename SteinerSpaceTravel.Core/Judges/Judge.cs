namespace SteinerSpaceTravel.Core.Judges;

public static class Judge
{
    private const long PlanetMultiplier = 5;
    private const long BaseNumerator = 100_000_000;
    private const long BaseDenominator = 1_000;

    public static long CalculateScore(Solution solution)
    {
        var totalEnergy = CalculateTotalEnergy(solution);
        return ToScore(totalEnergy);
    }

    private static long CalculateTotalEnergy(Solution solution)
    {
        long totalEnergy = 0;

        for (var i = 0; i + 1 < solution.Visits.Length; i++)
        {
            totalEnergy += CalculateEnergy(solution, i, i + 1);
        }

        return totalEnergy;
    }

    internal static long CalculateEnergy(Solution solution, int prevIndex, int nextIndex)
    {
        var prevType = solution.GetTypeAt(prevIndex);
        var prevPoint = solution.GetPointAt(prevIndex);
        var nextType = solution.GetTypeAt(nextIndex);
        var nextPoint = solution.GetPointAt(nextIndex);

        var baseEnergy = prevPoint.CalculateSquaredDistance(nextPoint);
        var prevMultiplier = GetEnergyMultiplier(prevType);
        var nextMultiplier = GetEnergyMultiplier(nextType);
        return baseEnergy * prevMultiplier * nextMultiplier;
    }

    private static long GetEnergyMultiplier(AstronomicalType type) => type switch
    {
        AstronomicalType.Planet => PlanetMultiplier,
        AstronomicalType.Station => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    private static long ToScore(long totalEnergy) 
        => (long)Math.Round(BaseNumerator / (BaseDenominator + Math.Sqrt(totalEnergy)));
}