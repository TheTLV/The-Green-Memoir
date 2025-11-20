using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace TheGreenMemoir.Unity.Managers
{
    /// <summary>
    /// Quản lý load scene với loading screen
    /// Hỗ trợ async loading để không freeze game
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader _instance;
        public static SceneLoader Instance => _instance;

        [Header("Loading UI")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private Slider loadingBar;
        [SerializeField] private TMPro.TextMeshProUGUI loadingText; // Dùng TMPro hoặc UnityEngine.UI.Text
        [SerializeField] private Image loadingBackground;

        [Header("Settings")]
        [SerializeField] private float minLoadingTime = 1f; // Tối thiểu hiển thị loading screen (giây)
        [SerializeField] private bool showProgress = true;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (loadingPanel != null)
                loadingPanel.SetActive(false);
        }

        /// <summary>
        /// Load scene với loading screen
        /// </summary>
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        /// <summary>
        /// Load scene với loading screen (async)
        /// </summary>
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            // Hiển thị loading screen
            if (loadingPanel != null)
                loadingPanel.SetActive(true);

            if (loadingBar != null)
                loadingBar.value = 0f;

            if (loadingText != null)
                loadingText.text = "Loading...";

            float startTime = Time.time;

            // Load scene async
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false; // Không tự động chuyển scene

            // Đợi load
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f); // 0.9 = 90% load xong

                if (loadingBar != null && showProgress)
                    loadingBar.value = progress;

                if (loadingText != null && showProgress)
                    loadingText.text = $"Loading... {(progress * 100):F0}%";

                // Đợi đến khi load 90% và đã đủ thời gian tối thiểu
                if (operation.progress >= 0.9f && (Time.time - startTime) >= minLoadingTime)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            // Ẩn loading screen sau khi load xong
            yield return new WaitForSeconds(0.1f);
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
        }

        /// <summary>
        /// Load scene ngay lập tức (không loading screen)
        /// </summary>
        public void LoadSceneImmediate(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Reload scene hiện tại
        /// </summary>
        public void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Quay về Title Screen (không hardcode, dùng từ GameSettingsSO nếu có)
        /// </summary>
        public void LoadTitleScreen()
        {
            // Có thể lấy từ GameSettingsSO.titleScreenScene nếu có
            LoadScene("TitleScreen");
        }

        /// <summary>
        /// [DEPRECATED] Quay về Main Menu - Dùng LoadTitleScreen() thay thế
        /// </summary>
        [System.Obsolete("Use LoadTitleScreen() instead")]
        public void LoadMainMenu()
        {
            LoadTitleScreen();
        }

        /// <summary>
        /// Load Game scene (không hardcode, dùng từ GameSettingsSO nếu có)
        /// </summary>
        public void LoadGame()
        {
            // Có thể lấy từ GameSettingsSO.gameScene nếu có
            LoadScene("Game");
        }
    }
}

