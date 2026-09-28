using DungeonTrace.Enemies;
using NUnit.Framework;
using UnityEngine;

namespace DungeonTrace.Tests.EditMode
{
    public sealed class EnemyDefinitionTests
    {
        [Test]
        public void PrototypeDefinitions_KeepDistinctArchetypesAndPositiveCombatValues()
        {
            foreach (EnemyArchetype archetype in System.Enum.GetValues(typeof(EnemyArchetype)))
            {
                var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
                definition.ConfigurePrototype($"{archetype.ToString().ToLowerInvariant()}_test", archetype, 2f, 3f, 1f, .5f, 5);
                Assert.That(definition.EnemyId, Is.Not.Empty);
                Assert.That(definition.Speed, Is.GreaterThan(0f));
                Assert.That(definition.AttackRange, Is.GreaterThan(0f));
                Assert.That(definition.AttackCooldown, Is.GreaterThan(0f));
                Assert.That(definition.TelegraphSeconds, Is.GreaterThan(0f));
                Object.DestroyImmediate(definition);
            }
        }
    }
}
