using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Events
{
    /// <summary>
    /// Sự kiện khi thu hoạch cây
    /// </summary>
    public class CropHarvestedEvent : IGameEvent
    {
        public TilePosition Position { get; }
        public ItemId HarvestedItemId { get; }
        public int Quantity { get; }

        public CropHarvestedEvent(TilePosition position, ItemId harvestedItemId, int quantity)
        {
            Position = position;
            HarvestedItemId = harvestedItemId;
            Quantity = quantity;
        }
    }
}

