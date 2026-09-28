using System.Collections.Generic;
using System.Linq;
using DungeonTrace.Generation;
using DungeonTrace.Rooms;
using NUnit.Framework;
using UnityEngine;

namespace DungeonTrace.Tests.EditMode
{
    public sealed class DungeonGenerationTests
    {
        [Test]
        public void SameSeedAndContentVersion_ProducesSameGraph()
        {
            using var fixture = new GenerationFixture();
            var builder = new DungeonGraphBuilder();
            Assert.That(builder.TryBuild(fixture.Config, fixture.Definitions, 481516, 0, out var first, out _), Is.True);
            Assert.That(builder.TryBuild(fixture.Config, fixture.Definitions, 481516, 0, out var second, out _), Is.True);
            Assert.That(first.Describe(), Is.EqualTo(second.Describe()));
            Assert.That(first.ActualSeed, Is.EqualTo(second.ActualSeed));
        }

        [Test]
        public void DefinitionOrder_DoesNotChangeResult()
        {
            using var fixture = new GenerationFixture(); var builder = new DungeonGraphBuilder();
            Assert.That(builder.TryBuild(fixture.Config, fixture.Definitions, 77, 0, out var ordered, out _), Is.True);
            Assert.That(builder.TryBuild(fixture.Config, fixture.Definitions.AsEnumerable().Reverse(), 77, 0, out var reversed, out _), Is.True);
            Assert.That(reversed.Describe(), Is.EqualTo(ordered.Describe()));
        }

        [Test]
        public void MandatoryRooms_AreReachableAndSocketsExist()
        {
            using var fixture = new GenerationFixture(); var builder = new DungeonGraphBuilder();
            Assert.That(builder.TryBuild(fixture.Config, fixture.Definitions, 12, 0, out var graph, out _), Is.True);
            Assert.That(DungeonValidator.Validate(graph, fixture.Config), Is.Empty);
            Assert.That(graph.Nodes.Count(node => node.Type == RoomType.Start), Is.EqualTo(1));
            Assert.That(graph.Nodes.Count(node => node.Type == RoomType.Boss), Is.EqualTo(1));
            Assert.That(graph.Connections.All(connection => graph.Find(connection.FromId).Definition.DoorSockets.Length > 0 && graph.Find(connection.ToId).Definition.DoorSockets.Length > 0), Is.True);
        }

        [Test]
        public void DerivedSeed_IsReproducible()
        {
            using var fixture = new GenerationFixture(); var builder = new DungeonGraphBuilder();
            Assert.That(builder.TryBuild(fixture.Config, fixture.Definitions, 9001, 3, out var first, out _), Is.True);
            Assert.That(builder.TryBuild(fixture.Config, fixture.Definitions, 9001, 3, out var second, out _), Is.True);
            Assert.That(first.ActualSeed, Is.EqualTo(second.ActualSeed));
            Assert.That(first.ActualSeed, Is.Not.EqualTo(first.OriginalSeed));
            Assert.That(first.Describe(), Is.EqualTo(second.Describe()));
        }

        [Test]
        public void ThousandSeeds_DoNotCreateSoftlock()
        {
            using var fixture = new GenerationFixture(); var builder = new DungeonGraphBuilder();
            for (var seed = 0; seed < 1000; seed++)
            {
                Assert.That(builder.TryBuild(fixture.Config, fixture.Definitions, seed, 0, out var graph, out var failure), Is.True, failure);
                Assert.That(DungeonValidator.Validate(graph, fixture.Config), Is.Empty, $"seed {seed}: {graph.Describe()}");
            }
        }

        private sealed class GenerationFixture : System.IDisposable
        {
            public DungeonGenerationConfig Config { get; } = ScriptableObject.CreateInstance<DungeonGenerationConfig>();
            public List<RoomDefinition> Definitions { get; } = new();
            public GenerationFixture()
            {
                Config.ConfigureForTests(9, 8, 9, 99, 0, 0, 0);
                foreach (RoomType type in System.Enum.GetValues(typeof(RoomType)))
                {
                    var definition = ScriptableObject.CreateInstance<RoomDefinition>();
                    definition.ConfigureAuthoring($"room-{type.ToString().ToLowerInvariant()}", "prototype-02", type, null, new[] { new RoomExitDefinition("north", RoomExitDirection.North), new RoomExitDefinition("south", RoomExitDirection.South) }, new TelemetryZoneDefinition[0]);
                    Definitions.Add(definition);
                }
            }
            public void Dispose() { Object.DestroyImmediate(Config); foreach (var definition in Definitions) Object.DestroyImmediate(definition); }
        }
    }
}
