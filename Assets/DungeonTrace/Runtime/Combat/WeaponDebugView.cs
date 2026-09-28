using UnityEngine;

namespace DungeonTrace.Combat
{
    /// <summary>Simple prefab-based viewmodel bob/recoil for combat verification.</summary>
    public sealed class WeaponDebugView : MonoBehaviour
    {
        [SerializeField] private Vector3 idleOffset = new(.28f, -.23f, .55f);
        private Vector3 recoil;

        public void Bind(WeaponController weapon)
        {
            if (weapon == null) return;
            weapon.ShotFired += OnShotFired;
        }

        private void LateUpdate()
        {
            recoil = Vector3.Lerp(recoil, Vector3.zero, 14f * Time.deltaTime);
            transform.localPosition = idleOffset + new Vector3(0f, Mathf.Sin(Time.time * 2f) * .012f, 0f) + recoil;
            transform.localRotation = Quaternion.Euler(recoil.y * -50f, 0f, 0f);
        }

        private void OnShotFired(WeaponShot shot) => recoil = new Vector3(0f, 0f, -.09f);
    }
}
