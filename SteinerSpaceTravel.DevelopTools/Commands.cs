using System.Security.Cryptography;
using System.Text;
using SteinerSpaceTravel.Core.Generators;

namespace SteinerSpaceTravel.DevelopTools;

public class Commands : ConsoleAppBase
{
    [Command("seed", "Generate random seeds.")]
    public async Task GenerateSeeds([Option("c", "テストケース数")] int count)
    {
        const string fileName = "seeds.txt";
        var buffer = new byte[8];

        await using var writer = new StreamWriter(fileName);
        using var rng = RandomNumberGenerator.Create();

        for (int i = 0; i < count; i++)
        {
            rng.GetBytes(buffer);
            var seed = BitConverter.ToUInt64(buffer);
            await writer.WriteLineAsync(seed.ToString());
        }
    }

    [Command("gen", "Generate testcases.")]
    public async Task GenerateTestCases([Option("s", "seedファイルのパス")] string seeds)
    {
        // ユーザー版と異なり、テストケース名は1からの連番とする
        const string inputDirectoryPath = "in";
        const string outputDirectoryPath = "out";

        try
        {
            if (!Directory.Exists(inputDirectoryPath))
            {
                Directory.CreateDirectory(inputDirectoryPath);
            }

            if (!Directory.Exists(outputDirectoryPath))
            {
                Directory.CreateDirectory(outputDirectoryPath);
            }

            // 全削除しておく
            foreach (var file in Directory.EnumerateFiles(inputDirectoryPath))
            {
                File.Delete(file);
            }

            foreach (var file in Directory.EnumerateFiles(outputDirectoryPath))
            {
                File.Delete(file);
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
                    Console.WriteLine($"{line}行目のパースに失敗しました。seedは64bit符号なし整数である必要があります。");
                    return;
                }

                outputCount++;
                var testCase = TestCaseGenerator.Generate(seed);
                var inputPath = Path.Join(inputDirectoryPath, $"testcase_{outputCount:000}.txt");
                var outputPath = Path.Join(outputDirectoryPath, $"testcase_{outputCount:000}.txt");
                await File.WriteAllTextAsync(inputPath, testCase.ToString(), Encoding.Default);
                await File.Create(outputPath).DisposeAsync();
                
                Console.WriteLine($"seed = {seed} のケースを生成しました。");
            }

            Console.WriteLine($"{outputCount}件のテストケースを生成しました。");
        }
        catch (IOException ex)
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
            Console.WriteLine($"ファイルが存在しません: {path}");
            return false;
        }
    }

    private static void WriteErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

}