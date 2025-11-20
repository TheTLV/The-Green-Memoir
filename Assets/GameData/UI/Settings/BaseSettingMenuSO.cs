using System.Collections.Generic;
using UnityEngine;
using TheGreenMemoir.Unity.UI;

namespace TheGreenMemoir.Unity.Data.Settings
{
    /// <summary>
    /// Base Setting Menu SO - Base class cho tất cả các menu settings
    /// Có thể mở rộng bằng cách thêm sub-menus
    /// </summary>
    public abstract class BaseSettingMenuSO : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("ID duy nhất của menu này")]
        public string menuId = "new_menu";
        
        [Tooltip("Tên hiển thị của menu")]
        public string menuName = "New Menu";
        
        [Tooltip("Icon của menu (optional)")]
        public Sprite menuIcon;
        
        [TextArea(2, 4)]
        [Tooltip("Mô tả menu (optional)")]
        public string description = "";

        [Header("Sub Menus")]
        [Tooltip("Danh sách sub-menus có thể thêm vào menu này")]
        public List<SubSettingMenuSO> subMenus = new List<SubSettingMenuSO>();

        [Header("Settings")]
        [Tooltip("Menu này có enabled không")]
        public bool isEnabled = true;
        
        [Tooltip("Menu này có visible không")]
        public bool isVisible = true;
        
        [Tooltip("Thứ tự hiển thị (số nhỏ hơn hiển thị trước)")]
        public int displayOrder = 0;

        /// <summary>
        /// Lấy tất cả sub-menus đã được thêm vào
        /// </summary>
        public List<SubSettingMenuSO> GetSubMenus()
        {
            return new List<SubSettingMenuSO>(subMenus);
        }

        /// <summary>
        /// Thêm sub-menu vào menu này
        /// </summary>
        public void AddSubMenu(SubSettingMenuSO subMenu)
        {
            if (subMenu != null && !subMenus.Contains(subMenu))
            {
                subMenus.Add(subMenu);
            }
        }

        /// <summary>
        /// Xóa sub-menu khỏi menu này
        /// </summary>
        public void RemoveSubMenu(SubSettingMenuSO subMenu)
        {
            if (subMenu != null)
            {
                subMenus.Remove(subMenu);
            }
        }

        /// <summary>
        /// Abstract method - Mỗi loại menu phải implement cách render UI của nó
        /// </summary>
        public abstract GameObject CreateUI(Transform parent, DynamicSettingsController controller);
    }
}

