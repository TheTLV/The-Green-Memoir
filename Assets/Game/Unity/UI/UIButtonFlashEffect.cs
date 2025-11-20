using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// UI Button Flash Effect - Tạo hiệu ứng flash khi click button
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class UIButtonFlashEffect : MonoBehaviour, IPointerClickHandler
    {
        [Header("Settings")]
        [Tooltip("Màu flash")]
        [SerializeField] private Color flashColor = new Color(1f, 1f, 0.8f);
        
        [Tooltip("Thời gian flash (giây)")]
        [SerializeField] private float flashDuration = 0.1f;

        private Image buttonImage;
        private Color originalColor;

        void Awake()
        {
            buttonImage = GetComponent<Image>();
            if (buttonImage != null)
            {
                originalColor = buttonImage.color;
            }
        }

        public void Setup(Color color)
        {
            flashColor = color;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (buttonImage != null)
            {
                StartCoroutine(FlashCoroutine());
            }
        }

        private System.Collections.IEnumerator FlashCoroutine()
        {
            // Flash
            buttonImage.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            
            // Trở về màu gốc
            buttonImage.color = originalColor;
        }
    }
}

