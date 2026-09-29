using DungeonTrace.Bosses;
using DungeonTrace.Items;
using NUnit.Framework;
using UnityEngine;

namespace DungeonTrace.Tests.EditMode
{
    public sealed class PhaseSixDomainTests
    {
        [Test] public void BossState_TransitionsToPhaseTwoAndReportsDeathOnce()
        {
            var definition = ScriptableObject.CreateInstance<BossDefinition>();
            definition.ConfigurePrototype("warden-test", BossArchetype.Warden, 100, .5f, 1f, 2);
            var state = new BossState(definition);
            state.ApplyDamage(50);
            Assert.That(state.Phase, Is.EqualTo(2));
            Assert.That(state.ApplyDamage(50), Is.True);
            Assert.That(state.ApplyDamage(1), Is.False);
            Object.DestroyImmediate(definition);
        }

        [Test] public void RunInventory_RejectsDuplicateUniqueItems()
        {
            var inventory = new RunItemInventory();
            Assert.That(inventory.TryAddUnique("item-skeleton-key"), Is.True);
            Assert.That(inventory.TryAddUnique("item-skeleton-key"), Is.False);
            Assert.That(inventory.Count, Is.EqualTo(1));
        }
    }
}
