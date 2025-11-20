using UnityEngine;
using UnityEditor;
using TheGreenMemoir.Unity.Data.Settings;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Custom Editor cho SettingMenuRegistrySO với ReorderableList
    /// </summary>
    [CustomEditor(typeof(SettingMenuRegistrySO))]
    public class SettingMenuRegistrySOEditor : ScriptableObjectListEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SettingMenuRegistrySO registry = (SettingMenuRegistrySO)target;

            // Draw default inspector với ReorderableList
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Collect All Menus"))
            {
                registry.CollectAllMenus();
                EditorUtility.SetDirty(registry);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

