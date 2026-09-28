using DungeonTrace.Domain;
using UnityEngine;
using PlayerHealth = DungeonTrace.Health.Health;

namespace DungeonTrace.Enemies
{
    public sealed class EnemyProjectile : MonoBehaviour
    {
        private EnemyDefinition definition;
        private Transform target;
        public void Launch(EnemyDefinition value, Transform targetTransform) { definition = value; target = targetTransform; Destroy(gameObject, 6f); }
        private void Update()
        {
            if (target == null || definition == null) return;
            var delta = target.position + Vector3.up - transform.position;
            transform.position += delta.normalized * definition.Speed * 4f * Time.deltaTime;
            if (delta.sqrMagnitude > .5f) return;
            var health = target.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.ApplyDamage(new DamageContext(definition.EnemyId, health.StableDamageableId, $"{definition.EnemyId}-spit-{Time.frameCount}", definition.Damage, DamageType.Energy, delta.magnitude, transform.position, delta));
            }
            else
            {
                Debug.LogWarning($"[EnemyProjectile] {definition.EnemyId} reached a target without Health", this);
            }
            Debug.Log($"[EnemyProjectile] {definition.EnemyId} hit player", this); Destroy(gameObject);
        }
    }
}
