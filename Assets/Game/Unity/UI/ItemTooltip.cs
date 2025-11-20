using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Item Tooltip - Hiển thị description khi hover vào item
    /// Tự động di chuyển theo mouse position
    /// </summary>
    public class ItemTooltip : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDescription;
        [SerializeField] private TextMeshProUGUI itemQuantity;
        [SerializeField] private RectTransform tooltipRect;

        [Header("Settings")]
        [SerializeField] private float offsetX = 10f;
        [SerializeField] private float offsetY = -10f;
#pragma warning disable CS0414 // Field is assigned but never used (may be used in future or set from Inspector)
        [SerializeField] private float padding = 10f;
#pragma warning restore CS0414

        private Canvas canvas;
        private Camera uiCamera;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                uiCamera = canvas.worldCamera;
            }
            else
            {
                uiCamera = null;
            }

            if (tooltipRect == null)
            {
                tooltipRect = GetComponent<RectTransform>();
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Show tooltip với item data
        /// </summary>
        public void Show(ItemDataSO itemData, int quantity, Vector2 mousePosition)
        {
            if (itemData == null)
            {
                Hide();
                return;
            }

            gameObject.SetActive(true);

            // Update content
            if (itemIcon != null)
            {
                itemIcon.sprite = itemData.icon;
                itemIcon.color = Color.white;
            }

            if (itemName != null)
            {
                itemName.text = itemData.itemName;
            }

            if (itemDescription != null)
            {
                itemDescription.text = itemData.description;
            }

            if (itemQuantity != null)
            {
                if (quantity > 1)
                {
                    itemQuantity.text = $"Quantity: {quantity}";
                    itemQuantity.gameObject.SetActive(true);
                }
                else
                {
                    itemQuantity.gameObject.SetActive(false);
                }
            }

            // Update position
            UpdatePosition(mousePosition);
        }

        /// <summary>
        /// Hide tooltip
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Update tooltip position theo mouse
        /// </summary>
        public void UpdatePosition(Vector2 mousePosition)
        {
            if (tooltipRect == null || canvas == null) return;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePosition,
                uiCamera,
                out localPoint
            );

            // Convert to canvas space
            localPoint = canvas.transform.TransformPoint(localPoint);

            // Get screen bounds
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 tooltipSize = tooltipRect.sizeDelta;

            // Calculate position with offset
            Vector2 targetPosition = mousePosition + new Vector2(offsetX, offsetY);

            // Keep tooltip on screen
            if (targetPosition.x + tooltipSize.x > screenSize.x)
            {
                targetPosition.x = mousePosition.x - tooltipSize.x - offsetX;
            }

            if (targetPosition.y - tooltipSize.y < 0)
            {
                targetPosition.y = mousePosition.y + tooltipSize.y - offsetY;
            }

            // Convert back to local space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                targetPosition,
                uiCamera,
                out localPoint
            );

            tooltipRect.anchoredPosition = localPoint;
        }

        private void Update()
        {
            // Update position nếu đang hiển thị
            if (gameObject.activeSelf)
            {
                Vector2 mousePos = UnityEngine.Input.mousePosition;
                UpdatePosition(mousePos);
            }
        }
    }
}

