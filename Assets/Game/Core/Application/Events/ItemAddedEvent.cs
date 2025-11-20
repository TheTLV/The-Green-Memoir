using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Events
{
    /// <summary>
    /// Sự kiện khi vật phẩm được thêm vào túi đồ
    /// </summary>
    public class ItemAddedEvent : IGameEvent
    {
        public PlayerId PlayerId { get; }
        public ItemId ItemId { get; }
        public int Quantity { get; }

        public ItemAddedEvent(PlayerId playerId, ItemId itemId, int quantity)
        {
            PlayerId = playerId;
            ItemId = itemId;
            Quantity = quantity;
        }
    }
}

