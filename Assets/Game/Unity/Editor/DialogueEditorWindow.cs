using UnityEngine;
using UnityEditor;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Dialogue Editor Window - Editor để tạo dialogues
    /// </summary>
    public class DialogueEditorWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        [MenuItem("Window/Game Designer/Dialogue Editor")]
        public static void ShowWindow()
        {
            GetWindow<DialogueEditorWindow>("Dialogue Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Dialogue Editor", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Dialogue Editor - Create dialogues and conversations", MessageType.Info);

            if (GUILayout.Button("Create New Dialogue"))
            {
                CreateNewDialogue();
            }

            // TODO: Visual dialogue editor (node-based, giống RPG Maker)
        }

        private void CreateNewDialogue()
        {
            // TODO: Create DialogueSO
            Debug.Log("Create New Dialogue");
        }
    }
}

