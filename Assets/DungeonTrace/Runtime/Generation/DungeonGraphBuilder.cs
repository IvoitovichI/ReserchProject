using System;
using System.Collections.Generic;
using System.Linq;
using DungeonTrace.Rooms;

namespace DungeonTrace.Generation
{
    public sealed class DungeonGraphBuilder
    {
        public bool TryBuild(DungeonGenerationConfig config, IEnumerable<RoomDefinition> sourceDefinitions, int originalSeed, int attempt, out DungeonGraph graph, out string failure)
        {
            graph = null; failure = null;
            if (config == null || !config.IsValid(out failure)) return false;
            var definitions = sourceDefinitions.Where(value => value != null && value.IsValid(out _)).OrderBy(value => value.RoomId, StringComparer.Ordinal).ToList();
            var required = new[] { RoomType.Start, RoomType.Combat, RoomType.Choice, RoomType.Boss };
            foreach (var type in required) if (!definitions.Any(definition => definition.Type == type)) { failure = $"No valid {type} RoomDefinition exists."; return false; }
            if (!definitions.Any(definition => definition.Type == RoomType.Treasure || (config.RewardMayBeShop && definition.Type == RoomType.Shop))) { failure = "No valid Treasure/Shop RoomDefinition exists."; return false; }
            var actualSeed = DeterministicRandom.Derive(originalSeed, 0x445447, attempt);
            var streams = new DungeonRandomStreams(actualSeed, attempt);
            var graphRng = streams.Graph; var roomRng = streams.Rooms;
            var combatCount = 4 + graphRng.Next(2);
            var rewardType = config.RewardMayBeShop && definitions.Any(value => value.Type == RoomType.Shop) && graphRng.NextBool(50) ? RoomType.Shop : RoomType.Treasure;
            if (!definitions.Any(value => value.Type == rewardType)) rewardType = RoomType.Treasure;
            var mainTypes = new List<RoomType> { RoomType.Start };
            for (var index = 0; index < combatCount; index++) mainTypes.Add(RoomType.Combat);
            mainTypes.Add(RoomType.Choice); mainTypes.Add(rewardType); mainTypes.Add(RoomType.Boss);
            if (mainTypes.Count < config.MinimumMainPathLength || mainTypes.Count > config.MaximumMainPathLength || mainTypes.Count > config.RoomCount) { failure = "Configured main path cannot contain the required room sequence."; return false; }
            graph = new DungeonGraph(originalSeed, actualSeed, attempt);
            for (var index = 0; index < mainTypes.Count; index++)
            {
                var definition = Select(definitions, mainTypes[index], ref roomRng);
                if (definition == null) { failure = $"No valid {mainTypes[index]} RoomDefinition exists."; graph = null; return false; }
                var id = $"room-{index:D2}"; graph.AddNode(new DungeonRoomNode(id, definition, true, false)); if (index > 0) graph.AddConnection($"room-{index - 1:D2}", id);
            }
            var next = mainTypes.Count;
            if (next < config.RoomCount && config.MaximumBranches > 0 && config.Allows(RoomType.Elite) && definitions.Any(value => value.Type == RoomType.Elite) && graphRng.NextBool(config.OptionalEliteChance))
            { AddBranch(graph, Select(definitions, RoomType.Elite, ref roomRng), next++, ref graphRng); }
            var secrets = 0;
            while (next < config.RoomCount && secrets < 2 && config.Allows(RoomType.Secret) && definitions.Any(value => value.Type == RoomType.Secret) && graphRng.NextBool(config.SecretRoomChance))
            { AddBranch(graph, Select(definitions, RoomType.Secret, ref roomRng), next++, ref graphRng); secrets++; }
            return true;
        }
        private static void AddBranch(DungeonGraph graph, RoomDefinition definition, int index, ref DeterministicRandom rng)
        { var candidates = graph.Nodes.Where(node => node.IsMainPath && node.Type == RoomType.Combat).ToList(); var parent = candidates[rng.Next(candidates.Count)]; var id = $"room-{index:D2}"; graph.AddNode(new DungeonRoomNode(id, definition, false, true)); graph.AddConnection(parent.InstanceId, id); }
        private static RoomDefinition Select(List<RoomDefinition> definitions, RoomType type, ref DeterministicRandom rng)
        { var choices = definitions.Where(value => value.Type == type && value.Weight > 0).ToList(); if (choices.Count == 0) return null; var total = choices.Sum(value => value.Weight); var roll = rng.Next(total); foreach (var choice in choices) { roll -= choice.Weight; if (roll < 0) return choice; } return choices[choices.Count - 1]; }
    }
}
