using System.Collections.Generic;
using System.Linq;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Domain.Entities
{
    /// <summary>
    /// Túi đồ của người chơi
    /// </summary>
    public class Inventory
    {
        private readonly List<InventorySlot> _slots;
        public int Capacity { get; }

        public Inventory(int capacity)
        {
            if (capacity <= 0)
                throw new System.ArgumentException("Capacity must be greater than 0", nameof(capacity));

            Capacity = capacity;
            _slots = new List<InventorySlot>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                _slots.Add(new InventorySlot());
            }
        }

        /// <summary>
        /// Thêm vật phẩm vào túi đồ
        /// </summary>
        public bool AddItem(Item item, int quantity)
        {
            if (item == null || quantity <= 0)
                return false;

            int remaining = quantity;

            // Tìm các ô đã có vật phẩm này (nếu stackable)
            if (item.IsStackable)
            {
                foreach (var slot in _slots)
                {
                    if (!slot.IsEmpty && slot.Item.Id == item.Id)
                    {
                        int canAdd = System.Math.Min(remaining, slot.GetRemainingCapacity());
                        if (canAdd > 0)
                        {
                            slot.Add(canAdd);
                            remaining -= canAdd;
                            if (remaining <= 0)
                                return true;
                        }
                    }
                }
            }

            // Tìm ô trống để thêm
            while (remaining > 0)
            {
                var emptySlot = _slots.FirstOrDefault(s => s.IsEmpty);
                if (emptySlot == null)
                    return false; // Hết chỗ

                int toAdd = System.Math.Min(remaining, item.MaxStackSize);
                emptySlot.Add(item, toAdd);
                remaining -= toAdd;
            }

            return true;
        }

        /// <summary>
        /// Xóa vật phẩm khỏi túi đồ
        /// </summary>
        public bool RemoveItem(ItemId itemId, int quantity)
        {
            if (quantity <= 0)
                return false;

            int remaining = quantity;

            foreach (var slot in _slots)
            {
                if (!slot.IsEmpty && slot.Item.Id == itemId)
                {
                    int toRemove = System.Math.Min(remaining, slot.Quantity);
                    slot.Remove(toRemove);
                    remaining -= toRemove;

                    if (remaining <= 0)
                        return true;
                }
            }

            return remaining == 0;
        }

        /// <summary>
        /// Kiểm tra xem có vật phẩm với số lượng đủ không
        /// </summary>
        public bool HasItem(ItemId itemId, int quantity)
        {
            int total = GetQuantity(itemId);
            return total >= quantity;
        }

        /// <summary>
        /// Lấy tổng số lượng vật phẩm
        /// </summary>
        public int GetQuantity(ItemId itemId)
        {
            return _slots
                .Where(s => !s.IsEmpty && s.Item.Id == itemId)
                .Sum(s => s.Quantity);
        }

        /// <summary>
        /// Lấy tất cả các ô
        /// </summary>
        public IEnumerable<InventorySlot> GetAllSlots()
        {
            return _slots.AsReadOnly();
        }

        /// <summary>
        /// Lấy ô tại index
        /// </summary>
        public InventorySlot GetSlot(int index)
        {
            if (index < 0 || index >= Capacity)
                return null;
            return _slots[index];
        }

        /// <summary>
        /// Kiểm tra xem túi đồ có đầy không
        /// </summary>
        public bool IsFull()
        {
            return _slots.All(s => !s.IsEmpty);
        }

        /// <summary>
        /// Đếm số ô trống
        /// </summary>
        public int GetEmptySlotCount()
        {
            return _slots.Count(s => s.IsEmpty);
        }
    }
}

