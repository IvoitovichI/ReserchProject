using System;
using System.Collections.Generic;
using DungeonTrace.Rooms;

namespace DungeonTrace.Generation
{
    public sealed class DungeonRoomNode
    {
        public string InstanceId { get; }
        public RoomDefinition Definition { get; }
        public RoomType Type => Definition.Type;
        public bool IsMainPath { get; }
        public bool IsOptional { get; }
        public DungeonRoomNode(string id, RoomDefinition definition, bool mainPath, bool optional) { InstanceId = id; Definition = definition; IsMainPath = mainPath; IsOptional = optional; }
    }
    public readonly struct DungeonConnection
    {
        public readonly string FromId; public readonly string ToId;
        public DungeonConnection(string from, string to) { FromId = from; ToId = to; }
    }
    public sealed class DungeonGraph
    {
        public int OriginalSeed { get; }
        public int ActualSeed { get; }
        public int Attempt { get; }
        public IReadOnlyList<DungeonRoomNode> Nodes => nodes;
        public IReadOnlyList<DungeonConnection> Connections => connections;
        private readonly List<DungeonRoomNode> nodes = new();
        private readonly List<DungeonConnection> connections = new();
        public DungeonGraph(int originalSeed, int actualSeed, int attempt) { OriginalSeed = originalSeed; ActualSeed = actualSeed; Attempt = attempt; }
        public void AddNode(DungeonRoomNode node) => nodes.Add(node);
        public void AddConnection(string from, string to) => connections.Add(new DungeonConnection(from, to));
        public DungeonRoomNode Find(string id) => nodes.Find(node => node.InstanceId == id);
        public string Describe()
        {
            var values = new List<string>();
            foreach (var node in nodes) values.Add($"{node.InstanceId}:{node.Definition.RoomId}:{node.Type}:{(node.IsMainPath ? "main" : "optional")}");
            return string.Join(" | ", values);
        }
    }
}
