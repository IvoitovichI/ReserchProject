using System.Collections.Generic;
using System.Linq;
using DungeonTrace.Rooms;

namespace DungeonTrace.Generation
{
    public sealed class DungeonValidationIssue
    {
        public string Message { get; }
        public DungeonValidationIssue(string message) => Message = message;
    }
    public static class DungeonValidator
    {
        public static IReadOnlyList<DungeonValidationIssue> Validate(DungeonGraph graph, DungeonGenerationConfig config)
        {
            var issues = new List<DungeonValidationIssue>();
            if (graph == null) { issues.Add(new DungeonValidationIssue("Dungeon graph is required.")); return issues; }
            var starts = graph.Nodes.Where(node => node.Type == RoomType.Start).ToList();
            var bosses = graph.Nodes.Where(node => node.Type == RoomType.Boss).ToList();
            if (starts.Count != 1) issues.Add(new DungeonValidationIssue("Exactly one Start room is required."));
            if (bosses.Count != 1) issues.Add(new DungeonValidationIssue("Exactly one Boss room is required."));
            var reachable = starts.Count == 1 ? Reachable(graph, starts[0].InstanceId) : new HashSet<string>();
            if (bosses.Count == 1 && !reachable.Contains(bosses[0].InstanceId)) issues.Add(new DungeonValidationIssue("Boss room is unreachable."));
            foreach (var node in graph.Nodes.Where(node => node.IsMainPath || node.Type == RoomType.Treasure || node.Type == RoomType.Shop))
                if (!reachable.Contains(node.InstanceId)) issues.Add(new DungeonValidationIssue($"Required room '{node.InstanceId}' is isolated."));
            if (!graph.Nodes.Any(node => node.Type == RoomType.Treasure || node.Type == RoomType.Shop)) issues.Add(new DungeonValidationIssue("A reachable Treasure or Shop room is required."));
            foreach (var connection in graph.Connections)
            {
                var from = graph.Find(connection.FromId); var to = graph.Find(connection.ToId);
                if (from == null || to == null) { issues.Add(new DungeonValidationIssue("Connection points to an unknown room.")); continue; }
                if (from.Definition.DoorSockets.Length == 0 || to.Definition.DoorSockets.Length == 0) issues.Add(new DungeonValidationIssue($"Connection '{from.InstanceId}' → '{to.InstanceId}' has no compatible sockets."));
            }
            var mainPath = graph.Nodes.Count(node => node.IsMainPath);
            if (mainPath < config.MinimumMainPathLength || mainPath > config.MaximumMainPathLength) issues.Add(new DungeonValidationIssue("Main path length is outside the configured limits."));
            var difficulty = graph.Nodes.Where(node => node.IsMainPath).Sum(node => node.Definition.DifficultyTier);
            if (difficulty > config.DifficultyBudget || difficulty < config.MinimumDifficultyBudget) issues.Add(new DungeonValidationIssue("Main path difficulty budget is not respected."));
            return issues;
        }
        private static HashSet<string> Reachable(DungeonGraph graph, string start)
        {
            var result = new HashSet<string> { start }; var pending = new Queue<string>(); pending.Enqueue(start);
            while (pending.Count > 0) { var current = pending.Dequeue(); foreach (var link in graph.Connections) { var next = link.FromId == current ? link.ToId : link.ToId == current ? link.FromId : null; if (next != null && result.Add(next)) pending.Enqueue(next); } }
            return result;
        }
    }
}
