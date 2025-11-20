using UnityEngine;
using UnityEngine.SceneManagement;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Unity.Presentation.UI;
using TheGreenMemoir.Unity.SaveLoad;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Pause Controller - Quản lý menu tạm dừng
    /// Hỗ trợ Inventory, Save/Load với multiple slots, Quit
    /// </summary>
    public class PauseController : MonoBehaviour
    {
        [Header("UI Panels")]
        [Tooltip("Panel chính của Pause Menu")]
        public GameObject pausePanel;
        
        [Tooltip("Panel chứa các nút menu (Resume, Inventory, Save, Load, Quit)")]
        public GameObject menuButtonsPanel;
        
        [Header("UI References")]
        [Tooltip("Inventory UI Controller (tự động tìm nếu để trống)")]
        public InventoryUIController inventoryUIController;
        
        [Tooltip("Save Slot List Controller (hiển thị danh sách save slots)")]
        public SaveSlotListController saveSlotListController;
        
        [Tooltip("Panel xác nhận Quit (optional)")]
        public GameObject quitConfirmPanel;
        
        [Header("Settings")]
        [Tooltip("Phím để mở/đóng pause menu")]
        public KeyCode pauseKey = KeyCode.Escape;
        
        [Tooltip("Có thể pause khi đang mở inventory/save menu không")]
        public bool canPauseWhenSubMenuOpen = false;

        private bool _isPaused = false;
        private bool _isSubMenuOpen = false; // Inventory hoặc Save/Load menu đang mở

        void Start()
        {
            // Tự động tìm InventoryUIController nếu chưa gán
            if (inventoryUIController == null)
                inventoryUIController = FindFirstObjectByType<InventoryUIController>();
            
            // Tự động tìm SaveSlotListController nếu chưa gán
            if (saveSlotListController == null)
                saveSlotListController = FindFirstObjectByType<SaveSlotListController>();
            
            // Setup SaveSlotListController callbacks
            if (saveSlotListController != null)
            {
                saveSlotListController.OnSlotSelected += OnSaveSlotSelected;
                saveSlotListController.OnBackClicked += OnSaveSlotBackClicked;
            }
            
            // Ẩn các panel ban đầu
            if (pausePanel != null)
                pausePanel.SetActive(false);
            if (menuButtonsPanel != null)
                menuButtonsPanel.SetActive(true); // Hiển thị buttons trong pause panel
            if (quitConfirmPanel != null)
                quitConfirmPanel.SetActive(false);
            if (saveSlotListController != null)
                saveSlotListController.HidePanel();
        }

        void Update()
        {
            // Chỉ cho phép pause nếu không có sub menu đang mở (trừ khi canPauseWhenSubMenuOpen = true)
            if (UnityEngine.Input.GetKeyDown(pauseKey))
            {
                if (!canPauseWhenSubMenuOpen && _isSubMenuOpen)
                    return;
                    
                TogglePause();
            }
        }

        void OnDestroy()
        {
            // Unsubscribe callbacks
            if (saveSlotListController != null)
            {
                saveSlotListController.OnSlotSelected -= OnSaveSlotSelected;
                saveSlotListController.OnBackClicked -= OnSaveSlotBackClicked;
            }
        }

        /// <summary>
        /// Mở/đóng pause menu
        /// </summary>
        public void TogglePause()
        {
            _isPaused = !_isPaused;
            
            if (pausePanel != null)
                pausePanel.SetActive(_isPaused);
            
            if (menuButtonsPanel != null && _isPaused)
                menuButtonsPanel.SetActive(true); // Hiển thị menu buttons khi pause
            
            // Đóng các sub menu khi đóng pause menu
            if (!_isPaused)
            {
                CloseAllSubMenus();
            }

            Time.timeScale = _isPaused ? 0f : 1f;
            GameState.AllowInput = !_isPaused;
            
            // Pause/Resume game time
            if (GameManager.TimeService != null)
            {
                var timeManager = GameManager.TimeService as Managers.TimeManager;
                if (timeManager != null)
                {
                    if (_isPaused)
                        timeManager.PauseTime();
                    else
                        timeManager.ResumeTime();
                }
            }
        }

        /// <summary>
        /// Đóng tất cả sub menus
        /// </summary>
        private void CloseAllSubMenus()
        {
            _isSubMenuOpen = false;
            
            // Đóng Inventory
            if (inventoryUIController != null)
                inventoryUIController.HideUI();
            
            // Đóng Save Slot List
            if (saveSlotListController != null)
                saveSlotListController.HidePanel();
            
            // Đóng Quit Confirm
            if (quitConfirmPanel != null)
                quitConfirmPanel.SetActive(false);
            
            // Hiển thị lại menu buttons
            if (menuButtonsPanel != null)
                menuButtonsPanel.SetActive(true);
        }

        // ===== Button Handlers =====

        /// <summary>
        /// Resume Game - Đóng pause menu
        /// </summary>
        public void OnResumeClicked()
        {
            TogglePause();
        }

        /// <summary>
        /// Mở Inventory
        /// </summary>
        public void OnInventoryClicked()
        {
            if (inventoryUIController != null)
            {
                _isSubMenuOpen = true;
                if (menuButtonsPanel != null)
                    menuButtonsPanel.SetActive(false); // Ẩn menu buttons
                inventoryUIController.ShowUI();
            }
            else
            {
                Debug.LogWarning("InventoryUIController not found! Please assign in Inspector.");
            }
        }

        /// <summary>
        /// Mở Save Menu (hiển thị danh sách save slots với mode Save)
        /// </summary>
        public void OnSaveClicked()
        {
            if (saveSlotListController != null)
            {
                _isSubMenuOpen = true;
                if (menuButtonsPanel != null)
                    menuButtonsPanel.SetActive(false); // Ẩn menu buttons
                saveSlotListController.ShowPanel(SaveSlotListController.SaveSlotMode.Save);
            }
            else
            {
                Debug.LogWarning("SaveSlotListController not found! Please assign in Inspector.");
                // Fallback: QuickSave
                var saveManager = SaveLoadManager.Instance;
                if (saveManager != null)
                {
                    saveManager.QuickSave();
                    Debug.Log("Game saved!");
                }
            }
        }

        /// <summary>
        /// Mở Load Menu (hiển thị danh sách save slots với mode Load)
        /// </summary>
        public void OnLoadClicked()
        {
            if (saveSlotListController != null)
            {
                _isSubMenuOpen = true;
                if (menuButtonsPanel != null)
                    menuButtonsPanel.SetActive(false); // Ẩn menu buttons
                saveSlotListController.ShowPanel(SaveSlotListController.SaveSlotMode.Load);
            }
            else
            {
                Debug.LogWarning("SaveSlotListController not found! Please assign in Inspector.");
                // Fallback: QuickLoad
                var saveManager = SaveLoadManager.Instance;
                if (saveManager != null)
                {
                    if (saveManager.HasSaveFile())
                    {
                        saveManager.QuickLoad();
                        Debug.Log("Game loaded!");
                        TogglePause(); // Đóng pause menu sau khi load
                    }
                    else
                    {
                        Debug.LogWarning("No save file found!");
                    }
                }
            }
        }

        /// <summary>
        /// Mở Quit Confirm Panel hoặc Quit trực tiếp
        /// </summary>
        public void OnQuitClicked()
        {
            if (quitConfirmPanel != null)
            {
                // Hiển thị confirm panel
                _isSubMenuOpen = true;
                if (menuButtonsPanel != null)
                    menuButtonsPanel.SetActive(false);
                quitConfirmPanel.SetActive(true);
            }
            else
            {
                // Quit trực tiếp (không có confirm)
                OnQuitToTitleScreenConfirmed();
            }
        }

        /// <summary>
        /// Xác nhận Quit to Title Screen
        /// </summary>
        public void OnQuitToTitleScreenConfirmed()
        {
            Time.timeScale = 1f;
            GameState.AllowInput = true;

            var sceneLoader = FindFirstObjectByType<Managers.SceneLoader>();
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
        /// Xác nhận Quit Game (thoát ứng dụng)
        /// </summary>
        public void OnQuitGameConfirmed()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        /// <summary>
        /// Hủy Quit (đóng confirm panel)
        /// </summary>
        public void OnQuitCancelled()
        {
            if (quitConfirmPanel != null)
                quitConfirmPanel.SetActive(false);
            if (menuButtonsPanel != null)
                menuButtonsPanel.SetActive(true);
            _isSubMenuOpen = false;
        }

        // ===== Save Slot List Callbacks =====

        /// <summary>
        /// Xử lý khi chọn save slot
        /// </summary>
        private void OnSaveSlotSelected(int slotIndex)
        {
            var saveManager = SaveLoadManager.Instance;
            if (saveManager == null)
            {
                Debug.LogError("SaveLoadManager not found!");
                return;
            }

            if (saveSlotListController != null)
            {
                var mode = saveSlotListController.Mode;
                
                if (mode == SaveSlotListController.SaveSlotMode.Save)
                {
                    // Save game vào slot
                    var gameState = saveManager.CreateGameState();
                    saveManager.SaveGameToSlot(slotIndex, gameState);
                    Debug.Log($"Game saved to slot {slotIndex + 1}!");
                    
                    // Refresh save slots list
                    saveSlotListController.RefreshSaveSlots();
                    
                    // Có thể đóng save menu sau khi save
                    // OnSaveSlotBackClicked();
                }
                else if (mode == SaveSlotListController.SaveSlotMode.Load)
                {
                    // Load game từ slot
                    if (saveManager.HasSaveFileInSlot(slotIndex))
                    {
                        saveManager.LoadGameFromSlot(slotIndex);
                        Debug.Log($"Game loaded from slot {slotIndex + 1}!");
                        
                        // Đóng pause menu sau khi load
                        TogglePause();
                    }
                    else
                    {
                        Debug.LogWarning($"No save file in slot {slotIndex + 1}!");
                    }
                }
            }
        }

        /// <summary>
        /// Xử lý khi click Back trong Save Slot List
        /// </summary>
        private void OnSaveSlotBackClicked()
        {
            _isSubMenuOpen = false;
            if (menuButtonsPanel != null)
                menuButtonsPanel.SetActive(true); // Hiển thị lại menu buttons
        }

        // ===== Settings =====

        public void OnSettingsClicked()
        {
            // Mở settings
            var settingsController = FindFirstObjectByType<SettingsController>();
            if (settingsController != null)
            {
                settingsController.ShowSettings();
                _isSubMenuOpen = true;
                if (menuButtonsPanel != null)
                    menuButtonsPanel.SetActive(false);
            }
            else
            {
                // Fallback: Tìm DynamicSettingsController
                var dynamicSettings = FindFirstObjectByType<DynamicSettingsController>();
                if (dynamicSettings != null)
                {
                    dynamicSettings.ShowSettings();
                    _isSubMenuOpen = true;
                    if (menuButtonsPanel != null)
                        menuButtonsPanel.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("SettingsController or DynamicSettingsController not found!");
                }
            }
        }

        // ===== Backward Compatibility =====

        /// <summary>
        /// [DEPRECATED] Quay về Main Menu - Dùng OnTitleScreenClicked() thay thế
        /// </summary>
        [System.Obsolete("Use OnQuitToTitleScreenConfirmed() instead")]
        public void OnTitleScreenClicked()
        {
            OnQuitToTitleScreenConfirmed();
        }

        /// <summary>
        /// [DEPRECATED] Quay về Main Menu - Dùng OnTitleScreenClicked() thay thế
        /// </summary>
        [System.Obsolete("Use OnQuitToTitleScreenConfirmed() instead")]
        public void OnMainMenuClicked()
        {
            OnQuitToTitleScreenConfirmed();
        }
    }
}

