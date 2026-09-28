using System;
using UnityEngine;

namespace DungeonTrace.Rooms
{
    public enum RoomType { Start, Combat, Choice, Treasure, Shop, Elite, Secret, Boss }
    public enum RoomExitDirection { North, East, South, West }

    [Serializable]
    public struct RoomExitDefinition
    {
        [SerializeField] private string stableId;
        [SerializeField] private RoomExitDirection direction;

        public string StableId => stableId;
        public RoomExitDirection Direction => direction;
        public RoomExitDefinition(string stableId, RoomExitDirection direction) { this.stableId = stableId; this.direction = direction; }
    }

    [Serializable]
    public struct EncounterVariant
    {
        [SerializeField] private string stableId;
        [SerializeField, Min(0)] private int weight;
        [SerializeField] private string[] tags;
        public string StableId => stableId;
        public int Weight => weight;
        public string[] Tags => tags;
        public EncounterVariant(string stableId, int weight, string[] tags) { this.stableId = stableId; this.weight = weight; this.tags = tags ?? Array.Empty<string>(); }
    }

    [Serializable]
    public struct TelemetryZoneDefinition
    {
        [SerializeField] private string stableId;
        [SerializeField] private string[] tags;
        public string StableId => stableId;
        public string[] Tags => tags;
        public TelemetryZoneDefinition(string stableId, string[] tags) { this.stableId = stableId; this.tags = tags ?? Array.Empty<string>(); }
    }

    [CreateAssetMenu(menuName = "Dungeon Trace/Rooms/Room Definition", fileName = "RoomDefinition")]
    public sealed class RoomDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string roomId;
        [SerializeField] private string contentVersion = "prototype-01";
        [SerializeField] private RoomType roomType;
        [SerializeField, Min(0)] private int difficultyTier;
        [SerializeField, Min(0)] private int weight = 1;
        [SerializeField] private string[] tags = Array.Empty<string>();
        [SerializeField] private GameObject roomPrefab;
        [SerializeField] private RoomExitDefinition[] doorSockets = Array.Empty<RoomExitDefinition>();
        [SerializeField] private EncounterVariant[] encounterVariants = Array.Empty<EncounterVariant>();
        [SerializeField] private TelemetryZoneDefinition[] telemetryZones = Array.Empty<TelemetryZoneDefinition>();
        [SerializeField] private Vector2 expectedCompletionSeconds = new(30f, 90f);

        // Kept solely for the existing runtime prototype builder; authored rooms use roomPrefab.
        [Header("Legacy prototype geometry")]
        [SerializeField, Min(2f)] private float width = 14f;
        [SerializeField, Min(2f)] private float depth = 14f;
        [SerializeField, Min(2f)] private float wallHeight = 4f;
        [SerializeField] private Vector3 playerSpawn = new(0f, 1.05f, -6f);
        [SerializeField] private RoomExitDefinition[] exits = Array.Empty<RoomExitDefinition>();

        public string StableId => roomId;
        public string RoomId => roomId;
        public string ContentVersion => contentVersion;
        public RoomType Type => roomType;
        public int DifficultyTier => difficultyTier;
        public int Weight => weight;
        public string[] Tags => tags;
        public GameObject RoomPrefab => roomPrefab;
        public RoomExitDefinition[] DoorSockets => doorSockets;
        public EncounterVariant[] EncounterVariants => encounterVariants;
        public TelemetryZoneDefinition[] TelemetryZones => telemetryZones;
        public Vector2 ExpectedCompletionSeconds => expectedCompletionSeconds;
        public float Width => width;
        public float Depth => depth;
        public float WallHeight => wallHeight;
        public Vector3 PlayerSpawn => playerSpawn;
        public RoomExitDefinition[] Exits => exits;

        public bool IsValid(out string error)
        {
            if (string.IsNullOrWhiteSpace(roomId)) { error = "Room stable ID is required."; return false; }
            if (string.IsNullOrWhiteSpace(contentVersion)) { error = "Content version is required."; return false; }
            if (weight < 0 || difficultyTier < 0) { error = "Weight and difficulty tier cannot be negative."; return false; }
            if (expectedCompletionSeconds.x < 0f || expectedCompletionSeconds.y < expectedCompletionSeconds.x) { error = "Expected completion range is invalid."; return false; }
            if (width < 2f || depth < 2f || wallHeight < 2f) { error = "Room dimensions must be at least two metres."; return false; }
            foreach (var exit in exits)
                if (string.IsNullOrWhiteSpace(exit.StableId)) { error = "Every room exit needs a stable ID."; return false; }
            error = null;
            return true;
        }

        public void ConfigureAuthoring(string id, string version, RoomType type, GameObject prefab, RoomExitDefinition[] sockets, TelemetryZoneDefinition[] zones)
        {
            roomId = id;
            contentVersion = version;
            roomType = type;
            roomPrefab = prefab;
            doorSockets = sockets ?? Array.Empty<RoomExitDefinition>();
            telemetryZones = zones ?? Array.Empty<TelemetryZoneDefinition>();
        }

        public static RoomDefinition CreatePrototype()
        {
            var definition = CreateInstance<RoomDefinition>();
            definition.roomId = "room-prototype-01";
            definition.contentVersion = "prototype-01";
            definition.roomType = RoomType.Start;
            definition.exits = new[] { new RoomExitDefinition("exit-prototype-01-north", RoomExitDirection.North) };
            definition.doorSockets = definition.exits;
            return definition;
        }
    }
}
