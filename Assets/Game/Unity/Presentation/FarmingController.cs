using UnityEngine;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Application.Commands;
using TheGreenMemoir.Core.Domain.Enums;
using TheGreenMemoir.Core.Domain.Entities;

namespace TheGreenMemoir.Unity.Presentation
{
    /// <summary>
    /// Farming Controller - Quản lý farming system
    /// Flexible: Không lỗi nếu thiếu components
    /// </summary>
    public class FarmingController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private PlayerId playerId = PlayerId.Default;
        [SerializeField] private ToolActionType currentTool = ToolActionType.Plow;

        [Header("References (Optional)")]
        [SerializeField] private FarmingUIController farmingUIController;

        private void Start()
        {
            // Tự động tìm FarmingUIController nếu chưa gán
            if (farmingUIController == null)
                farmingUIController = FindFirstObjectByType<FarmingUIController>();
        }

        /// <summary>
        /// Cuốc đất tại vị trí
        /// </summary>
        public void PlowTile(TilePosition position)
        {
            if (GameManager.FarmingService == null) return;

            var command = new PlowTileCommand(
                GameManager.FarmingService,
                position,
                playerId
            );

            if (command.CanExecute())
            {
                var result = GameManager.CommandInvoker.ExecuteCommand(command);
                if (!result.IsSuccess)
                {
                    Debug.LogWarning($"Plow failed: {result.ErrorMessage}");
                }
            }
        }

        /// <summary>
        /// Tưới nước tại vị trí
        /// </summary>
        public void WaterTile(TilePosition position)
        {
            if (GameManager.FarmingService == null) return;

            var command = new WaterTileCommand(
                GameManager.FarmingService,
                position,
                playerId
            );

            if (command.CanExecute())
            {
                var result = GameManager.CommandInvoker.ExecuteCommand(command);
                if (!result.IsSuccess)
                {
                    Debug.LogWarning($"Water failed: {result.ErrorMessage}");
                }
            }
        }

        /// <summary>
        /// Trồng cây tại vị trí
        /// </summary>
        public void PlantCrop(TilePosition position, CropId cropId)
        {
            if (GameManager.FarmingService == null) return;

            // Dùng PlantSeedByIdCommand để load crop từ database
            var command = new PlantSeedByIdCommand(
                GameManager.FarmingService,
                position,
                cropId,
                playerId
            );

            if (command.CanExecute())
            {
                var result = GameManager.CommandInvoker.ExecuteCommand(command);
                if (!result.IsSuccess)
                {
                    Debug.LogWarning($"Plant failed: {result.ErrorMessage}");
                }
            }
        }

        /// <summary>
        /// Thu hoạch tại vị trí
        /// </summary>
        public void HarvestCrop(TilePosition position)
        {
            if (GameManager.FarmingService == null) return;

            var command = new HarvestCropCommand(
                GameManager.FarmingService,
                position,
                playerId
            );

            if (command.CanExecute())
            {
                var result = GameManager.CommandInvoker.ExecuteCommand(command);
                if (!result.IsSuccess)
                {
                    Debug.LogWarning($"Harvest failed: {result.ErrorMessage}");
                }
            }
        }

        /// <summary>
        /// Set tool hiện tại
        /// </summary>
        public void SetTool(ToolActionType tool)
        {
            currentTool = tool;
            if (farmingUIController != null)
            {
                farmingUIController.SetTool(tool);
            }
        }

        /// <summary>
        /// Tự động xác định action dựa trên tile state
        /// </summary>
        public void InteractWithTile(TilePosition position)
        {
            if (GameManager.FarmingService == null || GameManager.TileStateDatabase == null) return;

            var tile = GameManager.FarmRepository?.GetTile(position);
            if (tile == null) return;

            var currentState = GameManager.TileStateDatabase.GetTileState(tile.CurrentStateId);

            // Tự động chọn action
            if (currentState != null)
            {
                if (currentState.canHarvest && tile.CanHarvest(currentState))
                {
                    HarvestCrop(position);
                }
                else if (currentState.canPlant && tile.CanPlant(currentState))
                {
                    // TODO: Cần chọn crop từ inventory
                    Debug.Log("Plant action - Need to select crop from inventory");
                }
                else if (currentState.canWater && tile.CanWater(currentState))
                {
                    WaterTile(position);
                }
                else if (currentState.canPlow && tile.CanPlow(currentState))
                {
                    PlowTile(position);
                }
            }
        }
    }
}

