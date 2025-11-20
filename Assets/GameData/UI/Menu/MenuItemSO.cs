using UnityEngine;
using UnityEngine.Events;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Menu Item SO - Định nghĩa một item trong menu (Item, Skill, Save, etc.)
    /// </summary>
    public enum MenuItemType
    {
        Action,      // Thực hiện action (Save, Quit, etc.)
        SubMenu,     // Mở menu con (Item menu, Skill menu, etc.)
        SceneLoad    // Load scene (Title Screen, etc.)
    }

    /// <summary>
    /// Menu Item ScriptableObject
    /// Tạo trong Unity Editor: Right Click → Create → Game → Menu → Menu Item
    /// </summary>
    [CreateAssetMenu(fileName = "MenuItem", menuName = "Game/Menu/Menu Item", order = 60)]
    public class MenuItemSO : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("Tên hiển thị của item menu (ví dụ: 'Item', 'Skill', 'Save')")]
        public string displayName = "New Item";

        [Tooltip("Icon của item menu (optional)")]
        public Sprite icon;

        [Header("Menu Type")]
        [Tooltip("Loại item menu")]
        public MenuItemType menuType = MenuItemType.Action;

        [Header("SubMenu (nếu menuType = SubMenu)")]
        [Tooltip("Menu con sẽ mở khi click vào item này (chỉ dùng nếu menuType = SubMenu)")]
        public MenuSO subMenu;

        [Header("Scene Load (nếu menuType = SceneLoad)")]
        [Tooltip("Tên scene sẽ load (chỉ dùng nếu menuType = SceneLoad)")]
        public string sceneToLoad = "";

        [Header("Action (nếu menuType = Action)")]
        [Tooltip("Actions sẽ được gọi khi click vào item này (chỉ dùng nếu menuType = Action)")]
        public UnityEvent onSelectAction;

        [Header("Settings")]
        [Tooltip("Item này có enabled không (có thể ẩn item nếu false)")]
        public bool isEnabled = true;

        [Tooltip("Item này có visible không (có thể ẩn item nếu false)")]
        public bool isVisible = true;

        [Tooltip("Item này có locked không (có thể disable item nếu true)")]
        public bool isLocked = false;

        [Tooltip("Message hiển thị nếu item bị locked (ví dụ: 'Coming soon!')")]
        public string lockedMessage = "Locked";
    }
}

