using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Enums;
using ToolActionType = TheGreenMemoir.Core.Domain.Enums.ToolActionType;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Tool Selection UI - Hiển thị và chọn tool
    /// Có ô tool được chọn mặc định và mũi tên tam giác để mở tool selection
    /// </summary>
    public class ToolSelectionUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Panel chứa tool selection (sẽ hiện/ẩn khi click mũi tên)")]
        [SerializeField] private GameObject toolSelectionPanel;
        
        [Tooltip("Button mũi tên tam giác để mở/đóng tool selection")]
        [SerializeField] private Button expandArrowButton;
        
        [Tooltip("Container chứa các tool buttons")]
        [SerializeField] private Transform toolContainer;
        
        [Tooltip("Button template cho tool (sẽ duplicate)")]
        [SerializeField] private Button toolButtonTemplate;
        
        [Tooltip("Ô tool được chọn (hiển thị tool hiện tại)")]
        [SerializeField] private Image selectedToolSlot;
        
        [Tooltip("Icon của tool được chọn")]
        [SerializeField] private Image selectedToolIcon;
        
        [Tooltip("Text hiển thị tên tool được chọn (optional)")]
        [SerializeField] private TextMeshProUGUI selectedToolNameText;

        [Header("References")]
        [Tooltip("Tool Interaction System")]
        [SerializeField] private Presentation.ToolInteractionSystem toolSystem;
        
        [Tooltip("Tool Description UI (hiển thị mô tả khi click vào tool đang chọn)")]
        [SerializeField] private ToolDescriptionUI toolDescriptionUI;
        
        [Tooltip("Button click vào ô tool được chọn (để hiển thị mô tả)")]
        [SerializeField] private Button selectedToolSlotButton;
        
        [Tooltip("Button đóng panel chọn tool (mũi tên trong panel)")]
        [SerializeField] private Button closePanelButton;
        
        [Tooltip("Button để deselect tool (thu lại tool, không chọn tool nào) - OPTIONAL, không bắt buộc")]
        [SerializeField] private Button deselectToolButton;

        [Header("Settings")]
        [Tooltip("Tool được chọn mặc định (null = không có tool)")]
        [SerializeField] private ToolDataSO defaultSelectedTool = null;
        
        [Tooltip("Danh sách tools có sẵn (có thể set trong Inspector hoặc tự động lấy từ GameDatabase)")]
        [SerializeField] private List<ToolDataSO> availableTools = new List<ToolDataSO>();

        private ToolDataSO currentSelectedTool = null;
        private List<Button> toolButtons = new List<Button>();
        private bool isPanelOpen = false;

        void Start()
        {
            // Ẩn panel mặc định
            if (toolSelectionPanel != null)
                toolSelectionPanel.SetActive(false);

            // Ẩn template button
            if (toolButtonTemplate != null)
                toolButtonTemplate.gameObject.SetActive(false);

            // Setup expand arrow button
            if (expandArrowButton != null)
            {
                expandArrowButton.onClick.AddListener(TogglePanel);
            }

            // Setup selected tool slot button (click để xem mô tả)
            if (selectedToolSlotButton != null)
            {
                selectedToolSlotButton.onClick.AddListener(OnSelectedToolSlotClicked);
            }

            // Setup close panel button (đóng panel chọn tool)
            if (closePanelButton != null)
            {
                closePanelButton.onClick.AddListener(ClosePanel);
            }

            // Setup deselect tool button (thu lại tool) - OPTIONAL
            if (deselectToolButton != null)
            {
                deselectToolButton.onClick.AddListener(DeselectTool);
                // Ẩn nút deselect mặc định (không bắt buộc)
                deselectToolButton.gameObject.SetActive(false);
            }

            // Tự động tìm tool system
            if (toolSystem == null)
                toolSystem = FindFirstObjectByType<Presentation.ToolInteractionSystem>();

            // Tự động tìm tool description UI
            if (toolDescriptionUI == null)
                toolDescriptionUI = FindFirstObjectByType<ToolDescriptionUI>();

            // Load tools từ GameDatabase nếu chưa có
            if (availableTools == null || availableTools.Count == 0)
            {
                LoadToolsFromDatabase();
            }

            // Set tool mặc định
            if (defaultSelectedTool != null)
            {
                SelectTool(defaultSelectedTool);
            }
            else
            {
                // Không có tool mặc định - hiển thị ô trống
                UpdateSelectedToolSlot(null);
            }

            // Refresh tool list
            RefreshToolList();
        }

        /// <summary>
        /// Load tools từ GameDatabase
        /// </summary>
        private void LoadToolsFromDatabase()
        {
            availableTools = new List<ToolDataSO>();
            
            var database = GameDatabaseManager.GetDatabase();
            if (database != null && database.tools != null)
            {
                foreach (var tool in database.tools)
                {
                    if (tool != null)
                    {
                        availableTools.Add(tool);
                    }
                }
            }
        }

        /// <summary>
        /// Mở/đóng tool selection panel
        /// </summary>
        public void TogglePanel()
        {
            isPanelOpen = !isPanelOpen;
            
            if (toolSelectionPanel != null)
            {
                toolSelectionPanel.SetActive(isPanelOpen);
            }

            if (isPanelOpen)
            {
                RefreshToolList();
            }
        }

        /// <summary>
        /// Đóng tool selection panel (không deselect tool)
        /// </summary>
        public void ClosePanel()
        {
            isPanelOpen = false;
            
            if (toolSelectionPanel != null)
            {
                toolSelectionPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Mở tool selection panel
        /// </summary>
        public void OpenPanel()
        {
            isPanelOpen = true;
            
            if (toolSelectionPanel != null)
            {
                toolSelectionPanel.SetActive(true);
            }

            RefreshToolList();
        }

        /// <summary>
        /// Làm mới danh sách tools
        /// </summary>
        public void RefreshToolList()
        {
            if (toolContainer == null || toolButtonTemplate == null)
                return;

            // Xóa các button cũ
            ClearToolButtons();

            // Tạo button cho mỗi tool
            foreach (var tool in availableTools)
            {
                if (tool == null)
                    continue;

                // Tạo button từ template
                Button toolButton = Instantiate(toolButtonTemplate, toolContainer);
                toolButton.gameObject.SetActive(true);

                // Set icon
                Image iconImage = toolButton.GetComponentInChildren<Image>();
                if (iconImage != null && tool.icon != null)
                {
                    iconImage.sprite = tool.icon;
                }

                // Set text (nếu có)
                TextMeshProUGUI text = toolButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = tool.toolName;
                }

                // Add listener
                ToolDataSO toolData = tool; // Capture for closure
                toolButton.onClick.RemoveAllListeners();
                toolButton.onClick.AddListener(() => OnToolButtonClicked(toolData));

                toolButtons.Add(toolButton);
            }
        }

        /// <summary>
        /// Xử lý khi click vào tool button
        /// </summary>
        private void OnToolButtonClicked(ToolDataSO tool)
        {
            SelectTool(tool);
            ClosePanel(); // Đóng panel sau khi chọn tool
        }

        /// <summary>
        /// Chọn tool
        /// </summary>
        public void SelectTool(ToolDataSO tool)
        {
            currentSelectedTool = tool;
            UpdateSelectedToolSlot(tool);

            // Cập nhật tool system (dùng ToolDataSO thay vì chỉ ToolActionType)
            if (toolSystem != null && tool != null)
            {
                toolSystem.SetTool(tool); // Gọi method mới với ToolDataSO
            }
        }

        /// <summary>
        /// Cập nhật UI ô tool được chọn
        /// </summary>
        private void UpdateSelectedToolSlot(ToolDataSO tool)
        {
            if (selectedToolIcon != null)
            {
                if (tool != null && tool.icon != null)
                {
                    selectedToolIcon.sprite = tool.icon;
                    selectedToolIcon.gameObject.SetActive(true);
                }
                else
                {
                    selectedToolIcon.gameObject.SetActive(false);
                }
            }

            if (selectedToolNameText != null)
            {
                selectedToolNameText.text = tool != null ? tool.toolName : "No Tool";
            }
        }

        /// <summary>
        /// Xử lý khi click vào ô tool được chọn (hiển thị mô tả)
        /// </summary>
        public void OnSelectedToolSlotClicked()
        {
            if (currentSelectedTool != null && toolDescriptionUI != null)
            {
                toolDescriptionUI.ShowToolDescription(currentSelectedTool);
            }
        }

        /// <summary>
        /// Deselect tool (thu lại tool, không chọn tool nào)
        /// OPTIONAL: Tool không cần deselect vì chỉ tác dụng lên ground tiles
        /// </summary>
        public void DeselectTool()
        {
            currentSelectedTool = null;
            UpdateSelectedToolSlot(null);

            // Cập nhật tool system
            if (toolSystem != null)
            {
                toolSystem.SetTool(ToolActionType.None);
            }
        }

        /// <summary>
        /// Lấy tool hiện tại được chọn
        /// </summary>
        public ToolDataSO GetCurrentSelectedTool()
        {
            return currentSelectedTool;
        }

        /// <summary>
        /// Xóa tất cả tool buttons
        /// </summary>
        private void ClearToolButtons()
        {
            foreach (var button in toolButtons)
            {
                if (button != null)
                {
                    Destroy(button.gameObject);
                }
            }
            toolButtons.Clear();
        }
    }
}

