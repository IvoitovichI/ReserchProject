using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DungeonTrace.Rooms;
using UnityEditor;
using UnityEngine;

namespace DungeonTrace.Editor
{
    /// <summary>
    /// Read-only audit for the authored TenRoomDungeonZone content profile.
    /// It deliberately does not select a canonical content version or alter assets.
    /// </summary>
    public static class TenRoomContentProfilePreflight
    {
        public const string GeneratedDungeonFolder = "Assets/DungeonTrace/GeneratedDungeon";
        public const string BootstrapPath = "Assets/DungeonTrace/Runtime/Bootstrap/GameBootstrap.cs";

        private static readonly TenRoomRoleSpec[] TenRoomRoles =
        {
            new("01_Start_Threshold", RoomType.Start, "none"),
            new("02_Combat_Antechamber", RoomType.Combat, "encounter-room-02-pursuer"),
            new("03_Combat_Crypt", RoomType.Combat, "encounter-room-03-spitter"),
            new("04_Choice_Crossroads", RoomType.Choice, "none"),
            new("05_Combat_Archive", RoomType.Combat, "encounter-room-05-charger"),
            new("06_Elite_Ossuary", RoomType.Elite, "encounter-room-06-warder, encounter-room-06-pursuer"),
            new("07_Treasure_Vault", RoomType.Treasure, "none"),
            new("08_Secret_Alcove", RoomType.Secret, "none"),
            new("09_Combat_Gauntlet", RoomType.Combat, "encounter-room-09-spitter, encounter-room-09-charger"),
            new("10_Boss_Sanctum", RoomType.Boss, "none")
        };

        [MenuItem("Dungeon Trace/Validation/Preflight TenRoom Content Profile")]
        public static void LogTenRoomReport()
        {
            var report = InspectTenRoom();
            if (report.IsValid)
                Debug.Log(report.Format());
            else
                Debug.LogError(report.Format());
        }

        public static TenRoomContentProfileReport InspectTenRoom()
        {
            var definitions = AssetDatabase.FindAssets("t:RoomDefinition", new[] { GeneratedDungeonFolder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath<RoomDefinition>(path))
                .Where(definition => definition != null)
                .GroupBy(definition => definition.Type)
                .ToDictionary(group => group.Key, group => group.OrderBy(definition => definition.StableId, StringComparer.Ordinal).First());

            var mappings = new List<TenRoomRoleMapping>(TenRoomRoles.Length);
            foreach (var role in TenRoomRoles)
            {
                definitions.TryGetValue(role.RoomType, out var definition);
                var prefab = definition == null ? null : definition.RoomPrefab;
                var prefabRoot = prefab == null ? null : prefab.GetComponent<RoomRoot>();
                mappings.Add(new TenRoomRoleMapping(
                    role.SceneRoomName,
                    role.RoomType,
                    AssetDatabase.GetAssetPath(prefab),
                    definition == null ? null : definition.StableId,
                    definition == null ? null : definition.ContentVersion,
                    prefabRoot == null ? null : prefabRoot.ContentVersion,
                    role.EncounterIds));
            }

            return Validate(new TenRoomContentProfileSnapshot(ReadTenRoomSessionContentVersion(), mappings));
        }

        public static TenRoomContentProfileReport Validate(TenRoomContentProfileSnapshot snapshot)
        {
            var issues = new List<string>();
            var rows = new List<TenRoomContentProfileRow>();
            var versions = new HashSet<string>(StringComparer.Ordinal);
            AddVersion(versions, snapshot.SessionContentVersion);

            foreach (var mapping in snapshot.Mappings ?? Array.Empty<TenRoomRoleMapping>())
            {
                rows.Add(new TenRoomContentProfileRow(
                    mapping.SceneRoomName,
                    mapping.RoomType,
                    mapping.PrefabPath,
                    mapping.RoomId,
                    mapping.EncounterIds,
                    mapping.DefinitionContentVersion,
                    mapping.PrefabRootContentVersion));

                if (string.IsNullOrWhiteSpace(mapping.RoomId))
                    issues.Add($"Missing room-role mapping: {mapping.RoomType} has no RoomDefinition with a stable room ID.");
                if (string.IsNullOrWhiteSpace(mapping.PrefabPath))
                    issues.Add($"Missing room-role mapping: {mapping.RoomType} has no prefab assigned by its RoomDefinition.");
                if (string.IsNullOrWhiteSpace(mapping.PrefabRootContentVersion))
                    issues.Add($"Missing prefab root content version: {mapping.SceneRoomName} ({mapping.PrefabPath ?? "no prefab"}).");

                AddVersion(versions, mapping.DefinitionContentVersion);
                AddVersion(versions, mapping.PrefabRootContentVersion);
            }

            foreach (var role in TenRoomRoles)
                if (!rows.Any(row => row.SceneRoomName == role.SceneRoomName))
                    issues.Add($"Missing room-role mapping: authored role '{role.SceneRoomName}' is absent from the profile.");

            if (string.IsNullOrWhiteSpace(snapshot.SessionContentVersion))
                issues.Add($"Could not read TenRoomDungeonZone session content version from {BootstrapPath}.");
            if (versions.Count > 1)
                issues.Add($"Content version mismatch: TenRoom profile contains {string.Join(", ", versions.OrderBy(version => version, StringComparer.Ordinal))}. A research build requires exactly one content version.");

            return new TenRoomContentProfileReport(snapshot.SessionContentVersion, rows, issues);
        }

        private static string ReadTenRoomSessionContentVersion()
        {
            var absolutePath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", BootstrapPath));
            if (!File.Exists(absolutePath)) return null;

            var source = File.ReadAllText(absolutePath);
            var match = Regex.Match(
            source,
                "SceneManager\\.GetActiveScene\\(\\)\\.name\\s*==\\s*\"TenRoomDungeonZone\"[\\s\\S]*?StartSession\\(new\\s+SessionConfig\\([^\\)]*?,\\s*\"(?<version>[^\"]+)\"\\)\\)",
                RegexOptions.CultureInvariant);
            return match.Success ? match.Groups["version"].Value : null;
        }

        private static void AddVersion(ISet<string> versions, string version)
        {
            if (!string.IsNullOrWhiteSpace(version)) versions.Add(version);
        }

        private readonly struct TenRoomRoleSpec
        {
            public TenRoomRoleSpec(string sceneRoomName, RoomType roomType, string encounterIds)
            {
                SceneRoomName = sceneRoomName;
                RoomType = roomType;
                EncounterIds = encounterIds;
            }

            public string SceneRoomName { get; }
            public RoomType RoomType { get; }
            public string EncounterIds { get; }
        }
    }

