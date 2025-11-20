using UnityEngine;
using TheGreenMemoir.Unity.UI;

namespace TheGreenMemoir.Unity.Data.Settings
{
    /// <summary>
    /// Main Settings Menu SO - Menu settings chính
    /// Có thể thêm sub-menus vào đây
    /// </summary>
    [CreateAssetMenu(fileName = "MainSettingsMenu", menuName = "Game/Settings/Main Menu", order = 100)]
    public class MainSettingsMenuSO : BaseSettingMenuSO
    {
        [Header("Main Menu Settings")]
        [Tooltip("Prefab UI cho main menu (optional, nếu không có sẽ tự động tạo)")]
        public GameObject mainMenuUIPrefab;

        public override GameObject CreateUI(Transform parent, DynamicSettingsController controller)
        {
            // Nếu có custom prefab, dùng nó
            if (mainMenuUIPrefab != null)
            {
                return Instantiate(mainMenuUIPrefab, parent);
            }

            // Nếu không, tạo container đơn giản
            var container = new GameObject("MainSettingsMenu");
            container.transform.SetParent(parent);
            var layout = container.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(20, 20, 20, 20);

            return container;
        }
    }
}

