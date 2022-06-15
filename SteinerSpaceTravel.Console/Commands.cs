using SteinerSpaceTravel.Core;
using System.Text;

namespace SteinerSpaceTravel.Console;

public class Commands : ConsoleAppBase
{
    [Command("gen", "Generate testcases.")]
    public void Generate([Option(0, "path to seed file.")] string path)
    {
        const string directoryPath = "in";

        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(path))
            {
                System.Console.WriteLine($"No such file: {path}");
                return;
            }

            using var reader = new StreamReader(path, Encoding.UTF8);

            while (reader.ReadLine() is string line)
            {
                line = line.Trim();
                if (line.Length == 0)
                {
                    continue;
                }

                if (!ulong.TryParse(line, out var seed))
                {
                    System.Console.WriteLine($"Parse failed: {line}");
                    return;
                }

                var testCase = TestCaseGenerator.Generate(seed);
                var outputPath = Path.Join(directoryPath, $"{seed:0000}.txt");
                File.WriteAllText(outputPath, testCase + Environment.NewLine);
            }
        }
        catch (IOException ex)
        {
            System.Console.WriteLine(ex.Message);
        }
    }
}