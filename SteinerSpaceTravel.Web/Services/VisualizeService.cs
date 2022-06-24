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

    public long Score { get; private set; }

    public string ErrorMessage { get; private set; }

    private TestCase? _testCase;

    private Solution? _solution;

    private static readonly string[] NewLines = { "\r\n", "\r", "\n" };

    public VisualizeService()
    {
        _input = string.Empty;
        _output = string.Empty;
        ErrorMessage = string.Empty;
        Seed = 0;
        Score = 0;
        CalculateScore();
    }

    public void Visualize(SKCanvas canvas, SKImageInfo imageInfo)
    {
        if (_testCase is null)
        {
            Visualizer.Visualize(canvas, imageInfo);
        }
        else if (_solution is null)
        {
            Visualizer.Visualize(_testCase, canvas, imageInfo);
        }
        else
        {
            Visualizer.Visualize(_solution, canvas, imageInfo);
        }
    }

    private void CalculateScore()
    {
        _testCase = null;
        _solution = null;
        Score = 0;
        ErrorMessage = string.Empty;

        try
        {
            _testCase = TestCaseParser.Parse(Input.Split(NewLines, StringSplitOptions.TrimEntries));
        }
        catch (ParseFailedException ex)
        {
            ErrorMessage = $"({ex.Message})";
            return;
        }

        try
        {
            _solution = SolutionParser.Parse(_testCase, Output.Split(NewLines, StringSplitOptions.TrimEntries));
        }
        catch (ParseFailedException ex)
        {
            if (Output.Trim().Length > 0)
            {
                ErrorMessage = $"({ex.Message})";
            }

            return;
        }

        var (score, message) = Judge.CalculateScore(_solution);

        Score = score;

        if (message is not null)
        {
            ErrorMessage = $"({message})";
        }
    }
}