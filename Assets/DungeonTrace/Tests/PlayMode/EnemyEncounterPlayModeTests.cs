using System.Collections;
using DungeonTrace.Enemies;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DungeonTrace.Tests.PlayMode
{
    public sealed class EnemyEncounterPlayModeTests
    {
        [UnityTest]
        public IEnumerator EncounterDirector_ClearsWhenItsLastEnemyIsDisabled()
        {
            var player = new GameObject("EncounterPlayer");
            var enemy = new GameObject("EncounterEnemy");
            var controller = enemy.AddComponent<EnemyController>();
            controller.Configure(CreateDefinition("test-pursuer", EnemyArchetype.Pursuer, 2f), player.transform, 0f);
            var director = new GameObject("EncounterDirector").AddComponent<EncounterDirector>();
            director.Configure(new[] { controller, controller });
            yield return null;

            Assert.That(director.ActiveEnemyCount, Is.EqualTo(1));
            Assert.That(director.IsCleared, Is.False);
            enemy.SetActive(false);
            yield return null;

            Assert.That(director.ActiveEnemyCount, Is.EqualTo(0));
            Assert.That(director.IsCleared, Is.True);
            Object.Destroy(player);
            Object.Destroy(enemy);
            Object.Destroy(director.gameObject);
        }

        [UnityTest]
        public IEnumerator Pursuer_UsesVisibleFallbackMovementWhenNoNavMeshIsAvailable()
        {
            var player = new GameObject("MovementPlayer");
            player.transform.position = new Vector3(0f, 0f, 10f);
            var enemy = new GameObject("MovingPursuer");
            var controller = enemy.AddComponent<EnemyController>();
            controller.Configure(CreateDefinition("test-moving-pursuer", EnemyArchetype.Pursuer, 4f), player.transform, 0f);
            yield return null;
            yield return null;

            Assert.That(enemy.transform.position.z, Is.GreaterThan(0f));
            Object.Destroy(player);
            Object.Destroy(enemy);
        }

        private static EnemyDefinition CreateDefinition(string id, EnemyArchetype archetype, float speed)
        {
            var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
            definition.ConfigurePrototype(id, archetype, speed, 1f, 1f, .1f, 1);
            return definition;
        }
    }
}
