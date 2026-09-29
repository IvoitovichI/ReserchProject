using DungeonTrace.Hub;
using NUnit.Framework;
using UnityEngine;

namespace DungeonTrace.Tests.EditMode
{
    public sealed class HubProfileTests
    {
        [Test] public void PurchaseReplaceRemoveAndUndo_KeepProfileConsistent()
        {
            var host = new GameObject("HubTest"); var hub = host.AddComponent<HubBuildController>(); hub.ConfigureForTests(new HubProfile());
            var slot = new HubSlotDefinition("slot-lamp", HubItemCategory.Cosmetic); var lamp = new HubItemDefinition("lamp", HubItemCategory.Cosmetic, 5); var plant = new HubItemDefinition("plant", HubItemCategory.Cosmetic, 5);
            Assert.That(hub.TryPurchaseAndPlace(lamp, slot), Is.True); Assert.That(hub.TryPurchaseAndPlace(plant, slot), Is.True); Assert.That(hub.Remove(slot.slotId), Is.True); Assert.That(hub.Undo(), Is.True);
            Assert.That(hub.Profile.placements, Has.Length.EqualTo(1)); Assert.That(hub.Profile.placements[0].itemId, Is.EqualTo("plant")); Object.DestroyImmediate(host);
        }
    }
}
