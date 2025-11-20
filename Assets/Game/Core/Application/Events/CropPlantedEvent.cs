using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Events
{
    /// <summary>
    /// Sự kiện khi trồng cây
    /// </summary>
    public class CropPlantedEvent : IGameEvent
    {
        public TilePosition Position { get; }
        public CropId CropId { get; }

        public CropPlantedEvent(TilePosition position, CropId cropId)
        {
            Position = position;
            CropId = cropId;
        }
    }
}

