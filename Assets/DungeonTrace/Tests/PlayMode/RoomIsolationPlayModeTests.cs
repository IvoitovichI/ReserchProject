using System.Collections;
using DungeonTrace.Rooms;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DungeonTrace.Tests.PlayMode
{
    public sealed class RoomIsolationPlayModeTests
    {
        [UnityTest]
        public IEnumerator IsolatedRoom_ValidatesItsRequiredMarkers()
        {
            var definition = ScriptableObject.CreateInstance<RoomDefinition>();
            definition.ConfigureAuthoring("room-playmode-isolated", "prototype-01", RoomType.Start, null, new[] { new RoomExitDefinition("door-north", RoomExitDirection.North) }, new TelemetryZoneDefinition[0]);
            var room = new GameObject("IsolatedRoom");
            var root = room.AddComponent<RoomRoot>(); root.Configure(definition, "prototype-01");
            room.AddComponent<RoomBounds>().Configure(new Vector3(0f, 1.25f, 0f), new Vector3(12f, 3.5f, 12f));
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube); floor.transform.SetParent(room.transform, false); floor.transform.localPosition = new Vector3(0f, -.25f, 0f); floor.transform.localScale = new Vector3(12f, .5f, 12f);
            var entry = new GameObject("PlayerEntry"); entry.transform.SetParent(room.transform, false); entry.transform.localPosition = new Vector3(0f, 0f, -4f); entry.AddComponent<PlayerEntry>();
            var socket = new GameObject("DoorSocket"); socket.transform.SetParent(room.transform, false); socket.transform.localPosition = new Vector3(0f, 0f, 6f); socket.transform.forward = Vector3.forward; socket.AddComponent<DoorSocket>().Configure("door-north", RoomExitDirection.North);
            yield return null;
            var issues = RoomValidator.Validate(root, new[] { definition });
            Assert.That(issues, Has.None.Matches<RoomValidationIssue>(issue => issue.Severity == RoomValidationSeverity.Error));
            Object.Destroy(room);
            Object.Destroy(definition);
        }
    }
}
