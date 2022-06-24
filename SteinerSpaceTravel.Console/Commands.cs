using SteinerSpaceTravel.Core.Generators;
using SteinerSpaceTravel.Core.Judges;
using SteinerSpaceTravel.Core.Parsers;
using System.Text;
using SkiaSharp;

namespace SteinerSpaceTravel.Console;

public class Commands : ConsoleAppBase
{
    private const int CanvasSize = 1100;

    [Command("gen", "Generate testcases.")]
    public async Task GenerateTestCases([Option("s", "seedファイルのパス")] string seeds)
    {
        const string directoryPath = "in";

        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!CheckFileExistence(seeds))
            {
                return;
            }

            using var reader = new StreamReader(seeds, Encoding.UTF8);

            var outputCount = 0;

            while (await reader.ReadLineAsync() is string line)
            {
                line = line.Trim();
                if (line.Length == 0)
                {
                    continue;
                }

                if (!ulong.TryParse(line, out var seed))
                {
                    System.Console.WriteLine($"{line}行目のパースに失敗しました。seedは64bit符号なし整数である必要があります。");
                    return;
                }

                var testCase = TestCaseGenerator.Generate(seed);
                var outputPath = Path.Join(directoryPath, $"{seed:0000}.txt");
                await File.WriteAllTextAsync(outputPath, testCase + Environment.NewLine, Encoding.UTF8);
                outputCount++;
            }

            System.Console.WriteLine($"{outputCount}件のテストケースを生成しました。");
        }
        catch (IOException ex)
        {
            WriteErrorMessage(ex.Message);
        }
    }

    [Command("judge", "Judge a testcase.")]
    public async Task JudgeTestCase([Option("i", "inputファイルのパス")] string input,
        [Option("o", "outputファイルのパス")] string output,
        [Option("v", "ビジュアライズ結果 (png) の出力先")] string? visualize = null)
    {
        try
        {
            if (!CheckFileExistence(input) || !CheckFileExistence(output))
            {
                return;
            }

            var testCaseText = await File.ReadAllLinesAsync(input, Encoding.UTF8);
            var solutionText = await File.ReadAllLinesAsync(output, Encoding.UTF8);

            var testCase = TestCaseParser.Parse(testCaseText);
            var solution = SolutionParser.Parse(testCase, solutionText);
            var (score, message) = Judge.CalculateScore(solution);

            if (message is not null)
            {
                WriteErrorMessage(message);
            }

            System.Console.WriteLine($"Score: {score}");

            if (visualize is not null)
            {
                var imageInfo = new SKImageInfo(CanvasSize, CanvasSize, SKColorType.Rgba8888, SKAlphaType.Opaque);
                using var bitmap = new SKBitmap(imageInfo);
                using var canvas = new SKCanvas(bitmap);

                Visualizer.Visualize(solution, canvas, imageInfo);
                var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
                await File.WriteAllBytesAsync(visualize, data.ToArray());
                System.Console.WriteLine($@"ビジュアライズ結果を""{visualize}""に保存しました。");
            }
        }
        catch (IOException ex)
        {
            WriteErrorMessage(ex.Message);
        }
        catch (ParseFailedException ex)
        {
            WriteErrorMessage(ex.Message);
        }
    }

    private static bool CheckFileExistence(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            System.Console.WriteLine($"ファイルが存在しません: {path}");
            return false;
        }
    }

    private static void WriteErrorMessage(string message)
    {
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine(message);
        System.Console.ResetColor();
    }
}