using UnityEngine;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Unity.Network;
using TheGreenMemoir.Unity.NPC;

namespace TheGreenMemoir.Unity.Managers
{
    /// <summary>
    /// Game Initializer - Khởi tạo tất cả systems từ GameSettingsSO
    /// Tự động enable/disable features dựa trên SO
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        [Header("Game Settings")]
        [Tooltip("Kéo GameSettingsSO vào đây. Nếu để trống, sẽ tự động tìm")]
        [SerializeField] private GameSettingsSO gameSettings;

        [Header("Auto Initialize")]
        [Tooltip("Tự động khởi tạo khi Start")]
        [SerializeField] private bool autoInitialize = true;

        private void Start()
        {
            if (autoInitialize)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Khởi tạo tất cả systems từ GameSettingsSO
        /// </summary>
        public void Initialize()
        {
            // Tìm GameSettingsSO nếu chưa gán
            if (gameSettings == null)
            {
                #if UNITY_EDITOR
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:GameSettingsSO");
                if (guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    gameSettings = UnityEditor.AssetDatabase.LoadAssetAtPath<GameSettingsSO>(path);
                }
                #endif
            }

            if (gameSettings == null)
            {
                Debug.LogWarning("GameSettingsSO not found! Using default settings.");
                return;
            }

            // Initialize Network Service Manager
            InitializeNetworkService();

            // Initialize NPC Friendship System
            InitializeNPCFriendship();

            // Initialize Friend System (chỉ online)
            InitializeFriendSystem();

            Debug.Log("Game initialized from GameSettingsSO");
        }

        private void InitializeNetworkService()
        {
            var networkManager = FindFirstObjectByType<NetworkServiceManager>();
            if (networkManager == null)
            {
                GameObject networkObj = new GameObject("NetworkServiceManager");
                networkManager = networkObj.AddComponent<NetworkServiceManager>();
            }

            // Refresh từ settings
            networkManager.RefreshFromSettings();
        }

        private void InitializeNPCFriendship()
        {
            if (!gameSettings.CanUseNPCFriendship())
            {
                // Tắt NPC Friendship System nếu không dùng
                var friendshipSystem = FindFirstObjectByType<NPCFriendshipSystem>();
                if (friendshipSystem != null)
                {
                    friendshipSystem.enabled = false;
                    friendshipSystem.gameObject.SetActive(false);
                }
                return;
            }

            // Tạo NPC Friendship System nếu chưa có
            var npcFriendship = FindFirstObjectByType<NPCFriendshipSystem>();
            if (npcFriendship == null)
            {
                GameObject npcObj = new GameObject("NPCFriendshipSystem");
                npcFriendship = npcObj.AddComponent<NPCFriendshipSystem>();
            }

            npcFriendship.enabled = true;
            npcFriendship.gameObject.SetActive(true);
        }

        private void InitializeFriendSystem()
        {
            // Friend System chỉ dùng cho online mode
            if (!gameSettings.CanUseOnlineFeatures() || !gameSettings.showFriendList)
            {
                var friendSystem = FindFirstObjectByType<FriendSystem>();
                if (friendSystem != null)
                {
                    friendSystem.enabled = false;
                    friendSystem.gameObject.SetActive(false);
                }
                return;
            }

            // Tạo Friend System nếu chưa có (chỉ online)
            var friendSys = FindFirstObjectByType<FriendSystem>();
            if (friendSys == null)
            {
                GameObject friendObj = new GameObject("FriendSystem");
                friendSys = friendObj.AddComponent<FriendSystem>();
            }

            friendSys.enabled = true;
            friendSys.gameObject.SetActive(true);
        }
    }
}

