using DungeonTrace.Telemetry;
using NUnit.Framework;

namespace DungeonTrace.Tests.EditMode
{
 public sealed class TelemetryTests
 {
  [Test] public void Counterbalance_IsStable() { Assert.That(Counterbalance.Assign("participant-01", 4), Is.EqualTo(Counterbalance.Assign("participant-01", 4))); }
  [Test] public void Validator_RejectsIncompleteRecords() { Assert.That(TelemetryValidator.IsValid(new TelemetryEvent()), Is.False); Assert.That(TelemetryValidator.IsValid(new TelemetryEvent { eventType="shot", sessionId="s", sequence=1 }), Is.True); }
 }
}
