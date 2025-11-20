using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// UI Button Hover Animation - Tạo animation khi hover button
    /// Dùng Coroutine thuần, không phụ thuộc DOTween
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class UIButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Settings")]
        [Tooltip("Scale khi hover")]
        [SerializeField] private float hoverScale = 1.05f;
        
        [Tooltip("Thời gian animation (giây)")]
        [SerializeField] private float animationDuration = 0.2f;

        private RectTransform rectTransform;
        private Vector3 originalScale;
        private Coroutine currentAnimation;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                originalScale = rectTransform.localScale;
            }
        }

        public void Setup(float scale)
        {
            hoverScale = scale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (rectTransform != null)
            {
                // Dừng animation hiện tại nếu có
                if (currentAnimation != null)
                {
                    StopCoroutine(currentAnimation);
                }
                
                // Bắt đầu animation scale lên
                currentAnimation = StartCoroutine(ScaleCoroutine(originalScale * hoverScale));
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (rectTransform != null)
            {
                // Dừng animation hiện tại nếu có
                if (currentAnimation != null)
                {
                    StopCoroutine(currentAnimation);
                }
                
                // Bắt đầu animation scale về
                currentAnimation = StartCoroutine(ScaleCoroutine(originalScale));
            }
        }

        private IEnumerator ScaleCoroutine(Vector3 targetScale)
        {
            Vector3 startScale = rectTransform.localScale;
            float elapsed = 0f;

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                
                // Ease out (smooth animation)
                t = 1f - Mathf.Pow(1f - t, 3f); // Cubic ease out
                
                rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }

            rectTransform.localScale = targetScale;
            currentAnimation = null;
        }
    }
}

