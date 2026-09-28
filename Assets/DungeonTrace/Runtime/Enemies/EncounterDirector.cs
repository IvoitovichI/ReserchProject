using System.Collections.Generic;
using UnityEngine;

namespace DungeonTrace.Enemies
{
    public sealed class EncounterDirector : MonoBehaviour
    {
        private readonly List<EnemyController> enemies = new();
        private bool cleared;
        public void Configure(IEnumerable<EnemyController> spawned) { enemies.Clear(); enemies.AddRange(spawned); Debug.Log($"[Encounter] started enemies={enemies.Count}", this); }
        private void Update()
        {
            if (cleared || enemies.Count == 0) return;
            for (var i = enemies.Count - 1; i >= 0; i--) if (enemies[i] == null || !enemies[i].gameObject.activeInHierarchy) enemies.RemoveAt(i);
            if (enemies.Count == 0) { cleared = true; Debug.Log("[Encounter] cleared", this); }
        }
    }
}
