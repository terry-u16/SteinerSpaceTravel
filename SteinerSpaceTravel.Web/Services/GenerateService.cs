using System.IO.Compression;
using System.Text;
using BlazorDownloadFile;
using SteinerSpaceTravel.Core.Generators;

namespace SteinerSpaceTravel.Web.Services;

public class GenerateService
{
    public ulong SeedFrom { get; set; }
    public ulong SeedTo { get; set; }
    public bool CanGenerate => SeedFrom <= SeedTo && !IsGenerating;
    public bool IsGenerating { get; private set; }
    public string Message { get; private set; }

    private CancellationTokenSource? _cancellationTokenSource;
    private readonly IBlazorDownloadFileService _blazorDownloadFileService;

    public GenerateService(IBlazorDownloadFileService blazorDownloadFileService)
    {
        SeedFrom = 0;
        SeedTo = 99;
        Message = string.Empty;
        _blazorDownloadFileService = blazorDownloadFileService;
    }

    public async Task DownloadSeedsAsync()
    {
        try
        {
            const string fileName = "in.zip";
            _cancellationTokenSource = new();
            IsGenerating = true;
            Message = "Generating.";

            using var zipStream = await CreateZipStreamAsync(_cancellationTokenSource.Token);
            await _blazorDownloadFileService.DownloadFile(fileName, zipStream, "application/zip");

            Message = "Completed.";
        }
        catch (TaskCanceledException)
        {
            Message = "Canceled.";
        }
        finally
        {
            IsGenerating = false;
        }
    }

    public void CancelDownload() => _cancellationTokenSource?.Cancel();

    private async Task<MemoryStream> CreateZipStreamAsync(CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();
        using var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);

        for (var seed = SeedFrom; seed <= SeedTo; seed++)
        {
            // delayを入れないとなぜか固まる……
            if (seed % 10 == 0)
            {
                await Task.Delay(1, cancellationToken);
            }

            var testCase = TestCaseGenerator.Generate(seed);
            await using var entry = zipArchive.CreateEntry(GetTestCaseFileName(seed)).Open();
            await using var writer = new StreamWriter(entry, Encoding.UTF8);
            await writer.WriteAsync(testCase.ToString() + Environment.NewLine);
        }

        return memoryStream;
    }
    
    private static string GetTestCaseFileName(ulong seed) => $"{seed:0000}.txt";
}
