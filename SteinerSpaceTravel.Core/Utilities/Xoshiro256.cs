using System.Diagnostics;
using System.Numerics;

namespace SteinerSpaceTravel.Core.Utilities;

/// <summary>
/// xoshiro256**
/// </summary>
/// <remarks>
/// <see cref="https://prng.di.unimi.it/"/>
/// </remarks>
internal class Xoshiro256
{
    private ulong _s0;
    private ulong _s1;
    private ulong _s2;
    private ulong _s3;

    public Xoshiro256(int seed) : this((ulong)seed) { }

    public Xoshiro256(ulong seed)
    {
        _s0 = SplitMix64(ref seed);
        _s1 = SplitMix64(ref seed);
        _s2 = SplitMix64(ref seed);
        _s3 = SplitMix64(ref seed);
    }

    public ulong Next()
    {
        var result = BitOperations.RotateLeft(_s1 * 5, 7) * 9;
        var t = _s1 << 17;

        _s2 ^= _s0;
        _s3 ^= _s1;
        _s1 ^= _s2;
        _s0 ^= _s3;
        _s2 ^= t;
        _s3 = BitOperations.RotateLeft(_s3, 45);

        return result;
    }

    public int Next(int exclusiveMax)
    {
        if (exclusiveMax <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(exclusiveMax));
        }

        return (int)Next((ulong)exclusiveMax);
    }

    public int Next(int inclusiveMin, int exclusiveMax)
    {
        if (inclusiveMin >= exclusiveMax)
        {
            throw new ArgumentOutOfRangeException(nameof(exclusiveMax));
        }

        return (int)Next((ulong)((long)exclusiveMax - inclusiveMin)) + inclusiveMin;
    }

    private ulong Next(ulong exclusiveMax)
    {
        Debug.Assert(exclusiveMax != 0);
        var bits = BitOperations.Log2(exclusiveMax - 1) + 1;
        Debug.Assert(bits < 63);
        var mask = (1UL << bits) - 1;

        while (true)
        {
            var result = Next() & mask;
            if (result < exclusiveMax)
            {
                return result;
            }
        }
    }

    /// <summary>
    /// splitmix64
    /// </summary>
    /// <remarks>
    /// <see cref="https://prng.di.unimi.it/splitmix64.c"/>
    /// </remarks>
    private static ulong SplitMix64(ref ulong x)
    {
        var z = x += 0x9e3779b97f4a7c15UL;
        z = (z ^ z >> 30) * 0xbf58476d1ce4e5b9UL;
        z = (z ^ z >> 27) * 0x94d049bb133111ebUL;
        return z ^ z >> 31;
    }
}