    public sealed class TenRoomContentProfileSnapshot
    {
        public TenRoomContentProfileSnapshot(string sessionContentVersion, IEnumerable<TenRoomRoleMapping> mappings)
        {
            SessionContentVersion = sessionContentVersion;
            Mappings = mappings == null ? Array.Empty<TenRoomRoleMapping>() : mappings.ToArray();
        }

        public string SessionContentVersion { get; }
        public IReadOnlyList<TenRoomRoleMapping> Mappings { get; }
    }

    public readonly struct TenRoomRoleMapping
    {
        public TenRoomRoleMapping(string sceneRoomName, RoomType roomType, string prefabPath, string roomId, string definitionContentVersion, string prefabRootContentVersion, string encounterIds)
        {
            SceneRoomName = sceneRoomName;
            RoomType = roomType;
            PrefabPath = prefabPath;
            RoomId = roomId;
            DefinitionContentVersion = definitionContentVersion;
            PrefabRootContentVersion = prefabRootContentVersion;
            EncounterIds = encounterIds;
        }

        public string SceneRoomName { get; }
        public RoomType RoomType { get; }
        public string PrefabPath { get; }
        public string RoomId { get; }
        public string DefinitionContentVersion { get; }
        public string PrefabRootContentVersion { get; }
        public string EncounterIds { get; }
    }

    public readonly struct TenRoomContentProfileRow
    {
        public TenRoomContentProfileRow(string sceneRoomName, RoomType roomType, string prefabPath, string roomId, string encounterIds, string definitionContentVersion, string prefabRootContentVersion)
        {
            SceneRoomName = sceneRoomName;
            RoomType = roomType;
            PrefabPath = prefabPath;
            RoomId = roomId;
            EncounterIds = encounterIds;
            DefinitionContentVersion = definitionContentVersion;
            PrefabRootContentVersion = prefabRootContentVersion;
        }

        public string SceneRoomName { get; }
        public RoomType RoomType { get; }
        public string PrefabPath { get; }
        public string RoomId { get; }
        public string EncounterIds { get; }
        public string DefinitionContentVersion { get; }
        public string PrefabRootContentVersion { get; }
    }

    public sealed class TenRoomContentProfileReport
    {
        public TenRoomContentProfileReport(string sessionContentVersion, IEnumerable<TenRoomContentProfileRow> rows, IEnumerable<string> issues)
        {
            SessionContentVersion = sessionContentVersion;
            Rows = rows == null ? Array.Empty<TenRoomContentProfileRow>() : rows.ToArray();
            Issues = issues == null ? Array.Empty<string>() : issues.ToArray();
        }

        public string SessionContentVersion { get; }
        public IReadOnlyList<TenRoomContentProfileRow> Rows { get; }
        public IReadOnlyList<string> Issues { get; }
        public bool IsValid => Issues.Count == 0;

        public string Format()
        {
            var output = new StringBuilder();
            output.AppendLine("[TenRoom Content Profile Preflight]");
            output.AppendLine($"Session content_version: {Display(SessionContentVersion)}");
            output.AppendLine("role | prefab | room_id | encounter_id(s) | definition_version | prefab_root_version");
            foreach (var row in Rows)
                output.AppendLine($"{row.SceneRoomName} ({row.RoomType}) | {Display(row.PrefabPath)} | {Display(row.RoomId)} | {Display(row.EncounterIds)} | {Display(row.DefinitionContentVersion)} | {Display(row.PrefabRootContentVersion)}");

            if (IsValid) output.AppendLine("Result: PASS — one complete content profile.");
            else
            {
                output.AppendLine("Result: FAIL — not valid for a research build.");
                foreach (var issue in Issues) output.AppendLine($"- {issue}");
            }

            return output.ToString().TrimEnd();
        }

        private static string Display(string value) => string.IsNullOrWhiteSpace(value) ? "<missing>" : value;
    }
}
