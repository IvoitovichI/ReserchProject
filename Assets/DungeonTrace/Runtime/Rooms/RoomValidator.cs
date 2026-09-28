using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DungeonTrace.Rooms
{
    public enum RoomValidationSeverity { Error, Warning }

    public readonly struct RoomValidationIssue
    {
        public RoomValidationSeverity Severity { get; }
        public string Message { get; }
        public UnityEngine.Object Context { get; }
        public RoomValidationIssue(RoomValidationSeverity severity, string message, UnityEngine.Object context) { Severity = severity; Message = message; Context = context; }
    }

    public static class RoomValidator
    {
        private const float PlayerRadius = .35f;
        private const float PlayerHeight = 1.8f;
        private const float MinimumEnemyDistance = 3f;

        public static List<RoomValidationIssue> Validate(RoomRoot root, IEnumerable<RoomDefinition> knownDefinitions = null)
        {
            var issues = new List<RoomValidationIssue>();
            if (root == null) { issues.Add(new(RoomValidationSeverity.Error, "RoomRoot is required.", null)); return issues; }
            var definition = root.Definition;
            if (definition == null) { issues.Add(new(RoomValidationSeverity.Error, "RoomRoot has no RoomDefinition.", root)); return issues; }
            if (!definition.IsValid(out var definitionError)) issues.Add(new(RoomValidationSeverity.Error, definitionError, definition));
            if (root.ContentVersion != definition.ContentVersion) issues.Add(new(RoomValidationSeverity.Error, "Prefab content version does not match its RoomDefinition.", root));
            ValidateUniqueRoomId(definition, knownDefinitions, issues);
            var bounds = root.GetComponentInChildren<RoomBounds>();
            if (bounds == null) issues.Add(new(RoomValidationSeverity.Error, "RoomBounds is required.", root));
            else ValidateBounds(root, bounds, issues);
            var entries = root.GetComponentsInChildren<PlayerEntry>(true);
            if (entries.Length != 1) issues.Add(new(RoomValidationSeverity.Error, "Exactly one PlayerEntry is required.", root));
            foreach (var entry in entries) ValidatePlayerEntry(root, entry, issues);
            var sockets = root.GetComponentsInChildren<DoorSocket>(true);
            ValidateSockets(root, definition, bounds, sockets, issues);
            ValidateEnemySpawns(root, entries, issues);
            ValidateRewards(root, bounds, issues);
            ValidateTelemetryZones(root, definition, issues);
            return issues;
        }

        private static void ValidateUniqueRoomId(RoomDefinition definition, IEnumerable<RoomDefinition> definitions, List<RoomValidationIssue> issues)
        {
            if (definitions == null) return;
            foreach (var other in definitions)
                if (other != null && other != definition && other.RoomId == definition.RoomId)
                { issues.Add(new(RoomValidationSeverity.Error, $"room_id '{definition.RoomId}' is not unique.", definition)); return; }
        }

        private static void ValidateSockets(RoomRoot root, RoomDefinition definition, RoomBounds bounds, DoorSocket[] sockets, List<RoomValidationIssue> issues)
        {
            var ids = new HashSet<string>();
            var definitionIds = new HashSet<string>();
            foreach (var expected in definition.DoorSockets) definitionIds.Add(expected.StableId);
            foreach (var socket in sockets)
            {
                if (string.IsNullOrWhiteSpace(socket.StableId) || !ids.Add(socket.StableId)) issues.Add(new(RoomValidationSeverity.Error, "DoorSocket stable ID is missing or duplicated.", socket));
                if (!definitionIds.Contains(socket.StableId)) issues.Add(new(RoomValidationSeverity.Error, "DoorSocket is absent from RoomDefinition.", socket));
                var expectedForward = DirectionVector(socket.Direction);
                if (Vector3.Angle(socket.transform.forward, expectedForward) > 5f) issues.Add(new(RoomValidationSeverity.Error, "DoorSocket forward must face its declared cardinal direction.", socket));
                if (bounds != null && !IsOnExpectedBoundsEdge(bounds.Bounds, socket.transform.position, socket.Direction)) issues.Add(new(RoomValidationSeverity.Error, "DoorSocket must be positioned on the matching RoomBounds edge.", socket));
                var center = socket.transform.position + socket.transform.forward * .5f + Vector3.up * (socket.OpeningHeight * .5f);
                var halfExtents = new Vector3(socket.OpeningWidth * .45f, socket.OpeningHeight * .45f, .2f);
                if (Physics.CheckBox(center, halfExtents, socket.transform.rotation, ~0, QueryTriggerInteraction.Ignore)) issues.Add(new(RoomValidationSeverity.Warning, "Potential obstacle in front of DoorSocket.", socket));
            }
            foreach (var expected in definition.DoorSockets)
                if (!ids.Contains(expected.StableId)) issues.Add(new(RoomValidationSeverity.Error, $"Definition DoorSocket '{expected.StableId}' has no prefab marker.", definition));
        }

        private static void ValidatePlayerEntry(RoomRoot root, PlayerEntry entry, List<RoomValidationIssue> issues)
        {
            if (Mathf.Abs(entry.CameraHeight - 1.65f) > .01f) issues.Add(new(RoomValidationSeverity.Error, "PlayerEntry camera height must be 1.65 m.", entry));
            var bottom = entry.transform.position + Vector3.up * PlayerRadius;
            var top = bottom + Vector3.up * (PlayerHeight - PlayerRadius * 2f);
            if (Physics.CheckCapsule(bottom, top, PlayerRadius, ~0, QueryTriggerInteraction.Ignore)) issues.Add(new(RoomValidationSeverity.Warning, "Player capsule space is obstructed at PlayerEntry.", entry));
            if (Physics.SphereCast(entry.transform.position + Vector3.up, PlayerRadius, entry.transform.forward, out _, 1f, ~0, QueryTriggerInteraction.Ignore)) issues.Add(new(RoomValidationSeverity.Warning, "PlayerEntry faces an obstacle within one metre.", entry));
            var camera = entry.transform.position + Vector3.up * entry.CameraHeight;
            if (Physics.Raycast(camera, Vector3.up, out var ceilingHit, .35f, ~0, QueryTriggerInteraction.Ignore)) issues.Add(new(RoomValidationSeverity.Error, $"Ceiling is too close to the 1.65 m camera ({ceilingHit.distance:F2} m clearance).", entry));
        }

        private static void ValidateEnemySpawns(RoomRoot root, PlayerEntry[] entries, List<RoomValidationIssue> issues)
        {
            foreach (var spawn in root.GetComponentsInChildren<EnemySpawnPoint>(true))
            {
                if (!NavMesh.SamplePosition(spawn.transform.position, out _, .15f, NavMesh.AllAreas)) issues.Add(new(RoomValidationSeverity.Error, "EnemySpawnPoint is not on NavMesh.", spawn));
                foreach (var entry in entries)
                    if (Vector3.Distance(spawn.transform.position, entry.transform.position) < MinimumEnemyDistance) issues.Add(new(RoomValidationSeverity.Error, "EnemySpawnPoint is too close to PlayerEntry (minimum 3 m).", spawn));
            }
        }

        private static void ValidateRewards(RoomRoot root, RoomBounds bounds, List<RoomValidationIssue> issues)
        {
            foreach (var reward in root.GetComponentsInChildren<RewardSpawnPoint>(true))
            {
                if (bounds != null && !bounds.Bounds.Contains(reward.transform.position)) issues.Add(new(RoomValidationSeverity.Error, "RewardSpawnPoint is outside RoomBounds.", reward));
                foreach (var socket in root.GetComponentsInChildren<DoorSocket>(true))
                    if (Vector3.Distance(reward.transform.position, socket.transform.position) < 1.25f) issues.Add(new(RoomValidationSeverity.Warning, "RewardSpawnPoint may block a doorway passage.", reward));
            }
        }

        private static void ValidateTelemetryZones(RoomRoot root, RoomDefinition definition, List<RoomValidationIssue> issues)
        {
            var ids = new HashSet<string>();
            foreach (var zone in root.GetComponentsInChildren<TelemetryZone>(true))
                if (string.IsNullOrWhiteSpace(zone.StableId) || !ids.Add(zone.StableId)) issues.Add(new(RoomValidationSeverity.Error, "TelemetryZone stable ID is missing or duplicated.", zone));
            foreach (var zone in definition.TelemetryZones)
                if (!ids.Contains(zone.StableId)) issues.Add(new(RoomValidationSeverity.Warning, $"Definition telemetry zone '{zone.StableId}' has no prefab marker.", definition));
        }

        private static void ValidateBounds(RoomRoot root, RoomBounds roomBounds, List<RoomValidationIssue> issues)
        {
            var foundGeometry = false;
            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
            {
                if (collider.isTrigger || collider.GetComponent<TelemetryZone>() != null) continue;
                foundGeometry = true;
                if (!roomBounds.Bounds.Contains(collider.bounds.min) || !roomBounds.Bounds.Contains(collider.bounds.max)) issues.Add(new(RoomValidationSeverity.Error, "RoomBounds does not contain room geometry.", collider));
            }
            if (!foundGeometry) issues.Add(new(RoomValidationSeverity.Warning, "No non-trigger geometry collider found to compare with RoomBounds.", roomBounds));
        }

        private static Vector3 DirectionVector(RoomExitDirection direction) => direction switch { RoomExitDirection.North => Vector3.forward, RoomExitDirection.East => Vector3.right, RoomExitDirection.South => Vector3.back, _ => Vector3.left };
        private static bool IsOnExpectedBoundsEdge(Bounds bounds, Vector3 position, RoomExitDirection direction)
        {
            const float tolerance = .3f;
            return direction switch
            {
                RoomExitDirection.North => Mathf.Abs(position.z - bounds.max.z) <= tolerance,
                RoomExitDirection.East => Mathf.Abs(position.x - bounds.max.x) <= tolerance,
                RoomExitDirection.South => Mathf.Abs(position.z - bounds.min.z) <= tolerance,
                _ => Mathf.Abs(position.x - bounds.min.x) <= tolerance
            };
        }
    }
}
