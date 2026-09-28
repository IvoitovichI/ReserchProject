using UnityEngine;

namespace DungeonTrace.Interaction
{
    public sealed class TraceConsole : MonoBehaviour, IInteractable
    {
        [SerializeField] private Renderer targetRenderer;
        private bool activated;
        public void Configure(Renderer rendererToUse) => targetRenderer = rendererToUse;
        public void Interact()
        {
            activated = !activated;
            if (targetRenderer != null) targetRenderer.material.color = activated ? Color.cyan : Color.yellow;
        }
    }
}
