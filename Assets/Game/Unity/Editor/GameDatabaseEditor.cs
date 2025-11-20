using UnityEngine;
using UnityEditor;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Custom Editor cho GameDatabase với ReorderableList
    /// </summary>
    [CustomEditor(typeof(GameDatabase))]
    public class GameDatabaseEditor : ScriptableObjectListEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GameDatabase database = (GameDatabase)target;

            EditorGUILayout.LabelField("Game Database", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Draw default inspector (sẽ tự động có ReorderableList từ base class)
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Validate Database"))
            {
                database.ValidateDatabase();
            }

            if (GUILayout.Button("Reinitialize Cache"))
            {
                database.Initialize();
                EditorUtility.SetDirty(database);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

