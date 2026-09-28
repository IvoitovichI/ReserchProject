using System.Collections.Generic;
using DungeonTrace.Domain;
using UnityEngine;

namespace DungeonTrace.Combat
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public sealed class OrbProjectile : MonoBehaviour
    {
        private readonly Collider[] overlapBuffer = new Collider[32];
        private WeaponDefinition definition;
        private string shotId;
        private LayerMask hitMask;
        private WeaponController owner;
        private bool resolved;

        public void Launch(WeaponDefinition weapon, string correlationId, LayerMask layers, WeaponController controller)
        {
            definition = weapon; shotId = correlationId; hitMask = layers; owner = controller;
            GetComponent<Rigidbody>().linearVelocity = transform.forward * definition.ProjectileSpeed;
            Destroy(gameObject, definition.Range / definition.ProjectileSpeed + 1f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (resolved) return;
            resolved = true;
            var hitCount = 0; var damage = 0; var uniqueTargets = new HashSet<IDamageable>();
            var count = Physics.OverlapSphereNonAlloc(transform.position, definition.ExplosionRadius, overlapBuffer, hitMask, QueryTriggerInteraction.Ignore);
            for (var index = 0; index < count; index++)
            {
                var damageable = overlapBuffer[index].GetComponentInParent(typeof(IDamageable)) as IDamageable;
                if (damageable == null || !uniqueTargets.Add(damageable)) continue;
                var targetId = damageable is IStableDamageable stable ? stable.StableDamageableId : string.Empty;
                var result = damageable.ApplyDamage(new DamageContext(definition.WeaponId, targetId, shotId, definition.Damage, DamageType.Explosive, Vector3.Distance(transform.position, overlapBuffer[index].ClosestPoint(transform.position)), transform.position, transform.forward));
                hitCount++; damage += result.AppliedDamage;
            }
            owner.ResolveProjectile(shotId, hitCount, damage);
            Destroy(gameObject);
        }
    }
}
