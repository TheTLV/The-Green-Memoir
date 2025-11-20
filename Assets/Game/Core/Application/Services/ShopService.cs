using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Services
{
    /// <summary>
    /// Dịch vụ xử lý mua/bán với NPC
    /// </summary>
    public class ShopService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;
        private readonly IItemDatabase _itemDatabase;

        public ShopService(
            IInventoryRepository inventoryRepository,
            IPlayerRepository playerRepository,
            IEventBus eventBus,
            IItemDatabase itemDatabase = null)
        {
            _inventoryRepository = inventoryRepository;
            _playerRepository = playerRepository;
            _eventBus = eventBus;
            _itemDatabase = itemDatabase;
        }

        /// <summary>
        /// Mua item từ NPC
        /// </summary>
        public ShopResult BuyItem(NPCId npcId, ItemId itemId, int quantity, PlayerId playerId)
        {
            // TODO: Lấy NPC inventory và check có item không
            // TODO: Check player có đủ tiền không
            // TODO: Trừ tiền player
            // TODO: Thêm item vào player inventory
            // TODO: Trừ item khỏi NPC inventory
            // TODO: Publish event

            return ShopResult.Success();
        }

        /// <summary>
        /// Bán item cho NPC
        /// </summary>
        public ShopResult SellItem(NPCId npcId, ItemId itemId, int quantity, PlayerId playerId)
        {
            var player = _playerRepository.GetPlayer(playerId);
            var inventory = _inventoryRepository.GetInventory(playerId);

            // Kiểm tra player có item không
            if (!inventory.HasItem(itemId, quantity))
            {
                return ShopResult.Failed("Not enough items");
            }

            // Lấy item data để tính giá
            if (_itemDatabase == null)
            {
                return ShopResult.Failed("ItemDatabase not available");
            }

            var item = _itemDatabase.GetItem(itemId);
            if (item == null || !item.HasTag(Core.Domain.Enums.ItemTag.Sellable))
            {
                return ShopResult.Failed("Item cannot be sold");
            }

            // TODO: Lấy giá bán từ NPC (có thể khác với giá trong ItemDataSO)
            // Tạm thời hardcode, sẽ implement NPC inventory sau
            // Cần tạo INPCRepository để lấy giá từ NPC
            int sellPrice = 10; // Tạm thời, sẽ implement đầy đủ sau

            if (sellPrice <= 0)
            {
                return ShopResult.Failed("Item has no sell price");
            }

            // Tính tổng tiền
            var totalMoney = new Money(sellPrice * quantity);

            // Trừ item khỏi inventory
            if (!inventory.RemoveItem(itemId, quantity))
            {
                return ShopResult.Failed("Failed to remove items");
            }

            // Thêm tiền
            player.AddMoney(totalMoney);

            // Save
            _inventoryRepository.SaveInventory(playerId, inventory);
            _playerRepository.SavePlayer(player);

            // Publish events
            _eventBus.Publish(new Events.ItemRemovedEvent(playerId, itemId, quantity));
            _eventBus.Publish(new Events.MoneyChangedEvent(playerId, player.Money, player.Money.Subtract(totalMoney)));

            return ShopResult.Success(totalMoney);
        }
    }

    /// <summary>
    /// Kết quả của giao dịch
    /// </summary>
    public class ShopResult
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
        public Money MoneyReceived { get; }

        private ShopResult(bool isSuccess, string errorMessage, Money moneyReceived)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            MoneyReceived = moneyReceived;
        }

        public static ShopResult Success(Money moneyReceived = default)
        {
            return new ShopResult(true, null, moneyReceived);
        }

        public static ShopResult Failed(string errorMessage)
        {
            return new ShopResult(false, errorMessage, default);
        }
    }
}

