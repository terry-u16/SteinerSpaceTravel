using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;
using Path = SixLabors.ImageSharp.Drawing.Path;

namespace SteinerSpaceTravel.Core.Judges;

public static class Visualizer
{
    private const int CanvasSize = 1000 + 2 * CanvasOffset;
    private const int CanvasOffset = 50;
    private const int StarInnerRadii = 7;
    private const int StarOuterRadii = 15;
    private const int CircleRadius = 7;
    private const int RectangularSize = CircleRadius * 2;
    private const int PathWidth = 3;
    private const double MinLogEnergy = 3.0;
    private const double MaxLogEnergy = 6.0;
    private static readonly PointF Offset = new(CanvasOffset, CanvasOffset);
    private static readonly PointF RectangularOffset = new(-RectangularSize * 0.5f, -RectangularSize * 0.5f);
    private static readonly SizeF Rectangular = new(RectangularSize, RectangularSize);
    private static readonly Color MinEnergyColor = Color.FromRgb(0x20, 0x20, 0x20);
    private static readonly Color MaxEnergyColor = Color.FromRgb(0xFF, 0x20, 0x20);

    public static Image<Rgba32> Visualize(TestCase testCase)
    {
        var image = new Image<Rgba32>(CanvasSize, CanvasSize, Color.White);

        image.Mutate(context => DrawPlanets(context, testCase));
        return image;
    }


    public static Image<Rgba32> Visualize(Solution solution)
    {
        var image = new Image<Rgba32>(CanvasSize, CanvasSize, Color.White);

        image.Mutate(context => DrawSolutionAll(context, solution));
        return image;
    }

    private static IImageProcessingContext DrawSolutionAll(IImageProcessingContext context, Solution solution)
    {
        context = DrawPlanets(context, solution.TestCase);
        context = DrawStations(context, solution);
        context = DrawLines(context, solution);
        return context;
    }

    private static IImageProcessingContext DrawPlanets(IImageProcessingContext context, TestCase testCase)
    {
        var points = testCase.Points;
        var planetPath0 = new Star(points[0].ToPointF() + Offset, 5, StarInnerRadii, StarOuterRadii);
        context = context.Fill(Color.DodgerBlue, planetPath0);

        for (var i = 1; i < points.Length; i++)
        {
            var planetPath = new EllipsePolygon(points[i].ToPointF() + Offset, CircleRadius);
            context = context.Fill(Color.Gold, planetPath);
        }

        return context;
    }

    private static IImageProcessingContext DrawStations(IImageProcessingContext context, Solution solution)
    {
        var stations = solution.Stations;

        for (var i = 0; i < stations.Length; i++)
        {
            var stationPath = new RectangularPolygon(stations[i].ToPointF() + Offset + RectangularOffset, Rectangular);
            context = context.Fill(Color.LightSlateGrey, stationPath);
        }

        return context;
    }

    private static IImageProcessingContext DrawLines(IImageProcessingContext context, Solution solution)
    {
        for (var i = 0; i + 1 < solution.Visits.Length; i++)
        {
            var prevPoint = solution.GetPointAt(i).ToPointF() + Offset;
            var nextPoint = solution.GetPointAt(i + 1).ToPointF() + Offset;
            var energy = Judge.CalculateEnergy(solution, i, i + 1);
            var color = InterpolateColor(energy);
            var lineSegment = new LinearLineSegment(prevPoint, nextPoint);
            var pen = Pens.Solid(color, PathWidth);
            var path = new Path(lineSegment);
            context = context.Draw(pen, path);
        }

        return context;
    }

    private static Color InterpolateColor(long energy)
    {
        // 違いが分かりやすいようにlogスケールにする
        var logEnergy = Math.Log10(energy + 1);
        var doubleX = (logEnergy - MinLogEnergy) / (MaxLogEnergy - MinLogEnergy);

        // [0, 1]の範囲にクリッピング
        doubleX = Math.Max(Math.Min(doubleX, 1.0), 0.0);
        var x = (float)doubleX;

        var minVector = (Vector4)MinEnergyColor;
        var maxVector = (Vector4)MaxEnergyColor;
        var colorVector = minVector * (1 - x) + maxVector * x;
        return new Color(colorVector);
    }
}