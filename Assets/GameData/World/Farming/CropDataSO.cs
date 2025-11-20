using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Crop Sprite Data - Chứa sprite và ngày chuyển đổi
    /// </summary>
    [System.Serializable]
    public class CropSpriteData
    {
        [Tooltip("Sprite hiển thị")]
        public Sprite sprite;

        [Tooltip("Số ngày để chuyển sang sprite tiếp theo (0 = sprite đầu tiên)")]
        [Range(0, 30)]
        public int dayToShow = 0;

        [Tooltip("Tên mô tả (để dễ quản lý)")]
        public string description = "";

        [Tooltip("✅ Đánh dấu sprite này là sprite TRƯỞNG THÀNH (có thể thu hoạch)\n" +
                 "Khi cây hiển thị sprite này, bạn có thể hiển thị icon/animation trên đầu cây.")]
        public bool isMature = false;
    }

    /// <summary>
    /// Crop State - Trạng thái của cây
    /// </summary>
    public enum CropState
    {
        Normal,  // Bình thường (chưa tưới)
        Wet,     // Đã tưới
        Wilted   // Đã chết héo
    }

    /// <summary>
    /// ScriptableObject cho Crop Data
    /// Tạo trong Unity Editor: Right Click → Create → Game → Crop Data
    /// </summary>
    [CreateAssetMenu(fileName = "NewCrop", menuName = "Game/Crop Data", order = 2)]
    public class CropDataSO : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("ID duy nhất của cây trồng")]
        public string cropId = "new_crop";
        
        [Tooltip("Tên cây trồng")]
        public string cropName = "New Crop";

        [Header("Visual - Growth Sprites (Bắt buộc)")]
        [Tooltip("Danh sách sprite cho các giai đoạn phát triển.\n" +
                 "💡 ĐƠN GIẢN: Chỉ cần thêm 1 sprite là đủ! Nếu chỉ có 1 sprite, sẽ dùng cho tất cả các giai đoạn.\n" +
                 "💡 Nhiều sprite: Thêm nhiều sprite với dayToShow khác nhau để cây lớn lên qua các ngày.\n" +
                 "💡 Nếu bạn có sprite riêng cho trạng thái 'ướt', hãy thêm vào 'Wet Sprites' bên dưới.")]
        public List<CropSpriteData> growthSprites = new List<CropSpriteData>();

        [Header("Visual - Wet Sprites (Tùy chọn)")]
        [Tooltip("Danh sách sprite cho trạng thái ĐÃ TƯỚI (tùy chọn).\n" +
                 "Nếu để trống, hệ thống sẽ tự động dùng 'Growth Sprites' với hiệu ứng 'ướt'.\n" +
                 "Chỉ thêm vào nếu bạn có sprite riêng cho cây đã tưới nước.")]
        public List<CropSpriteData> wetSprites = new List<CropSpriteData>();

        [Header("Visual - Wilted Sprites (Tùy chọn)")]
        [Tooltip("Danh sách sprite cho trạng thái ĐÃ CHẾT HÉO (tùy chọn).\n" +
                 "Nếu để trống, hệ thống sẽ tự động dùng sprite cuối cùng từ 'Growth Sprites'.\n" +
                 "⚠️ CHỈ CẦN 1 SPRITE: Cây héo sẽ mãi héo, không cần dayToShow (sẽ bỏ qua).")]
        public List<CropSpriteData> wiltedSprites = new List<CropSpriteData>();

        [Header("Growth Settings")]
        [Tooltip("Số ngày để cây trưởng thành")]
        [Range(1, 30)]
        public int daysToGrow = 5;
        
        [Tooltip("Số ngày không tưới sẽ héo")]
        [Range(1, 10)]
        public int daysToWilt = 2;

        [Header("Harvest Settings")]
        [Tooltip("Số lượng vật phẩm thu hoạch được")]
        [Range(1, 10)]
        public int harvestYield = 1;
        
        [Tooltip("ID của vật phẩm thu hoạch (phải có ItemDataSO tương ứng)")]
        public string harvestItemId = "corn";

        [Header("Seed Info")]
        [Tooltip("ID của hạt giống (phải có ItemDataSO tương ứng)")]
        public string seedItemId = "seed_corn";

        /// <summary>
        /// Chuyển đổi ScriptableObject thành Crop entity
        /// </summary>
        public Crop ToCrop()
        {
            return new Crop(
                new CropId(cropId),
                cropName,
                daysToGrow,
                daysToWilt,
                harvestYield,
                new ItemId(harvestItemId)
            );
        }

        /// <summary>
        /// Lấy sprite dựa trên số ngày và trạng thái
        /// Logic thực tế: Tưới nước → lớn lên, không tưới → héo
        /// 
        /// Đơn giản: Chỉ cần 1 sprite trong growthSprites là đủ!
        /// Nếu chỉ có 1 sprite, sẽ dùng sprite đó cho tất cả các giai đoạn.
        /// </summary>
        /// <param name="daysPlanted">Số ngày đã trồng</param>
        /// <param name="isWateredToday">Đã tưới nước hôm nay chưa</param>
        /// <param name="daysSinceWatered">Số ngày không tưới liên tiếp</param>
        /// <param name="daysToWilt">Số ngày không tưới sẽ héo</param>
        /// <returns>Sprite phù hợp hoặc null nếu không tìm thấy</returns>
        public Sprite GetSpriteForDay(int daysPlanted, bool isWateredToday, int daysSinceWatered, int daysToWilt)
        {
            // Kiểm tra héo: nếu không tưới quá lâu → héo
            bool isWilted = daysSinceWatered >= daysToWilt;
            
            if (isWilted)
            {
                // Tìm sprite héo (không cần dayToShow - cây héo sẽ mãi héo)
                var wiltedSprite = GetFirstSpriteFromList(wiltedSprites);
                if (wiltedSprite != null)
                    return wiltedSprite;
                
                // Nếu không có sprite héo riêng, dùng sprite cuối cùng từ growth sprites
                var lastGrowthSprite = GetLastSpriteFromList(growthSprites);
                if (lastGrowthSprite != null)
                    return lastGrowthSprite;
            }

            // Nếu đã tưới nước hôm nay → dùng wet sprites (nếu có) hoặc growth sprites
            if (isWateredToday)
            {
                var wetSprite = GetSpriteFromList(wetSprites, daysPlanted);
                if (wetSprite != null)
                    return wetSprite;
            }

            // Mặc định: dùng growth sprites
            var growthSprite = GetSpriteFromList(growthSprites, daysPlanted);
            if (growthSprite != null)
                return growthSprite;

            // Nếu không tìm thấy gì cả → trả về null (game sẽ xử lý)
            return null;
        }

        /// <summary>
        /// Lấy sprite từ danh sách dựa trên số ngày
        /// Đơn giản: Nếu chỉ có 1 sprite, dùng luôn sprite đó!
        /// </summary>
        private Sprite GetSpriteFromList(List<CropSpriteData> spriteList, int daysPlanted)
        {
            if (spriteList == null || spriteList.Count == 0)
                return null;

            // Lọc ra các sprite hợp lệ
            var validSprites = spriteList
                .Where(s => s != null && s.sprite != null)
                .ToList();

            if (validSprites.Count == 0)
                return null;

            // Nếu chỉ có 1 sprite → dùng luôn (đơn giản cho người nghèo 😄)
            if (validSprites.Count == 1)
                return validSprites[0].sprite;

            // Nếu có nhiều sprite → tìm sprite phù hợp dựa trên dayToShow
            var sortedSprites = validSprites
                .OrderBy(s => s.dayToShow)
                .ToList();

            // Tìm sprite phù hợp: sprite có dayToShow <= daysPlanted và gần nhất
            CropSpriteData selectedSprite = sortedSprites[0];
            foreach (var spriteData in sortedSprites)
            {
                if (spriteData.dayToShow <= daysPlanted)
                {
                    selectedSprite = spriteData;
                }
                else
                {
                    break;
                }
            }

            return selectedSprite.sprite;
        }

        /// <summary>
        /// Lấy sprite đầu tiên từ danh sách (dùng cho wilted - không cần dayToShow)
        /// </summary>
        private Sprite GetFirstSpriteFromList(List<CropSpriteData> spriteList)
        {
            if (spriteList == null || spriteList.Count == 0)
                return null;

            var validSprite = spriteList
                .FirstOrDefault(s => s != null && s.sprite != null);

            return validSprite?.sprite;
        }

        /// <summary>
        /// Lấy sprite cuối cùng từ danh sách (dùng cho héo nếu không có sprite riêng)
        /// </summary>
        private Sprite GetLastSpriteFromList(List<CropSpriteData> spriteList)
        {
            if (spriteList == null || spriteList.Count == 0)
                return null;

            var sortedSprites = spriteList
                .Where(s => s != null && s.sprite != null)
                .OrderBy(s => s.dayToShow)
                .ToList();

            if (sortedSprites.Count == 0)
                return null;

            return sortedSprites.Last().sprite;
        }

        /// <summary>
        /// Lấy sprite từ Crop entity (helper method - dễ sử dụng)
        /// </summary>
        public Sprite GetSpriteForCrop(Core.Domain.Entities.Crop crop)
        {
            if (crop == null)
                return null;

            return GetSpriteForDay(
                daysPlanted: crop.DaysPlanted,
                isWateredToday: crop.IsWateredToday,
                daysSinceWatered: crop.DaysSinceWatered,
                daysToWilt: crop.DaysToWilt
            );
        }

        /// <summary>
        /// Kiểm tra sprite hiện tại có phải sprite trưởng thành không
        /// Dùng để hiển thị icon/animation trên đầu cây khi đã trưởng thành
        /// </summary>
        /// <param name="crop">Crop entity</param>
        /// <returns>true nếu sprite hiện tại là sprite trưởng thành</returns>
        public bool IsCurrentSpriteMature(Core.Domain.Entities.Crop crop)
        {
            if (crop == null)
                return false;

            // Kiểm tra cây đã mature chưa (logic)
            if (!crop.IsMature)
                return false;

            // Lấy sprite hiện tại
            var currentSprite = GetSpriteForCrop(crop);
            if (currentSprite == null)
                return false;

            // Kiểm tra sprite hiện tại có đánh dấu isMature không
            return IsSpriteMature(currentSprite, crop.DaysPlanted);
        }

        /// <summary>
        /// Kiểm tra sprite có phải sprite trưởng thành không
        /// </summary>
        public bool IsSpriteMature(Sprite sprite, int daysPlanted)
        {
            if (sprite == null)
                return false;

            // Tìm sprite trong growthSprites
            var spriteData = growthSprites
                .FirstOrDefault(s => s != null && s.sprite == sprite);

            if (spriteData != null)
            {
                // Nếu có đánh dấu isMature → trả về true
                if (spriteData.isMature)
                    return true;
            }

            // Nếu không tìm thấy trong growthSprites, kiểm tra wetSprites
            spriteData = wetSprites
                .FirstOrDefault(s => s != null && s.sprite == sprite);

            if (spriteData != null && spriteData.isMature)
                return true;

            // Mặc định: sprite cuối cùng trong growthSprites = mature (nếu không có đánh dấu)
            var lastSprite = GetLastSpriteFromList(growthSprites);
            return sprite == lastSprite;
        }

        /// <summary>
        /// Lấy sprite trưởng thành (dùng để hiển thị icon/animation)
        /// </summary>
        public CropSpriteData GetMatureSpriteData()
        {
            // Tìm sprite có đánh dấu isMature
            var matureSprite = growthSprites
                .FirstOrDefault(s => s != null && s.sprite != null && s.isMature);

            if (matureSprite != null)
                return matureSprite;

            // Nếu không có, lấy sprite cuối cùng
            var sortedSprites = growthSprites
                .Where(s => s != null && s.sprite != null)
                .OrderBy(s => s.dayToShow)
                .ToList();

            if (sortedSprites.Count > 0)
                return sortedSprites.Last();

            return null;
        }


        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(cropId))
            {
                cropId = name.ToLower().Replace(" ", "_");
            }

            if (string.IsNullOrWhiteSpace(cropName))
            {
                cropName = name;
            }

            if (daysToGrow < 1)
            {
                daysToGrow = 1;
            }

            if (harvestYield < 1)
            {
                harvestYield = 1;
            }
        }
    }
}

