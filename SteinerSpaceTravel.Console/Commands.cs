using SixLabors.ImageSharp;
using SteinerSpaceTravel.Core.Generators;
using SteinerSpaceTravel.Core.Judges;
using SteinerSpaceTravel.Core.Parsers;
using System.Text;

namespace SteinerSpaceTravel.Console;

public class Commands : ConsoleAppBase
{
    [Command("gen", "Generate testcases.")]
    public void GenerateTestCases([Option("s", "seedファイルのパス")] string seeds)
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

            while (reader.ReadLine() is string line)
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
                File.WriteAllText(outputPath, testCase + Environment.NewLine);
                outputCount++;
            }

            System.Console.WriteLine($"{outputCount}件のテストケースを生成しました。");
        }
        catch (IOException ex)
        {
            WriteErrorMessage(ex);
        }
    }

    [Command("judge", "Judge a testcase.")]
    public void JudgeTestCase([Option("i", "inputファイルのパス")] string input,
        [Option("o", "outputファイルのパス")] string output,
        [Option("v", "ビジュアライズ結果 (png) の出力先")] string? visualize = null)
    {
        try
        {
            if (!CheckFileExistence(input) || !CheckFileExistence(output))
            {
                return;
            }

            var testCaseText = File.ReadAllLines(input);
            var solutionText = File.ReadAllLines(output);

            var testCase = TestCaseParser.Parse(testCaseText);
            var solution = SolutionParser.Parse(testCase, solutionText);
            var score = Judge.CalculateScore(solution);

            System.Console.WriteLine($"Score: {score}");

            if (visualize is not null)
            {
                var image = Visualizer.Visualize(solution);
                image.SaveAsPng(visualize);
                System.Console.WriteLine($@"ビジュアライズ結果を""{visualize}""に保存しました。");
            }
        }
        catch (IOException ex)
        {
            WriteErrorMessage(ex);
        }
        catch (ParseFailedException ex)
        {
            WriteErrorMessage(ex);
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

    private static void WriteErrorMessage(Exception ex)
    {
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine(ex.Message);
        System.Console.ResetColor();
    }
}