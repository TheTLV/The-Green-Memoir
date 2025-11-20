using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Core.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace TheGreenMemoir.Unity.Presentation.Views
{
    /// <summary>
    /// View để hiển thị inventory UI
    /// Có thể mở rộng với UI elements cụ thể
    /// </summary>
    public class InventoryView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform slotContainer;
        [SerializeField] private GameObject slotPrefab;

        private List<InventorySlotUI> _slotUIs = new List<InventorySlotUI>();

        /// <summary>
        /// Hiển thị inventory
        /// </summary>
        public void DisplayInventory(Inventory inventory)
        {
            if (inventory == null)
                return;

            // Tạo đủ slot UI nếu chưa có
            while (_slotUIs.Count < inventory.Capacity)
            {
                CreateSlotUI();
            }

            // Cập nhật từng slot
            int index = 0;
            foreach (var slot in inventory.GetAllSlots())
            {
                if (index < _slotUIs.Count)
                {
                    _slotUIs[index].UpdateSlot(slot);
                }
                index++;
            }
        }

        /// <summary>
        /// Hiển thị filtered inventory (chỉ hiển thị items có tag)
        /// </summary>
        public void DisplayFilteredInventory(Inventory inventory, Core.Domain.Enums.ItemTag filterTag)
        {
            if (inventory == null)
                return;

            // Lọc slots
            var filteredSlots = TheGreenMemoir.Unity.Presentation.UI.InventoryFilter
                .FilterByTag(inventory, filterTag)
                .ToList();

            // Tạo đủ slot UI
            while (_slotUIs.Count < filteredSlots.Count)
            {
                CreateSlotUI();
            }

            // Cập nhật slots
            for (int i = 0; i < _slotUIs.Count; i++)
            {
                if (i < filteredSlots.Count)
                {
                    _slotUIs[i].UpdateSlot(filteredSlots[i]);
                }
                else
                {
                    _slotUIs[i].UpdateSlot(null);
                }
            }
        }

        private void CreateSlotUI()
        {
            GameObject slotObj;
            if (slotPrefab != null && slotContainer != null)
            {
                slotObj = Instantiate(slotPrefab, slotContainer);
            }
            else
            {
                // Tạo slot đơn giản nếu không có prefab
                slotObj = new GameObject($"Slot_{_slotUIs.Count}");
                if (slotContainer != null)
                    slotObj.transform.SetParent(slotContainer);
                
                // Thêm Image và Text
                var image = slotObj.AddComponent<Image>();
                image.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                
                var textObj = new GameObject("Text");
                textObj.transform.SetParent(slotObj.transform);
                var text = textObj.AddComponent<TextMeshProUGUI>();
                text.text = "";
                text.fontSize = 12;
                text.alignment = TextAlignmentOptions.Center;
            }

            var slotUI = slotObj.GetComponent<InventorySlotUI>();
            if (slotUI == null)
                slotUI = slotObj.AddComponent<InventorySlotUI>();

            _slotUIs.Add(slotUI);
        }
    }

    /// <summary>
    /// UI component cho một slot trong inventory
    /// </summary>
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI quantityText;

        public void UpdateSlot(InventorySlot slot)
        {
            if (slot == null || slot.IsEmpty)
            {
                // Slot trống
                if (iconImage != null)
                    iconImage.sprite = null;
                if (quantityText != null)
                    quantityText.text = "";
                return;
            }

            // Cập nhật icon từ ItemDataSO
            if (iconImage != null)
            {
                var itemData = GetItemData(slot.Item.Id);
                if (itemData != null && itemData.icon != null)
                {
                    iconImage.sprite = itemData.icon;
                    iconImage.enabled = true;
                }
                else
                {
                    iconImage.sprite = null;
                    iconImage.enabled = false;
                }
            }

            // Cập nhật số lượng
            if (quantityText != null)
            {
                if (slot.Item.IsStackable && slot.Quantity > 1)
                    quantityText.text = slot.Quantity.ToString();
                else
                    quantityText.text = "";
            }
        }

        /// <summary>
        /// Lấy ItemDataSO từ ItemId
        /// </summary>
        private TheGreenMemoir.Unity.Data.ItemDataSO GetItemData(TheGreenMemoir.Core.Domain.ValueObjects.ItemId itemId)
        {
            var database = TheGreenMemoir.Unity.Managers.GameDatabaseManager.GetDatabase();
            if (database == null)
                return null;

            return database.GetItemData(itemId);
        }
    }
}

