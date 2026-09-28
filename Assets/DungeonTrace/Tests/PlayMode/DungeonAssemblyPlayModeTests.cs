using System.Collections;
using System.Collections.Generic;
using DungeonTrace.Generation;
using DungeonTrace.Rooms;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DungeonTrace.Tests.PlayMode
{
    public sealed class DungeonAssemblyPlayModeTests
    {
        [UnityTest]
        public IEnumerator PlaceholderDungeon_AssemblesWithStableRoomIds()
        {
            var prefab = CreatePlaceholderRoom(); var config = ScriptableObject.CreateInstance<DungeonGenerationConfig>(); config.ConfigureForTests(8, 8, 9, 99, 0, 0, 0);
            var definitions = new List<RoomDefinition>();
            foreach (RoomType type in System.Enum.GetValues(typeof(RoomType))) { var definition = ScriptableObject.CreateInstance<RoomDefinition>(); definition.ConfigureAuthoring($"play-{type}", "prototype-02", type, prefab, new[] { new RoomExitDefinition("north", RoomExitDirection.North), new RoomExitDefinition("south", RoomExitDirection.South) }, new TelemetryZoneDefinition[0]); definitions.Add(definition); }
            var builder = new DungeonGraphBuilder(); Assert.That(builder.TryBuild(config, definitions, 42, 0, out var graph, out var failure), Is.True, failure);
            var host = new GameObject("DungeonAssemblerTest").AddComponent<DungeonAssembler>(); Assert.That(host.TryAssemble(graph, out var result, out failure), Is.True, failure);
            yield return null;
            Assert.That(result.Rooms.Count, Is.EqualTo(graph.Nodes.Count)); foreach (var node in graph.Nodes) Assert.That(result.Rooms[node.InstanceId].StableInstanceId, Is.EqualTo(node.InstanceId));
            Object.Destroy(result.Root); Object.Destroy(host.gameObject); Object.Destroy(prefab); Object.Destroy(config); foreach (var definition in definitions) Object.Destroy(definition);
        }
        private static GameObject CreatePlaceholderRoom()
        {
            var room = new GameObject("PlaceholderRoom"); room.AddComponent<RoomRoot>(); room.AddComponent<RoomBounds>().Configure(new Vector3(0f, 1.5f, 0f), new Vector3(10f, 3f, 10f));
            foreach (var pair in new[] { ("north", new Vector3(0f, 0f, 5f), Vector3.forward), ("south", new Vector3(0f, 0f, -5f), Vector3.back) }) { var socket = new GameObject(pair.Item1); socket.transform.SetParent(room.transform); socket.transform.localPosition = pair.Item2; socket.transform.forward = pair.Item3; socket.AddComponent<DoorSocket>().Configure(pair.Item1, pair.Item1 == "north" ? RoomExitDirection.North : RoomExitDirection.South); }
            return room;
        }
    }
}
