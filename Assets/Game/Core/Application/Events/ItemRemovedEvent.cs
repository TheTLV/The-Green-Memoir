using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Events
{
    /// <summary>
    /// Sự kiện khi vật phẩm bị xóa khỏi túi đồ
    /// </summary>
    public class ItemRemovedEvent : IGameEvent
    {
        public PlayerId PlayerId { get; }
        public ItemId ItemId { get; }
        public int Quantity { get; }

        public ItemRemovedEvent(PlayerId playerId, ItemId itemId, int quantity)
        {
            PlayerId = playerId;
            ItemId = itemId;
            Quantity = quantity;
        }
    }
}

