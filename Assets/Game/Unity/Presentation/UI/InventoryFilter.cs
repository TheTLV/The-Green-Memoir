using System.Collections.Generic;
using System.Linq;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.Enums;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.Presentation.UI
{
    /// <summary>
    /// Hệ thống lọc vật phẩm trong inventory theo Tags
    /// </summary>
    public static class InventoryFilter
    {
        /// <summary>
        /// Lọc inventory theo tag
        /// </summary>
        public static IEnumerable<InventorySlot> FilterByTag(Inventory inventory, ItemTag filterTag)
        {
            if (inventory == null)
                return Enumerable.Empty<InventorySlot>();

            return inventory.GetAllSlots()
                .Where(slot => !slot.IsEmpty && slot.Item.Tags.HasTag(filterTag));
        }

        /// <summary>
        /// Lọc inventory theo nhiều tags (OR - có bất kỳ tag nào)
        /// </summary>
        public static IEnumerable<InventorySlot> FilterByAnyTag(Inventory inventory, ItemTag filterTags)
        {
            if (inventory == null)
                return Enumerable.Empty<InventorySlot>();

            return inventory.GetAllSlots()
                .Where(slot => !slot.IsEmpty && slot.Item.Tags.HasAnyTag(filterTags));
        }

        /// <summary>
        /// Lọc inventory theo nhiều tags (AND - phải có tất cả tags)
        /// </summary>
        public static IEnumerable<InventorySlot> FilterByAllTags(Inventory inventory, ItemTag filterTags)
        {
            if (inventory == null)
                return Enumerable.Empty<InventorySlot>();

            return inventory.GetAllSlots()
                .Where(slot => !slot.IsEmpty && slot.Item.Tags.HasAllTags(filterTags));
        }

        /// <summary>
        /// Lọc inventory theo tên (tìm kiếm)
        /// </summary>
        public static IEnumerable<InventorySlot> FilterByName(Inventory inventory, string searchTerm)
        {
            if (inventory == null || string.IsNullOrWhiteSpace(searchTerm))
                return inventory?.GetAllSlots() ?? Enumerable.Empty<InventorySlot>();

            var term = searchTerm.ToLower();
            return inventory.GetAllSlots()
                .Where(slot => !slot.IsEmpty && slot.Item.Name.ToLower().Contains(term));
        }

        /// <summary>
        /// Lấy tất cả items có tag cụ thể
        /// </summary>
        public static IEnumerable<Item> GetItemsByTag(Inventory inventory, ItemTag tag)
        {
            return FilterByTag(inventory, tag)
                .Select(slot => slot.Item)
                .Distinct();
        }

        /// <summary>
        /// Đếm số lượng items có tag cụ thể
        /// </summary>
        public static int CountItemsByTag(Inventory inventory, ItemTag tag)
        {
            return FilterByTag(inventory, tag).Count();
        }

        /// <summary>
        /// Kiểm tra xem có item với tag cụ thể không
        /// </summary>
        public static bool HasItemWithTag(Inventory inventory, ItemTag tag)
        {
            return FilterByTag(inventory, tag).Any();
        }
    }
}

