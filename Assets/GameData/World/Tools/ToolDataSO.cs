using UnityEngine;
using System.Collections.Generic;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.Enums;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// ScriptableObject cho Tool Data
    /// Tạo trong Unity Editor: Right Click → Create → Game → Tool Data
    /// </summary>
    [CreateAssetMenu(fileName = "NewTool", menuName = "Game/Tool Data", order = 3)]
    public class ToolDataSO : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("ID duy nhất của công cụ")]
        public string toolId = "new_tool";
        
        [Tooltip("Tên công cụ")]
        public string toolName = "New Tool";

        [Header("Visual")]
        [Tooltip("Icon hiển thị trong tool selection")]
        public Sprite icon;
        
        [Tooltip("Sprite khi cầm trên tay")]
        public Sprite heldSprite;
        
        [Header("Animation")]
        [Tooltip("Hoạt ảnh sử dụng tool (Animation Clip, optional) - Dùng cho tool có 1 animation")]
        public AnimationClip useAnimation;
        
        [Tooltip("Tool có animation 4 hướng không? (nếu có, dùng useAnimationUp/Down/Left/Right)")]
        public bool hasDirectionalAnimation = false;
        
        [Tooltip("Hoạt ảnh sử dụng tool hướng lên (nếu hasDirectionalAnimation = true)")]
        public AnimationClip useAnimationUp;
        
        [Tooltip("Hoạt ảnh sử dụng tool hướng xuống (nếu hasDirectionalAnimation = true)")]
        public AnimationClip useAnimationDown;
        
        [Tooltip("Hoạt ảnh sử dụng tool hướng trái (nếu hasDirectionalAnimation = true)")]
        public AnimationClip useAnimationLeft;
        
        [Tooltip("Hoạt ảnh sử dụng tool hướng phải (nếu hasDirectionalAnimation = true)")]
        public AnimationClip useAnimationRight;

        [Header("Properties")]
        [Tooltip("Loại hành động của công cụ")]
        public ToolActionType actionType = ToolActionType.None;
        
        [Tooltip("Tool này có vô hạn lần sử dụng không? (ví dụ: Hoe, Gloves)")]
        public bool isInfiniteUses = false;
        
        [Tooltip("Số lần sử dụng tối đa (chỉ dùng nếu isInfiniteUses = false, ví dụ: Watering Can = 16)")]
        [Range(1, 999)]
        public int maxUses = 100;
        
        [Tooltip("Tool này có thể refill (bơm lại) không? (ví dụ: Watering Can)")]
        public bool isRefillable = false;
        
        [Tooltip("Tool này có thể nâng cấp không? (ví dụ: mua upgrade ở cửa hàng)")]
        public bool isUpgradeable = false;
        
        [Tooltip("Level hiện tại của tool (chỉ dùng nếu isUpgradeable = true)")]
        [Range(1, 10)]
        public int toolLevel = 1;

        [Header("Energy Cost")]
        [Tooltip("Năng lượng tiêu tốn mỗi lần sử dụng")]
        [Range(0, 50)]
        public int energyCost = 5;
        
        [Header("Description")]
        [Tooltip("Mô tả chi tiết về công cụ (hiển thị khi click vào tool đang chọn)")]
        [TextArea(3, 6)]
        public string description = "Used for...";
        
        [Header("Interaction Properties")]
        [Tooltip("Tool này có thể tương tác với tile state nào?")]
        public List<string> canInteractWithTileStates = new List<string>();
        
        [Tooltip("Ví dụ: 'Normal' = cuốc đất, 'Plowed' = gieo hạt, 'Planted' = tưới nước, 'Growing' = thu hoạch")]
        [TextArea(2, 4)]
        public string interactionDescription = "Can interact with: Normal (to plow)";
        
        [Header("Special Tool")]
        [Tooltip("Tool đặc biệt - tạo bảng tương tác (ví dụ: găng tay để gieo hạt)")]
        public bool isSpecialTool = false;
        
        [Tooltip("Loại tương tác đặc biệt (ví dụ: 'SeedSelection' = chọn hạt giống từ inventory)")]
        public string specialInteractionType = "";
        
        [Tooltip("Item tag để filter items trong inventory (ví dụ: 'Seed' cho găng tay)")]
        public ItemTag filterItemTag = ItemTag.None;

        [Header("Unlock System")]
        [Tooltip("Công cụ này có mở khóa content mới không? (ví dụ: cần câu mở khóa câu cá)")]
        public bool unlocksContent = false;
        
        [Tooltip("ID của content được mở khóa (ví dụ: fishing, chopping)")]
        public string unlockedContentId = "";
        
        [Tooltip("Mô tả content được mở khóa")]
        [TextArea(2, 4)]
        public string unlockDescription = "Unlocks new content";
        
        [Header("Purchase Info")]
        [Tooltip("Có thể mua công cụ này không?")]
        public bool isPurchasable = true;
        
        [Tooltip("Giá mua (nếu có thể mua)")]
        public int buyPrice = 100;
        
        [Tooltip("Công cụ này có sẵn từ đầu không?")]
        public bool isDefaultTool = false;
        
        /// <summary>
        /// Kiểm tra tool có thể tương tác với tile state không
        /// </summary>
        public bool CanInteractWithTileState(string tileStateId)
        {
            if (canInteractWithTileStates == null || canInteractWithTileStates.Count == 0)
                return false;
                
            return canInteractWithTileStates.Contains(tileStateId);
        }

        /// <summary>
        /// Chuyển đổi ScriptableObject thành Tool entity
        /// </summary>
        public Tool ToTool()
        {
            return new Tool(
                new ToolId(toolId),
                toolName,
                actionType,
                maxUses
            );
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(toolId))
            {
                toolId = name.ToLower().Replace(" ", "_");
            }

            if (string.IsNullOrWhiteSpace(toolName))
            {
                toolName = name;
            }

            if (maxUses < 1)
            {
                maxUses = 1;
            }
        }
    }
}

