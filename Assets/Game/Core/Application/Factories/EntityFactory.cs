using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Interfaces;
using System.Collections.Generic;

namespace TheGreenMemoir.Core.Application.Factories
{
    /// <summary>
    /// Entity Factory - Factory Pattern
    /// Tạo các entities từ database với khả năng cache và tùy chỉnh
    /// </summary>
    public class EntityFactory : IEntityFactory
    {
        private readonly IItemDatabase _itemDatabase;
        private readonly ICropDatabase _cropDatabase;
        private readonly IToolDatabase _toolDatabase;

        // Cache để tránh tạo lại entities giống nhau
        private Dictionary<ItemId, Item> _itemCache = new Dictionary<ItemId, Item>();
        private Dictionary<CropId, Crop> _cropCache = new Dictionary<CropId, Crop>();
        private Dictionary<ToolId, Tool> _toolCache = new Dictionary<ToolId, Tool>();

        public EntityFactory(
            IItemDatabase itemDatabase,
            ICropDatabase cropDatabase,
            IToolDatabase toolDatabase)
        {
            _itemDatabase = itemDatabase;
            _cropDatabase = cropDatabase;
            _toolDatabase = toolDatabase;
        }

        public Item CreateItem(ItemId itemId)
        {
            // Kiểm tra cache trước
            if (_itemCache.TryGetValue(itemId, out var cachedItem))
            {
                return cachedItem;
            }

            // Tạo từ database
            var item = _itemDatabase.GetItem(itemId);
            if (item == null)
            {
                UnityEngine.Debug.LogError($"Item not found: {itemId.Value}");
                return null;
            }

            // Cache item (vì Item là immutable, có thể cache an toàn)
            _itemCache[itemId] = item;
            return item;
        }

        public Crop CreateCrop(CropId cropId)
        {
            // Crop không cache vì mỗi crop instance có state riêng (growth stage, etc.)
            var crop = _cropDatabase.GetCrop(cropId);
            if (crop == null)
            {
                UnityEngine.Debug.LogError($"Crop not found: {cropId}");
                return null;
            }

            return crop;
        }

        public Tool CreateTool(ToolId toolId)
        {
            // Tool không cache vì mỗi tool instance có state riêng (current uses, etc.)
            var tool = _toolDatabase.GetTool(toolId);
            if (tool == null)
            {
                UnityEngine.Debug.LogError($"Tool not found: {toolId}");
                return null;
            }

            return tool;
        }

        /// <summary>
        /// Clear cache (khi cần reload data)
        /// </summary>
        public void ClearCache()
        {
            _itemCache.Clear();
            _cropCache.Clear();
            _toolCache.Clear();
        }
    }
}

