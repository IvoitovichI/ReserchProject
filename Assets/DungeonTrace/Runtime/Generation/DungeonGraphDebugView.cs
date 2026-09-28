using System.Collections.Generic;
using DungeonTrace.Rooms;
using UnityEngine;

namespace DungeonTrace.Generation
{
    public sealed class DungeonGraphDebugView : MonoBehaviour
    {
        [SerializeField] private DungeonGenerationConfig config;
        [SerializeField] private RoomDefinition[] definitions;
        [SerializeField] private int seed = 12345;
        [TextArea, SerializeField] private string lastResult;
        [ContextMenu("Build Debug Graph")]
        public void BuildDebugGraph()
        {
            var builder = new DungeonGraphBuilder();
            for (var attempt = 0; attempt < 16; attempt++) if (builder.TryBuild(config, definitions, seed, attempt, out var graph, out var failure))
            { var issues = DungeonValidator.Validate(graph, config); var messages = new List<string>(); foreach (var issue in issues) messages.Add(issue.Message); lastResult = $"original={graph.OriginalSeed}; actual={graph.ActualSeed}; attempt={attempt}\n{graph.Describe()}\nvalidation: {(issues.Count == 0 ? "passed" : string.Join("; ", messages))}"; return; } else lastResult = failure;
        }
    }
}
