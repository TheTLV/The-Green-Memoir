using UnityEngine;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Enums;
using TheGreenMemoir.Core.Application.Commands;
using TheGreenMemoir.Unity.Presentation.UI;

namespace TheGreenMemoir.Unity.Presentation
{
    /// <summary>
    /// Hệ thống xử lý tương tác tool với tilemap
    /// Kiểm tra layer, trạng thái tile, và tạo commands
    /// </summary>
    public class ToolInteractionSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TilemapManager tilemapManager;
        [SerializeField] private SeedSelectionUI seedSelectionUI;
        [SerializeField] private TheGreenMemoir.Unity.UI.ToolInteractionMenu toolInteractionMenu;
        [SerializeField] private Managers.ToolManager toolManager;

        [Header("Settings")]
        [SerializeField] private PlayerId playerId = PlayerId.Default;
        [SerializeField] private ToolActionType currentTool = ToolActionType.Plow;
        [SerializeField] private TheGreenMemoir.Unity.Data.ToolDataSO currentToolData = null;

        /// <summary>
        /// Lấy tool data hiện tại (dùng bởi PlayerController để trigger animation)
        /// </summary>
        public TheGreenMemoir.Unity.Data.ToolDataSO GetCurrentToolData()
        {
            return currentToolData;
        }

        private void Awake()
        {
            if (tilemapManager == null)
                tilemapManager = FindFirstObjectByType<TilemapManager>();

            if (seedSelectionUI == null)
                seedSelectionUI = FindFirstObjectByType<SeedSelectionUI>();
            
            if (toolInteractionMenu == null)
                toolInteractionMenu = FindFirstObjectByType<TheGreenMemoir.Unity.UI.ToolInteractionMenu>();
            
            if (toolManager == null)
                toolManager = FindFirstObjectByType<Managers.ToolManager>();
        }

        /// <summary>
        /// Đặt tool hiện tại (từ ToolActionType)
        /// </summary>
        public void SetTool(ToolActionType tool)
        {
            currentTool = tool;
            currentToolData = null; // Reset tool data
            Debug.Log($"Tool changed to: {tool}");
        }

        /// <summary>
        /// Đặt tool hiện tại (từ ToolDataSO)
        /// </summary>
        public void SetTool(TheGreenMemoir.Unity.Data.ToolDataSO toolData)
        {
            if (toolData != null)
            {
                currentToolData = toolData;
                currentTool = toolData.actionType;
                Debug.Log($"Tool changed to: {toolData.toolName} ({toolData.actionType})");
            }
        }

        /// <summary>
        /// Xử lý khi click vào tile
        /// Tool chỉ tác dụng lên ground tiles (ruộng), không ảnh hưởng house/background
        /// </summary>
        public void OnTileClicked(Vector3Int tilePos)
        {
            // Nếu không có tool được chọn - không làm gì (silent fail)
            // Tool chỉ tác dụng lên ground tiles, không ảnh hưởng game khi click vào house/background
            if (currentTool == ToolActionType.None || currentToolData == null)
            {
                return; // Silent fail - tool không cần deselect
            }

            // Kiểm tra tool đặc biệt - hiển thị menu tương tác
            if (currentToolData.isSpecialTool && toolInteractionMenu != null)
            {
                HandleSpecialTool(tilePos);
                return;
            }

            // Kiểm tra tool có thể tương tác với tile state không
            // TODO: Cần implement GetTileState từ tile position (hiện tại chỉ có GetTileState với stateId)
            // Tạm thời bỏ qua check này, sẽ dùng CanInteractWithTile thay thế
            // var tileState = tilemapManager.GetTileState(tilePos);
            // if (tileState != null && !currentToolData.CanInteractWithTileState(tileState.stateId))
            // {
            //     Debug.LogWarning($"Cannot use {currentToolData.toolName} on tile state {tileState.stateId}");
            //     return;
            // }

            // Kiểm tra tool có thể tương tác với tile không (backward compatibility)
            // Tool chỉ tác dụng lên ground tiles (ruộng), không ảnh hưởng house/background
            if (!tilemapManager.CanInteractWithTile(tilePos, currentTool))
            {
                // Silent fail - tool không thể tương tác với tile này (có thể là house/background)
                return;
            }

            // Xử lý theo từng loại tool
            switch (currentTool)
            {
                case ToolActionType.Plow:
                    HandlePlow(tilePos);
                    break;

                case ToolActionType.Plant:
                    HandlePlant(tilePos);
                    break;

                case ToolActionType.Water:
                    HandleWater(tilePos);
                    break;

                case ToolActionType.Harvest:
                    HandleHarvest(tilePos);
                    break;

                default:
                    Debug.LogWarning($"Tool {currentTool} not implemented");
                    break;
            }
        }

