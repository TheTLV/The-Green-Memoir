using UnityEngine;
using System.Collections.Generic;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Tile State SO - Định nghĩa trạng thái tile (không cần sprite)
    /// Cho phép designer thêm state mới chỉ bằng SO
    /// </summary>
    [CreateAssetMenu(fileName = "TileState", menuName = "Game/Tile State", order = 36)]
    public class TileStateSO : ScriptableObject
    {
        [System.Serializable]
        public enum TileStateType
        {
            Normal,          // Đất bình thường
            Plowed,          // Đã cuốc
            Watered,         // Đã tưới
            Seeded,          // Đã gieo hạt
            SeededWatered,   // Đã gieo và tưới
            Growing,         // Đang phát triển
            Mature,          // Đã chín
            Dead,            // Đã chết
            Poisoned,        // Đất độc (custom)
            Dry,             // Đất khô (custom)
            Fertile          // Đất màu mỡ (custom)
        }

        [Header("State Info")]
        [Tooltip("ID của state (để reference)")]
        public string stateId;
        
        [Tooltip("Tên hiển thị")]
        public string displayName;
        
        [Tooltip("Loại state")]
        public TileStateType stateType = TileStateType.Normal;

        [Header("Properties")]
        [Tooltip("Có cho phép cây phát triển không")]
        public bool allowCropGrowth = false;
        
        [Tooltip("Có thể cuốc không")]
        public bool canPlow = false;
        
        [Tooltip("Có thể trồng không")]
        public bool canPlant = false;
        
        [Tooltip("Có thể tưới không")]
        public bool canWater = false;
        
        [Tooltip("Có thể thu hoạch không")]
        public bool canHarvest = false;

        [Header("Transitions")]
        [Tooltip("Danh sách các state có thể chuyển tiếp từ state này")]
        public List<TileStateSO> nextValidStates = new List<TileStateSO>();

        [Header("Visual (Optional - không bắt buộc)")]
        [Tooltip("Sprite cho state này (optional, game vẫn chạy được không có sprite)")]
        public Sprite sprite;
        
        [Tooltip("TileBase cho Unity Tilemap (optional)")]
        public UnityEngine.Tilemaps.TileBase tileBase;
        
        [Tooltip("Màu sắc để hiển thị (nếu không có sprite)")]
        public Color displayColor = Color.white;

        /// <summary>
        /// Kiểm tra xem có thể chuyển sang state khác không
        /// </summary>
        public bool CanTransitionTo(TileStateSO targetState)
        {
            if (targetState == null) return false;
            return nextValidStates.Contains(targetState);
        }

        /// <summary>
        /// Lấy state tiếp theo dựa trên action
        /// </summary>
        public TileStateSO GetNextStateForAction(string action)
        {
            foreach (var state in nextValidStates)
            {
                switch (action.ToLower())
                {
                    case "plow":
                        if (state.canPlow) return state;
                        break;
                    case "water":
                        if (state.canWater) return state;
                        break;
                    case "plant":
                        if (state.canPlant) return state;
                        break;
                }
            }
            return null;
        }
    }
}

