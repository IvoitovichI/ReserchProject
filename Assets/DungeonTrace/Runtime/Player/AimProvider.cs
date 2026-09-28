using UnityEngine;

namespace DungeonTrace.Player
{
    public sealed class AimProvider : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        public Camera PlayerCamera => playerCamera;
        public void Configure(Camera cameraToUse) => playerCamera = cameraToUse;
        public Ray GetAimRay() => playerCamera != null ? playerCamera.ViewportPointToRay(new Vector3(.5f, .5f)) : default;
    }
}
