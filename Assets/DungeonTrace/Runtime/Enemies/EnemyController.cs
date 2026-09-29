using DungeonTrace.Domain;
using DungeonTrace.Health;
using UnityEngine;
using UnityEngine.AI;
using PlayerHealth = DungeonTrace.Health.Health;

namespace DungeonTrace.Enemies
{
    public enum EnemyState { SpawnGrace, Chase, Telegraph, Attack, Recover, Recovery, Dead }

    [RequireComponent(typeof(PlayerHealth))]
    public sealed class EnemyController : MonoBehaviour
    {
        private EnemyDefinition definition;
        private Transform target;
        private PlayerHealth health;
        private Renderer[] renderers;
        private NavMeshAgent navMeshAgent;
        private EnemyState state;
        private float stateEndsAt;
        private bool deathHandled;
        private Vector3 lockedChargeDirection;
        private float chargeEndsAt;
        public EnemyState State => state;

        public void Configure(EnemyDefinition value, Transform targetTransform, float graceSeconds = 1f)
        {
            definition = value; target = targetTransform; navMeshAgent = GetComponent<NavMeshAgent>();
            if (state == EnemyState.SpawnGrace) stateEndsAt = Time.time + Mathf.Max(0f, graceSeconds);
        }

        private void Awake()
        {
            health = GetComponent<PlayerHealth>(); renderers = GetComponentsInChildren<Renderer>(); navMeshAgent = GetComponent<NavMeshAgent>(); health.Died += OnDied;
            ChangeState(EnemyState.SpawnGrace, 1f);
        }
        private void OnDestroy() { if (health != null) health.Died -= OnDied; }

        private void Update()
        {
            if (definition == null || target == null || state == EnemyState.Dead) return;
            if (Time.time < stateEndsAt) return;
            var flatTarget = target.position; flatTarget.y = transform.position.y;
            var distance = Vector3.Distance(transform.position, flatTarget);
            switch (state)
            {
                case EnemyState.SpawnGrace: ChangeState(EnemyState.Chase, 0f); break;
                case EnemyState.Chase:
                    if (definition.Archetype == EnemyArchetype.Warder) WarderReposition(flatTarget);
                    else if (definition.Archetype == EnemyArchetype.Spitter) SpitterPosition(flatTarget, distance);
                    else if (distance > definition.AttackRange) MoveTowards(flatTarget);
                    else ChangeState(EnemyState.Telegraph, definition.TelegraphSeconds);
                    break;
                case EnemyState.Telegraph:
                    if (definition.Archetype == EnemyArchetype.Charger) { lockedChargeDirection = (flatTarget - transform.position).normalized; chargeEndsAt = Time.time + .45f; }
                    ChangeState(EnemyState.Attack, 0f); break;
                case EnemyState.Attack: PerformArchetypeAttack(distance); break;
                case EnemyState.Recover: ChangeState(EnemyState.Chase, 0f); break;
                case EnemyState.Recovery: ChangeState(EnemyState.Chase, .35f); break;
            }
        }

        private void SpitterPosition(Vector3 point, float distance)
        {
            if (distance < definition.AttackRange * .8f) MoveTowards(transform.position - (point - transform.position));
            else if (distance > definition.AttackRange * 1.15f) MoveTowards(point);
            else ChangeState(EnemyState.Telegraph, definition.TelegraphSeconds);
        }
        private void WarderReposition(Vector3 point)
        {
            var orbit = Quaternion.Euler(0f, 90f, 0f) * (point - transform.position).normalized;
            MoveTowards(transform.position + orbit);
            if (Time.frameCount % 120 == 0) { transform.localScale = Vector3.one * 1.35f; Debug.Log($"[Enemy] {definition.EnemyId} shield pulse", this); }
        }
        private void PerformArchetypeAttack(float distance)
        {
            if (definition.Archetype == EnemyArchetype.Charger && Time.time < chargeEndsAt) { transform.position += lockedChargeDirection * definition.Speed * 3.5f * Time.deltaTime; return; }
            if (definition.Archetype == EnemyArchetype.Spitter) FireSpit(); else Attack(distance);
            ChangeState(EnemyState.Recover, definition.AttackCooldown);
        }
        private void FireSpit()
        {
            if (!HasLineOfSight())
            {
                Debug.Log($"[Enemy] {definition.EnemyId} spit cancelled: line of sight blocked", this);
                return;
            }
            var prefab = Resources.Load<GameObject>("DungeonTrace/Combat/EnemyProjectile");
            if (prefab == null) { Debug.LogWarning("[Enemy] Missing EnemyProjectile prefab", this); return; }
            var projectile = Instantiate(prefab, transform.position + Vector3.up, Quaternion.identity).GetComponent<EnemyProjectile>();
            projectile.Launch(definition, target);
            Debug.Log($"[Enemy] {definition.EnemyId} fired projectile", this);
        }

        private void MoveTowards(Vector3 point)
        {
            var direction = point - transform.position; direction.y = 0f;
            if (direction.sqrMagnitude < .01f) return;
            var agent = GetComponent<NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                if (agent.SetDestination(point) && agent.pathStatus != NavMeshPathStatus.PathInvalid) return;
                agent.ResetPath();
                agent.isStopped = true;
                Debug.LogWarning($"[Enemy] {definition.EnemyId} NavMesh path failed; using visible recovery movement", this);
                ChangeState(EnemyState.Recovery, .35f);
            }
            transform.position += direction.normalized * definition.Speed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);
        }
        private void Attack(float distance)
        {
            if (distance > definition.AttackRange + .15f) { Debug.Log($"[Enemy] {definition.EnemyId} attack cancelled: target out of range", this); return; }
            if (!HasLineOfSight()) { Debug.Log($"[Enemy] {definition.EnemyId} attack cancelled: line of sight blocked", this); return; }
            var targetHealth = target.GetComponent<PlayerHealth>();
            if (targetHealth == null || targetHealth.IsDead) return;
            targetHealth.ApplyDamage(new DamageContext(definition.EnemyId, targetHealth.StableDamageableId, $"{definition.EnemyId}-attack-{Time.frameCount}", definition.Damage, DamageType.Kinetic, distance, target.position, transform.forward));
            Debug.Log($"[Enemy] {definition.EnemyId} attacked player damage={definition.Damage}", this);
        }
        private bool HasLineOfSight()
        {
            var origin = transform.position + Vector3.up;
            var targetPoint = target.position + Vector3.up;
            var ray = targetPoint - origin;
            if (!Physics.Raycast(origin, ray.normalized, out var hit, ray.magnitude, ~0, QueryTriggerInteraction.Ignore)) return true;
            return hit.transform == target || hit.transform.IsChildOf(target);
        }
        private void ChangeState(EnemyState next, float duration)
        {
            state = next; stateEndsAt = Time.time + duration;
            var color = next == EnemyState.Telegraph ? Color.yellow : next == EnemyState.Attack ? Color.red : Color.cyan;
            foreach (var renderer in renderers) renderer.material.color = color;
            transform.localScale = next == EnemyState.Telegraph ? Vector3.one * 1.2f : Vector3.one;
            Debug.Log($"[Enemy] {definition?.EnemyId ?? name} state={next}", this);
        }
        private void OnDied()
        {
            if (deathHandled) return;
            deathHandled = true; ChangeState(EnemyState.Dead, 0f); gameObject.SetActive(false);
        }
    }
}
