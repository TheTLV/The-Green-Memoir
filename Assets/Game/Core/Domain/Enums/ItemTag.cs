using System;

namespace TheGreenMemoir.Core.Domain.Enums
{
    /// <summary>
    /// Tags/Flags cho Item - dùng để phân loại và lọc vật phẩm
    /// Có thể mở rộng dễ dàng bằng cách thêm flag mới
    /// </summary>
    [Flags]
    public enum ItemTag
    {
        None = 0,
        
        // Basic Properties
        Stackable = 1 << 0,           // Có thể xếp chồng
        
        // Item Types
        Seed = 1 << 1,                 // Hạt giống
        Consumable = 1 << 2,          // Có thể ăn/sử dụng
        QuestItem = 1 << 3,           // Vật phẩm nhiệm vụ
        Tool = 1 << 4,                // Công cụ
        Material = 1 << 5,            // Nguyên liệu
        Food = 1 << 6,               // Thức ăn
        Resource = 1 << 7,           // Tài nguyên
        
        // Special Properties
        Rare = 1 << 8,                // Hiếm
        Sellable = 1 << 9,           // Có thể bán
        Buyable = 1 << 10,           // Có thể mua
        Giftable = 1 << 11,          // Có thể tặng
        Craftable = 1 << 12,        // Có thể chế tạo
        
        // Usage Types
        Edible = 1 << 13,            // Có thể ăn (subset của Consumable)
        Drinkable = 1 << 14,         // Có thể uống
        Usable = 1 << 15,            // Có thể sử dụng (không ăn/uống)
        
        // Future expansion - thêm flag mới ở đây
        // Example: Fishable = 1 << 16, Plantable = 1 << 17, etc.
    }

    /// <summary>
    /// Extension methods cho ItemTag
    /// </summary>
    public static class ItemTagExtensions
    {
        /// <summary>
        /// Kiểm tra xem có tag không
        /// </summary>
        public static bool HasTag(this ItemTag tags, ItemTag tag)
        {
            return (tags & tag) == tag;
        }

        /// <summary>
        /// Thêm tag
        /// </summary>
        public static ItemTag AddTag(this ItemTag tags, ItemTag tag)
        {
            return tags | tag;
        }

        /// <summary>
        /// Xóa tag
        /// </summary>
        public static ItemTag RemoveTag(this ItemTag tags, ItemTag tag)
        {
            return tags & ~tag;
        }

        /// <summary>
        /// Kiểm tra xem có bất kỳ tag nào trong danh sách không
        /// </summary>
        public static bool HasAnyTag(this ItemTag tags, ItemTag filterTags)
        {
            return (tags & filterTags) != ItemTag.None;
        }

        /// <summary>
        /// Kiểm tra xem có tất cả tags trong danh sách không
        /// </summary>
        public static bool HasAllTags(this ItemTag tags, ItemTag filterTags)
        {
            return (tags & filterTags) == filterTags;
        }
    }
}

