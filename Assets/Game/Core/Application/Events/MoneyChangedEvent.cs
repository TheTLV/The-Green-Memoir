using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Events
{
    /// <summary>
    /// Sự kiện khi tiền thay đổi
    /// </summary>
    public class MoneyChangedEvent : IGameEvent
    {
        public PlayerId PlayerId { get; }
        public Money NewAmount { get; }
        public Money OldAmount { get; }

        public MoneyChangedEvent(PlayerId playerId, Money newAmount, Money oldAmount)
        {
            PlayerId = playerId;
            NewAmount = newAmount;
            OldAmount = oldAmount;
        }
    }
}

