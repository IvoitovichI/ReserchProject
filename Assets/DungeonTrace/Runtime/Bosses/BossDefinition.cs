using UnityEngine;

namespace DungeonTrace.Bosses
{
    public enum BossArchetype { Warden, Hoarder }

    [CreateAssetMenu(menuName = "Dungeon Trace/Bosses/Boss Definition", fileName = "BossDefinition")]
    public sealed class BossDefinition : ScriptableObject
    {
        [SerializeField] private string bossId = "warden-prototype-01";
        [SerializeField] private BossArchetype archetype;
        [SerializeField, Min(1)] private int maximumHealth = 300;
        [SerializeField, Range(.1f, .9f)] private float phaseTwoHealthFraction = .5f;
        [SerializeField, Min(0f)] private float introGraceSeconds = 1.5f;
        [SerializeField, Min(0)] private int summonCap = 2;

        public string BossId => bossId;
        public BossArchetype Archetype => archetype;
        public int MaximumHealth => maximumHealth;
        public float PhaseTwoHealthFraction => phaseTwoHealthFraction;
        public float IntroGraceSeconds => introGraceSeconds;
        public int SummonCap => summonCap;

        public bool IsValid(out string error)
        {
            if (string.IsNullOrWhiteSpace(bossId)) { error = "Boss stable ID is required."; return false; }
            if (maximumHealth < 1 || phaseTwoHealthFraction <= 0f || phaseTwoHealthFraction >= 1f || introGraceSeconds < 0f || summonCap < 0) { error = "Boss configuration is invalid."; return false; }
            error = null; return true;
        }

        public void ConfigurePrototype(string id, BossArchetype value, int health, float phaseTwoAt, float introSeconds, int cap)
        { bossId = id; archetype = value; maximumHealth = health; phaseTwoHealthFraction = phaseTwoAt; introGraceSeconds = introSeconds; summonCap = cap; }
    }

    public sealed class BossState
    {
        private readonly BossDefinition definition;
        private int currentHealth;
        private bool dead;
        public int CurrentHealth => currentHealth;
        public bool IsDead => dead;
        public int Phase => currentHealth <= definition.MaximumHealth * definition.PhaseTwoHealthFraction ? 2 : 1;
        public BossState(BossDefinition value) { definition = value; currentHealth = value.MaximumHealth; }
        public bool ApplyDamage(int amount)
        {
            if (dead || amount <= 0) return false;
            currentHealth = Mathf.Max(0, currentHealth - amount);
            if (currentHealth != 0) return false;
            dead = true;
            return true;
        }
    }
}
