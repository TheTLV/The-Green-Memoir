using UnityEngine;
using UnityEngine.Tilemaps;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Enums;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Application.Commands;

namespace TheGreenMemoir.Unity.Presentation
{
    /// <summary>
    /// Simple Farming Controller - Đơn giản hóa farming system
    /// Click chuột để cuốc/trồng/tưới
    /// </summary>
    public class SimpleFarmingController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TilemapManager tilemapManager;
        [SerializeField] private ToolInteractionSystem toolSystem;

        [Header("Current Tool")]
        [Tooltip("Tool hiện tại (tự động lấy từ quickslot nếu có)")]
        [SerializeField] private ToolActionType currentTool = ToolActionType.Plow;
        
        /// <summary>
        /// Set tool hiện tại (gọi từ quickslot UI)
        /// </summary>
        public void SetTool(ToolActionType tool)
        {
            currentTool = tool;
            Debug.Log($"Tool changed to: {tool}");
        }
        
        /// <summary>
        /// Lấy tool hiện tại
        /// </summary>
        public ToolActionType GetCurrentTool()
        {
            return currentTool;
        }

        [Header("Settings")]
        [SerializeField] private PlayerId playerId = PlayerId.Default;

        void Start()
        {
            if (tilemapManager == null)
                tilemapManager = FindFirstObjectByType<TilemapManager>();
            
            if (toolSystem == null)
                toolSystem = FindFirstObjectByType<ToolInteractionSystem>();
        }

        void Update()
        {
            // Click chuột để tương tác
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (tilemapManager != null && toolSystem != null)
                {
                    var tilePos = tilemapManager.GetTilePositionFromMouse();
                    var cellPos = new Vector3Int(tilePos.X, tilePos.Y, 0);
                    
                    // Set tool trước
                    toolSystem.SetTool(currentTool);
                    
                    // Gọi tool system
                    toolSystem.OnTileClicked(cellPos);
                }
            }

            // Đổi tool bằng số
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) currentTool = ToolActionType.Plow;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) currentTool = ToolActionType.Plant;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) currentTool = ToolActionType.Water;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4)) currentTool = ToolActionType.Harvest;
        }
    }
}

