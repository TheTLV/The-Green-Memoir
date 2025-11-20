using UnityEngine;
using UnityEditor;
using TheGreenMemoir.Unity.Data.Settings;

namespace TheGreenMemoir.Unity.Editor
{
    /// <summary>
    /// Editor script để dễ dàng thêm menu vào settings
    /// </summary>
    [CustomEditor(typeof(SettingMenuRegistrySO))]
    public class SettingMenuRegistryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var registry = (SettingMenuRegistrySO)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

            // Button để thêm sub-menu vào pause menu
            if (GUILayout.Button("Add New Sub Menu to Pause Menu"))
            {
                var newMenu = CreateInstance<SubSettingMenuSO>();
                newMenu.menuId = $"submenu_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
                newMenu.menuName = "New Sub Menu";
                
                var path = EditorUtility.SaveFilePanelInProject(
                    "Save Sub Menu",
                    "NewSubMenu",
                    "asset",
                    "Save the new sub menu"
                );

                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(newMenu, path);
                    AssetDatabase.SaveAssets();
                    registry.AddPauseMenuItem(newMenu);
                    EditorUtility.SetDirty(registry);
                }
            }

            // Button để collect all menus
            if (GUILayout.Button("Collect All Menus"))
            {
                registry.CollectAllMenus();
                EditorUtility.SetDirty(registry);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Menu Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total Menus: {registry.allMenus.Count}");
            EditorGUILayout.LabelField($"Pause Menu Items: {registry.pauseMenuItems.Count}");
        }
    }

    [CustomEditor(typeof(SubSettingMenuSO))]
    public class SubSettingMenuEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var subMenu = (SubSettingMenuSO)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

            // Button để thêm nested sub-menu
            if (GUILayout.Button("+ Add Nested Sub Menu"))
            {
                var newNestedMenu = CreateInstance<SubSettingMenuSO>();
                newNestedMenu.menuId = $"nested_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
                newNestedMenu.menuName = "New Nested Menu";
                
                var path = EditorUtility.SaveFilePanelInProject(
                    "Save Nested Sub Menu",
                    "NewNestedMenu",
                    "asset",
                    "Save the new nested sub menu"
                );

                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(newNestedMenu, path);
                    AssetDatabase.SaveAssets();
                    subMenu.AddNestedSubMenu(newNestedMenu);
                    EditorUtility.SetDirty(subMenu);
                }
            }

            // Button để thêm sub-menu
            if (GUILayout.Button("+ Add Sub Menu"))
            {
                var newSubMenu = CreateInstance<SubSettingMenuSO>();
                newSubMenu.menuId = $"submenu_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
                newSubMenu.menuName = "New Sub Menu";
                
                var path = EditorUtility.SaveFilePanelInProject(
                    "Save Sub Menu",
                    "NewSubMenu",
                    "asset",
                    "Save the new sub menu"
                );

                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(newSubMenu, path);
                    AssetDatabase.SaveAssets();
                    subMenu.AddSubMenu(newSubMenu);
                    EditorUtility.SetDirty(subMenu);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sub Menu Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Sub Menus: {subMenu.subMenus.Count}");
            EditorGUILayout.LabelField($"Nested Sub Menus: {subMenu.nestedSubMenus.Count}");
        }
    }
}

