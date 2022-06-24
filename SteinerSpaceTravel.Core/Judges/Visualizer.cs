using SkiaSharp;
using SteinerSpaceTravel.Core.Utilities;
using System.Numerics;

namespace SteinerSpaceTravel.Core.Judges;

public static class Visualizer
{
    private const int VirtualCanvasSize = 1000 + 2 * CanvasOffset;
    private const int CanvasOffset = 20;
    private const int EarthRadius = 10;
    private const int CircleRadius = 7;
    private const int RectangularSize = 16;
    private const int RectangularSizeHalf = RectangularSize / 2;
    private const int PathWidth = 3;
    private const double MinLogEnergy = 3.0;
    private const double MaxLogEnergy = 6.0;
    private static readonly Vector2 Offset = new(CanvasOffset, CanvasOffset);
    private static readonly SKColor MinEnergyColor = new(0x20, 0x20, 0x20);
    private static readonly SKColor MaxEnergyColor = new(0xFF, 0x20, 0x20);
    private static readonly SKColor White = new(255, 255, 255);
    private static readonly SKColor DodgerBlue = new(0x1E, 0x90, 0xFF);
    private static readonly SKColor Gold = new(0xFF, 0xD7, 0x00);
    private static readonly SKColor LightSlateGrey = new(0x77, 0x88, 0x99);

    public static void Visualize(SKCanvas canvas, SKImageInfo imageInfo) => canvas.Clear(White);

    public static void Visualize(TestCase testCase, SKCanvas canvas, SKImageInfo imageInfo)
    {
        var canvasScale = (float)Math.Min(imageInfo.Width, imageInfo.Height) / VirtualCanvasSize;
        canvas.Clear(White);
        DrawPlanets(canvas, testCase, canvasScale);
    }

    public static void Visualize(Solution solution, SKCanvas canvas, SKImageInfo imageInfo)
    {
        var canvasScale = (float)Math.Min(imageInfo.Width, imageInfo.Height) / VirtualCanvasSize;
        canvas.Clear(White);
        DrawPlanets(canvas, solution.TestCase, canvasScale);
        DrawStations(canvas, solution, canvasScale);
        DrawLines(canvas, solution, canvasScale);
    }

    private static void DrawPlanets(SKCanvas canvas, TestCase testCase, float canvasScale)
    {
        var points = testCase.Points;
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = DodgerBlue,
            IsAntialias = true
        };

        var p0 = AffineTransform(points[0], canvasScale);
        canvas.DrawCircle(p0, EarthRadius * canvasScale, paint);
        paint.Color = Gold;

        foreach (var point in points[1..])
        {
            var p = AffineTransform(point, canvasScale);
            canvas.DrawCircle(p, CircleRadius * canvasScale, paint);
        }
    }

    private static void DrawStations(SKCanvas canvas, Solution solution, float canvasScale)
    {
        var stations = solution.Stations;
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = LightSlateGrey,
            IsAntialias = true
        };

        foreach (var point in stations)
        {
            var p = AffineTransform(point, canvasScale);
            var x = p.X - RectangularSizeHalf * canvasScale;
            var y = p.Y - RectangularSizeHalf * canvasScale;
            var size = RectangularSize * canvasScale;
            canvas.DrawRect(x, y, size, size, paint);
        }
    }

    private static void DrawLines(SKCanvas canvas, Solution solution, float canvasScale)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = PathWidth * canvasScale,
            IsAntialias = true
        };

        for (int i = 0; i + 1 < solution.Visits.Length; i++)
        {
            var prevPoint = AffineTransform(solution.GetPointAt(i), canvasScale);
            var nextPoint = AffineTransform(solution.GetPointAt(i + 1), canvasScale);
            var energy = Judge.CalculateEnergy(solution, i, i + 1);
            paint.Color = InterpolateColor(energy);
            canvas.DrawLine(prevPoint, nextPoint, paint);
        }
    }

    private static SKColor InterpolateColor(long energy)
    {
        // 違いが分かりやすいようにlogスケールにする
        var logEnergy = Math.Log10(energy + 1);
        var x = (logEnergy - MinLogEnergy) / (MaxLogEnergy - MinLogEnergy);

        // [0, 1]の範囲にクリッピング
        x = Math.Max(Math.Min(x, 1.0), 0.0);
        return MinEnergyColor.Blend(MaxEnergyColor, x);
    }

    private static SKPoint AffineTransform(Point point, float scale) => ((point.ToVector2() + Offset) * scale).ToSkPoint();
}