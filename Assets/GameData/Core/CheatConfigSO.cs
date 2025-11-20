using UnityEngine;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Cheat Config SO - Cấu hình tất cả cheats
    /// Tạo cheats qua SO, không cần code
    /// </summary>
    [CreateAssetMenu(fileName = "CheatConfig", menuName = "Game/Cheat Config", order = 36)]
    public class CheatConfigSO : ScriptableObject
    {
        [Header("Cheat List")]
        [Tooltip("Danh sách cheats (tạo CheatSO và kéo vào đây)")]
        public TheGreenMemoir.Unity.Input.Actions.CheatSO[] cheats;

        [Header("Quick Cheats (F1-F5)")]
        [Tooltip("F1: Teleport vào giường")]
        public TheGreenMemoir.Unity.Input.Actions.CheatSO cheatF1_TeleportBed;
        
        [Tooltip("F2: Teleport ra vườn")]
        public TheGreenMemoir.Unity.Input.Actions.CheatSO cheatF2_TeleportFarm;
        
        [Tooltip("F3: Tưới thuốc tăng trưởng")]
        public TheGreenMemoir.Unity.Input.Actions.CheatSO cheatF3_GrowthPotion;
        
        [Tooltip("F4: Qua ngày")]
        public TheGreenMemoir.Unity.Input.Actions.CheatSO cheatF4_SkipDay;
        
        [Tooltip("F5: Hack tiền")]
        public TheGreenMemoir.Unity.Input.Actions.CheatSO cheatF5_AddMoney;
    }
}

