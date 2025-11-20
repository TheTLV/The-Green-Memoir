using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Unity.Audio;
using TheGreenMemoir.Unity.Input;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Settings Menu Controller - Quản lý màn hình cài đặt đầy đủ
    /// Bao gồm: Screen Settings, Volume, Event Scenes, Language, Key Configuration
    /// </summary>
    public class SettingsMenuController : MonoBehaviour
    {
        [Header("Settings Data")]
        [SerializeField] private GameSettingsDataSO settingsData;

        [Header("Screen Settings")]
        [SerializeField] private Toggle fullScreenToggle;
        [SerializeField] private Toggle windowedToggle;

        [Header("Volume Sliders (0-100)")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private TextMeshProUGUI bgmValueText;
        [SerializeField] private Slider seSlider;
        [SerializeField] private TextMeshProUGUI seValueText;
        [SerializeField] private Slider gameVoicesSlider;
        [SerializeField] private TextMeshProUGUI gameVoicesValueText;
        [SerializeField] private Slider eventVoicesSlider;
        [SerializeField] private TextMeshProUGUI eventVoicesValueText;

        [Header("Event Scenes Settings")]
        [SerializeField] private ToggleGroup textSpeedGroup;
        [SerializeField] private Toggle textSpeedSlowToggle;
        [SerializeField] private Toggle textSpeedNormalToggle;
        [SerializeField] private Toggle textSpeedFastToggle;
        [SerializeField] private Toggle textSpeedNoWaitToggle;
        
        [SerializeField] private ToggleGroup autoModeTextSpeedGroup;
        [SerializeField] private Toggle autoModeOffToggle;
        [SerializeField] private Toggle autoModeSlowToggle;
        [SerializeField] private Toggle autoModeNormalToggle;
        [SerializeField] private Toggle autoModeFastToggle;
        
        [SerializeField] private Toggle continuePlayingVoicesToggle;

        [Header("Language")]
        [SerializeField] private TMP_Dropdown languageDropdown;

        [Header("Key Configuration")]
        [SerializeField] private Transform keyConfigContainer;
        [SerializeField] private GameObject keyConfigRowPrefab;

        [Header("Buttons")]
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button defaultButton;

        [Header("Main Menu Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button galleryButton;
        [SerializeField] private Button configButton;
        [SerializeField] private Button exitGameButton;

        [Header("References")]
        [SerializeField] private InputActionManager inputManager;
        [SerializeField] private GameObject settingsPanel;

        private Dictionary<string, GameObject> _keyConfigRows = new Dictionary<string, GameObject>();
        private bool _waitingForKey;
        private InputActionSO _pendingAction;
        private KeyCode _pendingKeyCode;

        private void Awake()
        {
            if (settingsData == null)
            {
                Debug.LogError("SettingsMenuController: GameSettingsDataSO is not assigned!");
                return;
            }

            if (inputManager == null)
                inputManager = FindFirstObjectByType<InputActionManager>();

            LoadSettings();
            SetupUI();
        }

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
                    UpdateKeyConfigRow(_pendingAction);
                    _waitingForKey = false;
                    _pendingAction = null;
                }
            }
        }

        private void LoadSettings()
        {
            if (settingsData == null) return;

            // Load from PlayerPrefs hoặc dùng default từ SO
            settingsData.isFullScreen = PlayerPrefs.GetInt("IsFullScreen", settingsData.isFullScreen ? 1 : 0) == 1;
            settingsData.bgmVolume = PlayerPrefs.GetInt("BGMVolume", settingsData.bgmVolume);
            settingsData.seVolume = PlayerPrefs.GetInt("SEVolume", settingsData.seVolume);
            settingsData.gameVoicesVolume = PlayerPrefs.GetInt("GameVoicesVolume", settingsData.gameVoicesVolume);
            settingsData.eventVoicesVolume = PlayerPrefs.GetInt("EventVoicesVolume", settingsData.eventVoicesVolume);
            settingsData.textSpeed = (GameSettingsDataSO.TextSpeed)PlayerPrefs.GetInt("TextSpeed", (int)settingsData.textSpeed);
            settingsData.autoModeTextSpeed = (GameSettingsDataSO.AutoTextSpeed)PlayerPrefs.GetInt("AutoModeTextSpeed", (int)settingsData.autoModeTextSpeed);
            settingsData.continuePlayingVoices = PlayerPrefs.GetInt("ContinuePlayingVoices", settingsData.continuePlayingVoices ? 1 : 0) == 1;
            settingsData.currentLanguage = (GameSettingsDataSO.Language)PlayerPrefs.GetInt("CurrentLanguage", (int)settingsData.currentLanguage);

            // Apply screen settings
            Screen.fullScreen = settingsData.isFullScreen;
        }

        private void SetupUI()
        {
            // Screen Settings
            if (fullScreenToggle != null)
            {
                fullScreenToggle.isOn = settingsData.isFullScreen;
                fullScreenToggle.onValueChanged.AddListener(OnFullScreenChanged);
            }
            if (windowedToggle != null)
            {
                windowedToggle.isOn = !settingsData.isFullScreen;
                windowedToggle.onValueChanged.AddListener(OnWindowedChanged);
            }

            // Volume Sliders
            SetupVolumeSlider(bgmSlider, bgmValueText, settingsData.bgmVolume, OnBGMVolumeChanged);
            SetupVolumeSlider(seSlider, seValueText, settingsData.seVolume, OnSEVolumeChanged);
            SetupVolumeSlider(gameVoicesSlider, gameVoicesValueText, settingsData.gameVoicesVolume, OnGameVoicesVolumeChanged);
            SetupVolumeSlider(eventVoicesSlider, eventVoicesValueText, settingsData.eventVoicesVolume, OnEventVoicesVolumeChanged);

            // Event Scenes Settings
            SetupTextSpeedToggles();
            SetupAutoModeTextSpeedToggles();
            if (continuePlayingVoicesToggle != null)
            {
                continuePlayingVoicesToggle.isOn = settingsData.continuePlayingVoices;
                continuePlayingVoicesToggle.onValueChanged.AddListener(OnContinuePlayingVoicesChanged);
            }

            // Language Dropdown
            if (languageDropdown != null)
            {
                languageDropdown.ClearOptions();
                var options = new List<string>();
                foreach (var lang in System.Enum.GetValues(typeof(GameSettingsDataSO.Language)))
                {
                    options.Add(lang.ToString());
                }
                languageDropdown.AddOptions(options);
                languageDropdown.value = (int)settingsData.currentLanguage;
                languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
            }

            // Key Configuration
            BuildKeyConfigList();

            // Buttons
            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirmClicked);
            if (defaultButton != null)
                defaultButton.onClick.AddListener(OnDefaultClicked);

            // Main Menu Buttons
            if (newGameButton != null)
                newGameButton.onClick.AddListener(OnNewGameClicked);
            if (loadGameButton != null)
                loadGameButton.onClick.AddListener(OnLoadGameClicked);
            if (galleryButton != null)
                galleryButton.onClick.AddListener(OnGalleryClicked);
            if (configButton != null)
                configButton.onClick.AddListener(OnConfigClicked);
            if (exitGameButton != null)
                exitGameButton.onClick.AddListener(OnExitGameClicked);
        }

        private void SetupVolumeSlider(Slider slider, TextMeshProUGUI valueText, int value, UnityEngine.Events.UnityAction<float> onValueChanged)
        {
            if (slider != null)
            {
                slider.minValue = 0;
                slider.maxValue = 100;
                slider.wholeNumbers = true;
                slider.value = value;
                slider.onValueChanged.AddListener(onValueChanged);
            }
            if (valueText != null)
            {
                valueText.text = value.ToString();
            }
        }

        private void SetupTextSpeedToggles()
        {
            if (textSpeedGroup == null) return;

            // Set active toggle based on current setting
            switch (settingsData.textSpeed)
            {
                case GameSettingsDataSO.TextSpeed.Slow:
                    if (textSpeedSlowToggle != null) textSpeedSlowToggle.isOn = true;
                    break;
                case GameSettingsDataSO.TextSpeed.Normal:
                    if (textSpeedNormalToggle != null) textSpeedNormalToggle.isOn = true;
                    break;
                case GameSettingsDataSO.TextSpeed.Fast:
                    if (textSpeedFastToggle != null) textSpeedFastToggle.isOn = true;
                    break;
                case GameSettingsDataSO.TextSpeed.NoWait:
                    if (textSpeedNoWaitToggle != null) textSpeedNoWaitToggle.isOn = true;
                    break;
            }

            // Add listeners
            if (textSpeedSlowToggle != null)
                textSpeedSlowToggle.onValueChanged.AddListener((val) => { if (val) OnTextSpeedChanged(GameSettingsDataSO.TextSpeed.Slow); });
            if (textSpeedNormalToggle != null)
                textSpeedNormalToggle.onValueChanged.AddListener((val) => { if (val) OnTextSpeedChanged(GameSettingsDataSO.TextSpeed.Normal); });
            if (textSpeedFastToggle != null)
                textSpeedFastToggle.onValueChanged.AddListener((val) => { if (val) OnTextSpeedChanged(GameSettingsDataSO.TextSpeed.Fast); });
            if (textSpeedNoWaitToggle != null)
                textSpeedNoWaitToggle.onValueChanged.AddListener((val) => { if (val) OnTextSpeedChanged(GameSettingsDataSO.TextSpeed.NoWait); });
        }

        private void SetupAutoModeTextSpeedToggles()
        {
            if (autoModeTextSpeedGroup == null) return;

            // Set active toggle based on current setting
            switch (settingsData.autoModeTextSpeed)
            {
                case GameSettingsDataSO.AutoTextSpeed.Off:
                    if (autoModeOffToggle != null) autoModeOffToggle.isOn = true;
                    break;
                case GameSettingsDataSO.AutoTextSpeed.Slow:
                    if (autoModeSlowToggle != null) autoModeSlowToggle.isOn = true;
                    break;
                case GameSettingsDataSO.AutoTextSpeed.Normal:
                    if (autoModeNormalToggle != null) autoModeNormalToggle.isOn = true;
                    break;
                case GameSettingsDataSO.AutoTextSpeed.Fast:
                    if (autoModeFastToggle != null) autoModeFastToggle.isOn = true;
                    break;
            }

            // Add listeners
            if (autoModeOffToggle != null)
                autoModeOffToggle.onValueChanged.AddListener((val) => { if (val) OnAutoModeTextSpeedChanged(GameSettingsDataSO.AutoTextSpeed.Off); });
            if (autoModeSlowToggle != null)
                autoModeSlowToggle.onValueChanged.AddListener((val) => { if (val) OnAutoModeTextSpeedChanged(GameSettingsDataSO.AutoTextSpeed.Slow); });
            if (autoModeNormalToggle != null)
                autoModeNormalToggle.onValueChanged.AddListener((val) => { if (val) OnAutoModeTextSpeedChanged(GameSettingsDataSO.AutoTextSpeed.Normal); });
            if (autoModeFastToggle != null)
                autoModeFastToggle.onValueChanged.AddListener((val) => { if (val) OnAutoModeTextSpeedChanged(GameSettingsDataSO.AutoTextSpeed.Fast); });
        }

        private void BuildKeyConfigList()
        {
            if (keyConfigContainer == null || keyConfigRowPrefab == null || inputManager == null) return;

            // Clear existing rows
            foreach (Transform child in keyConfigContainer)
            {
                Destroy(child.gameObject);
            }
            _keyConfigRows.Clear();

            // Get actions from InputActionManager using reflection
            var field = typeof(InputActionManager).GetField("actions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var actions = field?.GetValue(inputManager) as List<InputActionSO>;
            if (actions == null) return;

            foreach (var action in actions)
            {
                if (action == null || string.IsNullOrEmpty(action.displayName)) continue;
                CreateKeyConfigRow(action);
            }
        }

        private void CreateKeyConfigRow(InputActionSO action)
        {
            if (keyConfigContainer == null || keyConfigRowPrefab == null) return;

            var row = Instantiate(keyConfigRowPrefab, keyConfigContainer);
            var texts = row.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (texts.Length > 0)
            {
                texts[0].text = action.displayName;
            }

            var button = row.GetComponentInChildren<Button>(true);
            if (button != null)
            {
                var buttonText = button.GetComponentInChildren<TextMeshProUGUI>(true);
                if (buttonText != null)
                {
                    buttonText.text = action.key.ToString();
                }
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => BeginRebind(action, button));
            }

            _keyConfigRows[action.actionId] = row;
        }

        private void UpdateKeyConfigRow(InputActionSO action)
        {
            if (!_keyConfigRows.TryGetValue(action.actionId, out var row)) return;

            var button = row.GetComponentInChildren<Button>(true);
            if (button != null)
            {
                var buttonText = button.GetComponentInChildren<TextMeshProUGUI>(true);
                if (buttonText != null)
                {
                    buttonText.text = action.key.ToString();
                }
            }
        }

        private void BeginRebind(InputActionSO action, Button button)
        {
            _pendingAction = action;
            _waitingForKey = true;
            if (button != null)
            {
                var buttonText = button.GetComponentInChildren<TextMeshProUGUI>(true);
                if (buttonText != null)
                {
                    buttonText.text = "Press any key...";
                }
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

        // Event Handlers
        private void OnFullScreenChanged(bool value)
        {
            if (value)
            {
                settingsData.isFullScreen = true;
                Screen.fullScreen = true;
                if (windowedToggle != null) windowedToggle.isOn = false;
            }
        }

        private void OnWindowedChanged(bool value)
        {
            if (value)
            {
                settingsData.isFullScreen = false;
                Screen.fullScreen = false;
                if (fullScreenToggle != null) fullScreenToggle.isOn = false;
            }
        }

        private void OnBGMVolumeChanged(float value)
        {
            int intValue = Mathf.RoundToInt(value);
            settingsData.SetBGMVolume(value / 100f);
            if (bgmValueText != null) bgmValueText.text = intValue.ToString();
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMusicVolume(settingsData.GetBGMVolumeFloat());
            }
        }

        private void OnSEVolumeChanged(float value)
        {
            int intValue = Mathf.RoundToInt(value);
            settingsData.SetSEVolume(value / 100f);
            if (seValueText != null) seValueText.text = intValue.ToString();
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(settingsData.GetSEVolumeFloat());
            }
        }

        private void OnGameVoicesVolumeChanged(float value)
        {
            int intValue = Mathf.RoundToInt(value);
            settingsData.SetGameVoicesVolume(value / 100f);
            if (gameVoicesValueText != null) gameVoicesValueText.text = intValue.ToString();
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetGameVoicesVolume(settingsData.GetGameVoicesVolumeFloat());
            }
        }

        private void OnEventVoicesVolumeChanged(float value)
        {
            int intValue = Mathf.RoundToInt(value);
            settingsData.SetEventVoicesVolume(value / 100f);
            if (eventVoicesValueText != null) eventVoicesValueText.text = intValue.ToString();
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetEventVoicesVolume(settingsData.GetEventVoicesVolumeFloat());
            }
        }

        private void OnTextSpeedChanged(GameSettingsDataSO.TextSpeed speed)
        {
            settingsData.textSpeed = speed;
            PlayerPrefs.SetInt("TextSpeed", (int)speed);
        }

        private void OnAutoModeTextSpeedChanged(GameSettingsDataSO.AutoTextSpeed speed)
        {
            settingsData.autoModeTextSpeed = speed;
            PlayerPrefs.SetInt("AutoModeTextSpeed", (int)speed);
        }

        private void OnContinuePlayingVoicesChanged(bool value)
        {
            settingsData.continuePlayingVoices = value;
            PlayerPrefs.SetInt("ContinuePlayingVoices", value ? 1 : 0);
        }

        private void OnLanguageChanged(int index)
        {
            settingsData.currentLanguage = (GameSettingsDataSO.Language)index;
            PlayerPrefs.SetInt("CurrentLanguage", index);
            // TODO: Implement language change system
        }

        private void OnConfirmClicked()
        {
            SaveSettings();
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        private void OnDefaultClicked()
        {
            if (settingsData != null)
            {
                settingsData.ResetToDefaults();
                LoadSettings();
                SetupUI();
            }
        }

        private void SaveSettings()
        {
            if (settingsData == null) return;

            PlayerPrefs.SetInt("IsFullScreen", settingsData.isFullScreen ? 1 : 0);
            PlayerPrefs.SetInt("BGMVolume", settingsData.bgmVolume);
            PlayerPrefs.SetInt("SEVolume", settingsData.seVolume);
            PlayerPrefs.SetInt("GameVoicesVolume", settingsData.gameVoicesVolume);
            PlayerPrefs.SetInt("EventVoicesVolume", settingsData.eventVoicesVolume);
            PlayerPrefs.SetInt("TextSpeed", (int)settingsData.textSpeed);
            PlayerPrefs.SetInt("AutoModeTextSpeed", (int)settingsData.autoModeTextSpeed);
            PlayerPrefs.SetInt("ContinuePlayingVoices", settingsData.continuePlayingVoices ? 1 : 0);
            PlayerPrefs.SetInt("CurrentLanguage", (int)settingsData.currentLanguage);
            PlayerPrefs.Save();
        }

        // Main Menu Button Handlers
        private void OnNewGameClicked()
        {
            // TODO: Implement new game
            Debug.Log("New Game clicked");
        }

        private void OnLoadGameClicked()
        {
            // TODO: Implement load game
            Debug.Log("Load Game clicked");
        }

        private void OnGalleryClicked()
        {
            // TODO: Implement gallery
            Debug.Log("Gallery clicked");
        }

        private void OnConfigClicked()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }

        private void OnExitGameClicked()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public void ShowSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }

        public void HideSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }
    }
}

