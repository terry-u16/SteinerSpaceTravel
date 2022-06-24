using SkiaSharp;
using SteinerSpaceTravel.Core.Generators;
using SteinerSpaceTravel.Core.Judges;
using SteinerSpaceTravel.Core.Parsers;

namespace SteinerSpaceTravel.Web.Services;

public class VisualizeService
{
    public ulong Seed
    {
        get => _seed;
        set
        {
            _seed = value;
            Input = TestCaseGenerator.Generate(_seed).ToString() + Environment.NewLine;
        }
    }

    private ulong _seed;

    public string Input { get; set; }

    public string Output { get; set; }

    private static readonly string[] NewLines = new string[] { "\r\n", "\r", "\n" };

    public VisualizeService()
    {
        Input = string.Empty;
        Output = string.Empty;
        Seed = 0;
    }

    public void Visualize(SKCanvas canvas, SKImageInfo imageInfo)
    {
        var input = TestCaseParser.Parse(Input.Split(NewLines, StringSplitOptions.TrimEntries));
        Visualizer.Visualize(input, canvas, imageInfo);
    }
}