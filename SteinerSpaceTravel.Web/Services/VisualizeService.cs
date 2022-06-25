using Cysharp.Web;
using Microsoft.JSInterop;
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

    private readonly IJSRuntime _jsRuntime;

    private static readonly string[] NewLines = { "\r\n", "\r", "\n" };

    public VisualizeService(IJSRuntime jsRuntime)
    {
        _input = string.Empty;
        _output = string.Empty;
        ErrorMessage = string.Empty;
        Seed = 0;
        Score = 0;
        _jsRuntime = jsRuntime;
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

    public async Task Tweet()
    {
        // https://developer.twitter.com/en/docs/twitter-for-websites/tweet-button/guides/web-intent
        const string webIntentUrl = "https://twitter.com/intent/tweet";
        const string url = "https://google.com/";
        const string hashtags = "test,visualizer";
        var text = $"XXXXXのseed={Seed}で{Score:#,##0}点を獲得しました！（ここに画像を貼ってね）";
        var tweetUrl = WebSerializer.ToQueryString(webIntentUrl, new { text, url, hashtags });
        await _jsRuntime.InvokeVoidAsync("open", tweetUrl, "_blank", "noopener");
    }
}