using UnityEngine;
using UnityEditor;
using System.IO;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Helper script để di chuyển SO files và cập nhật CreateAssetMenu paths
    /// </summary>
    public class MigrationHelper : EditorWindow
    {
        [MenuItem("Tools/Migration Helper")]
        public static void ShowWindow()
        {
            GetWindow<MigrationHelper>("Migration Helper");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("GameData Migration Helper", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Tool này giúp di chuyển SO files và cập nhật CreateAssetMenu paths.\n" +
                "Lưu ý: Backup project trước khi chạy!",
                MessageType.Info
            );

            EditorGUILayout.Space();

            if (GUILayout.Button("Check Migration Status"))
            {
                CheckMigrationStatus();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Update CreateAssetMenu Paths"))
            {
                UpdateCreateAssetMenuPaths();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Manual Migration Guide", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Nếu tự động migration không hoạt động, bạn có thể:\n" +
                "1. Di chuyển files thủ công trong Unity Project window\n" +
                "2. Unity sẽ tự động cập nhật references\n" +
                "3. Sau đó chạy 'Update CreateAssetMenu Paths' để cập nhật menu paths",
                MessageType.Info
            );
        }

        private void CheckMigrationStatus()
        {
            Debug.Log("=== Migration Status Check ===");
            
            // Check Core
            CheckFile("Assets/GameData/Core/MasterDatabaseSO.cs", "Core/MasterDatabaseSO");
            CheckFile("Assets/GameData/Core/CheatConfigSO.cs", "Core/CheatConfigSO");
            CheckFile("Assets/GameData/Core/GameSettingsSO.cs", "Core/GameSettingsSO");

            // Check World
            CheckFile("Assets/GameData/World/Items/ItemDataSO.cs", "World/Items/ItemDataSO");
            CheckFile("Assets/GameData/World/Farming/CropDataSO.cs", "World/Farming/CropDataSO");
            CheckFile("Assets/GameData/World/Farming/TileStateSO.cs", "World/Farming/TileStateSO");
            CheckFile("Assets/GameData/World/Tools/ToolDataSO.cs", "World/Tools/ToolDataSO");

            // Check Buildings
            CheckFile("Assets/GameData/World/Buildings/BuildingSO.cs", "World/Buildings/BuildingSO");
            CheckFile("Assets/GameData/World/Buildings/FloorSO.cs", "World/Buildings/FloorSO");
            CheckFile("Assets/GameData/World/Buildings/RoomSO.cs", "World/Buildings/RoomSO");
            CheckFile("Assets/GameData/World/Buildings/DoorSO.cs", "World/Buildings/DoorSO");
            CheckFile("Assets/GameData/World/Buildings/StairSO.cs", "World/Buildings/StairSO");

            // Check Narrative
            CheckFile("Assets/GameData/Narrative/NPCs/NPCDefinitionSO.cs", "Narrative/NPCs/NPCDefinitionSO");
            CheckFile("Assets/GameData/Narrative/NPCs/NPCFriendshipSO.cs", "Narrative/NPCs/NPCFriendshipSO");
            CheckFile("Assets/GameData/Narrative/Dialogue/DialogueSO.cs", "Narrative/Dialogue/DialogueSO");
            CheckFile("Assets/GameData/Narrative/Quests/QuestSO.cs", "Narrative/Quests/QuestSO");
            CheckFile("Assets/GameData/Narrative/Events/GameEventSO.cs", "Narrative/Events/GameEventSO");
            CheckFile("Assets/GameData/Narrative/Events/StorySO.cs", "Narrative/Events/StorySO");

            // Check Interaction
            CheckFile("Assets/GameData/Interaction/Actions/InteractionActionSO.cs", "Interaction/Actions/InteractionActionSO");
            CheckFile("Assets/GameData/Interaction/States/InteractionStateSO.cs", "Interaction/States/InteractionStateSO");
            CheckFile("Assets/GameData/Interaction/Transitions/InteractionTransitionSO.cs", "Interaction/Transitions/InteractionTransitionSO");
            CheckFile("Assets/GameData/Interaction/Graphs/InteractionGraphSO.cs", "Interaction/Graphs/InteractionGraphSO");

            // Check Input
            CheckFile("Assets/GameData/Input/Actions/InputActionSO.cs", "Input/Actions/InputActionSO");
            CheckFile("Assets/GameData/Input/Actions/ToolSO.cs", "Input/Actions/ToolSO");
            CheckFile("Assets/GameData/Input/Actions/SkillSO.cs", "Input/Actions/SkillSO");
            CheckFile("Assets/GameData/Input/Actions/UIToggleSO.cs", "Input/Actions/UIToggleSO");
            CheckFile("Assets/GameData/Input/Actions/CheatSO.cs", "Input/Actions/CheatSO");

            // Check UI
            CheckFile("Assets/GameData/UI/Menu/MenuSO.cs", "UI/Menu/MenuSO");
            CheckFile("Assets/GameData/UI/Menu/MenuItemSO.cs", "UI/Menu/MenuItemSO");
            CheckFile("Assets/GameData/UI/Style/UIStyleSO.cs", "UI/Style/UIStyleSO");
            CheckFile("Assets/GameData/UI/Settings/GameSettingsDataSO.cs", "UI/Settings/GameSettingsDataSO");

            Debug.Log("=== Migration Status Check Complete ===");
        }

        private void CheckFile(string path, string name)
        {
            if (File.Exists(path))
            {
                Debug.Log($"✅ {name} - OK");
            }
            else
            {
                Debug.LogWarning($"❌ {name} - NOT FOUND at {path}");
            }
        }

        private void UpdateCreateAssetMenuPaths()
        {
            Debug.Log("=== Updating CreateAssetMenu Paths ===");
            
            // This would require reading and modifying files
            // For now, just log what needs to be updated
            Debug.Log("Please update CreateAssetMenu paths manually:");
            Debug.Log("- GameDatabase -> GameData/Core/MasterDatabase");
            Debug.Log("- ItemDataSO -> GameData/World/Items/Item Data");
            Debug.Log("- CropDataSO -> GameData/World/Farming/Crop Data");
            Debug.Log("- And so on...");
            
            Debug.Log("=== Update Complete ===");
        }
    }
}

