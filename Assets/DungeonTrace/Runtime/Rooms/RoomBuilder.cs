using UnityEngine;

namespace DungeonTrace.Rooms
{
    public sealed class RoomBuilder : MonoBehaviour
    {
        [SerializeField] private RoomDefinition definition;
        [SerializeField] private Color floorColor = new(.15f, .15f, .18f);
        [SerializeField] private Color wallColor = new(.28f, .3f, .35f);

        public RoomDefinition Definition => definition;
        public void Configure(RoomDefinition roomDefinition) => definition = roomDefinition;

        public GameObject Build()
        {
            if (definition == null) { Debug.LogError("RoomBuilder requires a RoomDefinition.", this); return null; }
            if (!definition.IsValid(out var error)) { Debug.LogError($"Cannot build room '{definition.name}': {error}", this); return null; }

            var root = new GameObject($"Room_{definition.StableId}");
            root.transform.SetParent(transform, false);
            CreateBlock(root.transform, "Floor", new Vector3(0f, -.25f, 0f), new Vector3(definition.Width, .5f, definition.Depth), floorColor);
            CreateBlock(root.transform, "NorthWall", new Vector3(0f, definition.WallHeight / 2f, definition.Depth / 2f), new Vector3(definition.Width, definition.WallHeight, .5f), wallColor);
            CreateBlock(root.transform, "SouthWall", new Vector3(0f, definition.WallHeight / 2f, -definition.Depth / 2f), new Vector3(definition.Width, definition.WallHeight, .5f), wallColor);
            CreateBlock(root.transform, "EastWall", new Vector3(definition.Width / 2f, definition.WallHeight / 2f, 0f), new Vector3(.5f, definition.WallHeight, definition.Depth), wallColor);
            CreateBlock(root.transform, "WestWall", new Vector3(-definition.Width / 2f, definition.WallHeight / 2f, 0f), new Vector3(.5f, definition.WallHeight, definition.Depth), wallColor);
            foreach (var exit in definition.Exits) CreateExitMarker(root.transform, exit);
            return root;
        }

        private void CreateExitMarker(Transform parent, RoomExitDefinition exit)
        {
            var position = exit.Direction switch
            {
                RoomExitDirection.North => new Vector3(0f, 1f, definition.Depth / 2f - .35f),
                RoomExitDirection.East => new Vector3(definition.Width / 2f - .35f, 1f, 0f),
                RoomExitDirection.South => new Vector3(0f, 1f, -definition.Depth / 2f + .35f),
                _ => new Vector3(-definition.Width / 2f + .35f, 1f, 0f)
            };
            CreateBlock(parent, $"Exit_{exit.StableId}", position, new Vector3(1.5f, 2f, .2f), Color.magenta);
        }

        private static void CreateBlock(Transform parent, string name, Vector3 position, Vector3 scale, Color color)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.SetParent(parent, false);
            block.transform.localPosition = position;
            block.transform.localScale = scale;
            block.GetComponent<Renderer>().material.color = color;
        }
    }
}
