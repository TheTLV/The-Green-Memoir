using System.Collections.Generic;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Domain.Interfaces
{
    /// <summary>
    /// Repository cho Farm Tiles
    /// </summary>
    public interface IFarmRepository
    {
        FarmTile GetTile(TilePosition position);
        IEnumerable<FarmTile> GetAllTiles();
        void SaveTile(FarmTile tile);
        void SaveAllTiles(IEnumerable<FarmTile> tiles);
        bool TileExists(TilePosition position);
    }
}

