using DungeonTrace.Domain;
using DungeonTrace.Combat;
using NUnit.Framework;

namespace DungeonTrace.Tests.EditMode
{
    public sealed class DamageContractTests
    {
        [Test]
        public void DamageContext_ClampsInvalidPayloadValues()
        {
            var context = new DamageContext(null, null, null, -5, DamageType.Energy, -2f);

            Assert.That(context.SourceId, Is.Empty);
            Assert.That(context.TargetId, Is.Empty);
            Assert.That(context.CorrelationId, Is.Empty);
            Assert.That(context.BaseDamage, Is.EqualTo(0));
            Assert.That(context.Distance, Is.EqualTo(0f));
        }

        [Test]
        public void PrototypePulsePistol_HasAValidStableDefinition()
        {
            var definition = WeaponDefinition.CreatePrototypePulsePistol();
            Assert.That(definition.IsValid(out var reason), Is.True, reason);
            Assert.That(definition.WeaponId, Is.EqualTo("pulse-pistol-prototype-01"));
            Assert.That(definition.FireStrategy, Is.EqualTo(WeaponFireStrategy.Hitscan));
            UnityEngine.Object.DestroyImmediate(definition);
        }

        [Test]
        public void PrototypeWeaponDefinitions_DeclareTheirDistinctStrategies()
        {
            var scatter = WeaponDefinition.CreatePrototypeScatterBlaster();
            var arc = WeaponDefinition.CreatePrototypeArcRifle();
            var projectilePrefab = new UnityEngine.GameObject("OrbProjectileTest");
            projectilePrefab.AddComponent<UnityEngine.Rigidbody>();
            projectilePrefab.AddComponent<UnityEngine.SphereCollider>();
            projectilePrefab.AddComponent<OrbProjectile>();
            var orb = WeaponDefinition.CreatePrototypeOrbLauncher(projectilePrefab);

            Assert.That(scatter.IsValid(out var scatterReason), Is.True, scatterReason);
            Assert.That(arc.IsValid(out var arcReason), Is.True, arcReason);
            Assert.That(orb.IsValid(out var orbReason), Is.True, orbReason);
            Assert.That(scatter.FireStrategy, Is.EqualTo(WeaponFireStrategy.Scatter));
            Assert.That(scatter.PelletCount, Is.EqualTo(6));
            Assert.That(arc.FireStrategy, Is.EqualTo(WeaponFireStrategy.Charge));
            Assert.That(orb.FireStrategy, Is.EqualTo(WeaponFireStrategy.Projectile));

            UnityEngine.Object.DestroyImmediate(scatter);
            UnityEngine.Object.DestroyImmediate(arc);
            UnityEngine.Object.DestroyImmediate(orb);
            UnityEngine.Object.DestroyImmediate(projectilePrefab);
        }

        [Test]
        public void WeaponShot_UsesOneCorrelationIdAcrossAllPellets()
        {
            var shot = new WeaponShot("scatter-000001", "scatter-blaster-prototype-01", new UnityEngine.Ray(UnityEngine.Vector3.zero, UnityEngine.Vector3.forward), 6);
            var resolution = new WeaponShotResolution(shot.ShotId, 4, 32);

            Assert.That(shot.PelletCount, Is.EqualTo(6));
            Assert.That(resolution.ShotId, Is.EqualTo(shot.ShotId));
            Assert.That(resolution.HitCount, Is.EqualTo(4));
        }

        [Test]
        public void HealthState_DamageResultReportsDeathOnlyOnce()
        {
            var health = new HealthState(10);
            var first = health.ApplyDamage(new DamageContext("pistol", "target", "shot-1", 10, DamageType.Energy, 3f));
            var second = health.ApplyDamage(new DamageContext("pistol", "target", "shot-2", 10, DamageType.Energy, 3f));

            Assert.That(first.AppliedDamage, Is.EqualTo(10));
            Assert.That(first.TargetDied, Is.True);
            Assert.That(second.AppliedDamage, Is.Zero);
            Assert.That(second.TargetDied, Is.False);
        }
    }
}
