using UnityEngine;
using UnityEngine.SceneManagement;
using TheGreenMemoir.Unity.Managers;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// [DEPRECATED] Main Menu Controller - Dùng TitleScreenController thay thế
    /// </summary>
    [System.Obsolete("Use TitleScreenController instead")]
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject settingsPanel;
        public GameObject creditsPanel;
        public GameObject tutorialPanel;

        [Header("References")]
        public SceneLoader sceneLoader;
        public TutorialController tutorialController;

        private void Start()
        {
            if (sceneLoader == null)
                sceneLoader = FindObjectOfType<SceneLoader>();

            // Đảm bảo time scale = 1
            Time.timeScale = 1f;
        }

        public void OnStartClicked()
        {
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
            // Load game từ save file
            var saveManager = FindObjectOfType<SaveLoad.SaveLoadManager>();
            if (saveManager != null && saveManager.HasSaveFile())
            {
                saveManager.QuickLoad();
                OnStartClicked(); // Load xong thì vào game
            }
            else
            {
                Debug.LogWarning("No save file found!");
            }
        }

        public void OnSettingsClicked()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }

        public void OnTutorialClicked()
        {
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
                tutorialController = FindObjectOfType<TutorialController>();
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
            if (creditsPanel != null)
                creditsPanel.SetActive(true);
        }

        public void OnQuitClicked()
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}

