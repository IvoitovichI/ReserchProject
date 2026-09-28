using System;
using DungeonTrace.Domain;
using UnityEngine;

namespace DungeonTrace.Health
{
    public sealed class Health : MonoBehaviour, IStableDamageable
    {
        [SerializeField, Min(1)] private int maximum = 100;
        [SerializeField] private string stableDamageableId = "player-prototype-01";
        private HealthState state;
        public int Current => state?.Current ?? maximum;
        public bool IsDead => state != null && state.IsDead;
        public string StableDamageableId => stableDamageableId;
        public event Action<int> Damaged;
        public event Action Died;

        private void Awake() => state = new HealthState(maximum);
        public void ConfigureStableId(string value) => stableDamageableId = value ?? string.Empty;
        public int TakeDamage(int amount)
        {
            return ApplyDamage(new DamageContext(string.Empty, string.Empty, string.Empty, amount, DamageType.Kinetic, 0f)).AppliedDamage;
        }

        public DamageResult ApplyDamage(DamageContext context)
        {
            var result = state.ApplyDamage(context);
            if (result.AppliedDamage == 0) return result;
            Damaged?.Invoke(result.AppliedDamage);
            if (result.TargetDied) Died?.Invoke();
            return result;
        }
    }
}
