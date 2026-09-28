using UnityEngine;

namespace DungeonTrace.Enemies
{
    public enum EnemyArchetype { Pursuer, Spitter, Charger, Warder }

    [CreateAssetMenu(menuName = "Dungeon Trace/Enemies/Enemy Definition", fileName = "EnemyDefinition")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private string enemyId = "enemy_prototype";
        [SerializeField] private EnemyArchetype archetype;
        [SerializeField, Min(.1f)] private float speed = 2.5f;
        [SerializeField, Min(.1f)] private float attackRange = 2f;
        [SerializeField, Min(.1f)] private float attackCooldown = 1.5f;
        [SerializeField, Min(.1f)] private float telegraphSeconds = .6f;
        [SerializeField, Min(1)] private int damage = 10;
        public string EnemyId => enemyId;
        public EnemyArchetype Archetype => archetype;
        public float Speed => speed;
        public float AttackRange => attackRange;
        public float AttackCooldown => attackCooldown;
        public float TelegraphSeconds => telegraphSeconds;
        public int Damage => damage;
        public void ConfigurePrototype(string id, EnemyArchetype type, float movementSpeed, float range, float cooldown, float telegraph, int attackDamage)
        { enemyId = id; archetype = type; speed = movementSpeed; attackRange = range; attackCooldown = cooldown; telegraphSeconds = telegraph; damage = attackDamage; }
    }
}
