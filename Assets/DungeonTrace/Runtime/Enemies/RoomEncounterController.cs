using System;
using System.Collections.Generic;
using DungeonTrace.Player;
using UnityEngine;
using UnityEngine.AI;
using EnemyHealth = DungeonTrace.Health.Health;

namespace DungeonTrace.Enemies
{
    [Serializable]
    public struct EncounterSpawn
    {
        [SerializeField] private string stableId;
        [SerializeField] private EnemyArchetype archetype;
        [SerializeField] private Vector3 localPosition;

        public string StableId => stableId;
        public EnemyArchetype Archetype => archetype;
        public Vector3 LocalPosition => localPosition;

        public EncounterSpawn(string id, EnemyArchetype type, Vector3 position)
        {
            stableId = id;
            archetype = type;
            localPosition = position;
        }
    }

    [RequireComponent(typeof(BoxCollider))]
    public sealed class RoomEncounterController : MonoBehaviour
    {
        private const string EnemyPrefabPath = "DungeonTrace/Combat/CombatDummy";

        [SerializeField] private string stableEncounterId;
        [SerializeField] private EncounterSpawn[] spawns = Array.Empty<EncounterSpawn>();
        [SerializeField, Min(.1f)] private float spawnGraceSeconds = 1f;
        private readonly List<EnemyController> spawnedEnemies = new();
        private EncounterDirector director;
        private bool started;

        public string StableEncounterId => stableEncounterId;
        public bool Started => started;
        public bool Cleared => director != null && director.IsCleared;
        public int ActiveEnemyCount => director == null ? 0 : director.ActiveEnemyCount;

        public void Configure(string id, EncounterSpawn[] values)
        {
            stableEncounterId = id;
            spawns = values ?? Array.Empty<EncounterSpawn>();
            var trigger = GetComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.center = new Vector3(0f, 1.5f, 0f);
            trigger.size = new Vector3(11f, 3f, 11f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (started || other.GetComponent<PlayerStateController>() == null) return;
            StartEncounter(other.transform);
        }

        public bool StartEncounter(Transform player)
        {
            if (started || player == null) return false;
            var prefab = Resources.Load<GameObject>(EnemyPrefabPath);
            if (prefab == null)
            {
                Debug.LogError($"[Encounter] '{stableEncounterId}' cannot start: missing resource '{EnemyPrefabPath}'.", this);
                return false;
            }

            started = true;
            director = gameObject.AddComponent<EncounterDirector>();
            for (var index = 0; index < spawns.Length; index++)
            {
                var spawn = spawns[index];
                var position = transform.TransformPoint(spawn.LocalPosition);
                if (NavMesh.SamplePosition(position, out var hit, 2f, NavMesh.AllAreas)) position = hit.position;
                var enemy = Instantiate(prefab, position, Quaternion.identity);
                enemy.name = spawn.StableId;
                enemy.GetComponent<EnemyHealth>()?.ConfigureStableId(spawn.StableId);
                var agent = enemy.GetComponent<NavMeshAgent>();
                if (agent == null) agent = enemy.AddComponent<NavMeshAgent>();
                ConfigureAgent(agent, spawn.Archetype);
                var controller = enemy.GetComponent<EnemyController>() ?? enemy.AddComponent<EnemyController>();
                controller.Configure(CreateDefinition(spawn), player, spawnGraceSeconds);
                spawnedEnemies.Add(controller);
            }
            director.Configure(spawnedEnemies);
            Debug.Log($"[Encounter] '{stableEncounterId}' started enemies={spawnedEnemies.Count}", this);
            return true;
        }

        private static EnemyDefinition CreateDefinition(EncounterSpawn spawn)
        {
            var definition = ScriptableObject.CreateInstance<EnemyDefinition>();
            switch (spawn.Archetype)
            {
                case EnemyArchetype.Pursuer: definition.ConfigurePrototype(spawn.StableId, spawn.Archetype, 2.3f, 1.7f, 1.5f, .55f, 8); break;
                case EnemyArchetype.Spitter: definition.ConfigurePrototype(spawn.StableId, spawn.Archetype, 1.6f, 5f, 2f, .8f, 6); break;
                case EnemyArchetype.Charger: definition.ConfigurePrototype(spawn.StableId, spawn.Archetype, 4f, 2.2f, 2.5f, .9f, 14); break;
                default: definition.ConfigurePrototype(spawn.StableId, spawn.Archetype, 1.3f, 2.5f, 2f, .7f, 5); break;
            }
            return definition;
        }

        private static void ConfigureAgent(NavMeshAgent agent, EnemyArchetype archetype)
        {
            agent.speed = archetype == EnemyArchetype.Charger ? 4f : archetype == EnemyArchetype.Pursuer ? 2.3f : 1.6f;
            agent.angularSpeed = 540f;
            agent.acceleration = 18f;
            agent.stoppingDistance = archetype == EnemyArchetype.Spitter ? 4f : 1.25f;
            agent.radius = .3f;
            agent.height = 1.8f;
        }
    }
}
