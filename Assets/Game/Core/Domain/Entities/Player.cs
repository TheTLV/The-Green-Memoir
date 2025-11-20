using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Domain.Entities
{
    /// <summary>
    /// Người chơi
    /// </summary>
    public class Player
    {
        public PlayerId Id { get; }
        public Position Position { get; private set; }
        public Energy Energy { get; }
        public Money Money { get; private set; }
        public Inventory Inventory { get; }

        public Player(PlayerId id, Position position, Energy energy, Money money, Inventory inventory)
        {
            Id = id;
            Position = position;
            Energy = energy ?? throw new System.ArgumentNullException(nameof(energy));
            Money = money;
            Inventory = inventory ?? throw new System.ArgumentNullException(nameof(inventory));
        }

        /// <summary>
        /// Di chuyển người chơi
        /// </summary>
        public void Move(Position newPosition)
        {
            Position = newPosition;
        }

        /// <summary>
        /// Tiêu thụ năng lượng
        /// </summary>
        public bool ConsumeEnergy(int amount)
        {
            return Energy.Consume(amount);
        }

        /// <summary>
        /// Thêm tiền
        /// </summary>
        public void AddMoney(Money amount)
        {
            Money = Money.Add(amount);
        }

        /// <summary>
        /// Trừ tiền
        /// </summary>
        public bool RemoveMoney(Money amount)
        {
            if (Money.IsLessThan(amount))
                return false;

            Money = Money.Subtract(amount);
            return true;
        }

        /// <summary>
        /// Kiểm tra xem có thể thực hiện hành động không (đủ năng lượng)
        /// </summary>
        public bool CanPerformAction(int energyCost)
        {
            return Energy.Current >= energyCost;
        }

        /// <summary>
        /// Hồi phục năng lượng (khi ngủ)
        /// </summary>
        public void RestoreEnergy()
        {
            Energy.RestoreFull();
        }
    }
}

