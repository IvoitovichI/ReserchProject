using System;
using DungeonTrace.Generation;
using DungeonTrace.Combat;
using PlayerHealth = DungeonTrace.Health.Health;
using DungeonTrace.Rooms;
using UnityEditor;
using UnityEngine;

namespace DungeonTrace.Editor
{
    /// <summary>Creates only primitive placeholder content; rerunning replaces no existing assets.</summary>
    public static class PlaceholderRoomAssetGenerator
    {
        private const string Folder = "Assets/DungeonTrace/GeneratedDungeon";
        private const string Version = "prototype-02";

        [MenuItem("Dungeon Trace/Rooms/Create Generation Placeholder Assets")]
        public static void CreateAssets()
        {
            EnsureFolder("Assets", "DungeonTrace"); EnsureFolder("Assets/DungeonTrace", "GeneratedDungeon");
            var empty = CreatePrefab("Room_Empty", BuildEmpty);
            var table = CreatePrefab("Room_Table", BuildTable);
            var crypt = CreatePrefab("Room_Crypt", BuildCrypt);
            EnsureWalls(empty); EnsureWalls(table); EnsureWalls(crypt);
            foreach (RoomType type in Enum.GetValues(typeof(RoomType)))
            {
                var path = $"{Folder}/Definition_{type}.asset";
                if (AssetDatabase.LoadAssetAtPath<RoomDefinition>(path) != null) continue;
                var definition = ScriptableObject.CreateInstance<RoomDefinition>();
                var prefab = type == RoomType.Start || type == RoomType.Choice ? empty : type == RoomType.Elite || type == RoomType.Secret || type == RoomType.Boss ? crypt : table;
                definition.ConfigureAuthoring($"room-{type.ToString().ToLowerInvariant()}-02", Version, type, prefab, Sockets(), Array.Empty<TelemetryZoneDefinition>());
                AssetDatabase.CreateAsset(definition, path);
            }
            ConfigurePrefabRoot(empty, AssetDatabase.LoadAssetAtPath<RoomDefinition>($"{Folder}/Definition_Start.asset"));
            ConfigurePrefabRoot(table, AssetDatabase.LoadAssetAtPath<RoomDefinition>($"{Folder}/Definition_Combat.asset"));
            ConfigurePrefabRoot(crypt, AssetDatabase.LoadAssetAtPath<RoomDefinition>($"{Folder}/Definition_Boss.asset"));
            if (AssetDatabase.LoadAssetAtPath<DungeonGenerationConfig>($"{Folder}/DungeonGenerationConfig.asset") == null) AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DungeonGenerationConfig>(), $"{Folder}/DungeonGenerationConfig.asset");
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Selection.activeObject = empty;
            Debug.Log($"Created Dungeon Trace primitive room assets in {Folder} with content version {Version}.");
        }

        [MenuItem("Dungeon Trace/Create Debug Combat Assets")]
        public static void CreateDebugCombatAssets()
        {
            EnsureFolder("Assets", "Resources"); EnsureFolder("Assets/Resources", "DungeonTrace"); EnsureFolder("Assets/Resources/DungeonTrace", "Combat");
            CreateDebugPrefab("Assets/Resources/DungeonTrace/Combat/CombatDummy.prefab", BuildCombatDummy);
            CreateDebugPrefab("Assets/Resources/DungeonTrace/Combat/WeaponViewmodel.prefab", BuildWeaponViewmodel);
            CreateDebugPrefab("Assets/Resources/DungeonTrace/Combat/EnemyProjectile.prefab", BuildEnemyProjectile);
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Debug.Log("Created debug combat prefabs: CombatDummy and WeaponViewmodel.");
        }

