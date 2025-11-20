using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Adapter để kết nối GameDatabase với Core interfaces
    /// Cho phép Core sử dụng database mà không phụ thuộc Unity
    /// </summary>
    public class ItemDatabaseAdapter : IItemDatabase, ICropDatabase, IToolDatabase
    {
        private readonly GameDatabase _database;

        public ItemDatabaseAdapter(GameDatabase database)
        {
            _database = database ?? throw new System.ArgumentNullException(nameof(database));
        }

        // IItemDatabase
        public Item GetItem(ItemId itemId)
        {
            return _database.GetItem(itemId);
        }

        public bool HasItem(ItemId itemId)
        {
            return _database.GetItemData(itemId) != null;
        }

        // ICropDatabase
        public Crop GetCrop(CropId cropId)
        {
            return _database.GetCrop(cropId);
        }

        public bool HasCrop(CropId cropId)
        {
            return _database.GetCropData(cropId) != null;
        }

        // IToolDatabase
        public Tool GetTool(ToolId toolId)
        {
            return _database.GetTool(toolId);
        }

        public bool HasTool(ToolId toolId)
        {
            return _database.GetToolData(toolId) != null;
        }
    }
}

