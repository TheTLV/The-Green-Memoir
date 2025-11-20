using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Core.Domain.Enums;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Tool Description UI - Hiển thị mô tả chi tiết của tool khi click vào tool đang chọn
    /// </summary>
    public class ToolDescriptionUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Panel chứa mô tả tool")]
        [SerializeField] private GameObject descriptionPanel;
        
        [Tooltip("Text hiển thị tên tool")]
        [SerializeField] private TextMeshProUGUI toolNameText;
        
        [Tooltip("Text hiển thị mô tả tool")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        
        [Tooltip("Text hiển thị đặc tính tương tác")]
        [SerializeField] private TextMeshProUGUI interactionText;
        
        [Tooltip("Text hiển thị energy cost")]
        [SerializeField] private TextMeshProUGUI energyCostText;
        
        [Tooltip("Button đóng panel")]
        [SerializeField] private Button closeButton;

        [Header("Settings")]
        [Tooltip("Tự động đóng khi click ra ngoài")]
        [SerializeField] private bool closeOnClickOutside = true;

        private void Start()
        {
            // Ẩn panel mặc định
            if (descriptionPanel != null)
                descriptionPanel.SetActive(false);

            // Setup close button
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }
        }

        void Update()
        {
            // Đóng khi click ra ngoài
            if (closeOnClickOutside && descriptionPanel != null && descriptionPanel.activeSelf)
            {
                if (UnityEngine.Input.GetMouseButtonDown(0))
                {
                    // Kiểm tra click có nằm trong panel không
                    RectTransform rectTransform = descriptionPanel.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        Vector2 mousePos = UnityEngine.Input.mousePosition;
                        if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePos))
                        {
                            Hide();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Hiển thị mô tả tool
        /// </summary>
        public void ShowToolDescription(ToolDataSO tool)
        {
            if (tool == null)
            {
                Debug.LogWarning("Tool is null!");
                return;
            }

            if (descriptionPanel != null)
            {
                descriptionPanel.SetActive(true);
            }

            // Cập nhật tên tool
            if (toolNameText != null)
            {
                toolNameText.text = tool.toolName;
            }

            // Cập nhật mô tả
            if (descriptionText != null)
            {
                descriptionText.text = tool.description;
            }

            // Cập nhật đặc tính tương tác
            if (interactionText != null)
            {
                string interactionInfo = tool.interactionDescription;
                if (string.IsNullOrEmpty(interactionInfo) && tool.canInteractWithTileStates != null && tool.canInteractWithTileStates.Count > 0)
                {
                    interactionInfo = "Can interact with: " + string.Join(", ", tool.canInteractWithTileStates);
                }
                interactionText.text = interactionInfo;
            }

            // Cập nhật energy cost
            if (energyCostText != null)
            {
                energyCostText.text = $"-{tool.energyCost} Stamina";
                energyCostText.color = Color.red; // Màu đỏ cho cost
            }
        }

        /// <summary>
        /// Ẩn panel
        /// </summary>
        public void Hide()
        {
            if (descriptionPanel != null)
            {
                descriptionPanel.SetActive(false);
            }
        }
    }
}

