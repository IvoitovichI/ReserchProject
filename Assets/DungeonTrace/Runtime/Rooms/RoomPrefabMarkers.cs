using UnityEngine;

namespace DungeonTrace.Rooms
{
    public sealed class RoomRoot : MonoBehaviour
    {
        [SerializeField] private RoomDefinition definition;
        [SerializeField] private string contentVersion = "prototype-01";
        public RoomDefinition Definition => definition;
        public string ContentVersion => contentVersion;
        public void Configure(RoomDefinition roomDefinition, string version) { definition = roomDefinition; contentVersion = version; }
    }

    public sealed class DoorSocket : MonoBehaviour
    {
        [SerializeField] private string stableId;
        [SerializeField] private RoomExitDirection direction;
        [SerializeField, Min(.1f)] private float openingWidth = 1.5f;
        [SerializeField, Min(.1f)] private float openingHeight = 2f;
        public string StableId => stableId;
        public RoomExitDirection Direction => direction;
        public float OpeningWidth => openingWidth;
        public float OpeningHeight => openingHeight;
        public void Configure(string id, RoomExitDirection value) { stableId = id; direction = value; }
    }

    public sealed class PlayerEntry : MonoBehaviour
    {
        [SerializeField, Min(.1f)] private float cameraHeight = 1.65f;
        public float CameraHeight => cameraHeight;
    }

    public sealed class EnemySpawnPoint : MonoBehaviour { }
    public sealed class CoverMarker : MonoBehaviour { }
    public sealed class LoSMarker : MonoBehaviour { }
    public sealed class RewardSpawnPoint : MonoBehaviour { }

    public sealed class TelemetryZone : MonoBehaviour
    {
        [SerializeField] private string stableId;
        public string StableId => stableId;
        public void Configure(string id) => stableId = id;
    }

    public sealed class RoomBounds : MonoBehaviour
    {
        [SerializeField] private Vector3 center = new(0f, 1.5f, 0f);
        [SerializeField] private Vector3 size = new(12f, 3f, 12f);
        public Bounds Bounds => new(transform.TransformPoint(center), Vector3.Scale(size, transform.lossyScale));
        public void Configure(Vector3 valueCenter, Vector3 valueSize) { center = valueCenter; size = valueSize; }
        private void OnDrawGizmosSelected() { Gizmos.color = Color.cyan; Gizmos.DrawWireCube(Bounds.center, Bounds.size); }
    }
}
