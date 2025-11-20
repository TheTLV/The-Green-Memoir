using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Unity.SaveLoad;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Controller để hiển thị danh sách save slots
    /// Hỗ trợ Load và Save với multiple slots
    /// </summary>
    public class SaveSlotListController : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Panel chứa danh sách save slots")]
        [SerializeField] private GameObject saveSlotListPanel;
        
        [Tooltip("Container chứa các save slot buttons")]
        [SerializeField] private Transform saveSlotContainer;
        
        [Tooltip("Button template cho save slot (sẽ duplicate)")]
        [SerializeField] private Button saveSlotButtonTemplate;
        
        [Tooltip("Text hiển thị tiêu đề (ví dụ: 'Load Game' hoặc 'Save Game')")]
        [SerializeField] private TextMeshProUGUI titleText;
        
        [Tooltip("Button Back")]
        [SerializeField] private Button backButton;
        
        [Header("Settings")]
        [Tooltip("Mode: Load hoặc Save")]
        [SerializeField] private SaveSlotMode mode = SaveSlotMode.Load;
        
        /// <summary>
        /// Lấy mode hiện tại (public property)
        /// </summary>
        public SaveSlotMode Mode => mode;
        
        [Tooltip("Callback khi chọn slot để load/save")]
        public System.Action<int> OnSlotSelected;
        
        [Tooltip("Callback khi click Back")]
        public System.Action OnBackClicked;

        private List<Button> currentSlotButtons = new List<Button>();
        private SaveLoadManager saveManager;

        public enum SaveSlotMode
        {
            Load,  // Chế độ load (chỉ hiển thị slots có save)
            Save   // Chế độ save (hiển thị tất cả slots, có thể save đè)
        }

        private void Start()
        {
            saveManager = SaveLoadManager.Instance;
            
            if (saveSlotButtonTemplate != null)
            {
                saveSlotButtonTemplate.gameObject.SetActive(false);
            }
            
            if (backButton != null)
            {
                backButton.onClick.AddListener(() => 
                {
                    OnBackClicked?.Invoke();
                    HidePanel();
                });
            }
        }

        /// <summary>
        /// Hiển thị panel với mode cụ thể
        /// </summary>
        public void ShowPanel(SaveSlotMode panelMode)
        {
            mode = panelMode;
            
            if (saveSlotListPanel != null)
            {
                saveSlotListPanel.SetActive(true);
            }
            
            // Cập nhật tiêu đề
            if (titleText != null)
            {
                titleText.text = mode == SaveSlotMode.Load ? "Load Game" : "Save Game";
            }
            
            RefreshSaveSlots();
        }

        /// <summary>
        /// Ẩn panel
        /// </summary>
        public void HidePanel()
        {
            if (saveSlotListPanel != null)
            {
                saveSlotListPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Làm mới danh sách save slots
        /// </summary>
        public void RefreshSaveSlots()
        {
            if (saveManager == null)
            {
                Debug.LogError("SaveLoadManager not found!");
                return;
            }

            // Xóa các button cũ
            ClearSaveSlots();

            // Lấy danh sách save slots
            List<SaveSlotInfo> slots = saveManager.GetAllSaveSlots();

            // Tạo button cho mỗi slot
            if (saveSlotContainer != null && saveSlotButtonTemplate != null)
            {
                foreach (var slotInfo in slots)
                {
                    // Nếu là mode Load, chỉ hiển thị slots có save
                    if (mode == SaveSlotMode.Load && slotInfo.isEmpty)
                        continue;

                    // Tạo button từ template
                    Button slotButton = Instantiate(saveSlotButtonTemplate, saveSlotContainer);
                    slotButton.gameObject.SetActive(true);
                    
                    // Cập nhật text của button
                    UpdateSlotButton(slotButton, slotInfo);
                    
                    // Enable/disable button dựa trên mode
                    if (mode == SaveSlotMode.Load)
                    {
                        // Mode Load: chỉ enable nếu slot có save
                        slotButton.interactable = !slotInfo.isEmpty;
                    }
                    else
                    {
                        // Mode Save: enable tất cả slots (có thể save đè)
                        slotButton.interactable = true;
                    }
                    
                    // Add listener
                    int slotIndex = slotInfo.slotIndex; // Capture slot index
                    slotButton.onClick.RemoveAllListeners();
                    slotButton.onClick.AddListener(() => OnSlotButtonClicked(slotIndex));
                    
                    currentSlotButtons.Add(slotButton);
                }
            }
        }

        /// <summary>
        /// Cập nhật UI của slot button
        /// </summary>
        private void UpdateSlotButton(Button button, SaveSlotInfo slotInfo)
        {
            // Tìm các TextMeshProUGUI trong button
            TextMeshProUGUI[] texts = button.GetComponentsInChildren<TextMeshProUGUI>();
            
            if (slotInfo.isEmpty)
            {
                // Slot trống
                if (texts.Length > 0)
                {
                    texts[0].text = $"Slot {slotInfo.slotIndex + 1} - Empty";
                }
                if (texts.Length > 1)
                {
                    texts[1].text = "";
                }
                if (texts.Length > 2)
                {
                    texts[2].text = "";
                }
            }
            else
            {
                // Slot có save
                if (texts.Length > 0)
                {
                    string latestMarker = slotInfo.isLatest ? " [LATEST]" : "";
                    texts[0].text = $"Slot {slotInfo.slotIndex + 1}{latestMarker}";
                }
                if (texts.Length > 1)
                {
                    texts[1].text = $"Day {slotInfo.currentDay} - {slotInfo.GetPlayTimeFormatted()}";
                }
                if (texts.Length > 2)
                {
                    texts[2].text = slotInfo.GetSaveDateFormatted();
                }
            }
            
            // Có thể thêm icon hoặc màu sắc khác nhau
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                if (slotInfo.isLatest)
                {
                    // Màu vàng cho save gần nhất
                    buttonImage.color = new Color(1f, 1f, 0.8f, 1f);
                }
                else if (slotInfo.isEmpty)
                {
                    // Màu xám cho slot trống
                    buttonImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                }
                else
                {
                    // Màu trắng cho slot có save
                    buttonImage.color = Color.white;
                }
            }
        }

        /// <summary>
        /// Xử lý khi click vào slot button
        /// </summary>
        private void OnSlotButtonClicked(int slotIndex)
        {
            OnSlotSelected?.Invoke(slotIndex);
        }

        /// <summary>
        /// Xóa tất cả save slot buttons
        /// </summary>
        private void ClearSaveSlots()
        {
            foreach (var button in currentSlotButtons)
            {
                if (button != null)
                {
                    Destroy(button.gameObject);
                }
            }
            currentSlotButtons.Clear();
        }
    }
}

