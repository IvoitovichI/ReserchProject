using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
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
