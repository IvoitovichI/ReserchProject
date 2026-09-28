using DungeonTrace.Player;
using UnityEngine;

namespace DungeonTrace.Interaction
{
    public sealed class InteractionRaycaster : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float distance = 3f;
        [SerializeField] private LayerMask layers = ~0;
        private AimProvider aim;
        private PlayerInputReader input;
        private PlayerStateController playerState;

        public void Configure(AimProvider aimProvider, PlayerInputReader inputReader, PlayerStateController stateController = null, LayerMask? interactionLayers = null)
        {
            aim = aimProvider;
            input = inputReader;
            playerState = stateController;
            if (interactionLayers.HasValue) layers = interactionLayers.Value;
        }
        private void Awake() { aim ??= GetComponent<AimProvider>(); input ??= GetComponent<PlayerInputReader>(); playerState ??= GetComponent<PlayerStateController>(); }
        private void Update() { if (input != null && input.InteractPressedThisFrame) TryInteract(); }

        public bool TryInteract()
        {
            if (aim == null || aim.PlayerCamera == null || (playerState != null && !playerState.CanAcceptGameplayInput)) return false;
            if (!Physics.Raycast(aim.GetAimRay(), out var hit, distance, layers, QueryTriggerInteraction.Collide)) return false;
            var behaviours = hit.collider.GetComponentsInParent<MonoBehaviour>();
            foreach (var behaviour in behaviours)
            {
                if (behaviour is not IInteractable interactable) continue;
                interactable.Interact();
                return true;
            }

            return false;
        }
    }
}
