using System.Collections.Generic;
using DungeonTrace.Rooms;
using NUnit.Framework;
using UnityEngine;

namespace DungeonTrace.Tests.EditMode
{
    public sealed class RoomValidatorTests
    {
        [Test]
        public void ValidRoom_HasNoErrors()
        {
            var fixture = CreateValidRoom("room-validator-valid");
            try
            {
                var issues = RoomValidator.Validate(fixture.Root, new[] { fixture.Definition });
                Assert.That(issues, Has.None.Matches<RoomValidationIssue>(issue => issue.Severity == RoomValidationSeverity.Error));
            }
            finally { DestroyFixture(fixture); }
        }

        [Test]
        public void VersionMismatch_IsReported()
        {
            var fixture = CreateValidRoom("room-validator-version");
            try
            {
                fixture.Root.Configure(fixture.Definition, "wrong-version");
                var issues = RoomValidator.Validate(fixture.Root, new[] { fixture.Definition });
                Assert.That(Contains(issues, "Prefab content version"), Is.True);
            }
            finally { DestroyFixture(fixture); }
        }

        [Test]
        public void DuplicateRoomId_IsReported()
        {
            var fixture = CreateValidRoom("room-validator-duplicate");
            var duplicate = ScriptableObject.CreateInstance<RoomDefinition>();
            duplicate.ConfigureAuthoring("room-validator-duplicate", "prototype-01", RoomType.Start, null, new RoomExitDefinition[0], new TelemetryZoneDefinition[0]);
            try
            {
                var issues = RoomValidator.Validate(fixture.Root, new[] { fixture.Definition, duplicate });
                Assert.That(Contains(issues, "is not unique"), Is.True);
            }
            finally { Object.DestroyImmediate(duplicate); DestroyFixture(fixture); }
        }

        [Test]
        public void MisalignedDoorSocket_IsReported()
        {
            var fixture = CreateValidRoom("room-validator-door");
            try
            {
                fixture.Socket.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                var issues = RoomValidator.Validate(fixture.Root, new[] { fixture.Definition });
                Assert.That(Contains(issues, "forward must face"), Is.True);
            }
            finally { DestroyFixture(fixture); }
        }

        private static bool Contains(IEnumerable<RoomValidationIssue> issues, string phrase)
        {
            foreach (var issue in issues) if (issue.Message.Contains(phrase)) return true;
            return false;
        }

        private static (GameObject Object, RoomRoot Root, DoorSocket Socket, RoomDefinition Definition) CreateValidRoom(string id)
        {
            var definition = ScriptableObject.CreateInstance<RoomDefinition>();
            definition.ConfigureAuthoring(id, "prototype-01", RoomType.Start, null, new[] { new RoomExitDefinition("door-north", RoomExitDirection.North) }, new TelemetryZoneDefinition[0]);
            var roomObject = new GameObject("RoomTest");
            var root = roomObject.AddComponent<RoomRoot>();
            root.Configure(definition, "prototype-01");
            roomObject.AddComponent<RoomBounds>().Configure(new Vector3(0f, 1.25f, 0f), new Vector3(12f, 3.5f, 12f));
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.transform.SetParent(roomObject.transform, false);
            floor.transform.localPosition = new Vector3(0f, -.25f, 0f);
            floor.transform.localScale = new Vector3(12f, .5f, 12f);
            var entry = new GameObject("Entry"); entry.transform.SetParent(roomObject.transform, false); entry.transform.localPosition = new Vector3(0f, 0f, -4f); entry.AddComponent<PlayerEntry>();
            var socketObject = new GameObject("Socket"); socketObject.transform.SetParent(roomObject.transform, false); socketObject.transform.localPosition = new Vector3(0f, 0f, 6f); socketObject.transform.forward = Vector3.forward;
            var socket = socketObject.AddComponent<DoorSocket>(); socket.Configure("door-north", RoomExitDirection.North);
            return (roomObject, root, socket, definition);
        }

        private static void DestroyFixture((GameObject Object, RoomRoot Root, DoorSocket Socket, RoomDefinition Definition) fixture)
        {
            Object.DestroyImmediate(fixture.Object);
            Object.DestroyImmediate(fixture.Definition);
        }
    }
}
