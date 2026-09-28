using DungeonTrace.Domain;
using DungeonTrace.Flow;
using DungeonTrace.Rooms;
using NUnit.Framework;
using UnityEngine;

namespace DungeonTrace.Tests.EditMode
{
    public sealed class DomainTests
    {
        [Test] public void HealthState_ClampsDamageAndStopsAtZero()
        {
            var health = new HealthState(10);
            Assert.That(health.ApplyDamage(12), Is.EqualTo(10));
            Assert.That(health.Current, Is.EqualTo(0));
            Assert.That(health.ApplyDamage(1), Is.EqualTo(0));
        }

        [Test] public void ObservationMode_UsesConfiguredSeedAndDisablesAdaptation()
        {
            var config = new SessionConfig(ResearchMode.ResearchObservation, 77);
            var policy = new ResearchModePolicy();
            Assert.That(policy.ResolveSeed(config), Is.EqualTo(77));
            Assert.That(policy.IsAdaptiveDifficultyAllowed(config), Is.False);
        }

        [Test] public void Flow_TransitionsFromPlayingToPlayerDead()
        {
            var flow = new GameFlowController();
            flow.StartSession(new SessionConfig());
            flow.NotifyPlayerDied();
            Assert.That(flow.State, Is.EqualTo(GameFlowState.PlayerDead));
        }

        [Test] public void Flow_PauseCanOnlyBeToggledDuringActivePlay()
        {
            var flow = new GameFlowController();
            flow.StartSession(new SessionConfig());
            flow.Pause();
            Assert.That(flow.State, Is.EqualTo(GameFlowState.Paused));
            flow.Resume();
            Assert.That(flow.State, Is.EqualTo(GameFlowState.Playing));
            flow.NotifyPlayerDied();
            flow.Pause();
            Assert.That(flow.State, Is.EqualTo(GameFlowState.PlayerDead));
        }

        [Test] public void PrototypeRoom_HasStableIdAndValidExit()
        {
            var room = RoomDefinition.CreatePrototype();
            Assert.That(room.IsValid(out _), Is.True);
            Assert.That(room.StableId, Is.EqualTo("room-prototype-01"));
            Object.DestroyImmediate(room);
        }
    }
}
