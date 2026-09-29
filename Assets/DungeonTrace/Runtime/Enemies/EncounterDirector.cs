using System.Collections.Generic;
using UnityEngine;

namespace DungeonTrace.Enemies
{
    public sealed class EncounterDirector : MonoBehaviour
    {
        private readonly List<EnemyController> enemies = new();
        private bool cleared;
        public bool IsCleared => cleared;
        public int ActiveEnemyCount => enemies.Count;
        public void Configure(IEnumerable<EnemyController> spawned)
        {
            enemies.Clear();
            if (spawned != null) foreach (var enemy in spawned) if (enemy != null && !enemies.Contains(enemy)) enemies.Add(enemy);
            cleared = enemies.Count == 0;
            Debug.Log($"[Encounter] started enemies={enemies.Count}", this);
        }
        private void Update()
        {
            if (cleared || enemies.Count == 0) return;
            for (var i = enemies.Count - 1; i >= 0; i--) if (enemies[i] == null || !enemies[i].gameObject.activeInHierarchy) enemies.RemoveAt(i);
            if (enemies.Count == 0) { cleared = true; Debug.Log("[Encounter] cleared", this); }
        }
    }
}
