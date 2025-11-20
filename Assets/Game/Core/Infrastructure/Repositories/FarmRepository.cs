using System.Collections.Generic;
using System.Linq;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation cho Farm Tiles
    /// </summary>
    public class FarmRepository : IFarmRepository
    {
        private readonly Dictionary<TilePosition, FarmTile> _tiles = new Dictionary<TilePosition, FarmTile>();

        public FarmTile GetTile(TilePosition position)
        {
            if (!_tiles.ContainsKey(position))
            {
                _tiles[position] = new FarmTile(position, "normal"); // Default state ID
            }

            return _tiles[position];
        }

        public IEnumerable<FarmTile> GetAllTiles()
        {
            return _tiles.Values.ToList();
        }

        public void SaveTile(FarmTile tile)
        {
            _tiles[tile.Position] = tile;
            // TODO: Save to file
        }

        public void SaveAllTiles(IEnumerable<FarmTile> tiles)
        {
            foreach (var tile in tiles)
            {
                SaveTile(tile);
            }
        }

        public bool TileExists(TilePosition position)
        {
            return _tiles.ContainsKey(position);
        }
    }
}

