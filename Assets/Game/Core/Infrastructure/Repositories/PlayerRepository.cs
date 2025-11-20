using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation cho Player
    /// </summary>
    public class PlayerRepository : IPlayerRepository
    {
        private Player _cachedPlayer;

        public Player GetPlayer(PlayerId playerId)
        {
            // TODO: Load from save file
            // Hiện tại tạo player mới nếu chưa có
            if (_cachedPlayer == null)
            {
                _cachedPlayer = CreateDefaultPlayer(playerId);
            }

            return _cachedPlayer;
        }

        public void SavePlayer(Player player)
        {
            _cachedPlayer = player;
            // TODO: Save to file
        }

        public bool PlayerExists(PlayerId playerId)
        {
            return _cachedPlayer != null && _cachedPlayer.Id == playerId;
        }

        private Player CreateDefaultPlayer(PlayerId playerId)
        {
            var position = new Position(0, 0);
            var energy = new Energy(100, 100);
            var money = new Money(1000);
            var inventory = new Inventory(21); // 21 slots

            return new Player(playerId, position, energy, money, inventory);
        }
    }
}

