using System;

namespace DungeonTrace.Domain
{
    public sealed class HealthState
    {
        public int Maximum { get; }
        public int Current { get; private set; }
        public bool IsDead => Current == 0;

        public HealthState(int maximum)
        {
            if (maximum <= 0) throw new ArgumentOutOfRangeException(nameof(maximum));
            Maximum = maximum;
            Current = maximum;
        }

        public int ApplyDamage(int amount)
        {
            if (amount <= 0 || IsDead) return 0;
            var applied = Math.Min(amount, Current);
            Current -= applied;
            return applied;
        }

        public DamageResult ApplyDamage(DamageContext context)
        {
            var wasAlive = !IsDead;
            var applied = ApplyDamage(context.BaseDamage);
            return new DamageResult(applied, wasAlive && IsDead);
        }

        public int Restore(int amount)
        {
            if (amount <= 0 || IsDead) return 0;
            var restored = Math.Min(amount, Maximum - Current);
            Current += restored;
            return restored;
        }
    }
}
