using System.Security.Cryptography;

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
}