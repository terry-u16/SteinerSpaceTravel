namespace SteinerSpaceTravel.Core;

public class Solution
{
    private readonly Point[] _stations;
    public ReadOnlySpan<Point> Stations => _stations;

    private readonly Visit[] _visits;
    public ReadOnlySpan<Visit> Visits => _visits;

    public Solution(Point[] stations, Visit[] visits)
    {
        _stations = stations;
        _visits = visits;
    }
}