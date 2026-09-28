using UnityEngine;
using PlayerHealth = DungeonTrace.Health.Health;

namespace DungeonTrace.Combat
{
    [RequireComponent(typeof(PlayerHealth))]
    public sealed class CombatDebugDummy : MonoBehaviour
    {
        [SerializeField] private float bobHeight = .12f;
        [SerializeField] private float bobFrequency = 1.5f;
        private PlayerHealth health;
        private Vector3 startPosition;

        private void Awake()
        {
            health = GetComponent<PlayerHealth>();
            startPosition = transform.localPosition;
            health.Damaged += OnDamaged;
            health.Died += OnDied;
        }

        private void Update()
        {
            if (health.IsDead) return;
            transform.localPosition = startPosition + Vector3.up * (Mathf.Sin(Time.time * bobFrequency) * bobHeight);
            transform.Rotate(0f, 35f * Time.deltaTime, 0f, Space.Self);
        }

        private void OnDestroy()
        {
            if (health != null) { health.Damaged -= OnDamaged; health.Died -= OnDied; }
        }

        private void OnDamaged(int amount) => Debug.Log($"[CombatDummy] {health.StableDamageableId} took {amount}; hp={health.Current}", this);
        private void OnDied()
        {
            Debug.Log($"[CombatDummy] {health.StableDamageableId} destroyed", this);
            gameObject.SetActive(false);
        }
    }
}
