using SteinerSpaceTravel.Core.Generators;
using SteinerSpaceTravel.Core.Judges;
using SteinerSpaceTravel.Core.Parsers;
using System.Text;

namespace SteinerSpaceTravel.Console;

public class Commands : ConsoleAppBase
{
    [Command("gen", "Generate testcases.")]
    public void GenerateTestCases([Option(0, "path to seed file.")] string path)
    {
        const string directoryPath = "in";

        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!CheckFileExistence(path))
            {
                return;
            }

            using var reader = new StreamReader(path, Encoding.UTF8);

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
    public void JudgeTestCase([Option(0, "Path to input file.")] string inputPath,
        [Option(1, "Path to output file.")] string outputPath)
    {
        try
        {
            if (!CheckFileExistence(inputPath) || !CheckFileExistence(outputPath))
            {
                return;
            }

            var testCaseText = File.ReadAllLines(inputPath);
            var solutionText = File.ReadAllLines(outputPath);

            var testCase = TestCaseParser.Parse(testCaseText);
            var solution = SolutionParser.Parse(testCase, solutionText);
            var score = Judge.CalculateScore(solution);

            System.Console.WriteLine($"Score: {score}");
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