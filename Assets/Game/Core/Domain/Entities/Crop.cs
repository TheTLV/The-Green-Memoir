using System;
using TheGreenMemoir.Core.Domain.Enums;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Domain.Entities
{
    /// <summary>
    /// Cây trồng trên ô đất
    /// </summary>
    public class Crop
    {
        public CropId Id { get; }
        public string CropName { get; }
        public int DaysToGrow { get; }
        public int DaysToWilt { get; }
        public int HarvestYield { get; }
        public ItemId HarvestItemId { get; }

        public GrowthStage CurrentStage { get; private set; }
        public int DaysPlanted { get; private set; }
        public int DaysSinceWatered { get; private set; } // Số ngày không tưới liên tiếp
        public bool IsWateredToday { get; private set; } // Đã tưới hôm nay chưa
        public int TimesWatered { get; private set; } // Tổng số lần đã tưới
        public bool IsWilted => CurrentStage == GrowthStage.Wilted;
        public bool IsMature => CurrentStage == GrowthStage.Mature;
        
        /// <summary>
        /// Trạng thái tưới nước: true nếu đã tưới hôm nay, false nếu chưa
        /// </summary>
        public bool NeedsWater => !IsWateredToday && !IsWilted;

        public Crop(CropId id, string cropName, int daysToGrow, int daysToWilt, 
            int harvestYield, ItemId harvestItemId)
        {
            Id = id;
            CropName = cropName ?? throw new ArgumentNullException(nameof(cropName));
            DaysToGrow = daysToGrow;
            DaysToWilt = daysToWilt;
            HarvestYield = harvestYield;
            HarvestItemId = harvestItemId;
            CurrentStage = GrowthStage.Seed;
            DaysPlanted = 0;
            DaysSinceWatered = 0;
            IsWateredToday = false;
            TimesWatered = 0;
        }

        /// <summary>
        /// Tưới nước cho cây
        /// Logic thực tế: Tưới nước → cây lớn lên, không tưới → héo
        /// </summary>
        public void Water()
        {
            if (IsWilted)
                return; // Không thể tưới cây đã héo

            IsWateredToday = true;
            DaysSinceWatered = 0; // Reset số ngày không tưới
            TimesWatered++;
        }

        /// <summary>
        /// Cập nhật sự phát triển của cây (gọi mỗi ngày)
        /// Logic thực tế:
        /// - Nếu đã tưới nước → cây lớn lên
        /// - Nếu không tưới → tăng DaysSinceWatered
        /// - Nếu không tưới quá lâu (DaysToWilt) → héo
        /// </summary>
        public void UpdateGrowth(int daysPassed)
        {
            if (IsWilted)
                return;

            // Cập nhật số ngày đã trồng
            DaysPlanted += daysPassed;

            // Kiểm tra trạng thái tưới nước
            if (IsWateredToday)
            {
                // Đã tưới → cây lớn lên, reset counter
                DaysSinceWatered = 0;
                IsWateredToday = false; // Reset cho ngày hôm sau
            }
            else
            {
                // Chưa tưới → tăng số ngày không tưới
                DaysSinceWatered += daysPassed;
            }

            // Kiểm tra héo úa: nếu không tưới quá DaysToWilt ngày → héo
            if (DaysSinceWatered >= DaysToWilt)
            {
                CurrentStage = GrowthStage.Wilted;
                return;
            }

            // Cập nhật giai đoạn phát triển dựa trên số ngày đã trồng
            // Chỉ lớn lên nếu đã tưới nước (DaysSinceWatered = 0)
            if (DaysSinceWatered == 0) // Đã tưới nước
            {
                if (DaysPlanted >= DaysToGrow)
                {
                    CurrentStage = GrowthStage.Mature;
                }
                else if (DaysPlanted >= DaysToGrow * 0.75f)
                {
                    CurrentStage = GrowthStage.Growing;
                }
                else if (DaysPlanted >= DaysToGrow * 0.25f)
                {
                    CurrentStage = GrowthStage.Sprout;
                }
                else
                {
                    CurrentStage = GrowthStage.Seed;
                }
            }
            // Nếu chưa tưới (DaysSinceWatered > 0), giữ nguyên stage hiện tại
            // Cây sẽ không lớn lên nếu không tưới nước
        }

        /// <summary>
        /// Thu hoạch cây
        /// </summary>
        public HarvestResult Harvest()
        {
            if (!IsMature)
                return HarvestResult.Failed("Crop is not mature yet");

            return HarvestResult.Success(HarvestItemId, HarvestYield);
        }
    }

    /// <summary>
    /// Kết quả thu hoạch
    /// </summary>
    public class HarvestResult
    {
        public bool IsSuccess { get; }
        public ItemId ItemId { get; }
        public int Quantity { get; }
        public string ErrorMessage { get; }

        private HarvestResult(bool isSuccess, ItemId itemId, int quantity, string errorMessage)
        {
            IsSuccess = isSuccess;
            ItemId = itemId;
            Quantity = quantity;
            ErrorMessage = errorMessage;
        }

        public static HarvestResult Success(ItemId itemId, int quantity)
        {
            return new HarvestResult(true, itemId, quantity, null);
        }

        public static HarvestResult Failed(string errorMessage)
        {
            return new HarvestResult(false, default, 0, errorMessage);
        }
    }
}

