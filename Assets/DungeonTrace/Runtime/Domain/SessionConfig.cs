using System;
using UnityEngine;

namespace DungeonTrace.Domain
{
    public enum ResearchMode
    {
        Standard,
        ResearchObservation
    }

    [Serializable]
    public sealed class SessionConfig
    {
        [SerializeField] private ResearchMode mode = ResearchMode.Standard;
        [SerializeField] private int seed = 12345;
        [SerializeField] private string contentVersion = "prototype-01";

        public ResearchMode Mode => mode;
        public int Seed => seed;
        public string ContentVersion => contentVersion;

        public SessionConfig(ResearchMode mode = ResearchMode.Standard, int seed = 12345, string contentVersion = "prototype-01")
        {
            this.mode = mode;
            this.seed = seed;
            this.contentVersion = string.IsNullOrWhiteSpace(contentVersion) ? "prototype-01" : contentVersion;
        }
    }
}
