using UnityEngine;
using PlayerHealth = DungeonTrace.Health.Health;
using DungeonTrace.Enemies;
using System.Collections.Generic;

namespace DungeonTrace.Combat
{
    public sealed class CombatSandboxSpawner : MonoBehaviour
    {
        private const string DummyPath = "DungeonTrace/Combat/CombatDummy";
        private const string ViewPath = "DungeonTrace/Combat/WeaponViewmodel";

        public void Configure(Transform cameraTransform, WeaponController weapon)
        {
            SpawnViewmodel(cameraTransform, weapon);
            SpawnDummy(new Vector3(0f, 1f, 8f), "combat_dummy_alpha");
            SpawnDummy(new Vector3(-2.5f, 1f, 11f), "combat_dummy_beta");
            Debug.Log("[CombatSandbox] Ready: fire with LMB, inspect [Combat] logs in Console.", this);
        }

        public void SpawnEnemyEncounter(Transform player)
        {
            var prefab = Resources.Load<GameObject>(DummyPath);
            if (prefab == null) return;
            var definitions = new[]
            {
                Definition("pursuer_proto", EnemyArchetype.Pursuer, 2.2f, 1.7f, 1.5f, .55f, 8),
                Definition("spitter_proto", EnemyArchetype.Spitter, 1.5f, 5f, 2f, .8f, 6),
                Definition("charger_proto", EnemyArchetype.Charger, 4f, 2.2f, 2.5f, .9f, 14),
                Definition("warder_proto", EnemyArchetype.Warder, 1.2f, 2.5f, 2f, .7f, 5)
            };
            var positions = new[] { new Vector3(2.5f, 1f, 14f), new Vector3(-2.5f, 1f, 14f), new Vector3(4f, 1f, 18f), new Vector3(-4f, 1f, 18f) };
            var spawned = new List<EnemyController>();
            for (var index = 0; index < definitions.Length; index++)
            {
                var enemy = Instantiate(prefab, positions[index], Quaternion.identity);
                enemy.name = definitions[index].EnemyId;
                enemy.GetComponent<PlayerHealth>()?.ConfigureStableId(definitions[index].EnemyId);
                var controller = enemy.AddComponent<EnemyController>(); controller.Configure(definitions[index], player); spawned.Add(controller);
            }
            gameObject.AddComponent<EncounterDirector>().Configure(spawned);
        }

        private static EnemyDefinition Definition(string id, EnemyArchetype archetype, float speed, float range, float cooldown, float telegraph, int damage)
        {
            var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
            definition.ConfigurePrototype(id, archetype, speed, range, cooldown, telegraph, damage);
            return definition;
        }

        private static void SpawnViewmodel(Transform cameraTransform, WeaponController weapon)
        {
            var prefab = Resources.Load<GameObject>(ViewPath);
            if (prefab == null) { Debug.LogWarning("[CombatSandbox] Missing WeaponViewmodel prefab. Run Dungeon Trace/Create Debug Combat Assets."); return; }
            var view = Instantiate(prefab, cameraTransform, false).GetComponent<WeaponDebugView>();
            if (view != null) view.Bind(weapon);
        }

        private static void SpawnDummy(Vector3 position, string stableId)
        {
            var prefab = Resources.Load<GameObject>(DummyPath);
            if (prefab == null) { Debug.LogWarning("[CombatSandbox] Missing CombatDummy prefab. Run Dungeon Trace/Create Debug Combat Assets."); return; }
            var dummy = Instantiate(prefab, position, Quaternion.identity);
            dummy.name = stableId;
            dummy.GetComponent<PlayerHealth>()?.ConfigureStableId(stableId);
        }
    }
}
