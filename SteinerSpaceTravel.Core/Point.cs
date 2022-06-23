using SkiaSharp;

namespace SteinerSpaceTravel.Core;

public readonly record struct Point(int X, int Y)
{
    public long CalculateSquaredDistance(Point other)
    {
        var dx = (long)X - other.X;
        var dy = (long)Y - other.Y;
        return dx * dx + dy * dy;
    }

    internal SKPoint ToSkPoint() => new(X, Y);
}