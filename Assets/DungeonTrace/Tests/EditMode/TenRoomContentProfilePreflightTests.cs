using DungeonTrace.Editor;
using DungeonTrace.Rooms;
using NUnit.Framework;

namespace DungeonTrace.Tests.EditMode
{
    public sealed class TenRoomContentProfilePreflightTests
    {
        [Test]
        public void CurrentTenRoomProfile_ReportsPrototypeVersionMismatch()
        {
            var report = TenRoomContentProfilePreflight.InspectTenRoom();

            Assert.That(report.Rows, Has.Count.EqualTo(10));
            Assert.That(report.SessionContentVersion, Is.EqualTo("prototype-03"));
            Assert.That(report.IsValid, Is.False);
            Assert.That(report.Issues, Has.Some.Contains("prototype-01, prototype-02, prototype-03"));
        }

        [Test]
        public void MissingRoleMapping_FailsWithActionableError()
        {
            var snapshot = new TenRoomContentProfileSnapshot(
                "prototype-04",
                new[]
                {
                    new TenRoomRoleMapping("01_Start_Threshold", RoomType.Start, null, null, null, null, "none")
                });

            var report = TenRoomContentProfilePreflight.Validate(snapshot);

            Assert.That(report.IsValid, Is.False);
            Assert.That(report.Issues, Has.Some.Contains("Missing room-role mapping: Start"));
            Assert.That(report.Issues, Has.Some.Contains("02_Combat_Antechamber"));
        }
    }
}
