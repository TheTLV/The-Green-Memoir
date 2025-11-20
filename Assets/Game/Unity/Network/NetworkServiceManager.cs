using UnityEngine;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.Network
{
    /// <summary>
    /// Network Service Manager - Quản lý offline/online mode
    /// Tự động đọc từ GameSettingsSO, không cần cấu hình thủ công
    /// </summary>
    public class NetworkServiceManager : MonoBehaviour
    {
        private static NetworkServiceManager _instance;
        public static NetworkServiceManager Instance => _instance;

        [Header("Game Settings (optional)")]
        [Tooltip("Kéo GameSettingsSO vào đây. Nếu để trống, sẽ tự động tìm trong project")]
        [SerializeField] private GameSettingsSO gameSettings;

        [Header("Services")]
        [SerializeField] private OfflineNetworkService offlineService;
        [SerializeField] private OnlineNetworkService onlineService;

        private INetworkService _currentService;
        private bool _onlineMode = false;

        public bool IsOnlineMode => _onlineMode;
        public INetworkService CurrentService => _currentService;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                LoadGameSettings();
                Initialize();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void LoadGameSettings()
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

            // Đọc mode từ GameSettingsSO
            if (gameSettings != null)
            {
                _onlineMode = gameSettings.enableOnlineMode;
            }
            else
            {
                // Fallback: đọc từ PlayerPrefs hoặc mặc định offline
                _onlineMode = PlayerPrefs.GetInt("OnlineMode", 0) == 1;
            }
        }

        private void Initialize()
        {
            // Tạo services nếu chưa có
            if (offlineService == null)
            {
                GameObject offlineObj = new GameObject("OfflineNetworkService");
                offlineObj.transform.SetParent(transform);
                offlineService = offlineObj.AddComponent<OfflineNetworkService>();
            }

            if (onlineService == null)
            {
                GameObject onlineObj = new GameObject("OnlineNetworkService");
                onlineObj.transform.SetParent(transform);
                onlineService = onlineObj.AddComponent<OnlineNetworkService>();
            }

            // Chọn service dựa trên mode từ SO
            SwitchMode(_onlineMode);
        }

        /// <summary>
        /// Chuyển đổi giữa offline và online mode
        /// </summary>
        public void SwitchMode(bool online)
        {
            _onlineMode = online;

            // Disconnect service cũ
            if (_currentService != null && _currentService.IsConnected)
            {
                _currentService.Disconnect();
            }

            // Chọn service mới
            if (_onlineMode)
            {
                _currentService = onlineService;
                onlineService.gameObject.SetActive(true);
                offlineService.gameObject.SetActive(false);
            }
            else
            {
                _currentService = offlineService;
                offlineService.gameObject.SetActive(true);
                onlineService.gameObject.SetActive(false);
            }

            // Initialize service mới
            _currentService.Initialize(_onlineMode);

            Debug.Log($"Switched to {(_onlineMode ? "ONLINE" : "OFFLINE")} mode");
        }

        /// <summary>
        /// Lưu mode vào PlayerPrefs (nếu cho phép thay đổi runtime)
        /// </summary>
        public void SaveMode()
        {
            PlayerPrefs.SetInt("OnlineMode", _onlineMode ? 1 : 0);
        }

        /// <summary>
        /// Load mode từ GameSettingsSO (gọi khi cần refresh)
        /// </summary>
        public void RefreshFromSettings()
        {
            LoadGameSettings();
            SwitchMode(_onlineMode);
        }
    }
}

