using TheGreenMemoir.Core.Domain.Entities;

namespace TheGreenMemoir.Core.Domain.Entities
{
    /// <summary>
    /// Một ô trong túi đồ
    /// </summary>
    public class InventorySlot
    {
        public Item Item { get; private set; }
        public int Quantity { get; private set; }

        public bool IsEmpty => Item == null || Quantity == 0;

        public InventorySlot()
        {
            Item = null;
            Quantity = 0;
        }

        /// <summary>
        /// Kiểm tra xem có thể thêm số lượng này không
        /// </summary>
        public bool CanAdd(int amount)
        {
            if (IsEmpty)
                return amount > 0;

            if (Item == null)
                return false;

            if (!Item.IsStackable)
                return false;

            return Quantity + amount <= Item.MaxStackSize;
        }

        /// <summary>
        /// Thêm vật phẩm vào ô
        /// </summary>
        public void Add(Item item, int amount)
        {
            if (item == null || amount <= 0)
                return;

            if (IsEmpty)
            {
                Item = item;
                Quantity = amount;
            }
            else if (Item.Id == item.Id && CanAdd(amount))
            {
                Quantity += amount;
            }
        }

        /// <summary>
        /// Thêm số lượng vào ô hiện tại
        /// </summary>
        public void Add(int amount)
        {
            if (CanAdd(amount))
            {
                Quantity += amount;
            }
        }

        /// <summary>
        /// Xóa số lượng vật phẩm
        /// </summary>
        public void Remove(int amount)
        {
            if (amount <= 0)
                return;

            Quantity = System.Math.Max(0, Quantity - amount);

            if (Quantity == 0)
            {
                Item = null;
            }
        }

        /// <summary>
        /// Xóa toàn bộ vật phẩm
        /// </summary>
        public void Clear()
        {
            Item = null;
            Quantity = 0;
        }

        /// <summary>
        /// Lấy số lượng có thể thêm vào
        /// </summary>
        public int GetRemainingCapacity()
        {
            if (IsEmpty || Item == null)
                return 0;

            return Item.MaxStackSize - Quantity;
        }
    }
}

