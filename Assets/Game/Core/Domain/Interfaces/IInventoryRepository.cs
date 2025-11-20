using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Domain.Interfaces
{
    /// <summary>
    /// Repository cho Inventory
    /// </summary>
    public interface IInventoryRepository
    {
        Inventory GetInventory(PlayerId playerId);
        void SaveInventory(PlayerId playerId, Inventory inventory);
    }
}

