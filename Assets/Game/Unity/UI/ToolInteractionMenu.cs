using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Enums;
using TheGreenMemoir.Core.Domain.Entities;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Tool Interaction Menu - Bảng tương tác cho tool đặc biệt (ví dụ: găng tay để gieo hạt)
    /// Hiển thị danh sách items từ inventory (filter theo tag), dài hay ngắn tùy số lượng items
    /// </summary>
    public class ToolInteractionMenu : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Panel chứa menu tương tác")]
        [SerializeField] private GameObject interactionPanel;
        
        [Tooltip("Container chứa các item buttons")]
        [SerializeField] private Transform itemContainer;
        
        [Tooltip("Button template cho item (sẽ duplicate)")]
        [SerializeField] private Button itemButtonTemplate;
        
        [Tooltip("Text hiển thị tiêu đề (ví dụ: 'Select Seed')")]
        [SerializeField] private TextMeshProUGUI titleText;
        
        [Tooltip("Button Leave (hiển thị khi không có items)")]
        [SerializeField] private Button leaveButton;

        [Header("Settings")]
        [Tooltip("Player ID")]
        [SerializeField] private PlayerId playerId = PlayerId.Default;

        private List<Button> itemButtons = new List<Button>();
        private System.Action<ItemId> onItemSelected;
        private System.Action onLeaveClicked;

        private void Start()
        {
            // Ẩn panel mặc định
            if (interactionPanel != null)
                interactionPanel.SetActive(false);

            // Ẩn template button
            if (itemButtonTemplate != null)
                itemButtonTemplate.gameObject.SetActive(false);

            // Setup leave button
            if (leaveButton != null)
            {
                leaveButton.onClick.AddListener(() =>
                {
                    onLeaveClicked?.Invoke();
                    Hide();
                });
            }
        }

        /// <summary>
        /// Hiển thị menu tương tác với items từ inventory
        /// </summary>
        public void ShowInteractionMenu(ItemTag filterTag, string title, System.Action<ItemId> onItemSelected, System.Action onLeaveClicked)
        {
            this.onItemSelected = onItemSelected;
            this.onLeaveClicked = onLeaveClicked;

            // Cập nhật title
            if (titleText != null)
            {
                titleText.text = title;
            }

            // Lấy items từ inventory
            var inventory = GameManager.InventoryService?.GetInventory(playerId);
            if (inventory == null)
            {
                Debug.LogWarning("Inventory not found!");
                ShowOnlyLeave();
                return;
            }

            // Filter items theo tag
            var filteredItems = new List<Item>();
            foreach (var slot in inventory.GetAllSlots())
            {
                if (slot != null && !slot.IsEmpty && slot.Item != null)
                {
                    if (filterTag == ItemTag.None || slot.Item.Tags.HasTag(filterTag))
                    {
                        filteredItems.Add(slot.Item);
                    }
                }
            }

            // Hiển thị panel
            if (interactionPanel != null)
            {
                interactionPanel.SetActive(true);
            }

            // Nếu không có items, chỉ hiển thị Leave
            if (filteredItems.Count == 0)
            {
                ShowOnlyLeave();
                return;
            }

            // Hiển thị danh sách items
            ShowItems(filteredItems, inventory);
        }

        /// <summary>
        /// Hiển thị chỉ Leave button (khi không có items)
        /// </summary>
        private void ShowOnlyLeave()
        {
            ClearItemButtons();
            
            if (leaveButton != null)
            {
                leaveButton.gameObject.SetActive(true);
            }

            if (itemContainer != null)
            {
                itemContainer.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Hiển thị danh sách items
        /// </summary>
        private void ShowItems(List<Item> items, Core.Domain.Entities.Inventory inventory)
        {
            ClearItemButtons();

            if (itemContainer != null)
            {
                itemContainer.gameObject.SetActive(true);
            }

            if (leaveButton != null)
            {
                leaveButton.gameObject.SetActive(true);
            }

            var database = GameDatabaseManager.GetDatabase();
            if (database == null)
            {
                Debug.LogError("GameDatabase not found!");
                return;
            }

            // Tạo button cho mỗi item
            foreach (var item in items)
            {
                var itemData = database.GetItemData(item.Id);
                if (itemData == null)
                    continue;

                int quantity = inventory.GetQuantity(item.Id);

                // Tạo button từ template
                Button itemButton = Instantiate(itemButtonTemplate, itemContainer);
                itemButton.gameObject.SetActive(true);

                // Set icon
                Image iconImage = itemButton.GetComponentInChildren<Image>();
                if (iconImage != null && itemData.icon != null)
                {
                    iconImage.sprite = itemData.icon;
                }

                // Set text (tên + số lượng)
                TextMeshProUGUI text = itemButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = $"{item.Name} x{quantity}";
                }

                // Add listener
                ItemId itemId = item.Id; // Capture for closure
                itemButton.onClick.RemoveAllListeners();
                itemButton.onClick.AddListener(() =>
                {
                    onItemSelected?.Invoke(itemId);
                    Hide();
                });

                itemButtons.Add(itemButton);
            }
        }

        /// <summary>
        /// Ẩn panel
        /// </summary>
        public void Hide()
        {
            if (interactionPanel != null)
            {
                interactionPanel.SetActive(false);
            }

            ClearItemButtons();
        }

        /// <summary>
        /// Xóa tất cả item buttons
        /// </summary>
        private void ClearItemButtons()
        {
            foreach (var button in itemButtons)
            {
                if (button != null)
                {
                    Destroy(button.gameObject);
                }
            }
            itemButtons.Clear();
        }
    }
}

