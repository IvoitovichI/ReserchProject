using UnityEngine;

namespace DungeonTrace.Player
{
    public sealed class PlayerLookController : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField, Min(0f)] private float sensitivity = 0.12f;
        [SerializeField] private float minimumPitch = -80f;
        [SerializeField] private float maximumPitch = 80f;
        private PlayerInputReader input;
        private PlayerStateController playerState;
        private float pitch;

        public void Configure(Transform pivot, PlayerInputReader inputReader, PlayerStateController stateController = null) { cameraPivot = pivot; input = inputReader; playerState = stateController; }
        private void Awake() { input ??= GetComponent<PlayerInputReader>(); playerState ??= GetComponent<PlayerStateController>(); }
        private void Update()
        {
            if (cameraPivot == null || input == null || (playerState != null && !playerState.CanAcceptGameplayInput)) return;
            var look = input.Look * sensitivity;
            pitch = Mathf.Clamp(pitch - look.y, minimumPitch, maximumPitch);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            transform.Rotate(Vector3.up * look.x);
        }
    }
}
