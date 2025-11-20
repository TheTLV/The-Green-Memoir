using System.Linq;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Core.Application.Services
{
    /// <summary>
    /// Dịch vụ xử lý trồng trọt
    /// Dùng TileStateSO để quản lý states, không hardcode
    /// </summary>
    public class FarmingService
    {
        private readonly IFarmRepository _farmRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ITimeService _timeService;
        private readonly IEventBus _eventBus;
        private readonly ICropDatabase _cropDatabase;
        private readonly IItemDatabase _itemDatabase;
        private readonly ITileStateDatabase _tileStateDatabase;

        public FarmingService(
            IFarmRepository farmRepository,
            IInventoryRepository inventoryRepository,
            ITimeService timeService,
            IEventBus eventBus,
            ICropDatabase cropDatabase = null,
            IItemDatabase itemDatabase = null,
            ITileStateDatabase tileStateDatabase = null)
        {
            _farmRepository = farmRepository;
            _inventoryRepository = inventoryRepository;
            _timeService = timeService;
            _eventBus = eventBus;
            _cropDatabase = cropDatabase;
            _itemDatabase = itemDatabase;
            _tileStateDatabase = tileStateDatabase;
        }

        /// <summary>
        /// Cuốc đất (dùng TileStateSO)
        /// </summary>
        public FarmingResult PlowTile(TilePosition position, PlayerId playerId)
        {
            var tile = _farmRepository.GetTile(position);
            
            // Lấy current state từ TileStateSO
            var currentState = _tileStateDatabase?.GetTileState(tile.CurrentStateId);
            if (currentState == null)
            {
                // Fallback: dùng default state
                currentState = _tileStateDatabase?.GetTileStateByType(TileStateSO.TileStateType.Normal);
            }

            if (!tile.CanPlow(currentState))
            {
                return FarmingResult.Failed("Cannot plow this tile");
            }

            // Lấy next state từ TileStateSO
            var nextState = currentState?.GetNextStateForAction("plow");
            if (nextState == null)
            {
                // Fallback: dùng Plowed state
                nextState = _tileStateDatabase?.GetTileStateByType(TileStateSO.TileStateType.Plowed);
            }

            if (nextState == null)
            {
                return FarmingResult.Failed("Plowed state not found in database");
            }

            tile.Plow(nextState);
            _farmRepository.SaveTile(tile);

            return FarmingResult.Success();
        }

        /// <summary>
        /// Tưới nước (dùng TileStateSO)
        /// </summary>
        public FarmingResult WaterTile(TilePosition position, PlayerId playerId)
        {
            var tile = _farmRepository.GetTile(position);
            
            var currentState = _tileStateDatabase?.GetTileState(tile.CurrentStateId);
            if (currentState == null)
            {
                currentState = _tileStateDatabase?.GetTileStateByType(TileStateSO.TileStateType.Plowed);
            }

            if (!tile.CanWater(currentState))
            {
                return FarmingResult.Failed("Cannot water this tile");
            }

            // Lấy next state (Watered hoặc SeededWatered)
            var nextState = currentState?.GetNextStateForAction("water");
            if (nextState == null)
            {
                // Nếu đã trồng, dùng SeededWatered, nếu không dùng Watered
                if (tile.IsPlanted)
                {
                    nextState = _tileStateDatabase?.GetTileStateByType(TileStateSO.TileStateType.SeededWatered);
                }
                else
                {
                    nextState = _tileStateDatabase?.GetTileStateByType(TileStateSO.TileStateType.Watered);
                }
            }

            if (nextState == null)
            {
                return FarmingResult.Failed("Watered state not found in database");
            }

            tile.Water(nextState);
            _farmRepository.SaveTile(tile);

            return FarmingResult.Success();
        }

        /// <summary>
        /// Trồng hạt giống bằng CropId (load từ database)
        /// </summary>
        public FarmingResult PlantSeed(TilePosition position, CropId cropId, PlayerId playerId)
        {
            if (_cropDatabase == null)
            {
                return FarmingResult.Failed("CropDatabase not available");
            }

            var crop = _cropDatabase.GetCrop(cropId);
            if (crop == null)
            {
                return FarmingResult.Failed($"Crop with ID {cropId} not found in database");
            }

            return PlantSeed(position, crop, playerId);
        }

        /// <summary>
        /// Trồng hạt giống (dùng Crop entity và TileStateSO)
        /// </summary>
        public FarmingResult PlantSeed(TilePosition position, Crop crop, PlayerId playerId)
        {
            var tile = _farmRepository.GetTile(position);
            
            var currentState = _tileStateDatabase?.GetTileState(tile.CurrentStateId);
            if (currentState == null)
            {
                currentState = _tileStateDatabase?.GetTileStateByType(TileStateSO.TileStateType.Plowed);
            }

            if (!tile.CanPlant(currentState))
            {
                return FarmingResult.Failed("Cannot plant on this tile");
            }

            if (crop == null)
            {
                return FarmingResult.Failed("Crop is null");
            }

            // Lấy next state (Seeded hoặc Growing)
            var nextState = currentState?.GetNextStateForAction("plant");
            if (nextState == null)
            {
                nextState = _tileStateDatabase?.GetTileStateByType(TileStateSO.TileStateType.Seeded);
            }

            if (nextState == null)
            {
                return FarmingResult.Failed("Seeded state not found in database");
            }

            tile.Plant(crop, nextState);
            _farmRepository.SaveTile(tile);

            _eventBus.Publish(new Events.CropPlantedEvent(position, crop.Id));

            return FarmingResult.Success();
        }

        /// <summary>
        /// Thu hoạch cây
        /// </summary>
        public FarmingResult HarvestCrop(TilePosition position, PlayerId playerId)
        {
            var tile = _farmRepository.GetTile(position);

            if (!tile.CanHarvest())
            {
                return FarmingResult.Failed("Cannot harvest this tile");
            }

            var harvestResult = tile.Harvest();

            if (!harvestResult.IsSuccess)
            {
                return FarmingResult.Failed(harvestResult.ErrorMessage);
            }

            // Set state về Plowed sau khi thu hoạch
            var plowedState = _tileStateDatabase?.GetTileStateByType(TileStateSO.TileStateType.Plowed);
            if (plowedState != null)
            {
                tile.SetState(plowedState.stateId);
            }

            // Thêm vật phẩm thu hoạch vào túi đồ
            if (_itemDatabase != null)
            {
                var item = _itemDatabase.GetItem(harvestResult.ItemId);
                if (item != null)
                {
                    var inventory = _inventoryRepository.GetInventory(playerId);
                    if (inventory.AddItem(item, harvestResult.Quantity))
                    {
                        _inventoryRepository.SaveInventory(playerId, inventory);
                        _eventBus.Publish(new Events.ItemAddedEvent(playerId, harvestResult.ItemId, harvestResult.Quantity));
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Item {harvestResult.ItemId} not found in database after harvest!");
                }
            }

            _farmRepository.SaveTile(tile);

            _eventBus.Publish(new Events.CropHarvestedEvent(position, harvestResult.ItemId, harvestResult.Quantity));

            return FarmingResult.Success();
        }

        /// <summary>
        /// Cập nhật tất cả cây trồng (gọi mỗi ngày)
        /// </summary>
        public void UpdateAllCrops(int daysPassed)
        {
            var allTiles = _farmRepository.GetAllTiles().ToList();

            foreach (var tile in allTiles)
            {
                tile.UpdateCrop(daysPassed);
                
                // Update state nếu cây phát triển (dùng TileStateSO)
                if (tile.IsPlanted && tile.Crop != null)
                {
                    var currentState = _tileStateDatabase?.GetTileState(tile.CurrentStateId);
                    if (currentState != null && currentState.allowCropGrowth)
                    {
                        // Cây phát triển → chuyển sang Growing hoặc Mature
                        if (tile.Crop.IsMature)
                        {
                            var matureState = _tileStateDatabase?.GetTileStateByType(TileStateSO.TileStateType.Mature);
                            if (matureState != null)
                            {
                                tile.SetState(matureState.stateId);
                            }
                        }
                        else
                        {
                            var growingState = _tileStateDatabase?.GetTileStateByType(TileStateSO.TileStateType.Growing);
                            if (growingState != null)
                            {
                                tile.SetState(growingState.stateId);
                            }
                        }
                    }
                }
            }

            _farmRepository.SaveAllTiles(allTiles);
        }
    }

    /// <summary>
    /// Kết quả của hành động farming
    /// </summary>
    public class FarmingResult
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        private FarmingResult(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static FarmingResult Success()
        {
            return new FarmingResult(true, null);
        }

        public static FarmingResult Failed(string errorMessage)
        {
            return new FarmingResult(false, errorMessage);
        }
    }
}
