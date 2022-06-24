using SkiaSharp;
using System.Numerics;

namespace SteinerSpaceTravel.Core.Utilities;

internal static class VisualizerExtension
{
    public static SKColor Blend(this SKColor c1, SKColor c2, double ratio)
    {
        static byte BlendOne(byte a, byte b, double ratio) => (byte)(a * (1 - ratio) + b * ratio);

        var a = BlendOne(c1.Alpha, c2.Alpha, ratio);
        var r = BlendOne(c1.Red, c2.Red, ratio);
        var g = BlendOne(c1.Green, c2.Green, ratio);
        var b = BlendOne(c1.Blue, c2.Blue, ratio);
        return new SKColor(r, g, b, a);
    }

    public static Vector2 ToVector2(this Point point) => new(point.X, point.Y);
    public static SKPoint ToSkPoint(this Point point) => new(point.X, point.Y);
    public static SKPoint ToSkPoint(this Vector2 vector) => new(vector.X, vector.Y);
}