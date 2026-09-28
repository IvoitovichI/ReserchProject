using System;
using UnityEngine;

namespace DungeonTrace.Domain
{
    public enum DamageType
    {
        Kinetic,
        Energy,
        Explosive
    }

    public readonly struct DamageContext
    {
        public string SourceId { get; }
        public string TargetId { get; }
        public string CorrelationId { get; }
        public int BaseDamage { get; }
        public DamageType Type { get; }
        public float Distance { get; }
        public Vector3 HitPoint { get; }
        public Vector3 Direction { get; }

        public DamageContext(string sourceId, string targetId, string correlationId, int baseDamage, DamageType type, float distance)
            : this(sourceId, targetId, correlationId, baseDamage, type, distance, Vector3.zero, Vector3.zero) { }

        public DamageContext(string sourceId, string targetId, string correlationId, int baseDamage, DamageType type, float distance, Vector3 hitPoint, Vector3 direction)
        {
            SourceId = sourceId ?? string.Empty;
            TargetId = targetId ?? string.Empty;
            CorrelationId = correlationId ?? string.Empty;
            BaseDamage = Math.Max(0, baseDamage);
            Type = type;
            Distance = Math.Max(0f, distance);
            HitPoint = hitPoint;
            Direction = direction.sqrMagnitude > 0f ? direction.normalized : Vector3.zero;
        }
    }

    public readonly struct DamageResult
    {
        public int AppliedDamage { get; }
        public bool TargetDied { get; }

        public DamageResult(int appliedDamage, bool targetDied)
        {
            AppliedDamage = Math.Max(0, appliedDamage);
            TargetDied = targetDied;
        }
    }

    public interface IDamageable
    {
        DamageResult ApplyDamage(DamageContext context);
    }

    public interface IStableDamageable : IDamageable
    {
        string StableDamageableId { get; }
    }
}
