using Cysharp.Diagnostics;
using SkiaSharp;
using SteinerSpaceTravel.Core.Generators;
using SteinerSpaceTravel.Core.Judges;
using SteinerSpaceTravel.Core.Parsers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SteinerSpaceTravel.Console;

public class Commands : ConsoleAppBase
{
    private const int CanvasSize = 1100;

    [Command("gen", "Generate testcases.")]
    public async Task GenerateTestCases([Option("s", "seedファイルのパス")] string seeds)
    {
        const string directoryPath = "input";

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

            while (await reader.ReadLineAsync().ConfigureAwait(false) is string line)
            {
                if (Context.CancellationToken.IsCancellationRequested)
                {
                    System.Console.WriteLine("キャンセルされました。");
                    return;
                }

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
                await File.WriteAllTextAsync(outputPath, testCase.ToString(), Encoding.Default).ConfigureAwait(false);
                System.Console.WriteLine($"seed = {seed} のケースを生成しました。");
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

            var testCaseText = await File.ReadAllLinesAsync(input).ConfigureAwait(false);
            var solutionText = await File.ReadAllLinesAsync(output).ConfigureAwait(false);

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
                await File.WriteAllBytesAsync(visualize, data.ToArray()).ConfigureAwait(false);
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

    [Command("judge-all", "Judge all testcases.")]
    public async Task JudgeAll([Option("i", "inputファイルのフォルダ")] string input,
        [Option("c", "解答プログラムの実行コマンド")] string command,
        [Option("p", "並列数")] int parallelism = 1)
    {
        if (parallelism <= 0)
        {
            WriteErrorMessage("並列数は1以上の整数である必要があります。");
            return;
        }
        
        if (!CheckDirectoryExistence(input))
        {
            return;
        }

        long totalScore = 0;
        var fileCount = 0;
        var lockObject = new object();
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = parallelism, 
            CancellationToken = Context.CancellationToken
        };

        try
        {
            var files = Directory.EnumerateFiles(input, "*.txt");
            await Parallel.ForEachAsync(files, parallelOptions, async (fileName, cancellationToken) =>
                {
                    var testCaseName = Path.GetFileNameWithoutExtension(fileName);
                    Interlocked.Increment(ref fileCount);

                    try
                    {
                        var (score, message, elapsedMilliseconds) =
                            await RunProcessAsync(command, fileName, cancellationToken);

                        if (message is not null)
                        {
                            lock (lockObject)
                            {
                                WriteErrorMessage(message);
                            }
                        }

                        WriteResult(testCaseName, score, elapsedMilliseconds);
                        Interlocked.Add(ref totalScore, score);
                    }
                    catch (Exception ex)
                    {
                        lock (lockObject)
                        {
                            WriteErrorMessage($"[{testCaseName}] {ex.Message.Trim()}");
                            WriteResult(testCaseName, 0);
                        }
                    }
                });
        }
        catch (TaskCanceledException)
        {
            System.Console.WriteLine("キャンセルされました。");
            return;
        }

        if (fileCount == 0)
        {
            WriteErrorMessage("テストケースが見つかりませんでした。");
            return;
        }

        var averageScore = totalScore / fileCount;

        System.Console.WriteLine($"Testcase count: {fileCount}");
        System.Console.WriteLine($"Total score: {totalScore}");
        System.Console.WriteLine($"Average score: {averageScore}");
    }
    
    private static bool CheckDirectoryExistence(string path)
    {
        if (Directory.Exists(path))
        {
            return true;
        }
        else if (File.Exists(path))
        {
            WriteErrorMessage($"ファイルではなくディレクトリを指定してください: {path}");
            return false;
        }
        else
        {
            WriteErrorMessage($"ディレクトリが存在しません: {path}");
            return false;
        }
    }
    
    private static bool CheckFileExistence(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }
        else if (Directory.Exists(path))
        {
            WriteErrorMessage($"ディレクトリではなくファイルを指定してください: {path}");
            return false;
        }
        else
        {
            WriteErrorMessage($"ファイルが存在しません: {path}");
            return false;
        }
    }

    private static async Task<(long score, string? message, long elapsedMilliseconds)> RunProcessAsync(
        string command, string fileName, CancellationToken cancellationToken = default)
    {
        var (process, stdOut, _) = ProcessX.GetDualAsyncEnumerable(command);

        try
        {
            var testCaseString = await File.ReadAllLinesAsync(fileName, cancellationToken).ConfigureAwait(false);
            var testCase = TestCaseParser.Parse(testCaseString);

            try
            {
                if (!process.HasExited)
                {
                    await using var stdIn = process.StandardInput;
                    await stdIn.WriteAsync(string.Join('\n', testCaseString)).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                // 途中でプロセスが終了する可能性もあるが、その場合は無視
            }

            // process.TotalProcessorTimeを使用したいが、プロセス終了後に参照することができない
            var stopwatch = Stopwatch.StartNew();
            var output = new List<string>();
            await foreach (var line in stdOut.WithCancellation(cancellationToken))
            {
                output.Add(line);
            }

            stopwatch.Stop();

            var solution = SolutionParser.Parse(testCase, CollectionsMarshal.AsSpan(output));
            var (score, message) = Judge.CalculateScore(solution);
            return (score, message, stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            // deconstructしながらusingができないためfinally句で
            process.Dispose();
        }
    }

    private static void WriteResult(string testCase, long score) 
        => System.Console.WriteLine($"[{testCase}] score: {score}");

    private static void WriteResult(string testCase, long score, long elapsedMilliseconds) 
        => System.Console.WriteLine($"[{testCase}] score: {score} (time: ~{elapsedMilliseconds}ms)");

    private static void WriteErrorMessage(string message)
    {
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine(message);
        System.Console.ResetColor();
    }
}