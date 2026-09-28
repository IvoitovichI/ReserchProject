using UnityEngine;

namespace DungeonTrace.Combat
{
    public enum WeaponFireStrategy { Hitscan, Scatter, Charge, Projectile }

    [CreateAssetMenu(menuName = "Dungeon Trace/Combat/Weapon Definition", fileName = "WeaponDefinition")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        [SerializeField] private string weaponId = "pulse-pistol";
        [SerializeField] private WeaponFireStrategy fireStrategy = WeaponFireStrategy.Hitscan;
        [SerializeField, Min(1)] private int damage = 20;
        [SerializeField, Min(.01f)] private float fireInterval = .25f;
        [SerializeField, Min(.1f)] private float range = 35f;
        [SerializeField, Min(0f)] private float spreadDegrees;
        [SerializeField, Min(1)] private int pelletCount = 1;
        [SerializeField, Min(0f)] private float chargeSeconds;
        [SerializeField, Min(0f)] private float projectileSpeed;
        [SerializeField, Min(0f)] private float explosionRadius;
        [SerializeField, Range(.1f, 1f)] private float chargingMovementMultiplier = 1f;
        [SerializeField] private GameObject gameplayProjectilePrefab;

        public string WeaponId => weaponId;
        public WeaponFireStrategy FireStrategy => fireStrategy;
        public int Damage => damage;
        public float FireInterval => fireInterval;
        public float Range => range;
        public float SpreadDegrees => spreadDegrees;
        public int PelletCount => pelletCount;
        public float ChargeSeconds => chargeSeconds;
        public float ProjectileSpeed => projectileSpeed;
        public float ExplosionRadius => explosionRadius;
        public float ChargingMovementMultiplier => chargingMovementMultiplier;
        public GameObject GameplayProjectilePrefab => gameplayProjectilePrefab;

        public bool IsValid(out string reason)
        {
            if (string.IsNullOrWhiteSpace(weaponId)) { reason = "Weapon ID is required."; return false; }
            if (damage < 1) { reason = "Damage must be at least one."; return false; }
            if (fireInterval < .01f) { reason = "Fire interval must be positive."; return false; }
            if (range < .1f) { reason = "Range must be positive."; return false; }
            if (fireStrategy == WeaponFireStrategy.Scatter && pelletCount < 2) { reason = "Scatter weapons need at least two pellets."; return false; }
            if (fireStrategy == WeaponFireStrategy.Charge && chargeSeconds <= 0f) { reason = "Charge weapons need a charge duration."; return false; }
            if (fireStrategy == WeaponFireStrategy.Projectile && (projectileSpeed <= 0f || gameplayProjectilePrefab == null)) { reason = "Projectile weapons need speed and a projectile prefab."; return false; }
            reason = string.Empty;
            return true;
        }

        public static WeaponDefinition CreatePrototypePulsePistol()
        {
            var definition = CreateInstance<WeaponDefinition>();
            definition.weaponId = "pulse-pistol-prototype-01";
            definition.fireStrategy = WeaponFireStrategy.Hitscan;
            definition.damage = 20;
            definition.fireInterval = .25f;
            definition.range = 35f;
            return definition;
        }

        public static WeaponDefinition CreatePrototypeScatterBlaster() => CreatePrototype("scatter-blaster-prototype-01", WeaponFireStrategy.Scatter, 8, .7f, 18f, 7f, 6, 0f, 0f, 0f, null);
        public static WeaponDefinition CreatePrototypeArcRifle() => CreatePrototype("arc-rifle-prototype-01", WeaponFireStrategy.Charge, 45, .8f, 30f, 0f, 1, .65f, 0f, 0f, null);
        public static WeaponDefinition CreatePrototypeOrbLauncher(GameObject projectilePrefab) => CreatePrototype("orb-launcher-prototype-01", WeaponFireStrategy.Projectile, 35, .9f, 25f, 0f, 1, 0f, 9f, 3f, projectilePrefab);

        private static WeaponDefinition CreatePrototype(string id, WeaponFireStrategy strategy, int prototypeDamage, float interval, float prototypeRange, float spread, int pellets, float charge, float speed, float radius, GameObject prefab)
        {
            var definition = CreateInstance<WeaponDefinition>();
            definition.weaponId = id; definition.fireStrategy = strategy; definition.damage = prototypeDamage; definition.fireInterval = interval; definition.range = prototypeRange;
            definition.spreadDegrees = spread; definition.pelletCount = pellets; definition.chargeSeconds = charge; definition.projectileSpeed = speed; definition.explosionRadius = radius; definition.gameplayProjectilePrefab = prefab;
            return definition;
        }
    }
}
