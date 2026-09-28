namespace DungeonTrace.Domain
{
    /// <summary>Rules that keep an observation session reproducible and non-adaptive.</summary>
    public sealed class ResearchModePolicy
    {
        public bool IsAdaptiveDifficultyAllowed(SessionConfig config) => config.Mode != ResearchMode.ResearchObservation;

        public int ResolveSeed(SessionConfig config) => config.Seed;
    }
}
