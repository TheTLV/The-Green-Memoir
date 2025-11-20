using UnityEngine;
using UnityEditor;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Event Editor Window - Editor để tạo events
    /// </summary>
    public class EventEditorWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        [MenuItem("Window/Game Designer/Event Editor")]
        public static void ShowWindow()
        {
            GetWindow<EventEditorWindow>("Event Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Event Editor", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Event Editor - Create game events and triggers", MessageType.Info);

            if (GUILayout.Button("Create New Event"))
            {
                CreateNewEvent();
            }

            // TODO: Visual event editor (node-based)
        }

        private void CreateNewEvent()
        {
            // TODO: Create EventSO
            Debug.Log("Create New Event");
        }
    }
}

