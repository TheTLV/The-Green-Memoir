using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Enums;

namespace TheGreenMemoir.Core.Domain.Entities
{
    /// <summary>
    /// Vật phẩm trong game
    /// </summary>
    public class Item
    {
        public ItemId Id { get; }
        public string Name { get; }
        public string Description { get; }
        public int MaxStackSize { get; }
        public ItemTag Tags { get; }

        // Convenience properties (từ Tags)
        public bool IsStackable => Tags.HasTag(ItemTag.Stackable);
        public bool IsSeed => Tags.HasTag(ItemTag.Seed);
        public bool IsConsumable => Tags.HasTag(ItemTag.Consumable);
        public bool IsQuestItem => Tags.HasTag(ItemTag.QuestItem);
        public bool IsTool => Tags.HasTag(ItemTag.Tool);
        public bool IsFood => Tags.HasTag(ItemTag.Food);
        public bool IsEdible => Tags.HasTag(ItemTag.Edible);
        public bool IsSellable => Tags.HasTag(ItemTag.Sellable);
        public bool IsBuyable => Tags.HasTag(ItemTag.Buyable);

        public Item(ItemId id, string name, string description, ItemTag tags, int maxStackSize = 99)
        {
            Id = id;
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            Tags = tags;
            MaxStackSize = maxStackSize;
        }

        /// <summary>
        /// Constructor cũ (backward compatibility)
        /// </summary>
        public Item(ItemId id, string name, string description, bool isStackable = true, 
            int maxStackSize = 99, bool isSeed = false, bool isConsumable = false, bool isMissionItem = false)
        {
            Id = id;
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            MaxStackSize = maxStackSize;

            // Convert old boolean properties to tags
            ItemTag tags = ItemTag.None;
            if (isStackable) tags = tags.AddTag(ItemTag.Stackable);
            if (isSeed) tags = tags.AddTag(ItemTag.Seed);
            if (isConsumable) tags = tags.AddTag(ItemTag.Consumable);
            if (isMissionItem) tags = tags.AddTag(ItemTag.QuestItem);
            
            Tags = tags;
        }

        /// <summary>
        /// Kiểm tra xem có tag không
        /// </summary>
        public bool HasTag(ItemTag tag)
        {
            return Tags.HasTag(tag);
        }
    }
}

