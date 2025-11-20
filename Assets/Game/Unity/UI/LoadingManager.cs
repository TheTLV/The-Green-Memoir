using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using TheGreenMemoir.Unity.Managers;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Loading Manager - Quản lý loading screen
    /// Dùng cho scene Loading riêng
    /// </summary>
    public class LoadingManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider loadingBar;
        [SerializeField] private TextMeshProUGUI loadingText; // Dùng TMPro hoặc UnityEngine.UI.Text
        [SerializeField] private Image loadingBackground;

        [Header("Settings")]
        [SerializeField] private float minLoadingTime = 1f;

        private void Start()
        {
            string sceneToLoad = PlayerPrefs.GetString("SceneToLoad", "Game");
            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (loadingBar != null)
                loadingBar.value = 0f;

            if (loadingText != null)
                loadingText.text = "Loading...";

            float startTime = Time.time;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                if (loadingBar != null)
                    loadingBar.value = progress;

                if (loadingText != null)
                    loadingText.text = $"Loading... {(progress * 100):F0}%";

                if (operation.progress >= 0.9f && (Time.time - startTime) >= minLoadingTime)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}

