using System.Collections.Generic;
using DungeonTrace.Rooms;
using UnityEngine;

namespace DungeonTrace.Generation
{
    public sealed class DungeonRoomInstance : MonoBehaviour
    {
        [SerializeField] private string stableInstanceId;
        public string StableInstanceId => stableInstanceId;
        public void Configure(string value) => stableInstanceId = value;
    }
    public sealed class DungeonDoorway : MonoBehaviour
    {
        [SerializeField] private string stableId;
        [SerializeField] private string firstRoomId;
        [SerializeField] private string secondRoomId;
        public void Configure(string id, string first, string second) { stableId = id; firstRoomId = first; secondRoomId = second; }
    }
    public sealed class DungeonAssemblyResult
    {
        public GameObject Root { get; }
        public IReadOnlyDictionary<string, DungeonRoomInstance> Rooms => rooms;
        private readonly Dictionary<string, DungeonRoomInstance> rooms = new();
        public DungeonAssemblyResult(GameObject root) => Root = root;
        public void Add(string id, DungeonRoomInstance room) => rooms.Add(id, room);
    }
    public sealed class DungeonAssembler : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float doorwayGap = .02f;
        public bool TryAssemble(DungeonGraph graph, out DungeonAssemblyResult result, out string failure)
        {
            result = null; failure = null;
            if (graph == null) { failure = "Dungeon graph is required."; return false; }
            var root = new GameObject($"Dungeon_{graph.ActualSeed}"); var assembled = new DungeonAssemblyResult(root);
            foreach (var node in graph.Nodes)
            {
                if (node.Definition.RoomPrefab == null) { Destroy(root); failure = $"RoomDefinition '{node.Definition.RoomId}' has no prefab."; return false; }
                var room = Instantiate(node.Definition.RoomPrefab, root.transform).GetComponentInChildren<RoomRoot>();
                if (room == null || room.GetComponent<RoomBounds>() == null) { Destroy(root); failure = $"Prefab for '{node.Definition.RoomId}' requires RoomRoot and RoomBounds."; return false; }
                room.Configure(node.Definition, node.Definition.ContentVersion);
                var instance = room.gameObject.AddComponent<DungeonRoomInstance>(); instance.Configure(node.InstanceId); assembled.Add(node.InstanceId, instance);
            }
            var placed = new HashSet<string>();
            if (graph.Nodes.Count > 0) placed.Add(graph.Nodes[0].InstanceId);
            var remaining = new List<DungeonConnection>(graph.Connections);
            while (remaining.Count > 0)
            {
                var progressed = false;
                for (var index = remaining.Count - 1; index >= 0; index--)
                {
                    var link = remaining[index]; var fromPlaced = placed.Contains(link.FromId); var toPlaced = placed.Contains(link.ToId);
                    if (fromPlaced == toPlaced) continue;
                    var anchor = assembled.Rooms[fromPlaced ? link.FromId : link.ToId]; var target = assembled.Rooms[fromPlaced ? link.ToId : link.FromId];
                    if (!TryPlaceNextTo(anchor, target, assembled.Rooms.Values)) { Destroy(root); failure = $"Could not place '{target.StableInstanceId}' without RoomBounds overlap or socket conflict."; return false; }
                    CreateDoorway(root.transform, link, anchor, target); placed.Add(target.StableInstanceId); remaining.RemoveAt(index); progressed = true;
                }
                if (!progressed) { Destroy(root); failure = "Graph has a cycle or disconnected placement dependency."; return false; }
            }
            result = assembled; return true;
        }
        private bool TryPlaceNextTo(DungeonRoomInstance anchor, DungeonRoomInstance target, IEnumerable<DungeonRoomInstance> all)
        {
            var anchorSockets = anchor.GetComponentsInChildren<DoorSocket>(); var targetSockets = target.GetComponentsInChildren<DoorSocket>();
            foreach (var source in anchorSockets) foreach (var destination in targetSockets) for (var turn = 0; turn < 4; turn++)
            {
                target.transform.rotation = Quaternion.Euler(0f, turn * 90f, 0f);
                if (Vector3.Angle(source.transform.forward, -destination.transform.forward) > 1f) continue;
                // destination.localPosition must be transformed by the candidate root rotation before its doorway is aligned.
                target.transform.position = source.transform.position + source.transform.forward * doorwayGap - target.transform.rotation * destination.transform.localPosition;
                if (!Overlaps(target, all)) return true;
            }
            return false;
        }
        private static bool Overlaps(DungeonRoomInstance candidate, IEnumerable<DungeonRoomInstance> all)
        { var bounds = candidate.GetComponent<RoomBounds>().Bounds; foreach (var other in all) if (other != candidate && other.gameObject.activeInHierarchy && bounds.Intersects(other.GetComponent<RoomBounds>().Bounds)) return true; return false; }
        private static void CreateDoorway(Transform parent, DungeonConnection link, DungeonRoomInstance first, DungeonRoomInstance second)
        { var door = new GameObject($"Door_{link.FromId}_{link.ToId}"); door.transform.SetParent(parent, false); door.transform.position = (first.transform.position + second.transform.position) * .5f; door.AddComponent<DungeonDoorway>().Configure($"door-{link.FromId}-{link.ToId}", link.FromId, link.ToId); }
    }
}
