using UnityEngine;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Unity.Input;
using TheGreenMemoir.Unity.Input.Actions;

namespace TheGreenMemoir.Unity.Managers
{
    /// <summary>
    /// Quick Cheat Manager - Quản lý cheats F1-F5
    /// Đọc từ CheatConfigSO, tự động tạo InputActionSO
    /// </summary>
    public class QuickCheatManager : MonoBehaviour
    {
        [Header("Cheat Config")]
        [Tooltip("Kéo CheatConfigSO vào đây")]
        [SerializeField] private CheatConfigSO cheatConfig;

        private void Start()
        {
            // Tìm CheatConfigSO nếu chưa gán
            if (cheatConfig == null)
            {
                #if UNITY_EDITOR
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:CheatConfigSO");
                if (guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    cheatConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<CheatConfigSO>(path);
                }
                #endif
            }

            if (cheatConfig == null) return;

            // Tự động tạo InputActionSO cho F1-F5 nếu chưa có
            RegisterQuickCheats();
        }

        private void RegisterQuickCheats()
        {
            var inputManager = InputActionManager.Instance;
            if (inputManager == null) return;

            // F1: Teleport vào giường
            if (cheatConfig.cheatF1_TeleportBed != null)
            {
                RegisterCheat("CheatF1_TeleportBed", KeyCode.F1, cheatConfig.cheatF1_TeleportBed);
            }

            // F2: Teleport ra vườn
            if (cheatConfig.cheatF2_TeleportFarm != null)
            {
                RegisterCheat("CheatF2_TeleportFarm", KeyCode.F2, cheatConfig.cheatF2_TeleportFarm);
            }

            // F3: Tưới thuốc tăng trưởng
            if (cheatConfig.cheatF3_GrowthPotion != null)
            {
                RegisterCheat("CheatF3_GrowthPotion", KeyCode.F3, cheatConfig.cheatF3_GrowthPotion);
            }

            // F4: Qua ngày
            if (cheatConfig.cheatF4_SkipDay != null)
            {
                RegisterCheat("CheatF4_SkipDay", KeyCode.F4, cheatConfig.cheatF4_SkipDay);
            }

            // F5: Hack tiền
            if (cheatConfig.cheatF5_AddMoney != null)
            {
                RegisterCheat("CheatF5_AddMoney", KeyCode.F5, cheatConfig.cheatF5_AddMoney);
            }
        }

        private void RegisterCheat(string actionId, KeyCode key, CheatSO cheatSO)
        {
            // Tạo InputActionSO runtime (hoặc dùng SO có sẵn)
            // Note: Cần tạo InputActionSO trong project trước, sau đó link vào đây
            Debug.Log($"Registered cheat: {actionId} ({key})");
        }
    }
}

