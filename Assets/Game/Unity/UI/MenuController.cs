using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Unity.Managers;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Menu Controller - Quản lý menu động từ MenuSO
    /// Tự động tạo UI từ MenuSO, hỗ trợ nested menus
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [Header("Menu SO")]
        [Tooltip("Menu SO chính (Pause Menu, etc.)")]
        [SerializeField] private MenuSO rootMenuSO;

        [Header("UI References")]
        [Tooltip("Panel chứa menu (sẽ hiện/ẩn)")]
        [SerializeField] private GameObject menuPanel;

        [Tooltip("Container chứa các button menu items")]
        [SerializeField] private Transform menuItemsContainer;

        [Tooltip("Button template (sẽ duplicate để tạo các button)")]
        [SerializeField] private Button menuItemButtonTemplate;

        [Tooltip("Text hiển thị tiêu đề menu (optional)")]
        [SerializeField] private TextMeshProUGUI menuTitleText;

        [Tooltip("Button Back (để quay lại menu cha, optional)")]
        [SerializeField] private Button backButton;

        [Header("Settings")]
        [Tooltip("Phím để mở/đóng menu")]
        [SerializeField] private KeyCode toggleKey = KeyCode.Escape;

        [Tooltip("Có pause game khi mở menu không")]
        [SerializeField] private bool pauseGameOnOpen = true;

        private MenuSO currentMenu;
        private Stack<MenuSO> menuHistory = new Stack<MenuSO>(); // Lưu lịch sử menu để quay lại
        private List<Button> currentButtons = new List<Button>();
        private bool isMenuOpen = false;

        void Start()
        {
            if (rootMenuSO != null)
            {
                currentMenu = rootMenuSO;
            }

            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
                backButton.gameObject.SetActive(false);
            }

            // Ẩn template button (nếu có)
            if (menuItemButtonTemplate != null)
            {
                menuItemButtonTemplate.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(toggleKey))
            {
                if (isMenuOpen)
                {
                    if (currentMenu != null && currentMenu.canCloseWithEscape)
                    {
                        CloseMenu();
                    }
                    else if (menuHistory.Count > 0)
                    {
                        // Quay lại menu trước
                        GoBack();
                    }
                }
                else
                {
                    OpenMenu();
                }
            }
        }

        /// <summary>
        /// Mở menu
        /// </summary>
        public void OpenMenu()
        {
            if (currentMenu == null)
            {
                Debug.LogWarning("MenuController: No menu to open!");
                return;
            }

            isMenuOpen = true;

            if (menuPanel != null)
            {
                menuPanel.SetActive(true);
            }

            if (pauseGameOnOpen)
            {
                Time.timeScale = 0f;
                Core.Domain.Interfaces.GameState.AllowInput = false;
            }

            // Xóa lịch sử menu
            menuHistory.Clear();

            // Hiển thị menu
            ShowMenu(currentMenu);
        }

        /// <summary>
        /// Đóng menu
        /// </summary>
        public void CloseMenu()
        {
            isMenuOpen = false;

            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }

            if (pauseGameOnOpen)
            {
                Time.timeScale = 1f;
                Core.Domain.Interfaces.GameState.AllowInput = true;
            }

            // Xóa các button cũ
            ClearMenuItems();

            // Xóa lịch sử menu
            menuHistory.Clear();
        }

        /// <summary>
        /// Hiển thị menu từ MenuSO
        /// </summary>
        public void ShowMenu(MenuSO menu)
        {
            if (menu == null)
            {
                Debug.LogWarning("MenuController: MenuSO is null!");
                return;
            }

            currentMenu = menu;

            // Cập nhật tiêu đề
            if (menuTitleText != null)
            {
                menuTitleText.text = menu.menuTitle;
            }

            // Hiển thị/ẩn nút Back
            if (backButton != null)
            {
                backButton.gameObject.SetActive(menu.allowBackNavigation && menuHistory.Count > 0);
            }

            // Xóa các button cũ
            ClearMenuItems();

            // Tạo button cho mỗi menu item
            if (menuItemsContainer != null && menuItemButtonTemplate != null)
            {
                foreach (var item in menu.menuItems)
                {
                    if (item == null || !item.isVisible) continue;

                    // Tạo button từ template
                    Button button = Instantiate(menuItemButtonTemplate, menuItemsContainer);
                    button.gameObject.SetActive(true);

                    // Set text
                    TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.text = item.displayName;
                    }

                    // Set icon (nếu có)
                    Image buttonIcon = button.GetComponentInChildren<Image>();
                    if (buttonIcon != null && item.icon != null)
                    {
                        buttonIcon.sprite = item.icon;
                        buttonIcon.gameObject.SetActive(true);
                    }

                    // Enable/disable button
                    button.interactable = item.isEnabled && !item.isLocked;

                    // Add listener
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnMenuItemClicked(item));

                    currentButtons.Add(button);
                }
            }
            else
            {
                Debug.LogWarning("MenuController: menuItemsContainer or menuItemButtonTemplate is null!");
            }
        }

        /// <summary>
        /// Xử lý khi click vào menu item
        /// </summary>
        private void OnMenuItemClicked(MenuItemSO item)
        {
            if (item == null) return;

            if (item.isLocked)
            {
                Debug.Log(item.lockedMessage);
                // TODO: Hiển thị thông báo locked
                return;
            }

            switch (item.menuType)
            {
                case MenuItemType.Action:
                    // Thực hiện action
                    if (item.onSelectAction != null)
                    {
                        item.onSelectAction.Invoke();
                    }
                    break;

                case MenuItemType.SubMenu:
                    // Mở menu con
                    if (item.subMenu != null)
                    {
                        // Lưu menu hiện tại vào lịch sử
                        menuHistory.Push(currentMenu);
                        // Hiển thị menu con
                        ShowMenu(item.subMenu);
                    }
                    else
                    {
                        Debug.LogWarning($"MenuController: SubMenu is null for item '{item.displayName}'");
                    }
                    break;

                case MenuItemType.SceneLoad:
                    // Load scene
                    if (!string.IsNullOrEmpty(item.sceneToLoad))
                    {
                        // Đóng menu trước
                        CloseMenu();

                        var sceneLoader = SceneLoader.Instance;
                        if (sceneLoader != null)
                        {
                            sceneLoader.LoadScene(item.sceneToLoad);
                        }
                        else
                        {
                            SceneManager.LoadScene(item.sceneToLoad);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"MenuController: SceneToLoad is empty for item '{item.displayName}'");
                    }
                    break;
            }
        }

        /// <summary>
        /// Quay lại menu trước
        /// </summary>
        private void GoBack()
        {
            if (menuHistory.Count > 0)
            {
                MenuSO previousMenu = menuHistory.Pop();
                ShowMenu(previousMenu);
            }
            else
            {
                // Nếu không có menu trước, đóng menu
                CloseMenu();
            }
        }

        /// <summary>
        /// Xử lý khi click nút Back
        /// </summary>
        private void OnBackClicked()
        {
            GoBack();
        }

        /// <summary>
        /// Xóa tất cả menu items (buttons)
        /// </summary>
        private void ClearMenuItems()
        {
            foreach (var button in currentButtons)
            {
                if (button != null)
                {
                    Destroy(button.gameObject);
                }
            }
            currentButtons.Clear();
        }

        /// <summary>
        /// Set root menu (có thể gọi từ code)
        /// </summary>
        public void SetRootMenu(MenuSO menu)
        {
            rootMenuSO = menu;
            currentMenu = menu;
        }

        // ===== Action Helpers (để gọi từ UnityEvent trong MenuItemSO) =====

        /// <summary>
        /// Action: Save Game
        /// </summary>
        public void Action_SaveGame()
        {
            var saveManager = FindFirstObjectByType<SaveLoad.SaveLoadManager>();
            if (saveManager != null)
            {
                saveManager.QuickSave();
                Debug.Log("Game saved!");
            }
        }

        /// <summary>
        /// Action: Load Game
        /// </summary>
        public void Action_LoadGame()
        {
            var saveManager = FindFirstObjectByType<SaveLoad.SaveLoadManager>();
            if (saveManager != null)
            {
                if (saveManager.HasSaveFile())
                {
                    saveManager.QuickLoad();
                    Debug.Log("Game loaded!");
                    CloseMenu();
                }
                else
                {
                    Debug.LogWarning("No save file found!");
                }
            }
        }

        /// <summary>
        /// Action: Resume Game (đóng menu)
        /// </summary>
        public void Action_ResumeGame()
        {
            CloseMenu();
        }

        /// <summary>
        /// Action: Quit Game
        /// </summary>
        public void Action_QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        /// <summary>
        /// Action: Load Title Screen
        /// </summary>
        public void Action_LoadTitleScreen()
        {
            CloseMenu();
            Time.timeScale = 1f;
            Core.Domain.Interfaces.GameState.AllowInput = true;

            var sceneLoader = SceneLoader.Instance;
            if (sceneLoader != null)
            {
                sceneLoader.LoadTitleScreen();
            }
            else
            {
                SceneManager.LoadScene("TitleScreen");
            }
        }

        /// <summary>
        /// [DEPRECATED] Load Main Menu - Dùng Action_LoadTitleScreen() thay thế
        /// </summary>
        [System.Obsolete("Use Action_LoadTitleScreen() instead")]
        public void Action_LoadMainMenu()
        {
            Action_LoadTitleScreen();
        }

        /// <summary>
        /// Action: Open Settings (placeholder - override trong subclass)
        /// </summary>
        public void Action_OpenSettings()
        {
            Debug.Log("Open Settings");
            // TODO: Implement settings menu
        }

        /// <summary>
        /// Action: Open Inventory (placeholder - override trong subclass)
        /// </summary>
        public void Action_OpenInventory()
        {
            Debug.Log("Open Inventory");
            // TODO: Implement inventory menu
        }
    }
}

