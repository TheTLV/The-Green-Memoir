using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Core.Domain.Entities
{
    /// <summary>
    /// Ô đất trong nông trại
    /// Dùng TileStateSO thay vì enum để linh hoạt hơn
    /// </summary>
    public class FarmTile
    {
        public TilePosition Position { get; }
        public string CurrentStateId { get; private set; } // State ID từ TileStateSO
        public Crop Crop { get; private set; }
        public bool IsWatered { get; private set; }

        public bool IsPlanted => Crop != null;
        public bool IsEmpty => Crop == null;

        public FarmTile(TilePosition position, string initialStateId = "normal")
        {
            Position = position;
            CurrentStateId = initialStateId;
            Crop = null;
            IsWatered = false;
        }

        /// <summary>
        /// Kiểm tra xem có thể cuốc đất không (dùng TileStateSO)
        /// </summary>
        public bool CanPlow(TileStateSO currentState)
        {
            if (currentState == null) return false;
            return currentState.canPlow && !IsPlanted;
        }

        /// <summary>
        /// Cuốc đất (chỉ thay đổi state, không render)
        /// </summary>
        public void Plow(TileStateSO nextState)
        {
            if (nextState == null || !nextState.canPlow) return;
            CurrentStateId = nextState.stateId;
        }

        /// <summary>
        /// Kiểm tra xem có thể tưới nước không (dùng TileStateSO)
        /// </summary>
        public bool CanWater(TileStateSO currentState)
        {
            if (currentState == null) return false;
            return currentState.canWater && !IsWatered;
        }

        /// <summary>
        /// Tưới nước (chỉ thay đổi state, không render)
        /// </summary>
        public void Water(TileStateSO nextState)
        {
            if (nextState == null || !nextState.canWater) return;
            
            IsWatered = true;
            CurrentStateId = nextState.stateId;

            if (Crop != null)
            {
                Crop.Water();
            }
        }

        /// <summary>
        /// Kiểm tra xem có thể trồng cây không (dùng TileStateSO)
        /// </summary>
        public bool CanPlant(TileStateSO currentState)
        {
            if (currentState == null) return false;
            return currentState.canPlant && !IsPlanted;
        }

        /// <summary>
        /// Trồng cây (chỉ thay đổi state, không render)
        /// </summary>
        public void Plant(Crop crop, TileStateSO nextState)
        {
            if (crop == null || nextState == null || !nextState.canPlant) return;
            
            Crop = crop;
            CurrentStateId = nextState.stateId;
        }

        /// <summary>
        /// Kiểm tra xem có thể thu hoạch không (dùng TileStateSO)
        /// </summary>
        public bool CanHarvest(TileStateSO currentState = null)
        {
            if (currentState != null)
            {
                return currentState.canHarvest && IsPlanted && Crop != null && Crop.IsMature;
            }
            return IsPlanted && Crop != null && Crop.IsMature;
        }
        
        /// <summary>
        /// Set state (dùng khi harvest hoặc update)
        /// </summary>
        public void SetState(string stateId)
        {
            if (!string.IsNullOrWhiteSpace(stateId))
            {
                CurrentStateId = stateId;
            }
        }

        /// <summary>
        /// Thu hoạch cây
        /// </summary>
        public HarvestResult Harvest()
        {
            if (!CanHarvest())
                return HarvestResult.Failed("Cannot harvest this tile");

            var result = Crop.Harvest();
            
            if (result.IsSuccess)
            {
                Crop = null;
                // State sẽ được set từ FarmingService dựa trên TileStateSO
                IsWatered = false;
            }

            return result;
        }

        /// <summary>
        /// Cập nhật cây trồng (gọi mỗi ngày)
        /// </summary>
        public void UpdateCrop(int daysPassed)
        {
            if (Crop != null)
            {
                Crop.UpdateGrowth(daysPassed);

                // Nếu cây bị héo, xóa cây
                if (Crop.IsWilted)
                {
                    Crop = null;
                    // State sẽ được set từ FarmingService dựa trên TileStateSO
                    IsWatered = false;
                }
            }

            // Reset trạng thái tưới nước mỗi ngày
            if (daysPassed > 0)
            {
                IsWatered = false;
            }
        }
    }
}

