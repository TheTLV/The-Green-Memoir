using UnityEngine;
using UnityEditor;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Editor tools để quản lý GameDatabase dễ dàng hơn
    /// </summary>
    public class DatabaseEditor : EditorWindow
    {
        private GameDatabase database;
        private Vector2 scrollPosition;

        [MenuItem("Game/Database Editor")]
        public static void ShowWindow()
        {
            GetWindow<DatabaseEditor>("Database Editor");
        }

        private void OnEnable()
        {
            // Tự động tìm GameDatabase
            var guids = AssetDatabase.FindAssets("t:GameDatabase");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                database = AssetDatabase.LoadAssetAtPath<GameDatabase>(path);
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Game Database Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Database reference
            database = (GameDatabase)EditorGUILayout.ObjectField(
                "Game Database", 
                database, 
                typeof(GameDatabase), 
                false
            );

            if (database == null)
            {
                EditorGUILayout.HelpBox("Please assign a GameDatabase asset", MessageType.Warning);
                
                if (GUILayout.Button("Create New GameDatabase"))
                {
                    CreateNewDatabase();
                }
                return;
            }

            EditorGUILayout.Space();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Items section
            EditorGUILayout.LabelField($"Items ({database.items.Count})", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Item"))
            {
                CreateNewItem();
            }
            if (GUILayout.Button("Validate"))
            {
                database.ValidateDatabase();
            }
            EditorGUILayout.EndHorizontal();

            // Crops section
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Crops ({database.crops.Count})", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Crop"))
            {
                CreateNewCrop();
            }
            EditorGUILayout.EndHorizontal();

            // Tools section
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Tools ({database.tools.Count})", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Tool"))
            {
                CreateNewTool();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();

            EditorUtility.SetDirty(database);
        }

        private void CreateNewDatabase()
        {
            var path = EditorUtility.SaveFilePanelInProject(
                "Create GameDatabase",
                "GameDatabase",
                "asset",
                "Create a new GameDatabase asset"
            );

            if (!string.IsNullOrEmpty(path))
            {
                var newDatabase = CreateInstance<GameDatabase>();
                AssetDatabase.CreateAsset(newDatabase, path);
                AssetDatabase.SaveAssets();
                database = newDatabase;
            }
        }

        private void CreateNewItem()
        {
            var item = CreateInstance<ItemDataSO>();
            item.itemId = "new_item";
            item.itemName = "New Item";

            var path = EditorUtility.SaveFilePanelInProject(
                "Create Item Data",
                "NewItem",
                "asset",
                "Create a new ItemDataSO"
            );

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(item, path);
                AssetDatabase.SaveAssets();
                
                // Thêm vào database
                database.items.Add(item);
                EditorUtility.SetDirty(database);
                Selection.activeObject = item;
            }
        }

        private void CreateNewCrop()
        {
            var crop = CreateInstance<CropDataSO>();
            crop.cropId = "new_crop";
            crop.cropName = "New Crop";

            var path = EditorUtility.SaveFilePanelInProject(
                "Create Crop Data",
                "NewCrop",
                "asset",
                "Create a new CropDataSO"
            );

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(crop, path);
                AssetDatabase.SaveAssets();
                
                // Thêm vào database
                database.crops.Add(crop);
                EditorUtility.SetDirty(database);
                Selection.activeObject = crop;
            }
        }

        private void CreateNewTool()
        {
            var tool = CreateInstance<ToolDataSO>();
            tool.toolId = "new_tool";
            tool.toolName = "New Tool";

            var path = EditorUtility.SaveFilePanelInProject(
                "Create Tool Data",
                "NewTool",
                "asset",
                "Create a new ToolDataSO"
            );

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(tool, path);
                AssetDatabase.SaveAssets();
                
                // Thêm vào database
                database.tools.Add(tool);
                EditorUtility.SetDirty(database);
                Selection.activeObject = tool;
            }
        }
    }
}

