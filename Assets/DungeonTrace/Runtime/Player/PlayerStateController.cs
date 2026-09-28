using DungeonTrace.Flow;
using UnityEngine;
using PlayerHealth = DungeonTrace.Health.Health;

namespace DungeonTrace.Player
{
    /// <summary>Single gameplay-input guard for pause and player death.</summary>
    public sealed class PlayerStateController : MonoBehaviour
    {
        private PlayerInputReader input;
        private PlayerHealth health;
        private GameFlowController flow;

        public bool CanAcceptGameplayInput => flow != null && flow.State == GameFlowState.Playing && (health == null || !health.IsDead);

        public void Configure(PlayerInputReader inputReader, PlayerHealth playerHealth, GameFlowController gameFlow)
        {
            Unsubscribe();
            input = inputReader;
            health = playerHealth;
            flow = gameFlow;
            if (health != null) health.Died += OnPlayerDied;
        }

        private void Awake()
        {
            input ??= GetComponent<PlayerInputReader>();
            health ??= GetComponent<PlayerHealth>();
            if (health != null) health.Died += OnPlayerDied;
        }

        private void Update()
        {
            if (input == null || !input.PausePressedThisFrame || flow == null) return;
            if (flow.State == GameFlowState.Playing) flow.Pause();
            else if (flow.State == GameFlowState.Paused) flow.Resume();
        }

        private void OnDestroy() => Unsubscribe();

        private void OnPlayerDied() => flow?.NotifyPlayerDied();

        private void Unsubscribe()
        {
            if (health != null) health.Died -= OnPlayerDied;
        }
    }
}
