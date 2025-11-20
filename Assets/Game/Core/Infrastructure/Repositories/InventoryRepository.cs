using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation cho Inventory
    /// </summary>
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IPlayerRepository _playerRepository;

        public InventoryRepository(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public Inventory GetInventory(PlayerId playerId)
        {
            // Inventory được lưu trong Player
            var player = _playerRepository.GetPlayer(playerId);
            return player.Inventory;
        }

        public void SaveInventory(PlayerId playerId, Inventory inventory)
        {
            // Inventory được lưu cùng với Player
            var player = _playerRepository.GetPlayer(playerId);
            // Player đã có reference đến inventory, chỉ cần save player
            _playerRepository.SavePlayer(player);
        }
    }
}

