using UnityEngine;
using UnityEditor;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Game Designer Window - Cửa sổ editor để thiết kế game (giống RPG Maker)
    /// Mở: Window → Game Designer
    /// </summary>
    public class GameDesignerWindow : EditorWindow
    {
        private int _selectedTab = 0;
        private string[] _tabs = { "Database", "Maps", "Quests", "Events", "Dialogue", "Settings" };

        private Vector2 _scrollPosition;

        [MenuItem("Window/Game Designer")]
        public static void ShowWindow()
        {
            GetWindow<GameDesignerWindow>("Game Designer");
        }

        private void OnGUI()
        {
            // Tab selection
            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabs);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            switch (_selectedTab)
            {
                case 0:
                    DrawDatabaseTab();
                    break;
                case 1:
                    DrawMapsTab();
                    break;
                case 2:
                    DrawQuestsTab();
                    break;
                case 3:
                    DrawEventsTab();
                    break;
                case 4:
                    DrawDialogueTab();
                    break;
                case 5:
                    DrawSettingsTab();
                    break;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawDatabaseTab()
        {
            GUILayout.Label("Database Editor", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Tìm GameDatabase
            GameDatabase database = FindGameDatabase();
            if (database == null)
            {
                EditorGUILayout.HelpBox("GameDatabase not found! Create one first.", MessageType.Warning);
                if (GUILayout.Button("Create GameDatabase"))
                {
                    CreateGameDatabase();
                }
                return;
            }

            EditorGUILayout.ObjectField("Game Database", database, typeof(GameDatabase), false);

            GUILayout.Space(10);
            GUILayout.Label("Items", EditorStyles.boldLabel);
            if (GUILayout.Button("Create New Item"))
            {
                CreateNewItem();
            }

            GUILayout.Space(5);
            GUILayout.Label("Crops", EditorStyles.boldLabel);
            if (GUILayout.Button("Create New Crop"))
            {
                CreateNewCrop();
            }

            GUILayout.Space(5);
            GUILayout.Label("Tools", EditorStyles.boldLabel);
            if (GUILayout.Button("Create New Tool"))
            {
                CreateNewTool();
            }

            GUILayout.Space(5);
            GUILayout.Label("NPCs", EditorStyles.boldLabel);
            if (GUILayout.Button("Create New NPC"))
            {
                CreateNewNPC();
            }
        }

        private void DrawMapsTab()
        {
            GUILayout.Label("Map Editor", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Map Editor - Design your game maps visually", MessageType.Info);

            if (GUILayout.Button("Open Map Editor"))
            {
                MapEditorWindow.ShowWindow();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Create New Map"))
            {
                CreateNewMap();
            }
        }

        private void DrawQuestsTab()
        {
            GUILayout.Label("Quest Editor", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Quest Editor - Create quests and objectives", MessageType.Info);

            if (GUILayout.Button("Open Quest Editor"))
            {
                QuestEditorWindow.ShowWindow();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Create New Quest"))
            {
                CreateNewQuest();
            }
        }

        private void DrawEventsTab()
        {
            GUILayout.Label("Event Editor", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Event Editor - Create game events and triggers", MessageType.Info);

            if (GUILayout.Button("Open Event Editor"))
            {
                EventEditorWindow.ShowWindow();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Create New Event"))
            {
                CreateNewEvent();
            }
        }

        private void DrawDialogueTab()
        {
            GUILayout.Label("Dialogue Editor", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Dialogue Editor - Create dialogues and conversations", MessageType.Info);

            if (GUILayout.Button("Open Dialogue Editor"))
            {
                DialogueEditorWindow.ShowWindow();
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Create New Dialogue"))
            {
                CreateNewDialogue();
            }
        }

        private void DrawSettingsTab()
        {
            GUILayout.Label("Game Settings", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Configure game settings", MessageType.Info);

            // Online/Offline mode
            bool onlineMode = EditorPrefs.GetBool("GameDesigner_OnlineMode", false);
            bool newOnlineMode = EditorGUILayout.Toggle("Online Mode", onlineMode);
            if (newOnlineMode != onlineMode)
            {
                EditorPrefs.SetBool("GameDesigner_OnlineMode", newOnlineMode);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Export Game Data"))
            {
                ExportGameData();
            }

            if (GUILayout.Button("Import Game Data"))
            {
                ImportGameData();
            }
        }

        // Helper methods
        private GameDatabase FindGameDatabase()
        {
            string[] guids = AssetDatabase.FindAssets("t:GameDatabase");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<GameDatabase>(path);
            }
            return null;
        }

        private void CreateGameDatabase()
        {
            GameDatabase db = ScriptableObject.CreateInstance<GameDatabase>();
            string path = "Assets/Game/Data/GameDatabase.asset";
            AssetDatabase.CreateAsset(db, path);
            AssetDatabase.SaveAssets();
            Debug.Log($"Created GameDatabase at {path}");
        }

        private void CreateNewItem()
        {
            // TODO: Implement
            Debug.Log("Create New Item");
        }

        private void CreateNewCrop()
        {
            // TODO: Implement
            Debug.Log("Create New Crop");
        }

        private void CreateNewTool()
        {
            // TODO: Implement
            Debug.Log("Create New Tool");
        }

        private void CreateNewNPC()
        {
            // TODO: Implement
            Debug.Log("Create New NPC");
        }

        private void CreateNewMap()
        {
            // TODO: Implement
            Debug.Log("Create New Map");
        }

        private void CreateNewQuest()
        {
            // TODO: Implement
            Debug.Log("Create New Quest");
        }

        private void CreateNewEvent()
        {
            // TODO: Implement
            Debug.Log("Create New Event");
        }

        private void CreateNewDialogue()
        {
            // TODO: Implement
            Debug.Log("Create New Dialogue");
        }

        private void ExportGameData()
        {
            // TODO: Implement
            Debug.Log("Export Game Data");
        }

        private void ImportGameData()
        {
            // TODO: Implement
            Debug.Log("Import Game Data");
        }
    }
}