        /// <summary>
        /// Xử lý tool đặc biệt (ví dụ: găng tay để gieo hạt)
        /// </summary>
        private void HandleSpecialTool(Vector3Int tilePos)
        {
            if (currentToolData == null || toolInteractionMenu == null)
                return;

            // Kiểm tra loại tương tác đặc biệt
            if (currentToolData.specialInteractionType == "SeedSelection")
            {
                // Hiển thị menu chọn hạt giống từ inventory
                toolInteractionMenu.ShowInteractionMenu(
                    currentToolData.filterItemTag,
                    "Select Seed",
                    (itemId) => OnSeedSelectedForPlanting(tilePos, itemId),
                    () => Debug.Log("Leave clicked")
                );
            }
            else
            {
                Debug.LogWarning($"Unknown special interaction type: {currentToolData.specialInteractionType}");
            }
        }

        /// <summary>
        /// Xử lý khi chọn seed từ menu tương tác
        /// </summary>
        private void OnSeedSelectedForPlanting(Vector3Int tilePos, ItemId seedId)
        {
            var tilePosition = new TilePosition(tilePos.x, tilePos.y);
            
            // Lấy CropData từ seed
            var database = GameDatabaseManager.GetDatabase();
            if (database == null)
            {
                Debug.LogError("GameDatabase not found!");
                return;
            }

            var cropData = database.GetCropDataFromSeed(seedId);
            if (cropData == null)
            {
                Debug.LogWarning($"CropData not found for seed {seedId}");
                return;
            }

            // Kiểm tra còn seed trong inventory không
            var inventory = GameManager.InventoryService.GetInventory(playerId);
            if (!inventory.HasItem(seedId, 1))
            {
                Debug.LogWarning("Không còn hạt giống này!");
                return;
            }

            // Trừ seed khỏi inventory
            GameManager.InventoryService.RemoveItem(playerId, seedId, 1);

            // Trồng cây
            var command = new PlantSeedByIdCommand(
                GameManager.FarmingService,
                tilePosition,
                new CropId(cropData.cropId),
                playerId
            );

            var result = GameManager.CommandInvoker.ExecuteCommand(command);

            if (!result.IsSuccess)
            {
                // Trả lại seed nếu trồng thất bại
                GameManager.InventoryService.AddItemById(playerId, seedId, 1);
                Debug.LogWarning($"Plant failed: {result.ErrorMessage}");
            }
        }

        /// <summary>
        /// Xử lý cuốc đất
        /// </summary>
        private void HandlePlow(Vector3Int tilePos)
        {
            // Kiểm tra tool có thể sử dụng không (usage count)
            if (currentToolData != null && toolManager != null)
            {
                var toolState = toolManager.GetToolState(currentToolData.toolId);
                if (toolState == null || !toolState.CanUse())
                {
                    Debug.LogWarning($"Tool {currentToolData.toolName} cannot be used (out of uses or not unlocked)");
                    return;
                }
            }

            var tilePosition = new TilePosition(tilePos.x, tilePos.y);
            var command = new PlowTileCommand(
                GameManager.FarmingService,
                tilePosition,
                playerId
            );

            var result = GameManager.CommandInvoker.ExecuteCommand(command);
            
            if (result.IsSuccess)
            {
                // Đổi sprite tile
                tilemapManager.SetTileState(tilePos, "plowed");
                
                // Giảm usage count
                if (currentToolData != null && toolManager != null)
                {
                    toolManager.UseTool(currentToolData.toolId);
                }
            }
            else
            {
                Debug.LogWarning($"Plow failed: {result.ErrorMessage}");
            }
        }

        /// <summary>
        /// Xử lý trồng cây - hiển thị seed selection UI
        /// </summary>
        private void HandlePlant(Vector3Int tilePos)
        {
            var tilePosition = new TilePosition(tilePos.x, tilePos.y);

            // Hiển thị UI chọn hạt giống
            if (seedSelectionUI != null)
            {
                seedSelectionUI.ShowSeedSelection(tilePosition);
            }
            else
            {
                Debug.LogWarning("SeedSelectionUI not found!");
            }
        }

        /// <summary>
        /// Xử lý tưới nước
        /// </summary>
        private void HandleWater(Vector3Int tilePos)
        {
            var tilePosition = new TilePosition(tilePos.x, tilePos.y);
            // TODO: Tạo WaterTileCommand
            // var command = new WaterTileCommand(...);
            // GameManager.CommandInvoker.ExecuteCommand(command);
            
            Debug.Log($"Water tile at {tilePosition}");
        }

        /// <summary>
        /// Xử lý thu hoạch
        /// </summary>
        private void HandleHarvest(Vector3Int tilePos)
        {
            var tilePosition = new TilePosition(tilePos.x, tilePos.y);
            var command = new HarvestCropCommand(
                GameManager.FarmingService,
                tilePosition,
                playerId
            );

            var result = GameManager.CommandInvoker.ExecuteCommand(command);
            
            if (!result.IsSuccess)
            {
                Debug.LogWarning($"Harvest failed: {result.ErrorMessage}");
            }
        }

        /// <summary>
        /// Xử lý click chuột (gọi từ InputHandler)
        /// </summary>
        public void HandleMouseClick()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                var tilePos = tilemapManager.GetTilePositionFromMouse();
                var cellPos = new Vector3Int(tilePos.X, tilePos.Y, 0);
                OnTileClicked(cellPos);
            }
        }
    }
}

