using System.Collections.Generic;

namespace DungeonTrace.Items
{
    public sealed class RunItemInventory
    {
        private readonly HashSet<string> uniqueItemIds = new();
        public int Count => uniqueItemIds.Count;
        public bool TryAddUnique(string itemId) => !string.IsNullOrWhiteSpace(itemId) && uniqueItemIds.Add(itemId);
        public bool Contains(string itemId) => uniqueItemIds.Contains(itemId ?? string.Empty);
    }
}
