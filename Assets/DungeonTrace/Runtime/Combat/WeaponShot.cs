using UnityEngine;

namespace DungeonTrace.Combat
{
    public readonly struct WeaponShot
    {
        public string ShotId { get; }
        public string WeaponId { get; }
        public Ray AimRay { get; }
        public int PelletCount { get; }

        public WeaponShot(string shotId, string weaponId, Ray aimRay, int pelletCount)
        {
            ShotId = shotId ?? string.Empty;
            WeaponId = weaponId ?? string.Empty;
            AimRay = aimRay;
            PelletCount = Mathf.Max(1, pelletCount);
        }
    }

    public readonly struct WeaponShotResolution
    {
        public string ShotId { get; }
        public int HitCount { get; }
        public int DamageDealt { get; }
        public WeaponShotResolution(string shotId, int hitCount, int damageDealt) { ShotId = shotId ?? string.Empty; HitCount = Mathf.Max(0, hitCount); DamageDealt = Mathf.Max(0, damageDealt); }
    }
}
