using System;

namespace DungeonTrace.Generation
{
    /// <summary>Small, explicit RNG used only by generation. It deliberately never touches UnityEngine.Random.</summary>
    public struct DeterministicRandom
    {
        private ulong state;

        public DeterministicRandom(int seed) => state = Mix((uint)seed + 0x9E3779B97F4A7C15UL);
        public int Next(int exclusiveMax)
        {
            if (exclusiveMax <= 0) throw new ArgumentOutOfRangeException(nameof(exclusiveMax));
            state ^= state >> 12; state ^= state << 25; state ^= state >> 27;
            return (int)((state * 2685821657736338717UL) % (uint)exclusiveMax);
        }
        public bool NextBool(int percent) => Next(100) < percent;
        public static int Derive(int seed, int stream, int attempt = 0) => unchecked((int)Mix((uint)seed ^ ((ulong)(uint)stream << 32) ^ (uint)attempt));
        private static ulong Mix(ulong value)
        {
            value ^= value >> 30; value *= 0xBF58476D1CE4E5B9UL;
            value ^= value >> 27; value *= 0x94D049BB133111EBUL;
            return value ^ (value >> 31);
        }
    }

    public readonly struct DungeonRandomStreams
    {
        public readonly DeterministicRandom Graph;
        public readonly DeterministicRandom Rooms;
        public readonly DeterministicRandom Encounters;
        public readonly DeterministicRandom Loot;
        public readonly DeterministicRandom Boss;
        public DungeonRandomStreams(int seed, int attempt)
        {
            Graph = new DeterministicRandom(DeterministicRandom.Derive(seed, 1, attempt));
            Rooms = new DeterministicRandom(DeterministicRandom.Derive(seed, 2, attempt));
            Encounters = new DeterministicRandom(DeterministicRandom.Derive(seed, 3, attempt));
            Loot = new DeterministicRandom(DeterministicRandom.Derive(seed, 4, attempt));
            Boss = new DeterministicRandom(DeterministicRandom.Derive(seed, 5, attempt));
        }
    }
}
