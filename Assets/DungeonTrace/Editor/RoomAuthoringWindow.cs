using System.Collections.Generic;
using DungeonTrace.Rooms;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DungeonTrace.Editor
{
    public sealed class RoomAuthoringWindow : EditorWindow
    {
        private const string TemplateFolder = "Assets/DungeonTrace/GeneratedRooms";
        private ScrollView results;
        private Label summary;

        [MenuItem("Dungeon Trace/Rooms/Room Authoring")]
        public static void Open() => GetWindow<RoomAuthoringWindow>("Room Authoring");

        public void CreateGUI()
        {
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DungeonTrace/Editor/RoomAuthoringWindow.uxml");
            if (tree == null) { rootVisualElement.Add(new Label("Room authoring UI asset could not be loaded.")); return; }
            tree.CloneTree(rootVisualElement);
            rootVisualElement.Q<Button>("createTemplateButton").clicked += CreateTemplate;
            rootVisualElement.Q<Button>("scanButton").clicked += ScanSelected;
            rootVisualElement.Q<Button>("validateButton").clicked += ValidateSelected;
            summary = rootVisualElement.Q<Label>("summaryLabel");
            results = rootVisualElement.Q<ScrollView>("resultsScroll");
            ScanSelected();
        }

        private static RoomRoot SelectedRoot()
        {
            if (Selection.activeGameObject != null) return Selection.activeGameObject.GetComponentInParent<RoomRoot>();
            if (Selection.activeObject is GameObject prefab) return prefab.GetComponentInChildren<RoomRoot>(true);
            return null;
        }

        private void ScanSelected()
        {
            results?.Clear();
            var root = SelectedRoot();
            if (root == null) { SetSummary("Select a GameObject or prefab containing RoomRoot."); return; }
            SetSummary($"Found: {root.GetComponentsInChildren<DoorSocket>(true).Length} DoorSocket, {root.GetComponentsInChildren<PlayerEntry>(true).Length} PlayerEntry, {root.GetComponentsInChildren<EnemySpawnPoint>(true).Length} EnemySpawnPoint, {root.GetComponentsInChildren<RewardSpawnPoint>(true).Length} RewardSpawnPoint, {root.GetComponentsInChildren<TelemetryZone>(true).Length} TelemetryZone, {root.GetComponentsInChildren<RoomBounds>(true).Length} RoomBounds.");
        }

        private void ValidateSelected()
        {
            var root = SelectedRoot();
            if (root == null) { SetSummary("Select a GameObject or prefab containing RoomRoot."); return; }
            var issues = RoomValidator.Validate(root, LoadDefinitions());
            results.Clear();
            var errors = 0;
            foreach (var issue in issues)
            {
                if (issue.Severity == RoomValidationSeverity.Error) errors++;
                var context = issue.Context;
                var button = new Button(() => Focus(context)) { text = $"{issue.Severity}: {issue.Message}" };
                button.AddToClassList("issue-button");
                button.AddToClassList(issue.Severity == RoomValidationSeverity.Error ? "error-issue" : "warning-issue");
                results.Add(button);
            }
            SetSummary(issues.Count == 0 ? "Validation passed." : $"Validation found {errors} error(s) and {issues.Count - errors} warning(s). Click a result to select its object.");
        }

        private static void Focus(Object context)
        {
            if (context == null) return;
            Selection.activeObject = context;
            EditorGUIUtility.PingObject(context);
        }

        private static IEnumerable<RoomDefinition> LoadDefinitions()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:RoomDefinition"))
            {
                var definition = AssetDatabase.LoadAssetAtPath<RoomDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (definition != null) yield return definition;
            }
        }

        private void CreateTemplate()
        {
            EnsureFolder("Assets", "DungeonTrace");
            EnsureFolder("Assets/DungeonTrace", "GeneratedRooms");
            var definition = ScriptableObject.CreateInstance<RoomDefinition>();
            var definitionPath = AssetDatabase.GenerateUniqueAssetPath(TemplateFolder + "/RoomDefinition_Start.asset");
            AssetDatabase.CreateAsset(definition, definitionPath);

            var rootObject = new GameObject("Room_Start_Placeholder");
            var root = rootObject.AddComponent<RoomRoot>();
            rootObject.AddComponent<RoomBounds>().Configure(new Vector3(0f, 1.25f, 0f), new Vector3(12f, 3.5f, 12f));
            CreateBlock(rootObject.transform, "Floor", new Vector3(0f, -.25f, 0f), new Vector3(12f, .5f, 12f));
            var entry = new GameObject("PlayerEntry"); entry.transform.SetParent(rootObject.transform, false); entry.transform.localPosition = new Vector3(0f, 0f, -4f); entry.AddComponent<PlayerEntry>();
            var socket = new GameObject("DoorSocket_North"); socket.transform.SetParent(rootObject.transform, false); socket.transform.localPosition = new Vector3(0f, 0f, 6f); socket.transform.forward = Vector3.forward; socket.AddComponent<DoorSocket>().Configure("door-start-north", RoomExitDirection.North);
            var zone = new GameObject("TelemetryZone_Entry"); zone.transform.SetParent(rootObject.transform, false); zone.AddComponent<BoxCollider>().isTrigger = true; zone.AddComponent<TelemetryZone>().Configure("zone-start-entry");
            root.Configure(definition, "prototype-01");
            var prefabPath = AssetDatabase.GenerateUniqueAssetPath(TemplateFolder + "/Room_Start_Placeholder.prefab");
            var prefab = PrefabUtility.SaveAsPrefabAsset(rootObject, prefabPath);
            definition.ConfigureAuthoring("room-start-placeholder", "prototype-01", RoomType.Start, prefab, new[] { new RoomExitDefinition("door-start-north", RoomExitDirection.North) }, new[] { new TelemetryZoneDefinition("zone-start-entry", new string[0]) });
            EditorUtility.SetDirty(definition);
            AssetDatabase.SaveAssets();
            DestroyImmediate(rootObject);
            Selection.activeObject = prefab;
            SetSummary("Created placeholder RoomDefinition and prefab. Select the prefab and run validation.");
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child);
        }

        private static void CreateBlock(Transform parent, string objectName, Vector3 localPosition, Vector3 localScale)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = objectName;
            block.transform.SetParent(parent, false);
            block.transform.localPosition = localPosition;
            block.transform.localScale = localScale;
        }

        private void SetSummary(string value) { if (summary != null) summary.text = value; }
    }
}
