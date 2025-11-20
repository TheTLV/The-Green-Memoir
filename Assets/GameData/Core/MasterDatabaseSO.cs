using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Database chính để quản lý tất cả ScriptableObjects
    /// Đặt trong scene hoặc Resources để load
    /// </summary>
    [CreateAssetMenu(fileName = "GameDatabase", menuName = "Game/Game Database", order = 0)]
    public class GameDatabase : ScriptableObject
    {
        [Header("Item Database")]
        [Tooltip("Danh sách tất cả ItemDataSO trong game")]
        public List<ItemDataSO> items = new List<ItemDataSO>();

        [Header("Crop Database")]
        [Tooltip("Danh sách tất cả CropDataSO trong game")]
        public List<CropDataSO> crops = new List<CropDataSO>();

        [Header("Tool Database")]
        [Tooltip("Danh sách tất cả ToolDataSO trong game")]
        public List<ToolDataSO> tools = new List<ToolDataSO>();

        [Header("Tile State Database")]
        [Tooltip("Danh sách tất cả TileStateSO trong game")]
        public List<TileStateSO> tileStates = new List<TileStateSO>();

        [Header("Building Database")]
        [Tooltip("Danh sách tất cả BuildingSO trong game")]
        public List<BuildingSO> buildings = new List<BuildingSO>();

        // Cache dictionaries để tìm nhanh
        private Dictionary<ItemId, ItemDataSO> _itemCache;
        private Dictionary<CropId, CropDataSO> _cropCache;
        private Dictionary<ToolId, ToolDataSO> _toolCache;
        private Dictionary<string, TileStateSO> _tileStateCache;
        private Dictionary<string, BuildingSO> _buildingCache;

        /// <summary>
        /// Khởi tạo cache (gọi khi game start)
        /// </summary>
        public void Initialize()
        {
            _itemCache = new Dictionary<ItemId, ItemDataSO>();
            _cropCache = new Dictionary<CropId, CropDataSO>();
            _toolCache = new Dictionary<ToolId, ToolDataSO>();
            _tileStateCache = new Dictionary<string, TileStateSO>();
            _buildingCache = new Dictionary<string, BuildingSO>();

            foreach (var item in items)
            {
                if (item != null && !string.IsNullOrWhiteSpace(item.itemId))
                {
                    _itemCache[new ItemId(item.itemId)] = item;
                }
            }

            foreach (var crop in crops)
            {
                if (crop != null && !string.IsNullOrWhiteSpace(crop.cropId))
                {
                    _cropCache[new CropId(crop.cropId)] = crop;
                }
            }

            foreach (var tool in tools)
            {
                if (tool != null && !string.IsNullOrWhiteSpace(tool.toolId))
                {
                    _toolCache[new ToolId(tool.toolId)] = tool;
                }
            }

            foreach (var tileState in tileStates)
            {
                if (tileState != null && !string.IsNullOrWhiteSpace(tileState.stateId))
                {
                    _tileStateCache[tileState.stateId] = tileState;
                }
            }

            foreach (var building in buildings)
            {
                if (building != null && !string.IsNullOrWhiteSpace(building.buildingId))
                {
                    _buildingCache[building.buildingId] = building;
                }
            }

            Debug.Log($"GameDatabase initialized: {_itemCache.Count} items, {_cropCache.Count} crops, {_toolCache.Count} tools, {_tileStateCache.Count} tile states, {_buildingCache.Count} buildings");
        }

        /// <summary>
        /// Lấy ItemDataSO từ ItemId
        /// </summary>
        public ItemDataSO GetItemData(ItemId itemId)
        {
            if (_itemCache == null)
                Initialize();

            _itemCache.TryGetValue(itemId, out var itemData);
            return itemData;
        }

        /// <summary>
        /// Lấy Item entity từ ItemId
        /// </summary>
        public Item GetItem(ItemId itemId)
        {
            var itemData = GetItemData(itemId);
            return itemData?.ToItem();
        }

        /// <summary>
        /// Lấy CropDataSO từ CropId
        /// </summary>
        public CropDataSO GetCropData(CropId cropId)
        {
            if (_cropCache == null)
                Initialize();

            _cropCache.TryGetValue(cropId, out var cropData);
            return cropData;
        }

        /// <summary>
        /// Lấy Crop entity từ CropId
        /// </summary>
        public Crop GetCrop(CropId cropId)
        {
            var cropData = GetCropData(cropId);
            return cropData?.ToCrop();
        }

        /// <summary>
        /// Lấy ToolDataSO từ ToolId
        /// </summary>
        public ToolDataSO GetToolData(ToolId toolId)
        {
            if (_toolCache == null)
                Initialize();

            _toolCache.TryGetValue(toolId, out var toolData);
            return toolData;
        }

        /// <summary>
        /// Lấy Tool entity từ ToolId
        /// </summary>
        public Tool GetTool(ToolId toolId)
        {
            var toolData = GetToolData(toolId);
            return toolData?.ToTool();
        }

        /// <summary>
        /// Lấy CropDataSO từ Seed ItemId (tìm crop có seedItemId trùng)
        /// </summary>
        public CropDataSO GetCropDataFromSeed(ItemId seedItemId)
        {
            if (_cropCache == null)
                Initialize();

            return crops.FirstOrDefault(c => 
                c != null && 
                new ItemId(c.seedItemId) == seedItemId
            );
        }

        /// <summary>
        /// Lấy TileStateSO từ state ID
        /// </summary>
        public TileStateSO GetTileState(string stateId)
        {
            if (_tileStateCache == null)
                Initialize();

            if (string.IsNullOrWhiteSpace(stateId)) return null;
            _tileStateCache.TryGetValue(stateId, out var state);
            return state;
        }

        /// <summary>
        /// Lấy BuildingSO từ building ID
        /// </summary>
        public BuildingSO GetBuilding(string buildingId)
        {
            if (_buildingCache == null)
                Initialize();

            if (string.IsNullOrWhiteSpace(buildingId)) return null;
            _buildingCache.TryGetValue(buildingId, out var building);
            return building;
        }

        /// <summary>
        /// Validate database (gọi trong Editor)
        /// </summary>
        [ContextMenu("Validate Database")]
        public void ValidateDatabase()
        {
            var errors = new List<string>();

            // Check duplicate IDs
            var itemIds = items.Where(i => i != null).Select(i => i.itemId).ToList();
            var duplicateItems = itemIds.GroupBy(id => id).Where(g => g.Count() > 1);
            foreach (var dup in duplicateItems)
            {
                errors.Add($"Duplicate Item ID: {dup.Key}");
            }

            var cropIds = crops.Where(c => c != null).Select(c => c.cropId).ToList();
            var duplicateCrops = cropIds.GroupBy(id => id).Where(g => g.Count() > 1);
            foreach (var dup in duplicateCrops)
            {
                errors.Add($"Duplicate Crop ID: {dup.Key}");
            }

            var tileStateIds = tileStates.Where(t => t != null).Select(t => t.stateId).ToList();
            var duplicateTileStates = tileStateIds.GroupBy(id => id).Where(g => g.Count() > 1);
            foreach (var dup in duplicateTileStates)
            {
                errors.Add($"Duplicate TileState ID: {dup.Key}");
            }

            var buildingIds = buildings.Where(b => b != null).Select(b => b.buildingId).ToList();
            var duplicateBuildings = buildingIds.GroupBy(id => id).Where(g => g.Count() > 1);
            foreach (var dup in duplicateBuildings)
            {
                errors.Add($"Duplicate Building ID: {dup.Key}");
            }

            if (errors.Count > 0)
            {
                Debug.LogError($"Database Validation Errors:\n{string.Join("\n", errors)}");
            }
            else
            {
                Debug.Log("Database validation passed!");
            }
        }
    }
}

