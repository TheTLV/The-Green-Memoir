using System.Collections.Generic;
using UnityEngine;
using TheGreenMemoir.Unity.UI;

namespace TheGreenMemoir.Unity.Data.Settings
{
    /// <summary>
    /// Sub Setting Menu SO - Menu con có thể nested vào menu cha
    /// Có thể có sub-menus riêng của nó (nested menus)
    /// </summary>
    [CreateAssetMenu(fileName = "SubSettingMenu", menuName = "Game/Settings/Sub Menu", order = 101)]
    public class SubSettingMenuSO : BaseSettingMenuSO
    {
        [Header("Sub Menu Settings")]
        [Tooltip("Loại sub-menu này (để controller biết cách render)")]
        public SubMenuType subMenuType = SubMenuType.Custom;

        [Tooltip("Prefab UI tùy chỉnh cho menu này (nếu subMenuType = Custom)")]
        public GameObject customUIPrefab;

        [Tooltip("Dữ liệu tùy chỉnh cho menu này (có thể là bất kỳ SO nào)")]
        public ScriptableObject customData;

        [Header("Nested Sub Menus")]
        [Tooltip("Sub-menus của sub-menu này (nested menus)")]
        public List<SubSettingMenuSO> nestedSubMenus = new List<SubSettingMenuSO>();

        /// <summary>
        /// Loại sub-menu
        /// </summary>
        public enum SubMenuType
        {
            Custom,              // Menu tùy chỉnh với prefab riêng
            VolumeSettings,      // Menu volume settings
            KeyConfiguration,   // Menu key configuration
            DisplaySettings,    // Menu display settings
            AudioSettings,      // Menu audio settings
            GameplaySettings,   // Menu gameplay settings
            LanguageSettings,   // Menu language settings
            EventSceneSettings  // Menu event scene settings
        }

        public override GameObject CreateUI(Transform parent, DynamicSettingsController controller)
        {
            // Nếu có custom prefab, dùng nó
            if (customUIPrefab != null)
            {
                var instance = Instantiate(customUIPrefab, parent);
                return instance;
            }

            // Nếu không, controller sẽ tự động tạo UI dựa trên subMenuType
            return controller.CreateSubMenuUI(this, parent);
        }

        /// <summary>
        /// Lấy tất cả nested sub-menus
        /// </summary>
        public List<SubSettingMenuSO> GetNestedSubMenus()
        {
            return new List<SubSettingMenuSO>(nestedSubMenus);
        }

        /// <summary>
        /// Thêm nested sub-menu
        /// </summary>
        public void AddNestedSubMenu(SubSettingMenuSO nestedMenu)
        {
            if (nestedMenu != null && !nestedSubMenus.Contains(nestedMenu))
            {
                nestedSubMenus.Add(nestedMenu);
            }
        }

        /// <summary>
        /// Xóa nested sub-menu
        /// </summary>
        public void RemoveNestedSubMenu(SubSettingMenuSO nestedMenu)
        {
            if (nestedSubMenus.Remove(nestedMenu))
            {
                // Removed successfully
            }
        }
    }
}

