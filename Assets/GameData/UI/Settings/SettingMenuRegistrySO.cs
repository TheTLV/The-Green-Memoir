using System.Collections.Generic;
using UnityEngine;

namespace TheGreenMemoir.Unity.Data.Settings
{
    /// <summary>
    /// Setting Menu Registry SO - Quản lý tất cả các menu settings
    /// Đăng ký các menu để controller tự động tìm và hiển thị
    /// </summary>
    [CreateAssetMenu(fileName = "SettingMenuRegistry", menuName = "Game/Settings/Registry", order = 100)]
    public class SettingMenuRegistrySO : ScriptableObject
    {
        [Header("Main Settings Menu")]
        [Tooltip("Menu settings chính (có thể có sub-menus)")]
        public BaseSettingMenuSO mainSettingsMenu;

        [Header("Pause Menu Settings")]
        [Tooltip("Các menu sẽ hiển thị trong pause menu")]
        public List<SubSettingMenuSO> pauseMenuItems = new List<SubSettingMenuSO>();

        [Header("All Registered Menus")]
        [Tooltip("Tất cả các menu đã đăng ký (tự động collect từ mainSettingsMenu và pauseMenuItems)")]
        public List<BaseSettingMenuSO> allMenus = new List<BaseSettingMenuSO>();

        [Header("Settings")]
        [Tooltip("Tự động collect menus khi validate")]
        public bool autoCollectMenus = true;

        private void OnValidate()
        {
            if (autoCollectMenus)
            {
                CollectAllMenus();
            }
        }

        /// <summary>
        /// Thu thập tất cả menus từ mainSettingsMenu và pauseMenuItems
        /// </summary>
        public void CollectAllMenus()
        {
            allMenus.Clear();

            // Thêm main settings menu
            if (mainSettingsMenu != null && !allMenus.Contains(mainSettingsMenu))
            {
                allMenus.Add(mainSettingsMenu);
                CollectSubMenus(mainSettingsMenu);
            }

            // Thêm pause menu items
            foreach (var item in pauseMenuItems)
            {
                if (item != null && !allMenus.Contains(item))
                {
                    allMenus.Add(item);
                    CollectSubMenus(item);
                }
            }
        }

        /// <summary>
        /// Đệ quy thu thập tất cả sub-menus
        /// </summary>
        private void CollectSubMenus(BaseSettingMenuSO menu)
        {
            if (menu is SubSettingMenuSO subMenu)
            {
                // Thu thập nested sub-menus
                foreach (var nested in subMenu.nestedSubMenus)
                {
                    if (nested != null && !allMenus.Contains(nested))
                    {
                        allMenus.Add(nested);
                        CollectSubMenus(nested);
                    }
                }
            }

            // Thu thập sub-menus
            foreach (var sub in menu.subMenus)
            {
                if (sub != null && !allMenus.Contains(sub))
                {
                    allMenus.Add(sub);
                    CollectSubMenus(sub);
                }
            }
        }

        /// <summary>
        /// Lấy tất cả menus cho pause menu
        /// </summary>
        public List<SubSettingMenuSO> GetPauseMenuItems()
        {
            return new List<SubSettingMenuSO>(pauseMenuItems);
        }

        /// <summary>
        /// Thêm menu vào pause menu
        /// </summary>
        public void AddPauseMenuItem(SubSettingMenuSO menu)
        {
            if (menu != null && !pauseMenuItems.Contains(menu))
            {
                pauseMenuItems.Add(menu);
                if (autoCollectMenus)
                {
                    CollectAllMenus();
                }
            }
        }

        /// <summary>
        /// Xóa menu khỏi pause menu
        /// </summary>
        public void RemovePauseMenuItem(SubSettingMenuSO menu)
        {
            if (pauseMenuItems.Remove(menu) && autoCollectMenus)
            {
                CollectAllMenus();
            }
        }

        /// <summary>
        /// Tìm menu theo ID
        /// </summary>
        public BaseSettingMenuSO FindMenuById(string menuId)
        {
            foreach (var menu in allMenus)
            {
                if (menu != null && menu.menuId == menuId)
                {
                    return menu;
                }
            }
            return null;
        }

        /// <summary>
        /// Lấy tất cả menus đã enabled và visible
        /// </summary>
        public List<BaseSettingMenuSO> GetEnabledMenus()
        {
            var result = new List<BaseSettingMenuSO>();
            foreach (var menu in allMenus)
            {
                if (menu != null && menu.isEnabled && menu.isVisible)
                {
                    result.Add(menu);
                }
            }
            // Sắp xếp theo displayOrder
            result.Sort((a, b) => a.displayOrder.CompareTo(b.displayOrder));
            return result;
        }
    }
}

