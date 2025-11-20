using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Story Controller - Hiển thị story từ StorySO
    /// Tự động đọc từ SO, không cần code
    /// </summary>
    public class StoryController : MonoBehaviour
    {
        [Header("Story SO")]
        [Tooltip("Kéo StorySO vào đây. Nếu để trống, sẽ tự động tìm")]
        [SerializeField] private StorySO storySO;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI storyText;
        [SerializeField] private GameObject skipHint; // "Nhấn Space để bỏ qua"

        private void Start()
        {
            // Tìm StorySO nếu chưa gán
            if (storySO == null)
            {
                #if UNITY_EDITOR
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:StorySO");
                if (guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    storySO = UnityEditor.AssetDatabase.LoadAssetAtPath<StorySO>(path);
                }
                #endif
            }

            if (storySO != null)
            {
                StartCoroutine(ShowStory());
            }
            else
            {
                Debug.LogWarning("StorySO not found! Loading next scene...");
                SceneManager.LoadScene("Game");
            }
        }

        IEnumerator ShowStory()
        {
            storyText.text = "";
            
            // Type từng chữ (có thể tua nhanh)
            foreach (char c in storySO.storyText)
            {
                storyText.text += c;
                
                // Đợi với tốc độ có thể thay đổi khi giữ phím
                float targetWaitTime = storySO.typingSpeed;
                float elapsedTime = 0f;
                
                while (elapsedTime < targetWaitTime)
                {
                    float deltaTime = Time.deltaTime;
                    if (UnityEngine.Input.GetKey(storySO.skipKey))
                    {
                        deltaTime *= storySO.fastForwardSpeedMultiplier;
                    }
                    elapsedTime += deltaTime;
                    yield return null;
                }
            }
            
            // Đợi (có thể tua nhanh)
            float waitTime = storySO.waitAfterTyping;
            float elapsedWaitTime = 0f;
            while (elapsedWaitTime < waitTime)
            {
                float deltaTime = Time.deltaTime;
                if (UnityEngine.Input.GetKey(storySO.skipKey))
                {
                    deltaTime *= storySO.fastForwardSpeedMultiplier;
                }
                elapsedWaitTime += deltaTime;
                yield return null;
            }
            
            // Fade out (có thể tua nhanh)
            float alpha = 1f;
            while (alpha > 0f)
            {
                float fadeSpeed = storySO.fadeSpeed;
                if (UnityEngine.Input.GetKey(storySO.skipKey))
                {
                    fadeSpeed *= storySO.fastForwardSpeedMultiplier;
                }
                alpha -= Time.deltaTime * fadeSpeed;
                storyText.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            
            // Load next scene
            SceneManager.LoadScene(storySO.nextSceneName);
        }

        void Update()
        {
            // Không còn skip ngay lập tức nữa, chỉ tua nhanh khi giữ phím
            // Logic tua nhanh được xử lý trong coroutine ShowStory()
        }
    }
}

