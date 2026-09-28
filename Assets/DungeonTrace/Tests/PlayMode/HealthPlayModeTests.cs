using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PlayerHealth = DungeonTrace.Health.Health;

namespace DungeonTrace.Tests.PlayMode
{
    public sealed class HealthPlayModeTests
    {
        [UnityTest] public IEnumerator Health_RaisesDeathOnceAtZero()
        {
            var gameObject = new GameObject("HealthTest");
            var health = gameObject.AddComponent<PlayerHealth>();
            yield return null;
            var deaths = 0;
            health.Died += () => deaths++;
            health.TakeDamage(100);
            health.TakeDamage(1);
            Assert.That(deaths, Is.EqualTo(1));
            Object.Destroy(gameObject);
        }
    }
}
