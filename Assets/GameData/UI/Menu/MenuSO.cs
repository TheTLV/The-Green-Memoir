using System.Collections.Generic;
using UnityEngine;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Menu SO - Định nghĩa cấu trúc menu (Pause Menu, Item Menu, etc.)
    /// Tạo trong Unity Editor: Right Click → Create → Game → Menu → Menu
    /// </summary>
    [CreateAssetMenu(fileName = "Menu", menuName = "Game/Menu/Menu", order = 61)]
    public class MenuSO : ScriptableObject
    {
        [Header("Menu Info")]
        [Tooltip("Tiêu đề của menu (ví dụ: 'Pause Menu', 'Inventory')")]
        public string menuTitle = "Menu";

        [Tooltip("Icon của menu (optional)")]
        public Sprite menuIcon;

        [Header("Menu Items")]
        [Tooltip("Danh sách các item trong menu")]
        public List<MenuItemSO> menuItems = new List<MenuItemSO>();

        [Header("Navigation")]
        [Tooltip("Menu cha (để quay lại menu trước, optional)")]
        public MenuSO parentMenu;

        [Tooltip("Có thể quay lại menu cha bằng phím Back/Escape không")]
        public bool allowBackNavigation = true;

        [Header("Settings")]
        [Tooltip("Menu này có thể đóng bằng Escape không")]
        public bool canCloseWithEscape = true;
    }
}

