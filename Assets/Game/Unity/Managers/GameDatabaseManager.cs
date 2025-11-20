using UnityEngine;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.Managers
{
    /// <summary>
    /// Manager để load và quản lý GameDatabase
    /// Tự động load từ Resources hoặc reference trực tiếp
    /// </summary>
    public class GameDatabaseManager : MonoBehaviour
    {
        [Header("Database Reference")]
        [Tooltip("Kéo GameDatabase asset vào đây, hoặc để null để load từ Resources")]
        [SerializeField] private GameDatabase database;

        [Header("Auto Load Settings")]
        [Tooltip("Tự động load từ Resources/GameDatabase nếu không có reference")]
        [SerializeField] private bool autoLoadFromResources = true;

        private static GameDatabaseManager _instance;
        public static GameDatabase Database { get; private set; }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                LoadDatabase();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void LoadDatabase()
        {
            // Load từ reference trước
            if (database != null)
            {
                Database = database;
            }
            // Nếu không có, load từ Resources
            else if (autoLoadFromResources)
            {
                Database = Resources.Load<GameDatabase>("GameDatabase");
            }

            if (Database == null)
            {
                Debug.LogError("GameDatabase not found! Please create one and assign it or put it in Resources/GameDatabase.asset");
                return;
            }

            Database.Initialize();
            Debug.Log("GameDatabase loaded successfully");
        }

        /// <summary>
        /// Lấy database (static access)
        /// Đảm bảo database được load trước khi trả về
        /// </summary>
        public static GameDatabase GetDatabase()
        {
            // Nếu Database chưa được load và có instance, thử load lại
            if (Database == null)
            {
                // Tìm instance trong scene nếu chưa có
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<GameDatabaseManager>();
                }
                
                // Nếu có instance, load database
                if (_instance != null)
                {
                    _instance.LoadDatabase();
                }
            }
            return Database;
        }
    }
}

