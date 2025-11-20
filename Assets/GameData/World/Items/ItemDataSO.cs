using UnityEngine;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Enums;
using TheGreenMemoir.Unity.Attributes;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// ScriptableObject cho Item Data
    /// Tạo trong Unity Editor: Right Click → Create → Game → Item Data
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item Data", order = 1)]
    public class ItemDataSO : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("ID duy nhất của vật phẩm (ví dụ: corn, wheat, seed_corn)")]
        public string itemId = "new_item";
        
        [Tooltip("Tên hiển thị của vật phẩm")]
        public string itemName = "New Item";
        
        [TextArea(3, 5)]
        [Tooltip("Mô tả vật phẩm")]
        public string description = "Item description";

        [Header("Visual")]
        [Tooltip("Icon hiển thị trong inventory")]
        public Sprite icon;

        [Header("Properties")]
        [Tooltip("Số lượng tối đa trong 1 stack")]
        [Range(1, 999)]
        public int maxStackSize = 99;

        [Header("Item Tags")]
        [Tooltip("Tags/Flags của vật phẩm - dùng để phân loại và lọc")]
        [EnumFlags]
        public ItemTag tags = ItemTag.Stackable;

        [Header("Legacy Properties (Auto-converted to Tags)")]
        [Tooltip("⚠️ DEPRECATED: Dùng Tags thay vì các checkbox này")]
        public bool isStackable = true;
        public bool isSeed = false;
        public bool isConsumable = false;
        public bool isMissionItem = false;

        [Header("Value")]
        [Tooltip("Giá bán (nếu có thể bán)")]
        public int sellPrice = 0;
        
        [Tooltip("Giá mua (nếu có thể mua)")]
        public int buyPrice = 0;

        /// <summary>
        /// Chuyển đổi ScriptableObject thành Item entity
        /// </summary>
        public Item ToItem()
        {
            // Convert legacy properties to tags nếu chưa set tags
            var itemTags = tags;
            if (itemTags == ItemTag.None || 
                (itemTags == ItemTag.Stackable && isStackable))
            {
                // Auto-convert từ legacy properties
                if (isStackable) itemTags = itemTags.AddTag(ItemTag.Stackable);
                if (isSeed) itemTags = itemTags.AddTag(ItemTag.Seed);
                if (isConsumable) itemTags = itemTags.AddTag(ItemTag.Consumable);
                if (isMissionItem) itemTags = itemTags.AddTag(ItemTag.QuestItem);
            }

            return new Item(
                new ItemId(itemId),
                itemName,
                description,
                itemTags,
                maxStackSize
            );
        }

        /// <summary>
        /// Validate data khi tạo trong Editor
        /// </summary>
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                itemId = name.ToLower().Replace(" ", "_");
            }

            if (string.IsNullOrWhiteSpace(itemName))
            {
                itemName = name;
            }

            if (maxStackSize < 1)
            {
                maxStackSize = 1;
            }
        }
    }
}

