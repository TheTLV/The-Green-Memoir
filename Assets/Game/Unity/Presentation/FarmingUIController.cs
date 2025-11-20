using UnityEngine;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Application.Commands;
using TheGreenMemoir.Core.Domain.Enums;

namespace TheGreenMemoir.Unity.Presentation
{
    /// <summary>
    /// Controller để xử lý tương tác với đất (cuốc, trồng, thu hoạch)
    /// </summary>
    public class FarmingUIController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private PlayerId playerId = PlayerId.Default;
        [SerializeField] private ToolActionType currentTool = ToolActionType.Plow;

        /// <summary>
        /// Được gọi khi click vào tile
        /// </summary>
        public void OnTileClicked(TilePosition position)
        {
            ICommand command = CreateCommand(currentTool, position);

            if (command != null && command.CanExecute())
            {
                var result = GameManager.CommandInvoker.ExecuteCommand(command);
                
                if (!result.IsSuccess)
                {
                    Debug.LogWarning($"Farming action failed: {result.ErrorMessage}");
                }
            }
        }

        /// <summary>
        /// Đặt tool hiện tại
        /// </summary>
        public void SetTool(ToolActionType tool)
        {
            currentTool = tool;
        }

        /// <summary>
        /// Tạo command dựa trên tool và vị trí
        /// </summary>
        private ICommand CreateCommand(ToolActionType tool, TilePosition position)
        {
            switch (tool)
            {
                case ToolActionType.Plow:
                    return new PlowTileCommand(
                        GameManager.FarmingService,
                        position,
                        playerId
                    );

                case ToolActionType.Harvest:
                    return new HarvestCropCommand(
                        GameManager.FarmingService,
                        position,
                        playerId
                    );

                // TODO: Thêm các tool khác (Water, Plant)
                default:
                    Debug.LogWarning($"Tool {tool} not implemented yet");
                    return null;
            }
        }

        /// <summary>
        /// Tự động xác định action dựa trên tile state
        /// </summary>
        public void OnTileInteract(TilePosition position)
        {
            // Có thể tự động chọn action dựa trên tile state
            // Ví dụ: nếu tile chưa cuốc thì cuốc, nếu đã cuốc thì trồng, v.v.
            OnTileClicked(position);
        }
    }
}

