using UnityEngine;
using UnityEngine.SceneManagement;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Unity.Audio;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Title Screen Controller - Quản lý màn hình tiêu đề (Title Screen)
    /// </summary>
    public class TitleScreenController : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject settingsPanel;
        public GameObject creditsPanel;
        public GameObject tutorialPanel;

        [Header("Load Game")]
        [Tooltip("SaveSlotListController để hiển thị danh sách save files")]
        public SaveSlotListController saveSlotListController;

        [Header("References")]
        public SceneLoader sceneLoader;
        public TutorialController tutorialController;

        private void Start()
        {
            if (sceneLoader == null)
                sceneLoader = FindFirstObjectByType<SceneLoader>();

            // Đảm bảo time scale = 1
            Time.timeScale = 1f;
        }

        public void OnStartClicked()
        {
            PlayButtonSound();
            // Load Intro scene trước, sau đó Intro sẽ tự động load Game
            if (sceneLoader != null)
            {
                sceneLoader.LoadScene("Intro");
            }
            else
            {
                SceneManager.LoadScene("Intro");
            }
        }

        public void OnLoadGameClicked()
        {
            PlayButtonSound();
            // Hiển thị SaveSlotListController với mode Load
            if (saveSlotListController != null)
            {
                saveSlotListController.ShowPanel(SaveSlotListController.SaveSlotMode.Load);
                
                // Setup callback khi chọn slot
                saveSlotListController.OnSlotSelected = OnSaveSlotSelected;
                saveSlotListController.OnBackClicked = OnLoadGameBackClicked;
            }
            else
            {
                Debug.LogWarning("SaveSlotListController not found! Please assign in Inspector.");
            }
        }

        /// <summary>
        /// Xử lý khi chọn save slot để load
        /// </summary>
        private void OnSaveSlotSelected(int slotIndex)
        {
            var saveManager = SaveLoad.SaveLoadManager.Instance;
            if (saveManager == null)
            {
                saveManager = FindFirstObjectByType<SaveLoad.SaveLoadManager>();
            }

            if (saveManager != null)
            {
                // Load game từ slot
                var loadedState = saveManager.LoadGameFromSlot(slotIndex);
                if (loadedState != null)
                {
                    // Load scene từ save state
                    if (!string.IsNullOrEmpty(loadedState.currentSceneName))
                    {
                        if (sceneLoader != null)
                        {
                            sceneLoader.LoadScene(loadedState.currentSceneName);
                        }
                        else
                        {
                            SceneManager.LoadScene(loadedState.currentSceneName);
                        }
                    }
                    else
                    {
                        // Nếu không có scene name, load game scene mặc định
                        OnStartClicked();
                    }
                }
                else
                {
                    Debug.LogWarning($"Failed to load game from slot {slotIndex}");
                }
            }
            else
            {
                Debug.LogError("SaveLoadManager not found!");
            }
        }

        /// <summary>
        /// Xử lý khi click Back trong Load Game panel
        /// </summary>
        private void OnLoadGameBackClicked()
        {
            // Đóng panel (SaveSlotListController sẽ tự đóng)
        }

        public void OnSettingsClicked()
        {
            PlayButtonSound();
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }

        public void OnTutorialClicked()
        {
            PlayButtonSound();
            // Ưu tiên dùng TutorialController nếu có
            if (tutorialController != null)
            {
                tutorialController.ShowTutorial();
            }
            // Nếu không có TutorialController, dùng tutorialPanel trực tiếp
            else if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }
            else
            {
                // Tự động tìm TutorialController trong scene
                tutorialController = FindFirstObjectByType<TutorialController>();
                if (tutorialController != null)
                {
                    tutorialController.ShowTutorial();
                }
                else
                {
                    Debug.LogWarning("TutorialPanel or TutorialController not found! Please assign one in Inspector.");
                }
            }
        }

        public void OnCreditsClicked()
        {
            PlayButtonSound();
            if (creditsPanel != null)
                creditsPanel.SetActive(true);
        }

        public void OnQuitClicked()
        {
            PlayButtonSound();
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        private void PlayButtonSound()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }
        }
    }
}

