using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Unity.Data.Settings;
using TheGreenMemoir.Unity.Audio;
using TheGreenMemoir.Unity.Input;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Dynamic Settings Controller - Tự động tạo UI từ SettingMenuRegistrySO
    /// Hỗ trợ nested menus, sub-menus, và custom menus
    /// </summary>
    public class DynamicSettingsController : MonoBehaviour
    {
        [Header("Registry")]
        [SerializeField] private SettingMenuRegistrySO registry;

        [Header("UI Prefabs")]
        [SerializeField] private GameObject menuButtonPrefab;
        [SerializeField] private GameObject subMenuButtonPrefab;
        [SerializeField] private GameObject menuPanelPrefab;
        [SerializeField] private GameObject subMenuPanelPrefab;

        [Header("Containers")]
        [SerializeField] private Transform mainMenuContainer;
        [SerializeField] private Transform subMenuContainer;
        [SerializeField] private Transform currentMenuContainer;

        [Header("Navigation")]
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI currentMenuTitle;

        [Header("References")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameSettingsDataSO settingsData;
        [SerializeField] private InputActionManager inputManager;

        private Stack<BaseSettingMenuSO> _menuStack = new Stack<BaseSettingMenuSO>();
        private Dictionary<string, GameObject> _createdMenus = new Dictionary<string, GameObject>();
        private BaseSettingMenuSO _currentMenu;

        private void Awake()
        {
            if (inputManager == null)
                inputManager = FindFirstObjectByType<InputActionManager>();

            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);
        }

        private void Start()
        {
            if (registry != null)
            {
                registry.CollectAllMenus();
                LoadMainMenu();
            }
        }

        /// <summary>
        /// Load main menu từ registry
        /// </summary>
        public void LoadMainMenu()
        {
            if (registry == null || registry.mainSettingsMenu == null) return;

            ClearCurrentMenu();
            _menuStack.Clear();
            OpenMenu(registry.mainSettingsMenu);
        }

        /// <summary>
        /// Mở một menu
        /// </summary>
        public void OpenMenu(BaseSettingMenuSO menu)
        {
            if (menu == null || !menu.isEnabled || !menu.isVisible) return;

            // Push vào stack
            if (_currentMenu != null)
            {
                _menuStack.Push(_currentMenu);
            }

            _currentMenu = menu;
            RenderMenu(menu);

            // Update UI
            if (currentMenuTitle != null)
            {
                currentMenuTitle.text = menu.menuName;
            }

            if (backButton != null)
            {
                backButton.gameObject.SetActive(_menuStack.Count > 0);
            }
        }

        /// <summary>
        /// Render menu UI
        /// </summary>
        private void RenderMenu(BaseSettingMenuSO menu)
        {
            if (menu == null) return;

            // Clear container
            ClearCurrentMenu();

            // Tạo UI cho menu chính
            var menuUI = menu.CreateUI(GetCurrentContainer(), this);
            if (menuUI != null)
            {
                _createdMenus[menu.menuId] = menuUI;
            }

            // Tạo buttons cho sub-menus
            if (menu.subMenus != null && menu.subMenus.Count > 0)
            {
                CreateSubMenuButtons(menu.subMenus, GetCurrentContainer());
            }
        }

        /// <summary>
        /// Tạo buttons cho sub-menus
        /// </summary>
        private void CreateSubMenuButtons(List<SubSettingMenuSO> subMenus, Transform container)
        {
            if (subMenus == null || container == null) return;

            foreach (var subMenu in subMenus)
            {
                if (subMenu == null || !subMenu.isEnabled || !subMenu.isVisible) continue;

                GameObject buttonObj = null;
                Button button = null;
                TextMeshProUGUI text = null;

                if (subMenuButtonPrefab != null)
                {
                    buttonObj = Instantiate(subMenuButtonPrefab, container);
                }
                else if (menuButtonPrefab != null)
                {
                    buttonObj = Instantiate(menuButtonPrefab, container);
                }
                else
                {
                    // Tạo button đơn giản
                    buttonObj = new GameObject($"Button_{subMenu.menuId}");
                    buttonObj.transform.SetParent(container);
                    button = buttonObj.AddComponent<Button>();
                    text = buttonObj.AddComponent<TextMeshProUGUI>();
                    if (text != null)
                    {
                        text.text = subMenu.menuName;
                    }
                }

                // Setup button
                if (button == null)
                {
                    button = buttonObj.GetComponent<Button>();
                }
                if (button != null)
                {
                    button.onClick.AddListener(() => OpenSubMenu(subMenu));
                }

                // Setup text
                if (text == null)
                {
                    text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                }
                if (text != null)
                {
                    text.text = subMenu.menuName;
                }

                // Setup icon nếu có
                if (subMenu.menuIcon != null)
                {
                    var image = buttonObj.GetComponentInChildren<Image>();
                    if (image != null)
                    {
                        image.sprite = subMenu.menuIcon;
                    }
                }
            }
        }

        /// <summary>
        /// Mở sub-menu
        /// </summary>
        public void OpenSubMenu(SubSettingMenuSO subMenu)
        {
            if (subMenu == null) return;

            // Tạo panel cho sub-menu
            GameObject subMenuPanel = null;
            if (subMenuPanelPrefab != null)
            {
                subMenuPanel = Instantiate(subMenuPanelPrefab, GetCurrentContainer());
            }
            else
            {
                subMenuPanel = new GameObject($"SubMenu_{subMenu.menuId}");
                subMenuPanel.transform.SetParent(GetCurrentContainer());
                var rectTransform = subMenuPanel.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
            }

            // Tạo UI cho sub-menu
            var subMenuUI = subMenu.CreateUI(subMenuPanel.transform, this);
            if (subMenuUI == null)
            {
                // Nếu không có custom UI, tạo UI mặc định dựa trên subMenuType
                subMenuUI = CreateSubMenuUI(subMenu, subMenuPanel.transform);
            }

            // Tạo buttons cho nested sub-menus
            if (subMenu.nestedSubMenus != null && subMenu.nestedSubMenus.Count > 0)
            {
                CreateSubMenuButtons(subMenu.nestedSubMenus, subMenuPanel.transform);
            }

            // Push vào stack
            if (_currentMenu != null)
            {
                _menuStack.Push(_currentMenu);
            }
            _currentMenu = subMenu;

            // Update UI
            if (currentMenuTitle != null)
            {
                currentMenuTitle.text = subMenu.menuName;
            }

            if (backButton != null)
            {
                backButton.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Tạo UI cho sub-menu dựa trên subMenuType
        /// </summary>
        public GameObject CreateSubMenuUI(SubSettingMenuSO subMenu, Transform parent)
        {
            if (subMenu == null || parent == null) return null;

            GameObject ui = null;

            switch (subMenu.subMenuType)
            {
                case SubSettingMenuSO.SubMenuType.VolumeSettings:
                    ui = CreateVolumeSettingsUI(subMenu, parent);
                    break;
                case SubSettingMenuSO.SubMenuType.KeyConfiguration:
                    ui = CreateKeyConfigurationUI(subMenu, parent);
                    break;
                case SubSettingMenuSO.SubMenuType.DisplaySettings:
                    ui = CreateDisplaySettingsUI(subMenu, parent);
                    break;
                case SubSettingMenuSO.SubMenuType.AudioSettings:
                    ui = CreateAudioSettingsUI(subMenu, parent);
                    break;
                case SubSettingMenuSO.SubMenuType.LanguageSettings:
                    ui = CreateLanguageSettingsUI(subMenu, parent);
                    break;
                case SubSettingMenuSO.SubMenuType.EventSceneSettings:
                    ui = CreateEventSceneSettingsUI(subMenu, parent);
                    break;
                default:
                    // Custom hoặc không xác định - tạo container rỗng
                    ui = new GameObject($"SubMenuUI_{subMenu.menuId}");
                    ui.transform.SetParent(parent);
                    break;
            }

            return ui;
        }

        /// <summary>
        /// Tạo UI cho Volume Settings
        /// </summary>
        private GameObject CreateVolumeSettingsUI(SubSettingMenuSO subMenu, Transform parent)
        {
            var container = new GameObject("VolumeSettingsContainer");
            container.transform.SetParent(parent);
            var layout = container.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(10, 10, 10, 10);

            if (settingsData == null) return container;

            // BGM Volume
            CreateVolumeSlider(container.transform, "BGM", settingsData.bgmVolume, (val) =>
            {
                settingsData.SetBGMVolume(val / 100f);
                if (AudioManager.Instance != null)
                    AudioManager.Instance.SetMusicVolume(settingsData.GetBGMVolumeFloat());
            });

            // SE Volume
            CreateVolumeSlider(container.transform, "SE", settingsData.seVolume, (val) =>
            {
                settingsData.SetSEVolume(val / 100f);
                if (AudioManager.Instance != null)
                    AudioManager.Instance.SetSFXVolume(settingsData.GetSEVolumeFloat());
            });

            // Game Voices Volume
            CreateVolumeSlider(container.transform, "Game Voices", settingsData.gameVoicesVolume, (val) =>
            {
                settingsData.SetGameVoicesVolume(val / 100f);
                if (AudioManager.Instance != null)
                    AudioManager.Instance.SetGameVoicesVolume(settingsData.GetGameVoicesVolumeFloat());
            });

            // Event Voices Volume
            CreateVolumeSlider(container.transform, "Event Voices", settingsData.eventVoicesVolume, (val) =>
            {
                settingsData.SetEventVoicesVolume(val / 100f);
                if (AudioManager.Instance != null)
                    AudioManager.Instance.SetEventVoicesVolume(settingsData.GetEventVoicesVolumeFloat());
            });

            return container;
        }

        /// <summary>
        /// Tạo volume slider
        /// </summary>
        private void CreateVolumeSlider(Transform parent, string label, int value, System.Action<float> onValueChanged)
        {
            var row = new GameObject($"VolumeRow_{label}");
            row.transform.SetParent(parent);
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;

            // Label
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(row.transform);
            var labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 16;

            // Slider
            var sliderObj = new GameObject("Slider");
            sliderObj.transform.SetParent(row.transform);
            var slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.wholeNumbers = true;
            slider.value = value;
            slider.onValueChanged.AddListener((val) => onValueChanged?.Invoke(val));

            // Value Text
            var valueObj = new GameObject("Value");
            valueObj.transform.SetParent(row.transform);
            var valueText = valueObj.AddComponent<TextMeshProUGUI>();
            valueText.text = value.ToString();
            valueText.fontSize = 16;
            slider.onValueChanged.AddListener((val) => valueText.text = Mathf.RoundToInt(val).ToString());
        }

        /// <summary>
        /// Tạo UI cho Key Configuration
        /// </summary>
        private GameObject CreateKeyConfigurationUI(SubSettingMenuSO subMenu, Transform parent)
        {
            var container = new GameObject("KeyConfigContainer");
            container.transform.SetParent(parent);
            var layout = container.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 5;

            if (inputManager == null) return container;

            // Get actions from InputActionManager
            var field = typeof(InputActionManager).GetField("actions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var actions = field?.GetValue(inputManager) as List<InputActionSO>;
            if (actions == null) return container;

            foreach (var action in actions)
            {
                if (action == null || string.IsNullOrEmpty(action.displayName)) continue;
                CreateKeyConfigRow(container.transform, action);
            }

            return container;
        }

        /// <summary>
        /// Tạo row cho key config
        /// </summary>
        private void CreateKeyConfigRow(Transform parent, InputActionSO action)
        {
            var row = new GameObject($"KeyRow_{action.actionId}");
            row.transform.SetParent(parent);
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;

            // Action Name
            var nameObj = new GameObject("ActionName");
            nameObj.transform.SetParent(row.transform);
            var nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = action.displayName;
            nameText.fontSize = 14;

            // Key Button
            var buttonObj = new GameObject("KeyButton");
            buttonObj.transform.SetParent(row.transform);
            var button = buttonObj.AddComponent<Button>();
            var buttonText = buttonObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = action.key.ToString();
            buttonText.fontSize = 14;
            button.onClick.AddListener(() => BeginRebindKey(action, buttonText));
        }

        private bool _waitingForKey;
        private InputActionSO _pendingAction;
        private TextMeshProUGUI _pendingText;

        private void Update()
        {
            if (_waitingForKey && UnityEngine.Input.anyKeyDown)
            {
                var newKey = DetectPressedKey();
                if (newKey != KeyCode.None && _pendingAction != null)
                {
                    if (inputManager != null)
                    {
                        inputManager.Rebind(_pendingAction.actionId, newKey);
                    }
                    _pendingAction.key = newKey;
                    if (_pendingText != null)
                    {
                        _pendingText.text = newKey.ToString();
                    }
                    _waitingForKey = false;
                    _pendingAction = null;
                    _pendingText = null;
                }
            }
        }

        private void BeginRebindKey(InputActionSO action, TextMeshProUGUI text)
        {
            _pendingAction = action;
            _pendingText = text;
            _waitingForKey = true;
            if (text != null)
            {
                text.text = "Press any key...";
            }
        }

        private KeyCode DetectPressedKey()
        {
            foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (UnityEngine.Input.GetKeyDown(k) && k != KeyCode.Mouse0 && k != KeyCode.Mouse1 && k != KeyCode.Mouse2)
                {
                    return k;
                }
            }
            return KeyCode.None;
        }

        /// <summary>
        /// Tạo UI cho Display Settings
        /// </summary>
        private GameObject CreateDisplaySettingsUI(SubSettingMenuSO subMenu, Transform parent)
        {
            var container = new GameObject("DisplaySettingsContainer");
            container.transform.SetParent(parent);
            // TODO: Implement display settings UI
            return container;
        }

        /// <summary>
        /// Tạo UI cho Audio Settings
        /// </summary>
        private GameObject CreateAudioSettingsUI(SubSettingMenuSO subMenu, Transform parent)
        {
            var container = new GameObject("AudioSettingsContainer");
            container.transform.SetParent(parent);
            // TODO: Implement audio settings UI
            return container;
        }

        /// <summary>
        /// Tạo UI cho Language Settings
        /// </summary>
        private GameObject CreateLanguageSettingsUI(SubSettingMenuSO subMenu, Transform parent)
        {
            var container = new GameObject("LanguageSettingsContainer");
            container.transform.SetParent(parent);
            // TODO: Implement language settings UI
            return container;
        }

        /// <summary>
        /// Tạo UI cho Event Scene Settings
        /// </summary>
        private GameObject CreateEventSceneSettingsUI(SubSettingMenuSO subMenu, Transform parent)
        {
            var container = new GameObject("EventSceneSettingsContainer");
            container.transform.SetParent(parent);
            // TODO: Implement event scene settings UI
            return container;
        }

        /// <summary>
        /// Lấy container hiện tại
        /// </summary>
        private Transform GetCurrentContainer()
        {
            if (currentMenuContainer != null) return currentMenuContainer;
            if (subMenuContainer != null) return subMenuContainer;
            if (mainMenuContainer != null) return mainMenuContainer;
            return transform;
        }

        /// <summary>
        /// Xóa menu hiện tại
        /// </summary>
        private void ClearCurrentMenu()
        {
            if (currentMenuContainer != null)
            {
                foreach (Transform child in currentMenuContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        /// <summary>
        /// Quay lại menu trước
        /// </summary>
        private void OnBackClicked()
        {
            if (_menuStack.Count > 0)
            {
                var previousMenu = _menuStack.Pop();
                _currentMenu = previousMenu;
                RenderMenu(previousMenu);

                if (currentMenuTitle != null)
                {
                    currentMenuTitle.text = previousMenu.menuName;
                }

                if (backButton != null)
                {
                    backButton.gameObject.SetActive(_menuStack.Count > 0);
                }
            }
            else
            {
                // Quay về main menu
                LoadMainMenu();
            }
        }

        /// <summary>
        /// Hiển thị settings panel
        /// </summary>
        public void ShowSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
            LoadMainMenu();
        }

        /// <summary>
        /// Ẩn settings panel
        /// </summary>
        public void HideSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }
    }
}

