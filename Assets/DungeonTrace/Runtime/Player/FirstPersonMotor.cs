using UnityEngine;

namespace DungeonTrace.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonMotor : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float walkSpeed = 5f;
        [SerializeField] private float gravity = -20f;
        private CharacterController controller;
        private PlayerInputReader input;
        private PlayerStateController playerState;
        private float verticalVelocity;

        public void Configure(CharacterController characterController, PlayerInputReader inputReader, PlayerStateController stateController = null)
        {
            controller = characterController;
            input = inputReader;
            playerState = stateController;
        }

        private void Awake() { controller ??= GetComponent<CharacterController>(); input ??= GetComponent<PlayerInputReader>(); playerState ??= GetComponent<PlayerStateController>(); }

        private void Update()
        {
            if (controller == null || input == null || (playerState != null && !playerState.CanAcceptGameplayInput)) return;
            var horizontal = transform.right * input.Move.x + transform.forward * input.Move.y;
            if (horizontal.sqrMagnitude > 1f) horizontal.Normalize();
            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;
            controller.Move((horizontal * walkSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
        }
    }
}
