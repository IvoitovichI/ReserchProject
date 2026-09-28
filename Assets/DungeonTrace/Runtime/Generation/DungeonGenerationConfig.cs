using System;
using System.Collections.Generic;
using DungeonTrace.Rooms;
using UnityEngine;

namespace DungeonTrace.Generation
{
    [CreateAssetMenu(menuName = "Dungeon Trace/Generation/Dungeon Generation Config", fileName = "DungeonGenerationConfig")]
    public sealed class DungeonGenerationConfig : ScriptableObject
    {
        [SerializeField, Min(8)] private int roomCount = 9;
        [SerializeField, Min(7)] private int minimumMainPathLength = 8;
        [SerializeField, Min(8)] private int maximumMainPathLength = 9;
        [SerializeField, Min(0)] private int difficultyBudget = 16;
        [SerializeField, Min(0)] private int minimumDifficultyBudget = 8;
        [SerializeField, Min(0)] private int maximumBranches = 1;
        [SerializeField, Range(0, 100)] private int optionalEliteChance = 35;
        [SerializeField, Range(0, 100)] private int secretRoomChance = 25;
        [SerializeField] private bool rewardMayBeShop = true;
        [SerializeField] private RoomType[] allowedRoomTypes = { RoomType.Start, RoomType.Combat, RoomType.Choice, RoomType.Treasure, RoomType.Shop, RoomType.Elite, RoomType.Secret, RoomType.Boss };
        public int RoomCount => roomCount;
        public int MinimumMainPathLength => minimumMainPathLength;
        public int MaximumMainPathLength => maximumMainPathLength;
        public int DifficultyBudget => difficultyBudget;
        public int MinimumDifficultyBudget => minimumDifficultyBudget;
        public int MaximumBranches => maximumBranches;
        public int OptionalEliteChance => optionalEliteChance;
        public int SecretRoomChance => secretRoomChance;
        public bool RewardMayBeShop => rewardMayBeShop;
        public IReadOnlyList<RoomType> AllowedRoomTypes => allowedRoomTypes;
        public bool Allows(RoomType type) { foreach (var allowed in allowedRoomTypes) if (allowed == type) return true; return false; }
        public bool IsValid(out string error)
        {
            if (roomCount < 8 || minimumMainPathLength < 8 || maximumMainPathLength < minimumMainPathLength) { error = "Room and main-path limits are invalid."; return false; }
            if (!Allows(RoomType.Start) || !Allows(RoomType.Combat) || !Allows(RoomType.Choice) || !Allows(RoomType.Treasure) || !Allows(RoomType.Boss)) { error = "Start, Combat, Choice, Treasure, and Boss types must be allowed."; return false; }
            error = null; return true;
        }
        public void ConfigureForTests(int count, int minPath, int maxPath, int budget, int branches, int eliteChance, int secretChance)
        { roomCount = count; minimumMainPathLength = minPath; maximumMainPathLength = maxPath; difficultyBudget = budget; minimumDifficultyBudget = 0; maximumBranches = branches; optionalEliteChance = eliteChance; secretRoomChance = secretChance; }
    }
}
