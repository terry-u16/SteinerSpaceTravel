using SkiaSharp;
using SteinerSpaceTravel.Core;
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

    public string Input
    {
        get => _input;
        set
        {
            _input = value;
            CalculateScore();
        }
    }

    private string _input;

    public string Output
    {
        get => _output;
        set
        {
            _output = value;
            CalculateScore();
        }
    }

    private string _output;

    public long Score { get; set; }

    public string ErrorMessage { get; set; }

    private static readonly string[] NewLines = new string[] { "\r\n", "\r", "\n" };

    public VisualizeService()
    {
        _input = string.Empty;
        _output = string.Empty;
        ErrorMessage = string.Empty;
        Seed = 0;
        Score = 0;
    }

    public void CalculateScore()
    {
        TestCase testCase;
        Solution solution;

        Score = 0;
        ErrorMessage = string.Empty;

        try
        {
            testCase = TestCaseParser.Parse(Input.Split(NewLines, StringSplitOptions.TrimEntries));
        }
        catch (ParseFailedException ex)
        {
            ErrorMessage = $"({ex.Message})";
            return;
        }

        try
        {
            solution = SolutionParser.Parse(testCase, Output.Split(NewLines, StringSplitOptions.TrimEntries));
        }
        catch (ParseFailedException ex)
        {
            if (Output.Trim().Length > 0)
            {
                ErrorMessage = $"({ex.Message})";
            }

            return;
        }

        Score = Judge.CalculateScore(solution);
    }

    public void Visualize(SKCanvas canvas, SKImageInfo imageInfo)
    {
        TestCase testCase;
        Solution solution;

        try
        {
            testCase = TestCaseParser.Parse(Input.Split(NewLines, StringSplitOptions.TrimEntries));
        }
        catch (ParseFailedException)
        {
            Visualizer.Visualize(canvas, imageInfo);
            return;
        }

        try
        {
            solution = SolutionParser.Parse(testCase, Output.Split(NewLines, StringSplitOptions.TrimEntries));
        }
        catch (ParseFailedException)
        {
            Visualizer.Visualize(testCase, canvas, imageInfo);
            return;
        }

        Visualizer.Visualize(solution, canvas, imageInfo);
    }
}