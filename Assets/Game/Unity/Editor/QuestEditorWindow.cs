using UnityEngine;
using UnityEditor;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Quest Editor Window - Editor để tạo quests
    /// </summary>
    public class QuestEditorWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        [MenuItem("Window/Game Designer/Quest Editor")]
        public static void ShowWindow()
        {
            GetWindow<QuestEditorWindow>("Quest Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Quest Editor", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Quest Editor - Create and manage quests", MessageType.Info);

            if (GUILayout.Button("Create New Quest"))
            {
                CreateNewQuest();
            }

            // TODO: List quests, edit quests, etc.
        }

        private void CreateNewQuest()
        {
            // TODO: Create QuestSO
            Debug.Log("Create New Quest");
        }
    }
}

