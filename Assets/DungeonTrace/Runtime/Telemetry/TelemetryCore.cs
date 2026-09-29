using System;
using System.IO;
using UnityEngine;

namespace DungeonTrace.Telemetry
{
    [Serializable] public sealed class TelemetryEvent { public string eventType; public string sessionId; public int sequence; public float time; public string payload; }
    [Serializable] public sealed class SessionManifest { public int schemaVersion = 1; public string sessionId; public string participantPseudonym; public int seed; public string contentVersion; public int condition; }
    public static class Counterbalance
    {
        public static int Assign(string pseudonym, int conditionCount)
        {
            if (string.IsNullOrWhiteSpace(pseudonym) || conditionCount < 1) return 0;
            unchecked { var hash = 17; foreach (var c in pseudonym) hash = hash * 31 + c; return (hash & int.MaxValue) % conditionCount; }
        }
    }
    public sealed class JsonlTelemetrySink : IDisposable
    {
        private readonly StreamWriter writer; private int sequence;
        public JsonlTelemetrySink(string path) { Directory.CreateDirectory(Path.GetDirectoryName(path)); writer = new StreamWriter(path, true); }
        public void Write(string type, string session, string payload) { writer.WriteLine(JsonUtility.ToJson(new TelemetryEvent { eventType = type ?? string.Empty, sessionId = session ?? string.Empty, sequence = ++sequence, time = Time.realtimeSinceStartup, payload = payload ?? string.Empty })); }
        public void Flush() => writer.Flush(); public void Dispose() { writer?.Dispose(); }
    }
    public static class TelemetryValidator
    {
        public static bool IsValid(TelemetryEvent value) => value != null && !string.IsNullOrWhiteSpace(value.eventType) && !string.IsNullOrWhiteSpace(value.sessionId) && value.sequence > 0;
        public static bool IsValid(SessionManifest value) => value != null && value.schemaVersion > 0 && !string.IsNullOrWhiteSpace(value.sessionId) && !string.IsNullOrWhiteSpace(value.participantPseudonym) && !string.IsNullOrWhiteSpace(value.contentVersion);
    }
}
