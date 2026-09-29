using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonTrace.Hub
{
    public enum HubItemCategory { Cosmetic, Trophy, StartingModule, Archive }

    [Serializable] public sealed class HubItemDefinition
    {
        public string itemId;
        public HubItemCategory category;
        public int price;
        public HubItemDefinition(string id, HubItemCategory type, int cost) { itemId = id; category = type; price = Mathf.Max(0, cost); }
    }
    [Serializable] public sealed class HubSlotDefinition
    {
        public string slotId;
        public HubItemCategory category;
        public HubSlotDefinition(string id, HubItemCategory type) { slotId = id; category = type; }
    }
    [Serializable] public sealed class HubPlacement { public string slotId; public string itemId; }
    [Serializable] public sealed class HubProfile
    {
        public int schemaVersion = 1;
        public int coins = 20;
        public HubPlacement[] placements = Array.Empty<HubPlacement>();
    }
    public sealed class HubProfileStore
    {
        private const string Key = "dungeon-trace.hub-profile";
        public HubProfile Load() { var json = PlayerPrefs.GetString(Key, string.Empty); return string.IsNullOrWhiteSpace(json) ? new HubProfile() : JsonUtility.FromJson<HubProfile>(json) ?? new HubProfile(); }
        public void Save(HubProfile profile) { PlayerPrefs.SetString(Key, JsonUtility.ToJson(profile)); PlayerPrefs.Save(); }
        public void Reset() { PlayerPrefs.DeleteKey(Key); PlayerPrefs.Save(); }
    }
    public sealed class HubBuildController : MonoBehaviour
    {
        [SerializeField] private HubProfile profile;
        private readonly HubProfileStore store = new();
        private readonly Stack<HubPlacement> undo = new();
        private bool persist = true;
        public HubProfile Profile => profile;
        public event Action Changed;
        private void Awake() => profile = store.Load();
        public void ConfigureForTests(HubProfile value) { profile = value ?? new HubProfile(); persist = false; }
        public bool TryPurchaseAndPlace(HubItemDefinition item, HubSlotDefinition slot)
        {
            if (item == null || slot == null || item.category != slot.category || profile.coins < item.price) return false;
            var values = new List<HubPlacement>(profile.placements ?? Array.Empty<HubPlacement>());
            var index = values.FindIndex(value => value.slotId == slot.slotId);
            if (index >= 0) { undo.Push(values[index]); values[index] = new HubPlacement { slotId = slot.slotId, itemId = item.itemId }; }
            else values.Add(new HubPlacement { slotId = slot.slotId, itemId = item.itemId });
            profile.coins -= item.price; profile.placements = values.ToArray(); Save(); return true;
        }
        public bool Remove(string slotId)
        {
            var values = new List<HubPlacement>(profile.placements ?? Array.Empty<HubPlacement>()); var index = values.FindIndex(value => value.slotId == slotId);
            if (index < 0) return false; undo.Push(values[index]); values.RemoveAt(index); profile.placements = values.ToArray(); Save(); return true;
        }
        public bool Undo()
        {
            if (undo.Count == 0) return false; var prior = undo.Pop(); var values = new List<HubPlacement>(profile.placements ?? Array.Empty<HubPlacement>()); var index = values.FindIndex(value => value.slotId == prior.slotId);
            if (index >= 0) values[index] = prior; else values.Add(prior); profile.placements = values.ToArray(); Save(); return true;
        }
        public void ResetProfile() { if (persist) store.Reset(); profile = new HubProfile(); undo.Clear(); Changed?.Invoke(); }
        private void Save() { if (persist) store.Save(profile); Changed?.Invoke(); }
    }
}
