using UnityEngine;
using TheGreenMemoir.Core.Application.Services;
using TheGreenMemoir.Core.Application.Commands;
using TheGreenMemoir.Core.Application.Factories;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Infrastructure.Repositories;
using TheGreenMemoir.Core.Infrastructure.Services;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.Managers
{
    /// <summary>
    /// Game Manager - Service Locator Pattern
    /// Khởi tạo và quản lý tất cả services, repositories
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        // Infrastructure Services
        public static IEventBus EventBus { get; private set; }
        public static ITimeService TimeService { get; private set; }

        // Database
        public static IItemDatabase ItemDatabase { get; private set; }
        public static ICropDatabase CropDatabase { get; private set; }
        public static IToolDatabase ToolDatabase { get; private set; }
        public static ITileStateDatabase TileStateDatabase { get; private set; }

        // Repositories
        public static IPlayerRepository PlayerRepository { get; private set; }
        public static IFarmRepository FarmRepository { get; private set; }
        public static IInventoryRepository InventoryRepository { get; private set; }

        // Application Services
        public static PlayerService PlayerService { get; private set; }
        public static FarmingService FarmingService { get; private set; }
        public static InventoryService InventoryService { get; private set; }
        public static ShopService ShopService { get; private set; }

        // Command Invoker
        public static CommandInvoker CommandInvoker { get; private set; }

        // Factory
        public static IEntityFactory EntityFactory { get; private set; }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeServices();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeServices()
        {
            // Initialize Infrastructure
            EventBus = new EventBus();
            TimeService = GetComponent<TimeManager>() ?? gameObject.AddComponent<TimeManager>();

            // Initialize Database (phải có GameDatabaseManager trong scene)
            InitializeDatabase();

            // Initialize Repositories
            PlayerRepository = new PlayerRepository();
            FarmRepository = new FarmRepository();
            InventoryRepository = new InventoryRepository(PlayerRepository);

            // Initialize Application Services (với Database)
            PlayerService = new PlayerService(PlayerRepository, EventBus);
            InventoryService = new InventoryService(InventoryRepository, EventBus, ItemDatabase);
            FarmingService = new FarmingService(
                FarmRepository, 
                InventoryRepository, 
                TimeService, 
                EventBus,
                CropDatabase,
                ItemDatabase,
                TileStateDatabase
            );
            ShopService = new ShopService(InventoryRepository, PlayerRepository, EventBus, ItemDatabase);

            // Initialize Command Invoker
            CommandInvoker = new CommandInvoker();

            // Initialize Entity Factory
            EntityFactory = new EntityFactory(ItemDatabase, CropDatabase, ToolDatabase);

            Debug.Log("GameManager initialized successfully");
        }

        private void InitializeDatabase()
        {
            // Tìm GameDatabaseManager trong scene
            var dbManager = FindFirstObjectByType<GameDatabaseManager>();
            if (dbManager == null)
            {
                Debug.LogWarning("GameDatabaseManager not found in scene! Creating one...");
                var dbManagerObj = new GameObject("GameDatabaseManager");
                dbManager = dbManagerObj.AddComponent<GameDatabaseManager>();
                // Đảm bảo database được load ngay sau khi tạo
                // GetDatabase() sẽ tự động load nếu Database = null
            }

            // Lấy database (GetDatabase() sẽ tự động load nếu chưa có)
            var database = GameDatabaseManager.GetDatabase();
            if (database == null)
            {
                Debug.LogError("GameDatabase is null! Please create a GameDatabase asset and assign it to GameDatabaseManager in the Inspector, or put it in Resources/GameDatabase.asset");
                return;
            }

            // Tạo adapters
            var adapter = new ItemDatabaseAdapter(database);
            ItemDatabase = adapter;
            CropDatabase = adapter;
            ToolDatabase = adapter;
            
            var tileStateAdapter = new TileStateDatabaseAdapter(database);
            TileStateDatabase = tileStateAdapter;

            Debug.Log("Database initialized successfully");
        }

        private void Start()
        {
            // Initialize audio nếu có
            if (Audio.AudioManager.Instance != null)
            {
                // AudioManager sẽ tự động initialize
            }
        }

        /// <summary>
        /// Load Title Screen scene
        /// </summary>
        public void LoadTitleScreen()
        {
            var sceneLoader = FindFirstObjectByType<SceneLoader>();
            if (sceneLoader != null)
            {
                sceneLoader.LoadTitleScreen();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScreen");
            }
        }

        /// <summary>
        /// Load Tutorial scene
        /// </summary>
        public void LoadTutorial()
        {
            var sceneLoader = FindFirstObjectByType<SceneLoader>();
            if (sceneLoader != null)
            {
                sceneLoader.LoadScene("Tutorial");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
            }
        }

        /// <summary>
        /// Load Game scene
        /// </summary>
        public void LoadGame()
        {
            var sceneLoader = FindFirstObjectByType<SceneLoader>();
            if (sceneLoader != null)
            {
                sceneLoader.LoadScene("Game");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}

