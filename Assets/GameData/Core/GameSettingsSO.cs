using UnityEngine;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Game Settings SO - Cấu hình toàn bộ game qua Inspector
    /// Chọn mode, tính năng, etc. - Tất cả làm qua SO, không cần tool riêng
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings", order = 0)]
    public class GameSettingsSO : ScriptableObject
    {
        [Header("Network Mode")]
        [Tooltip("Bật để chạy online mode, tắt để chạy offline mode")]
        public bool enableOnlineMode = false;

        [Header("NPC Features")]
        [Tooltip("Bật để có hệ thống độ thân mật với NPC")]
        public bool enableNPCFriendship = false;
        [Tooltip("Bật để có hệ thống kết bạn với NPC (khác với friendship)")]
        public bool enableNPCFriendshipSystem = false;
        [Tooltip("Bật để có hệ thống quest từ NPC")]
        public bool enableNPCQuests = true;
        [Tooltip("Bật để có hệ thống dialogue với NPC")]
        public bool enableNPCDialogue = true;

        [Header("Player Features")]
        [Tooltip("Bật để có hệ thống level")]
        public bool enableLevelSystem = false;
        [Tooltip("Bật để có hệ thống skill points")]
        public bool enableSkillSystem = false;
        [Tooltip("Bật để có hệ thống achievement")]
        public bool enableAchievementSystem = false;

        [Header("Gameplay Features")]
        [Tooltip("Bật để có hệ thống crafting")]
        public bool enableCrafting = false;
        [Tooltip("Bật để có hệ thống trading")]
        public bool enableTrading = false;
        [Tooltip("Bật để có hệ thống guild")]
        public bool enableGuild = false;
        [Tooltip("Bật để có hệ thống battle/combat")]
        public bool enableBattle = false;

        [Header("UI Features")]
        [Tooltip("Hiển thị nút Online/Offline mode trong menu")]
        public bool showModeSelector = false; // Nếu false, tự động dùng mode từ SO
        [Tooltip("Hiển thị friend list (chỉ online mode)")]
        public bool showFriendList = false; // Chỉ hiện nếu online mode

        [Header("Default Values")]
        [Tooltip("Max friendship points với NPC")]
        public int maxFriendshipPoints = 1000;
        [Tooltip("Starting level (nếu có level system)")]
        public int startingLevel = 1;
        [Tooltip("Max level (nếu có level system)")]
        public int maxLevel = 100;

        /// <summary>
        /// Kiểm tra có thể dùng online features không
        /// </summary>
        public bool CanUseOnlineFeatures()
        {
            return enableOnlineMode;
        }

        /// <summary>
        /// Kiểm tra có thể dùng NPC friendship không
        /// </summary>
        public bool CanUseNPCFriendship()
        {
            return enableNPCFriendship || enableNPCFriendshipSystem;
        }
    }
}

