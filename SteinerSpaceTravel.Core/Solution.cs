namespace SteinerSpaceTravel.Core;

public class Solution
{
    public Point[] Stations { get; }
    public Visit[] Visits { get; }

    public Solution(Point[] stations, Visit[] visits)
    {
        Stations = stations;
        Visits = visits;
    }
}