        private static GameObject CreatePrefab(string name, Action<GameObject> decorate)
        {
            var path = $"{Folder}/{name}.prefab"; var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path); if (existing != null) return existing;
            var root = new GameObject(name); root.AddComponent<RoomRoot>(); root.AddComponent<RoomBounds>().Configure(new Vector3(0f, 1.5f, 0f), new Vector3(12f, 3f, 12f));
            CreateFloor(root.transform); CreateMarkers(root.transform); decorate(root);
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path); UnityEngine.Object.DestroyImmediate(root); return prefab;
        }
        private static void ConfigurePrefabRoot(GameObject prefab, RoomDefinition definition)
        { var root = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(prefab)); root.GetComponent<RoomRoot>().Configure(definition, Version); PrefabUtility.SaveAsPrefabAsset(root, AssetDatabase.GetAssetPath(prefab)); PrefabUtility.UnloadPrefabContents(root); }
        private static RoomExitDefinition[] Sockets() => new[] { new RoomExitDefinition("door-north", RoomExitDirection.North), new RoomExitDefinition("door-east", RoomExitDirection.East), new RoomExitDefinition("door-south", RoomExitDirection.South), new RoomExitDefinition("door-west", RoomExitDirection.West) };
        private static void CreateMarkers(Transform parent)
        {
            var entry = new GameObject("PlayerEntry"); entry.transform.SetParent(parent, false); entry.transform.localPosition = new Vector3(0f, 0f, -4f); entry.AddComponent<PlayerEntry>();
            AddSocket(parent, "door-north", new Vector3(0f, 0f, 6f), Vector3.forward, RoomExitDirection.North); AddSocket(parent, "door-east", new Vector3(6f, 0f, 0f), Vector3.right, RoomExitDirection.East); AddSocket(parent, "door-south", new Vector3(0f, 0f, -6f), Vector3.back, RoomExitDirection.South); AddSocket(parent, "door-west", new Vector3(-6f, 0f, 0f), Vector3.left, RoomExitDirection.West);
        }
        private static void EnsureWalls(GameObject prefab)
        {
            var path = AssetDatabase.GetAssetPath(prefab); var root = PrefabUtility.LoadPrefabContents(path);
            if (root.transform.Find("NorthWallLeft") == null)
            {
                const float wallHeight = 3f; const float half = 6f;
                Block(root.transform, "NorthWallLeft", new Vector3(-3.625f, wallHeight / 2f, half), new Vector3(4.75f, wallHeight, .35f), new Color(.18f, .25f, .32f));
                Block(root.transform, "NorthWallRight", new Vector3(3.625f, wallHeight / 2f, half), new Vector3(4.75f, wallHeight, .35f), new Color(.18f, .25f, .32f));
                Block(root.transform, "SouthWallLeft", new Vector3(-3.625f, wallHeight / 2f, -half), new Vector3(4.75f, wallHeight, .35f), new Color(.18f, .25f, .32f));
                Block(root.transform, "SouthWallRight", new Vector3(3.625f, wallHeight / 2f, -half), new Vector3(4.75f, wallHeight, .35f), new Color(.18f, .25f, .32f));
                Block(root.transform, "EastWallFront", new Vector3(half, wallHeight / 2f, -3.625f), new Vector3(.35f, wallHeight, 4.75f), new Color(.18f, .25f, .32f));
                Block(root.transform, "EastWallBack", new Vector3(half, wallHeight / 2f, 3.625f), new Vector3(.35f, wallHeight, 4.75f), new Color(.18f, .25f, .32f));
                Block(root.transform, "WestWallFront", new Vector3(-half, wallHeight / 2f, -3.625f), new Vector3(.35f, wallHeight, 4.75f), new Color(.18f, .25f, .32f));
                Block(root.transform, "WestWallBack", new Vector3(-half, wallHeight / 2f, 3.625f), new Vector3(.35f, wallHeight, 4.75f), new Color(.18f, .25f, .32f));
            }
            PrefabUtility.SaveAsPrefabAsset(root, path); PrefabUtility.UnloadPrefabContents(root);
        }
        private static void CreateDebugPrefab(string path, Action<GameObject> decorate)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;
            var root = new GameObject(System.IO.Path.GetFileNameWithoutExtension(path)); decorate(root);
            PrefabUtility.SaveAsPrefabAsset(root, path); UnityEngine.Object.DestroyImmediate(root);
        }
        private static void BuildCombatDummy(GameObject root)
        {
            root.AddComponent<BoxCollider>(); root.AddComponent<PlayerHealth>(); root.AddComponent<CombatDebugDummy>();
            Block(root.transform, "DummyBody", new Vector3(0f, 0f, 0f), new Vector3(1.2f, 2f, .7f), new Color(.8f, .18f, .18f));
            Block(root.transform, "DummyHead", new Vector3(0f, 1.35f, 0f), new Vector3(.7f, .55f, .55f), new Color(.95f, .62f, .2f));
        }
        private static void BuildWeaponViewmodel(GameObject root)
        {
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube); body.name = "PulsePistolBody"; body.transform.SetParent(root.transform, false); body.transform.localPosition = new Vector3(0f, 0f, .12f); body.transform.localScale = new Vector3(.16f, .11f, .42f); body.GetComponent<Renderer>().sharedMaterial.color = new Color(.08f, .65f, .95f); UnityEngine.Object.DestroyImmediate(body.GetComponent<Collider>());
            var barrel = GameObject.CreatePrimitive(PrimitiveType.Cube); barrel.name = "PulsePistolBarrel"; barrel.transform.SetParent(root.transform, false); barrel.transform.localPosition = new Vector3(0f, .03f, .4f); barrel.transform.localScale = new Vector3(.09f, .06f, .25f); barrel.GetComponent<Renderer>().sharedMaterial.color = new Color(.85f, .95f, 1f); UnityEngine.Object.DestroyImmediate(barrel.GetComponent<Collider>());
            root.AddComponent<WeaponDebugView>();
        }
        private static void BuildEnemyProjectile(GameObject root)
        {
            var core = GameObject.CreatePrimitive(PrimitiveType.Sphere); core.name = "SpitOrb"; core.transform.SetParent(root.transform, false); core.transform.localScale = Vector3.one * .3f; core.GetComponent<Renderer>().sharedMaterial.color = Color.magenta; UnityEngine.Object.DestroyImmediate(core.GetComponent<Collider>());
            root.AddComponent<DungeonTrace.Enemies.EnemyProjectile>();
        }
        private static void AddSocket(Transform parent, string id, Vector3 position, Vector3 forward, RoomExitDirection direction) { var socket = new GameObject(id); socket.transform.SetParent(parent, false); socket.transform.localPosition = position; socket.transform.forward = forward; socket.AddComponent<DoorSocket>().Configure(id, direction); }
        private static void CreateFloor(Transform parent) => Block(parent, "Floor", new Vector3(0f, -.25f, 0f), new Vector3(12f, .5f, 12f), new Color(.2f, .22f, .25f));
        private static void BuildEmpty(GameObject root) { }
        private static void BuildTable(GameObject root)
        {
            Block(root.transform, "TableTop", new Vector3(0f, 1.1f, 0f), new Vector3(3.2f, .2f, 1.6f), new Color(.32f, .18f, .08f));
            for (var x = -1; x <= 1; x += 2) for (var z = -1; z <= 1; z += 2) Block(root.transform, "TableLeg", new Vector3(x * 1.35f, .5f, z * .55f), new Vector3(.18f, 1f, .18f), new Color(.2f, .1f, .04f));
            Chair(root.transform, new Vector3(-2.3f, .5f, 0f)); Chair(root.transform, new Vector3(2.3f, .5f, 0f));
        }
        private static void Chair(Transform parent, Vector3 position) { Block(parent, "ChairSeat", position, new Vector3(.8f, .15f, .8f), new Color(.18f, .1f, .04f)); Block(parent, "ChairBack", position + new Vector3(0f, .55f, .32f), new Vector3(.8f, .9f, .12f), new Color(.18f, .1f, .04f)); }
        private static void BuildCrypt(GameObject root)
        {
            Block(root.transform, "Sarcophagus", new Vector3(0f, .45f, 0f), new Vector3(2f, .9f, 3.4f), new Color(.25f, .28f, .3f));
            var positions = new[] { new Vector3(-3.8f, 1.5f, -3.8f), new Vector3(3.8f, 1.5f, -3.8f), new Vector3(-3.8f, 1.5f, 3.8f), new Vector3(3.8f, 1.5f, 3.8f), new Vector3(0f, 1.5f, 4.2f) };
            foreach (var position in positions) { var column = GameObject.CreatePrimitive(PrimitiveType.Cylinder); column.name = "CryptColumn"; column.transform.SetParent(root.transform, false); column.transform.localPosition = position; column.transform.localScale = new Vector3(.55f, 1.5f, .55f); column.GetComponent<Renderer>().sharedMaterial.color = new Color(.42f, .44f, .46f); }
        }
        private static void Block(Transform parent, string name, Vector3 position, Vector3 scale, Color color) { var block = GameObject.CreatePrimitive(PrimitiveType.Cube); block.name = name; block.transform.SetParent(parent, false); block.transform.localPosition = position; block.transform.localScale = scale; block.GetComponent<Renderer>().sharedMaterial.color = color; }
        private static void EnsureFolder(string parent, string child) { var path = parent + "/" + child; if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child); }
    }
}
