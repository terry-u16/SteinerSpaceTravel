using SkiaSharp;
using SteinerSpaceTravel.Core.Utilities;

namespace SteinerSpaceTravel.Core.Judges;

public static class Visualizer
{
    private const int CanvasSize = 1000 + 2 * CanvasOffset;
    private const int CanvasOffset = 50;
    private const int EarthRadius = 10;
    private const int CircleRadius = 7;
    private const int RectangularSize = CircleRadius * 2;
    private const int RectangularSizeHalf = RectangularSize / 2;
    private const int PathWidth = 3;
    private const double MinLogEnergy = 3.0;
    private const double MaxLogEnergy = 6.0;
    private static readonly SKPoint Offset = new(CanvasOffset, CanvasOffset);
    private static readonly SKColor MinEnergyColor = new(0x20, 0x20, 0x20);
    private static readonly SKColor MaxEnergyColor = new(0xFF, 0x20, 0x20);
    private static readonly SKColor White = new(255, 255, 255);
    private static readonly SKColor DodgerBlue = new(0x1E, 0x90, 0xFF);
    private static readonly SKColor Gold = new(0xFF, 0xD7, 0x00);
    private static readonly SKColor LightSlateGrey = new(0x77, 0x88, 0x99);
    

    public static SKBitmap Visualize(TestCase testCase)
    {
        var bitmap = new SKBitmap(CanvasSize, CanvasSize, SKColorType.Rgba8888, SKAlphaType.Opaque);
        var canvas = new SKCanvas(bitmap);
        canvas.Clear(White);
        DrawPlanets(canvas, testCase);
        return bitmap;
    }


    public static SKBitmap Visualize(Solution solution)
    {
        var bitmap = new SKBitmap(CanvasSize, CanvasSize, SKColorType.Rgba8888, SKAlphaType.Opaque);
        var canvas = new SKCanvas(bitmap);
        canvas.Clear(White);
        DrawPlanets(canvas, solution.TestCase);
        DrawStations(canvas, solution);
        DrawLines(canvas, solution);
        return bitmap;
    }

    private static void DrawPlanets(SKCanvas canvas, TestCase testCase)
    {
        var points = testCase.Points;
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = DodgerBlue,
            IsAntialias = true
        };

        canvas.DrawCircle(points[0].ToSkPoint() + Offset, EarthRadius, paint);
        paint.Color = Gold;

        foreach (var point in points[1..])
        {
            canvas.DrawCircle(point.ToSkPoint() + Offset, CircleRadius, paint);
        }
    }

    private static void DrawStations(SKCanvas canvas, Solution solution)
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
            var p = point.ToSkPoint() + Offset;
            var x = p.X - RectangularSizeHalf;
            var y = p.Y - RectangularSizeHalf;
            canvas.DrawRect(x, y, RectangularSize, RectangularSize, paint);
        }
    }

    private static void DrawLines(SKCanvas canvas, Solution solution)
    {
        using var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = PathWidth,
            IsAntialias = true
        };

        for (int i = 0; i + 1 < solution.Visits.Length; i++)
        {
            var prevPoint = solution.GetPointAt(i).ToSkPoint() + Offset;
            var nextPoint = solution.GetPointAt(i + 1).ToSkPoint() + Offset;
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
}