using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Core.Domain.Enums;

namespace TheGreenMemoir.Unity.Presentation.UI
{
    /// <summary>
    /// Panel hiển thị thông tin chi tiết của item khi click vào
    /// Tự động hiển thị các thuộc tính dựa trên Tags
    /// </summary>
    public class ItemInfoPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI propertiesText;
        [SerializeField] private TextMeshProUGUI valueText;
        
        [Header("Tag Icons (Optional)")]
        [SerializeField] private Transform tagContainer;
        [SerializeField] private GameObject tagIconPrefab;

        private ItemDataSO _currentItemData;
        private Item _currentItem;

        /// <summary>
        /// Hiển thị thông tin item
        /// </summary>
        public void ShowItem(ItemId itemId)
        {
            var database = Managers.GameDatabaseManager.GetDatabase();
            if (database == null)
            {
                Debug.LogError("GameDatabase not found!");
                return;
            }

            _currentItemData = database.GetItemData(itemId);
            if (_currentItemData == null)
            {
                Debug.LogWarning($"ItemData not found for {itemId}");
                return;
            }

            _currentItem = _currentItemData.ToItem();
            UpdateUI();
        }

        /// <summary>
        /// Hiển thị thông tin item từ Item entity
        /// </summary>
        public void ShowItem(Item item, ItemDataSO itemData = null)
        {
            _currentItem = item;
            _currentItemData = itemData;
            
            // Nếu chưa có ItemDataSO, load từ database
            if (_currentItemData == null)
            {
                var database = Managers.GameDatabaseManager.GetDatabase();
                if (database != null)
                {
                    _currentItemData = database.GetItemData(item.Id);
                }
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_currentItem == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            // Icon
            if (iconImage != null && _currentItemData != null)
            {
                iconImage.sprite = _currentItemData.icon;
                iconImage.enabled = _currentItemData.icon != null;
            }

            // Name
            if (nameText != null)
            {
                nameText.text = _currentItem.Name;
            }

            // Description
            if (descriptionText != null)
            {
                descriptionText.text = _currentItem.Description;
            }

            // Properties
            if (propertiesText != null)
            {
                propertiesText.text = BuildPropertiesText();
            }

            // Value
            if (valueText != null && _currentItemData != null)
            {
                valueText.text = BuildValueText();
            }

            // Tags
            UpdateTagIcons();
        }

        private string BuildPropertiesText()
        {
            var props = new System.Text.StringBuilder();

            // Stackable
            if (_currentItem.IsStackable)
            {
                props.AppendLine($"Stackable: {_currentItem.MaxStackSize} max");
            }
            else
            {
                props.AppendLine("Not stackable");
            }

            // Tags
            var tags = _currentItem.Tags;
            if (tags != ItemTag.None)
            {
                props.AppendLine("\nTags:");
                if (tags.HasTag(ItemTag.Seed)) props.AppendLine("• Seed");
                if (tags.HasTag(ItemTag.Food)) props.AppendLine("• Food");
                if (tags.HasTag(ItemTag.Consumable)) props.AppendLine("• Consumable");
                if (tags.HasTag(ItemTag.Edible)) props.AppendLine("• Edible");
                if (tags.HasTag(ItemTag.Drinkable)) props.AppendLine("• Drinkable");
                if (tags.HasTag(ItemTag.QuestItem)) props.AppendLine("• Quest Item");
                if (tags.HasTag(ItemTag.Tool)) props.AppendLine("• Tool");
                if (tags.HasTag(ItemTag.Material)) props.AppendLine("• Material");
                if (tags.HasTag(ItemTag.Resource)) props.AppendLine("• Resource");
                if (tags.HasTag(ItemTag.Rare)) props.AppendLine("• Rare");
                if (tags.HasTag(ItemTag.Craftable)) props.AppendLine("• Craftable");
                if (tags.HasTag(ItemTag.Giftable)) props.AppendLine("• Giftable");
            }

            return props.ToString();
        }

        private string BuildValueText()
        {
            var value = new System.Text.StringBuilder();

            if (_currentItemData.sellPrice > 0)
            {
                value.AppendLine($"Sell Price: {_currentItemData.sellPrice}G");
            }

            if (_currentItemData.buyPrice > 0)
            {
                value.AppendLine($"Buy Price: {_currentItemData.buyPrice}G");
            }

            return value.ToString();
        }

        private void UpdateTagIcons()
        {
            if (tagContainer == null || tagIconPrefab == null)
                return;

            // Clear existing tags
            foreach (Transform child in tagContainer)
            {
                Destroy(child.gameObject);
            }

            // Create tag icons
            var tags = _currentItem.Tags;
            // Có thể thêm icon cho từng tag ở đây nếu cần
        }

        /// <summary>
        /// Ẩn panel
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

