using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Services
{
    /// <summary>
    /// Dịch vụ xử lý người chơi
    /// </summary>
    public class PlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventBus _eventBus;

        public PlayerService(IPlayerRepository playerRepository, IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Di chuyển người chơi
        /// </summary>
        public void MovePlayer(PlayerId playerId, Position newPosition)
        {
            var player = _playerRepository.GetPlayer(playerId);
            player.Move(newPosition);
            _playerRepository.SavePlayer(player);
        }

        /// <summary>
        /// Tiêu thụ năng lượng
        /// </summary>
        public bool ConsumeEnergy(PlayerId playerId, int amount)
        {
            var player = _playerRepository.GetPlayer(playerId);

            if (player.ConsumeEnergy(amount))
            {
                _playerRepository.SavePlayer(player);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Thêm tiền
        /// </summary>
        public void AddMoney(PlayerId playerId, Money amount)
        {
            var player = _playerRepository.GetPlayer(playerId);
            var oldMoney = player.Money;
            player.AddMoney(amount);
            _playerRepository.SavePlayer(player);

            _eventBus.Publish(new Events.MoneyChangedEvent(playerId, player.Money, oldMoney));
        }

        /// <summary>
        /// Trừ tiền
        /// </summary>
        public bool RemoveMoney(PlayerId playerId, Money amount)
        {
            var player = _playerRepository.GetPlayer(playerId);
            var oldMoney = player.Money;

            if (player.RemoveMoney(amount))
            {
                _playerRepository.SavePlayer(player);
                _eventBus.Publish(new Events.MoneyChangedEvent(playerId, player.Money, oldMoney));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Kiểm tra xem có thể thực hiện hành động không
        /// </summary>
        public bool CanPerformAction(PlayerId playerId, int energyCost)
        {
            var player = _playerRepository.GetPlayer(playerId);
            return player.CanPerformAction(energyCost);
        }

        /// <summary>
        /// Lấy thông tin người chơi
        /// </summary>
        public Player GetPlayer(PlayerId playerId)
        {
            return _playerRepository.GetPlayer(playerId);
        }

        /// <summary>
        /// Hồi phục năng lượng (khi ngủ)
        /// </summary>
        public void RestoreEnergy(PlayerId playerId)
        {
            var player = _playerRepository.GetPlayer(playerId);
            player.RestoreEnergy();
            _playerRepository.SavePlayer(player);
        }
    }
}

