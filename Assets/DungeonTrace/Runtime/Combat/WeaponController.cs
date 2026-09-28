using System;
using DungeonTrace.Domain;
using DungeonTrace.Player;
using UnityEngine;

namespace DungeonTrace.Combat
{
    /// <summary>Owns two weapon slots and emits gameplay facts; telemetry and presentation remain observers.</summary>
    public sealed class WeaponController : MonoBehaviour
    {
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private bool diagnosticLogs = true;
        private AimProvider aim;
        private PlayerInputReader input;
        private readonly WeaponDefinition[] slots = new WeaponDefinition[2];
        private PlayerStateController playerState;
        private float nextFireTime;
        private float chargeStartedAt = -1f;
        private int activeSlot;
        private int shotSequence;

        public WeaponDefinition ActiveWeapon => slots[activeSlot];
        public int ActiveSlot => activeSlot;
        public event Action<WeaponShot> ShotFired;
        public event Action<WeaponShotResolution> ShotResolved;
        public event Action<WeaponDefinition> WeaponEquipped;

        public void Configure(AimProvider aimProvider, PlayerInputReader inputReader, WeaponDefinition primary, PlayerStateController stateController = null) => Configure(aimProvider, inputReader, primary, null, stateController);

        public void Configure(AimProvider aimProvider, PlayerInputReader inputReader, WeaponDefinition primary, WeaponDefinition secondary, PlayerStateController stateController = null)
        {
            aim = aimProvider; input = inputReader; slots[0] = primary; slots[1] = secondary; playerState = stateController; activeSlot = 0; chargeStartedAt = -1f;
        }

        private void Update()
        {
            if (input == null || aim == null || ActiveWeapon == null || !CanAct()) return;
            if (input.SwitchWeaponPressedThisFrame) TrySwitchWeapon();
            var weapon = ActiveWeapon;
            if (weapon.FireStrategy == WeaponFireStrategy.Charge)
            {
                if (input.FirePressedThisFrame) chargeStartedAt = Time.time;
                if (input.FireReleasedThisFrame) ReleaseCharge();
                return;
            }
            if (input.FirePressedThisFrame || input.AlternateFirePressedThisFrame) TryFire();
        }

        public bool TrySwitchWeapon()
        {
            if (!CanAct() || slots[1 - activeSlot] == null) return false;
            activeSlot = 1 - activeSlot; chargeStartedAt = -1f;
            WeaponEquipped?.Invoke(ActiveWeapon);
            return true;
        }

        public bool TryFire()
        {
            if (!CanAct() || ActiveWeapon == null || Time.time < nextFireTime || ActiveWeapon.FireStrategy == WeaponFireStrategy.Charge) return false;
            Fire(ActiveWeapon, 1f);
            return true;
        }

        private void ReleaseCharge()
        {
            if (chargeStartedAt < 0f || Time.time < nextFireTime) { chargeStartedAt = -1f; return; }
            var charge = Mathf.Clamp01((Time.time - chargeStartedAt) / ActiveWeapon.ChargeSeconds);
            chargeStartedAt = -1f;
            if (charge > 0f) Fire(ActiveWeapon, charge);
        }

        private void Fire(WeaponDefinition weapon, float chargeMultiplier)
        {
            nextFireTime = Time.time + weapon.FireInterval;
            var shot = new WeaponShot($"{weapon.WeaponId}-shot-{++shotSequence:D6}", weapon.WeaponId, aim.GetAimRay(), weapon.FireStrategy == WeaponFireStrategy.Scatter ? weapon.PelletCount : 1);
            ShotFired?.Invoke(shot);
            if (diagnosticLogs) Debug.Log($"[Combat] fired id={shot.ShotId} weapon={shot.WeaponId} pellets={shot.PelletCount}", this);
            if (weapon.FireStrategy == WeaponFireStrategy.Projectile) { FireProjectile(weapon, shot); return; }
            var hits = 0; var damage = 0;
            for (var pellet = 0; pellet < shot.PelletCount; pellet++)
            {
                var direction = GetShotDirection(shot.AimRay.direction, weapon.SpreadDegrees, pellet, shot.PelletCount);
                if (!Physics.Raycast(shot.AimRay.origin, direction, out var hit, weapon.Range, hitMask, QueryTriggerInteraction.Ignore)) continue;
                if (!TryApplyDamage(hit, weapon, shot.ShotId, Mathf.RoundToInt(weapon.Damage * chargeMultiplier), out var applied)) continue;
                hits++; damage += applied;
            }
            ResolveShot(shot.ShotId, hits, damage);
        }

        private void FireProjectile(WeaponDefinition weapon, WeaponShot shot)
        {
            if (weapon.GameplayProjectilePrefab == null) { ResolveShot(shot.ShotId, 0, 0); return; }
            var projectileObject = Instantiate(weapon.GameplayProjectilePrefab, shot.AimRay.origin, Quaternion.LookRotation(shot.AimRay.direction));
            var projectile = projectileObject.GetComponent<OrbProjectile>();
            if (projectile == null) { Destroy(projectileObject); ResolveShot(shot.ShotId, 0, 0); return; }
            projectile.Launch(weapon, shot.ShotId, hitMask, this);
        }

        internal void ResolveProjectile(string shotId, int hitCount, int damageDealt) => ResolveShot(shotId, hitCount, damageDealt);

        private void ResolveShot(string shotId, int hitCount, int damageDealt)
        {
            ShotResolved?.Invoke(new WeaponShotResolution(shotId, hitCount, damageDealt));
            if (diagnosticLogs) Debug.Log($"[Combat] resolved id={shotId} hits={hitCount} damage={damageDealt}", this);
        }

        internal bool TryApplyDamage(RaycastHit hit, WeaponDefinition weapon, string shotId, int damage, out int appliedDamage)
        {
            appliedDamage = 0;
            var damageable = hit.collider.GetComponentInParent(typeof(IDamageable)) as IDamageable;
            if (damageable == null) return false;
            var targetId = damageable is IStableDamageable stable ? stable.StableDamageableId : string.Empty;
            var result = damageable.ApplyDamage(new DamageContext(weapon.WeaponId, targetId, shotId, damage, DamageType.Energy, hit.distance, hit.point, hit.normal));
            appliedDamage = result.AppliedDamage;
            if (diagnosticLogs) Debug.Log($"[Combat] hit id={shotId} target={targetId} damage={appliedDamage} killed={result.TargetDied}", hit.collider);
            return true;
        }

        private bool CanAct() => playerState == null || playerState.CanAcceptGameplayInput;

        private static Vector3 GetShotDirection(Vector3 forward, float spreadDegrees, int pelletIndex, int pelletCount)
        {
            if (spreadDegrees <= 0f || pelletCount <= 1) return forward.normalized;
            var angle = pelletIndex * 2.39996323f;
            var radius = Mathf.Sqrt((pelletIndex + .5f) / pelletCount) * Mathf.Tan(spreadDegrees * Mathf.Deg2Rad);
            return Quaternion.LookRotation(forward) * new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 1f).normalized;
        }
    }
}
