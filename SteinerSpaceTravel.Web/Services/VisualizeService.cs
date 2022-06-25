using BlazorDownloadFile;
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
    
    public bool CanDownloadImage => _testCase != null;

    public bool CanTweet => Score != 0;

    private TestCase? _testCase;

    private Solution? _solution;

    private readonly IJSRuntime _jsRuntime;

    private readonly IBlazorDownloadFileService _blazorDownloadFileService;

    private static readonly string[] NewLines = { "\r\n", "\r", "\n" };

    public VisualizeService(IJSRuntime jsRuntime, IBlazorDownloadFileService blazorDownloadFileService)
    {
        _input = string.Empty;
        _output = string.Empty;
        ErrorMessage = string.Empty;
        Seed = 0;
        Score = 0;
        _jsRuntime = jsRuntime;
        _blazorDownloadFileService = blazorDownloadFileService;
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

    public async Task DownloadImageAsync()
    {
        const int canvasSize = 1100;
        const string fileName = "vis.png";
        var imageInfo = new SKImageInfo(canvasSize, canvasSize, SKColorType.Rgba8888, SKAlphaType.Opaque);
        using var bitmap = new SKBitmap(imageInfo);
        using var canvas = new SKCanvas(bitmap);

        Visualize(canvas, imageInfo);

        var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        await _blazorDownloadFileService.DownloadFile(fileName, data.ToArray(), "image/png");
    }

    public async Task TweetAsync()
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