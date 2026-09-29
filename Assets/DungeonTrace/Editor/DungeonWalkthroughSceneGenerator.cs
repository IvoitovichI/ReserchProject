using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace DungeonTrace.Editor
{
    public static class DungeonWalkthroughSceneGenerator
    {
        private const string ScenePath = "Assets/Scenes/DungeonWalkthrough.unity";
        private const string AssetsPath = "Assets/DungeonTrace/GeneratedDungeon/";

        [MenuItem("Dungeon Trace/Scenes/Create Dungeon Walkthrough")]
        public static void CreateScene()
        {
            var empty = AssetDatabase.LoadAssetAtPath<GameObject>(AssetsPath + "Room_Empty.prefab");
            var table = AssetDatabase.LoadAssetAtPath<GameObject>(AssetsPath + "Room_Table.prefab");
            var crypt = AssetDatabase.LoadAssetAtPath<GameObject>(AssetsPath + "Room_Crypt.prefab");
            if (empty == null || table == null || crypt == null) { Debug.LogError("Create Generation Placeholder Assets before creating the walkthrough scene."); return; }
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var start = Object.Instantiate(empty, new Vector3(0f, 0f, 0f), Quaternion.identity); start.name = "01_Empty_Start"; CreateRoomWalls(start.transform);
            var combat = Object.Instantiate(table, new Vector3(0f, 0f, 12.02f), Quaternion.identity); combat.name = "02_Table_Combat"; CreateRoomWalls(combat.transform);
            var boss = Object.Instantiate(crypt, new Vector3(0f, 0f, 24.04f), Quaternion.identity); boss.name = "03_Crypt_Boss"; CreateRoomWalls(boss.transform);
            CreateDoorFrame("Doorway_Empty_To_Table", new Vector3(0f, 1.5f, 6.01f));
            CreateDoorFrame("Doorway_Table_To_Crypt", new Vector3(0f, 1.5f, 18.03f));
            var lighting = new GameObject("Directional Light"); var light = lighting.AddComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.15f; lighting.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
            var marker = new GameObject("DungeonWalkthrough");
            EditorSceneManager.SaveScene(scene, ScenePath);
            Selection.activeObject = marker;
            Debug.Log("Created DungeonWalkthrough: Empty → Table → Crypt. Enter Play mode to walk between rooms.");
        }
        [MenuItem("Dungeon Trace/Scenes/Create Ten Room Dungeon Zone")]
        public static void CreateTenRoomDungeonZone()
        {
            var empty = AssetDatabase.LoadAssetAtPath<GameObject>(AssetsPath + "Room_Empty.prefab");
            var table = AssetDatabase.LoadAssetAtPath<GameObject>(AssetsPath + "Room_Table.prefab");
            var crypt = AssetDatabase.LoadAssetAtPath<GameObject>(AssetsPath + "Room_Crypt.prefab");
            if (empty == null || table == null || crypt == null)
            {
                Debug.LogError("Create Generation Placeholder Assets before creating the ten-room dungeon zone.");
                return;
            }

            const float roomSpacing = 12.02f;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var roomPrefabs = new[] { empty, table, crypt, empty, table, crypt, table, empty, table, crypt };
            var roomNames = new[]
            {
                "01_Start_Threshold", "02_Combat_Antechamber", "03_Combat_Crypt", "04_Choice_Crossroads",
                "05_Combat_Archive", "06_Elite_Ossuary", "07_Treasure_Vault", "08_Secret_Alcove",
                "09_Combat_Gauntlet", "10_Boss_Sanctum"
            };

            for (var index = 0; index < roomPrefabs.Length; index++)
            {
                var room = Object.Instantiate(roomPrefabs[index], new Vector3(0f, 0f, index * roomSpacing), Quaternion.identity);
                room.name = roomNames[index];
                CreateRoomWalls(room.transform);
                CreateRoomLight(room.transform, index);
                CreateEncounter(room, index);
                if (index > 0) CreateDoorFrame($"Doorway_{index:D2}_To_{index + 1:D2}", new Vector3(0f, 1.5f, index * roomSpacing - roomSpacing * .5f));
            }

            var lighting = new GameObject("Directional Light");
            var light = lighting.AddComponent<Light>();
            light.type = LightType.Directional; light.intensity = .8f; light.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
            var zone = new GameObject("TenRoomDungeonZone");
            var surface = zone.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.BuildNavMesh();
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/TenRoomDungeonZone.unity");
            Debug.Log("Created TenRoomDungeonZone: 10 connected rooms, from Start Threshold to Boss Sanctum. Enter Play mode to traverse the full route.");
        }

        private static void CreateRoomLight(Transform room, int index)
        {
            var lightRoot = new GameObject("RoomLight");
            lightRoot.transform.SetParent(room, false);
            lightRoot.transform.localPosition = new Vector3(0f, 2.7f, 0f);
            var light = lightRoot.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 10f;
            light.intensity = 2.2f;
            light.color = index == 9 ? new Color(.95f, .25f, .18f) : index % 2 == 0 ? new Color(.2f, .65f, 1f) : new Color(.3f, 1f, .65f);
        }

        private static void CreateEncounter(GameObject room, int index)
        {
            if (index != 1 && index != 2 && index != 4 && index != 5 && index != 8) return;
            var encounter = room.AddComponent<Enemies.RoomEncounterController>();
            var prefix = $"encounter-room-{index + 1:D2}";
            var spawns = index switch
            {
                1 => new[] { new Enemies.EncounterSpawn(prefix + "-pursuer", Enemies.EnemyArchetype.Pursuer, new Vector3(2.5f, 1f, 2f)) },
                2 => new[] { new Enemies.EncounterSpawn(prefix + "-spitter", Enemies.EnemyArchetype.Spitter, new Vector3(-2.5f, 1f, 2f)) },
                4 => new[] { new Enemies.EncounterSpawn(prefix + "-charger", Enemies.EnemyArchetype.Charger, new Vector3(2.5f, 1f, 2f)) },
                5 => new[] { new Enemies.EncounterSpawn(prefix + "-warder", Enemies.EnemyArchetype.Warder, new Vector3(-2.5f, 1f, 2f)), new Enemies.EncounterSpawn(prefix + "-pursuer", Enemies.EnemyArchetype.Pursuer, new Vector3(2.5f, 1f, 2f)) },
                _ => new[] { new Enemies.EncounterSpawn(prefix + "-spitter", Enemies.EnemyArchetype.Spitter, new Vector3(-2.5f, 1f, 2f)), new Enemies.EncounterSpawn(prefix + "-charger", Enemies.EnemyArchetype.Charger, new Vector3(2.5f, 1f, 2f)) }
            };
            encounter.Configure(prefix, spawns);
        }

        private static void CreateDoorFrame(string name, Vector3 center)
        {
            var root = new GameObject(name); CreateBlock(root.transform, "LeftPillar", center + new Vector3(-.9f, 0f, 0f), new Vector3(.18f, 3f, .28f)); CreateBlock(root.transform, "RightPillar", center + new Vector3(.9f, 0f, 0f), new Vector3(.18f, 3f, .28f)); CreateBlock(root.transform, "Lintel", center + new Vector3(0f, 1.4f, 0f), new Vector3(2f, .2f, .28f));
        }
        private static void CreateRoomWalls(Transform room)
        {
            const float half = 6f; const float height = 3f;
            var color = new Color(.14f, .24f, .34f);
            Wall(room, "North", new Vector3(-3.625f, height / 2f, half), new Vector3(4.75f, height, .35f), color); Wall(room, "North", new Vector3(3.625f, height / 2f, half), new Vector3(4.75f, height, .35f), color);
            Wall(room, "South", new Vector3(-3.625f, height / 2f, -half), new Vector3(4.75f, height, .35f), color); Wall(room, "South", new Vector3(3.625f, height / 2f, -half), new Vector3(4.75f, height, .35f), color);
            Wall(room, "East", new Vector3(half, height / 2f, -3.625f), new Vector3(.35f, height, 4.75f), color); Wall(room, "East", new Vector3(half, height / 2f, 3.625f), new Vector3(.35f, height, 4.75f), color);
            Wall(room, "West", new Vector3(-half, height / 2f, -3.625f), new Vector3(.35f, height, 4.75f), color); Wall(room, "West", new Vector3(-half, height / 2f, 3.625f), new Vector3(.35f, height, 4.75f), color);
        }
        private static void Wall(Transform parent, string side, Vector3 localPosition, Vector3 scale, Color color)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube); wall.name = $"Wall_{side}"; wall.transform.SetParent(parent, false); wall.transform.localPosition = localPosition; wall.transform.localScale = scale; wall.GetComponent<Renderer>().sharedMaterial.color = color;
        }
        private static void CreateBlock(Transform parent, string name, Vector3 position, Vector3 scale) { var block = GameObject.CreatePrimitive(PrimitiveType.Cube); block.name = name; block.transform.SetParent(parent); block.transform.position = position; block.transform.localScale = scale; block.GetComponent<Renderer>().sharedMaterial.color = new Color(.12f, .45f, .52f); }
    }
}
