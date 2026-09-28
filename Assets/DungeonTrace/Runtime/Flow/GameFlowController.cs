using System;
using DungeonTrace.Domain;

namespace DungeonTrace.Flow
{
    public enum GameFlowState { Booting, Playing, Paused, PlayerDead }

    public sealed class GameFlowController
    {
        public GameFlowState State { get; private set; } = GameFlowState.Booting;
        public SessionConfig Session { get; private set; }
        public event Action<GameFlowState> StateChanged;

        public void StartSession(SessionConfig session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
            ChangeState(GameFlowState.Playing);
        }

        public void NotifyPlayerDied()
        {
            if (State == GameFlowState.Playing || State == GameFlowState.Paused) ChangeState(GameFlowState.PlayerDead);
        }

        public void Pause()
        {
            if (State == GameFlowState.Playing) ChangeState(GameFlowState.Paused);
        }

        public void Resume()
        {
            if (State == GameFlowState.Paused) ChangeState(GameFlowState.Playing);
        }

        private void ChangeState(GameFlowState state)
        {
            if (State == state) return;
            State = state;
            StateChanged?.Invoke(state);
        }
    }
}
