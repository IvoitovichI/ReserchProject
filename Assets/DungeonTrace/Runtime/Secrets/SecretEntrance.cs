using DungeonTrace.Interaction;
using UnityEngine;

namespace DungeonTrace.Secrets
{
    public enum SecretEntranceState { ClosedHidden, Revealed, Open }

    public sealed class SecretEntrance : MonoBehaviour, IInteractable
    {
        [SerializeField] private string stableSecretId = "secret-prototype-01";
        [SerializeField] private SecretEntranceState state = SecretEntranceState.ClosedHidden;
        private bool rewardClaimed;
        public string StableSecretId => stableSecretId;
        public SecretEntranceState State => state;
        public bool RewardClaimed => rewardClaimed;
        public void Configure(string id) => stableSecretId = id;
        public void Reveal() { if (state == SecretEntranceState.ClosedHidden) state = SecretEntranceState.Revealed; }
        public void Interact() { if (state == SecretEntranceState.Revealed) state = SecretEntranceState.Open; }
        public bool TryClaimReward() { if (state != SecretEntranceState.Open || rewardClaimed) return false; rewardClaimed = true; return true; }
    }
}
