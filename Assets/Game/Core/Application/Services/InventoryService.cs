using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Services
{
    /// <summary>
    /// Dịch vụ xử lý túi đồ
    /// </summary>
    public class InventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IEventBus _eventBus;
        private readonly IItemDatabase _itemDatabase;

        public InventoryService(
            IInventoryRepository inventoryRepository, 
            IEventBus eventBus,
            IItemDatabase itemDatabase = null)
        {
            _inventoryRepository = inventoryRepository;
            _eventBus = eventBus;
            _itemDatabase = itemDatabase;
        }

        /// <summary>
        /// Thêm vật phẩm vào túi đồ
        /// </summary>
        public bool AddItem(PlayerId playerId, Item item, int quantity)
        {
            if (item == null || quantity <= 0)
                return false;

            var inventory = _inventoryRepository.GetInventory(playerId);

            if (inventory.AddItem(item, quantity))
            {
                _inventoryRepository.SaveInventory(playerId, inventory);
                _eventBus.Publish(new Events.ItemAddedEvent(playerId, item.Id, quantity));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Xóa vật phẩm khỏi túi đồ
        /// </summary>
        public bool RemoveItem(PlayerId playerId, ItemId itemId, int quantity)
        {
            if (quantity <= 0)
                return false;

            var inventory = _inventoryRepository.GetInventory(playerId);

            if (inventory.RemoveItem(itemId, quantity))
            {
                _inventoryRepository.SaveInventory(playerId, inventory);
                _eventBus.Publish(new Events.ItemRemovedEvent(playerId, itemId, quantity));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Kiểm tra xem có vật phẩm với số lượng đủ không
        /// </summary>
        public bool HasItem(PlayerId playerId, ItemId itemId, int quantity)
        {
            var inventory = _inventoryRepository.GetInventory(playerId);
            return inventory.HasItem(itemId, quantity);
        }

        /// <summary>
        /// Lấy túi đồ của người chơi
        /// </summary>
        public Inventory GetInventory(PlayerId playerId)
        {
            return _inventoryRepository.GetInventory(playerId);
        }

        /// <summary>
        /// Lấy số lượng vật phẩm
        /// </summary>
        public int GetQuantity(PlayerId playerId, ItemId itemId)
        {
            var inventory = _inventoryRepository.GetInventory(playerId);
            return inventory.GetQuantity(itemId);
        }

        /// <summary>
        /// Thêm vật phẩm bằng ItemId (load từ database)
        /// </summary>
        public bool AddItemById(PlayerId playerId, ItemId itemId, int quantity)
        {
            if (_itemDatabase == null)
            {
                UnityEngine.Debug.LogError("ItemDatabase not available! Cannot add item by ID.");
                return false;
            }

            var item = _itemDatabase.GetItem(itemId);
            if (item == null)
            {
                UnityEngine.Debug.LogWarning($"Item with ID {itemId} not found in database!");
                return false;
            }

            return AddItem(playerId, item, quantity);
        }
    }
}

