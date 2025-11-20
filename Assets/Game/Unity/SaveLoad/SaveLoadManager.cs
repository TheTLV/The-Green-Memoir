using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Unity.Managers;

namespace TheGreenMemoir.Unity.SaveLoad
{
    /// <summary>
    /// Quản lý Save/Load game
    /// Lưu vào file binary hoặc JSON
    /// Hỗ trợ multiple save slots
    /// </summary>
    public class SaveLoadManager : MonoBehaviour, ISaveService
    {
        private static SaveLoadManager _instance;
        public static SaveLoadManager Instance => _instance;

        [Header("Settings")]
        [SerializeField] private string saveFileName = "savegame.dat";
        [SerializeField] private bool useJsonFormat = false; // true = JSON, false = Binary
        [SerializeField] private int maxSaveSlots = 10; // Số lượng save slots tối đa
        [SerializeField] private int defaultSlotIndex = 0; // Slot mặc định (dùng cho QuickSave/QuickLoad)

        private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);
        private string SaveDirectory => Application.persistentDataPath;
        
        private float totalPlayTime = 0f; // Tổng thời gian chơi (giây)
        private DateTime gameStartTime; // Thời điểm bắt đầu chơi
        private int currentSlotIndex = 0; // Slot hiện tại đang chơi

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                gameStartTime = DateTime.Now;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            // Cập nhật thời gian chơi
            if (Time.timeScale > 0f)
            {
                totalPlayTime += Time.deltaTime;
            }
        }

        /// <summary>
        /// Lấy đường dẫn file save cho slot
        /// </summary>
        private string GetSavePathForSlot(int slotIndex)
        {
            string fileName = useJsonFormat ? $"savegame_{slotIndex}.json" : $"savegame_{slotIndex}.dat";
            return Path.Combine(SaveDirectory, fileName);
        }

        /// <summary>
        /// Lưu game (backward compatibility - lưu vào slot mặc định)
        /// </summary>
        public void SaveGame(GameState state)
        {
            SaveGameToSlot(defaultSlotIndex, state);
        }

        /// <summary>
        /// Load game (backward compatibility - load từ slot mặc định)
        /// </summary>
        public GameState LoadGame()
        {
            return LoadGameFromSlot(defaultSlotIndex);
        }

        /// <summary>
        /// Kiểm tra có file save không (backward compatibility - kiểm tra slot mặc định)
        /// </summary>
        public bool HasSaveFile()
        {
            return HasSaveFileInSlot(defaultSlotIndex);
        }

        /// <summary>
        /// Xóa file save (backward compatibility - xóa slot mặc định)
        /// </summary>
        public void DeleteSaveFile()
        {
            DeleteSaveFileInSlot(defaultSlotIndex);
        }

        /// <summary>
        /// Lưu game (Binary format)
        /// </summary>
        private void SaveGameBinary(GameState state)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(SavePath, FileMode.Create))
            {
                formatter.Serialize(stream, state);
            }
        }

        /// <summary>
        /// Load game (Binary format)
        /// </summary>
        private GameState LoadGameBinary()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(SavePath, FileMode.Open))
            {
                return (GameState)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Lưu game (JSON format)
        /// </summary>
        private void SaveGameJSON(GameState state)
        {
            string json = JsonUtility.ToJson(state, true);
            File.WriteAllText(SavePath, json);
        }

        /// <summary>
        /// Load game (JSON format)
        /// </summary>
        private GameState LoadGameJSON()
        {
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<GameState>(json);
        }

        /// <summary>
        /// Tạo GameState từ game hiện tại
        /// </summary>
        public GameState CreateGameState()
        {
            var state = new GameState();

            // Lưu player data
            if (GameManager.PlayerService != null)
            {
                var player = GameManager.PlayerService.GetPlayer(PlayerId.Default);
                if (player != null)
                {
                    state.playerMoney = (int)player.Money.Amount; // Money.Amount là long, convert sang int
                    state.playerEnergy = player.Energy.Current;
                    state.playerMaxEnergy = player.Energy.Max;
                    state.playerPositionX = (int)player.Position.X; // Position có thể là float, convert sang int
                    state.playerPositionY = (int)player.Position.Y;
                }
            }

            // Lưu inventory
            if (GameManager.InventoryService != null)
            {
                var inventory = GameManager.InventoryService.GetInventory(PlayerId.Default);
                if (inventory != null)
                {
                    state.inventoryItems = new System.Collections.Generic.List<GameState.InventoryItemData>();
                    foreach (var slot in inventory.GetAllSlots())
                    {
                        if (slot != null && !slot.IsEmpty && slot.Item != null)
                        {
                            state.inventoryItems.Add(new GameState.InventoryItemData
                            {
                                itemId = slot.Item.Id.Value,
                                quantity = slot.Quantity
                            });
                        }
                    }
                }
            }

            // Lưu farm tiles
            if (GameManager.FarmRepository != null)
            {
                state.farmTiles = new System.Collections.Generic.List<GameState.FarmTileData>();
                var allTiles = GameManager.FarmRepository.GetAllTiles();
                foreach (var tile in allTiles)
                {
                    var tileData = new GameState.FarmTileData
                    {
                        x = tile.Position.X,
                        y = tile.Position.Y,
                        stateId = tile.CurrentStateId ?? "normal",
                        cropId = tile.Crop != null ? tile.Crop.Id.ToString() : "",
                        growthStage = tile.Crop != null ? tile.Crop.DaysPlanted : 0,
                        growthProgress = tile.Crop != null ? (tile.Crop.IsMature ? 1f : (float)tile.Crop.DaysPlanted / Mathf.Max(1, tile.Crop.DaysToGrow)) : 0f
                    };
                    state.farmTiles.Add(tileData);
                }
            }
            else
            {
                state.farmTiles = new System.Collections.Generic.List<GameState.FarmTileData>();
            }

            // Lưu thời gian (optional - có thể bỏ nếu dùng thời gian Unity)
            // Note: Time sẽ được chỉnh trong Unity, không cần lưu trong save
            // Nếu muốn lưu time, uncomment phần dưới:
            /*
            if (GameManager.TimeService != null)
            {
                state.currentDay = GameManager.TimeService.CurrentDay;
                state.currentHour = GameManager.TimeService.CurrentHour;
                state.currentMinute = GameManager.TimeService.CurrentMinute;
            }
            else
            {
                state.currentDay = 1;
                state.currentHour = 6;
                state.currentMinute = 0;
            }
            */

            // Lưu Scene hiện tại
            state.currentSceneName = SceneManager.GetActiveScene().name;
            
            // Lưu metadata (sẽ được cập nhật lại khi save vào slot)
            state.saveDate = DateTime.Now;
            state.playTimeInSeconds = totalPlayTime;
            state.saveSlotIndex = currentSlotIndex;
            
            // Player name (nếu có)
            if (string.IsNullOrEmpty(state.playerName))
            {
                state.playerName = "Player";
            }
            
            // Lưu Quest States (OPTIONAL - chỉ lưu nếu có Quest System)
            state.questStates = new List<GameState.QuestStateData>();
            // TODO: Implement khi có Quest System
            // if (QuestManager.Instance != null) { ... }
            
            // Lưu World Items (OPTIONAL - chỉ lưu nếu có World Item System)
            state.worldItems = new List<GameState.WorldItemData>();
            // TODO: Implement khi có World Item System
            
            // Lưu NPC States (OPTIONAL - chỉ lưu nếu có NPC System với trading)
            state.npcStates = new List<GameState.NPCStateData>();
            SaveNPCStates(state);
            
            // Lưu Building States (OPTIONAL - chỉ lưu nếu có Building System)
            state.buildingStates = new List<GameState.BuildingStateData>();
            // TODO: Implement khi có Building System
            
            // Lưu Door States (OPTIONAL - chỉ lưu nếu có Door System)
            state.doorStates = new List<GameState.DoorStateData>();
            // TODO: Implement khi có Door System - có thể lấy từ BuildingDoor components
            
            // Lưu Quest Item Giver States
            state.questItemGiverStates = new List<GameState.QuestItemGiverStateData>();
            SaveQuestItemGiverStates(state);
            
            // Lưu Tool States
            var toolManager = FindFirstObjectByType<Managers.ToolManager>();
            if (toolManager != null)
            {
                toolManager.SaveToolStates(state);
            }
            else
            {
                state.toolStates = new List<GameState.ToolStateData>();
            }

            return state;
        }
        
        /// <summary>
        /// Lưu trạng thái NPC (inventory, money, dialogue, etc.)
        /// Flexible: Chỉ lưu NPC có trading system, không lỗi nếu không có NPC
        /// </summary>
        private void SaveNPCStates(GameState state)
        {
            // Tìm tất cả NPCController trong scene hiện tại
            var npcControllers = FindObjectsByType<Unity.NPC.NPCController>(FindObjectsSortMode.None);
            if (npcControllers == null || npcControllers.Length == 0)
            {
                return; // Không có NPC, không cần lưu
            }

            string currentScene = SceneManager.GetActiveScene().name;

            foreach (var npcController in npcControllers)
            {
                if (npcController == null) continue;

                // Chỉ lưu NPC có trading system
                if (!npcController.CanTrade)
                {
                    continue; // Skip NPC không có trading
                }

                var npcState = new GameState.NPCStateData
                {
                    npcId = npcController.GetNPCId(), // Dùng GetNPCId() để lấy ID
                    sceneName = currentScene,
                    hasTradingSystem = true,
                    money = npcController.GetMoney(),
                    inventoryItems = new List<GameState.NPCStateData.NPCInventoryItem>()
                };

                // Lưu inventory
                var inventory = npcController.GetInventory();
                if (inventory != null)
                {
                    foreach (var tradeItem in inventory)
                    {
                        if (tradeItem != null && tradeItem.itemData != null)
                        {
                            npcState.inventoryItems.Add(new GameState.NPCStateData.NPCInventoryItem
                            {
                                itemId = tradeItem.itemData.itemId ?? "",
                                quantity = tradeItem.quantity,
                                buyPrice = tradeItem.buyPrice,
                                sellPrice = tradeItem.sellPrice
                            });
                        }
                    }
                }

                state.npcStates.Add(npcState);
            }
        }

        /// <summary>
        /// Lưu trạng thái QuestItemGiver (đã tặng quà chưa)
        /// </summary>
        private void SaveQuestItemGiverStates(GameState state)
        {
            // Tìm tất cả QuestItemGiver trong scene hiện tại
            var questItemGivers = FindObjectsByType<Unity.NPC.QuestItemGiver>(FindObjectsSortMode.None);
            foreach (var giver in questItemGivers)
            {
                // Sử dụng reflection để lấy hasGiven (private field)
                var hasGivenField = typeof(Unity.NPC.QuestItemGiver).GetField("hasGiven", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (hasGivenField != null)
                {
                    bool hasGiven = (bool)hasGivenField.GetValue(giver);
                    state.questItemGiverStates.Add(new GameState.QuestItemGiverStateData
                    {
                        giverId = giver.gameObject.name, // Dùng GameObject name làm ID
                        sceneName = SceneManager.GetActiveScene().name,
                        hasGiven = hasGiven
                    });
                }
            }
        }

        /// <summary>
        /// Áp dụng GameState vào game
        /// </summary>
        private void ApplyGameState(GameState state)
        {
            // Load player data
            if (GameManager.PlayerService != null)
            {
                var player = GameManager.PlayerService.GetPlayer(PlayerId.Default);
                if (player != null)
                {
                    // Restore money
                    var currentMoney = player.Money;
                    var targetMoney = new Money(state.playerMoney);
                    if (targetMoney.Amount > currentMoney.Amount)
                    {
                        GameManager.PlayerService.AddMoney(PlayerId.Default, new Money(targetMoney.Amount - currentMoney.Amount));
                    }

                    // Restore position (Position constructor nhận int, convert từ float)
                    GameManager.PlayerService.MovePlayer(PlayerId.Default, new Position((int)state.playerPositionX, (int)state.playerPositionY));

                    // Restore energy
                    // Note: Energy không có public setter, cần dùng Restore hoặc tạo player mới
                    // Tạm thời skip vì Energy sẽ được restore khi player được tạo lại từ save
                }
            }

            // Load inventory
            if (GameManager.InventoryService != null && state.inventoryItems != null)
            {
                foreach (var itemData in state.inventoryItems)
                {
                    var itemId = new ItemId(itemData.itemId);
                    // Dùng AddItemById thay vì AddItem (vì AddItem cần Item entity, không phải ItemId)
                    GameManager.InventoryService.AddItemById(PlayerId.Default, itemId, itemData.quantity);
                }
            }

            // Load farm tiles
            if (GameManager.FarmRepository != null && GameManager.FarmingService != null && state.farmTiles != null)
            {
                foreach (var tileData in state.farmTiles)
                {
                    var position = new TilePosition(tileData.x, tileData.y);
                    var tile = GameManager.FarmRepository.GetTile(position);
                    
                    // Restore state
                    if (!string.IsNullOrEmpty(tileData.stateId))
                    {
                        tile.SetState(tileData.stateId);
                    }
                    
                    // Restore crop nếu có
                    if (!string.IsNullOrEmpty(tileData.cropId) && GameManager.CropDatabase != null)
                    {
                        var cropId = new CropId(tileData.cropId);
                        var crop = GameManager.CropDatabase.GetCrop(cropId);
                        if (crop != null)
                        {
                            // Tạo crop mới với days planted
                            // Note: Crop entity có thể cần constructor hoặc method để set days planted
                            // Tạm thời dùng PlantSeed để restore
                            var plantResult = GameManager.FarmingService.PlantSeed(position, crop, PlayerId.Default);
                            if (plantResult.IsSuccess && tile.Crop != null)
                            {
                                // Set days planted nếu Crop có property
                                // TODO: Cần kiểm tra Crop entity có method SetDaysPlanted không
                            }
                        }
                    }
                }
            }

            // Load time (optional - có thể bỏ nếu dùng thời gian Unity)
            // Note: Time sẽ được chỉnh trong Unity, không cần load từ save
            // Nếu muốn load time, uncomment phần dưới:
            /*
            if (GameManager.TimeService != null)
            {
                var timeManager = GameManager.TimeService as TimeManager;
                if (timeManager != null)
                {
                    timeManager.SetTime(state.currentDay, state.currentHour, state.currentMinute);
                }
            }
            */
            
            // Load Scene (nếu khác scene hiện tại)
            if (!string.IsNullOrEmpty(state.currentSceneName))
            {
                string currentScene = SceneManager.GetActiveScene().name;
                if (currentScene != state.currentSceneName)
                {
                    // Load scene mới (sẽ được xử lý bởi SceneLoader)
                    Debug.Log($"Need to load scene: {state.currentSceneName}");
                    // TODO: Load scene và sau đó restore position
                }
            }
            
            // Load Quest States (OPTIONAL - chỉ load nếu có Quest System)
            if (state.questStates != null && state.questStates.Count > 0)
            {
                // TODO: Restore quest states khi có Quest System
                // if (QuestManager.Instance != null) { ... }
            }
            
            // Load World Items (OPTIONAL - chỉ load nếu có World Item System)
            if (state.worldItems != null && state.worldItems.Count > 0)
            {
                // TODO: Restore world items (spawn items chưa pick up) khi có World Item System
            }
            
            // Load NPC States (OPTIONAL - chỉ load nếu có NPC System với trading)
            if (state.npcStates != null && state.npcStates.Count > 0)
            {
                LoadNPCStates(state);
            }
            
            // Load Building States (OPTIONAL - chỉ load nếu có Building System)
            if (state.buildingStates != null && state.buildingStates.Count > 0)
            {
                // TODO: Restore building states khi có Building System
            }
            
            // Load Door States (OPTIONAL - chỉ load nếu có Door System)
            if (state.doorStates != null && state.doorStates.Count > 0)
            {
                // TODO: Restore door states khi có Door System
            }
            
            // Load Quest Item Giver States (OPTIONAL - chỉ load nếu có trong scene)
            if (state.questItemGiverStates != null && state.questItemGiverStates.Count > 0)
            {
                try
                {
                    LoadQuestItemGiverStates(state);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to load QuestItemGiver states: {e.Message}");
                }
            }
            
            // Load Tool States
            var toolManager = FindFirstObjectByType<Managers.ToolManager>();
            if (toolManager != null && state.toolStates != null)
            {
                toolManager.LoadToolStates(state);
            }
        }
        
        /// <summary>
        /// Load trạng thái NPC (inventory, money, dialogue, etc.)
        /// Flexible: Chỉ load NPC có trading system, không lỗi nếu không có NPC
        /// </summary>
        private void LoadNPCStates(GameState state)
        {
            if (state.npcStates == null || state.npcStates.Count == 0)
            {
                return; // Không có NPC states để load
            }

            string currentScene = SceneManager.GetActiveScene().name;

            // Tìm tất cả NPCController trong scene hiện tại
            var npcControllers = FindObjectsByType<Unity.NPC.NPCController>(FindObjectsSortMode.None);
            if (npcControllers == null || npcControllers.Length == 0)
            {
                return; // Không có NPC trong scene, không cần load
            }

            foreach (var npcState in state.npcStates)
            {
                // Chỉ load NPC trong scene hiện tại
                if (npcState.sceneName != currentScene)
                {
                    continue;
                }

                // Tìm NPCController tương ứng (tìm bằng GetNPCId() hoặc gameObject.name)
                var npcController = System.Array.Find(npcControllers, npc => 
                    npc != null && (npc.GetNPCId() == npcState.npcId || npc.gameObject.name == npcState.npcId));
                if (npcController == null)
                {
                    continue; // NPC không tồn tại trong scene
                }

                // Chỉ load NPC có trading system
                if (!npcState.hasTradingSystem || !npcController.CanTrade)
                {
                    continue; // Skip NPC không có trading
                }

                // Restore money
                npcController.SetMoney(npcState.money);

                // Restore inventory
                if (npcState.inventoryItems != null && npcState.inventoryItems.Count > 0)
                {
                    var database = Managers.GameDatabaseManager.GetDatabase();
                    if (database != null && database.items != null)
                    {
                        var restoredInventory = new List<Unity.NPC.NPCController.NPCTradeItem>();
                        
                        foreach (var itemData in npcState.inventoryItems)
                        {
                            if (string.IsNullOrEmpty(itemData.itemId)) continue;

                            // Tìm ItemDataSO từ database
                            var itemSO = database.items.Find(item => item != null && item.itemId == itemData.itemId);
                            if (itemSO != null)
                            {
                                restoredInventory.Add(new Unity.NPC.NPCController.NPCTradeItem(
                                    itemSO,
                                    itemData.quantity,
                                    itemData.buyPrice,
                                    itemData.sellPrice
                                ));
                            }
                        }

                        // Set inventory
                        npcController.SetInventory(restoredInventory);
                    }
                }
                else
                {
                    // Clear inventory nếu không có items trong save
                    npcController.SetInventory(new List<Unity.NPC.NPCController.NPCTradeItem>());
                }
            }
        }

        /// <summary>
        /// Load trạng thái QuestItemGiver
        /// </summary>
        private void LoadQuestItemGiverStates(GameState state)
        {
            // Tìm tất cả QuestItemGiver trong scene hiện tại
            var questItemGivers = FindObjectsByType<Unity.NPC.QuestItemGiver>(FindObjectsSortMode.None);
            foreach (var giver in questItemGivers)
            {
                // Tìm state tương ứng
                var giverState = state.questItemGiverStates.FirstOrDefault(s => 
                    s.giverId == giver.gameObject.name && 
                    s.sceneName == SceneManager.GetActiveScene().name);
                
                if (giverState != null)
                {
                    // Sử dụng reflection để set hasGiven
                    var hasGivenField = typeof(Unity.NPC.QuestItemGiver).GetField("hasGiven", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (hasGivenField != null)
                    {
                        hasGivenField.SetValue(giver, giverState.hasGiven);
                    }
                }
            }
        }

        /// <summary>
        /// Quick save (lưu nhanh) - Lưu vào slot mặc định
        /// </summary>
        public void QuickSave()
        {
            var state = CreateGameState();
            SaveGameToSlot(defaultSlotIndex, state);
        }

        /// <summary>
        /// Quick load (load nhanh) - Load từ slot mặc định
        /// </summary>
        public void QuickLoad()
        {
            LoadGameFromSlot(defaultSlotIndex);
        }

        // ========== MULTIPLE SAVE SLOTS ==========

        /// <summary>
        /// Lưu game vào slot cụ thể
        /// </summary>
        public void SaveGameToSlot(int slotIndex, GameState state)
        {
            try
            {
                if (slotIndex < 0 || slotIndex >= maxSaveSlots)
                {
                    Debug.LogError($"Invalid slot index: {slotIndex}. Must be between 0 and {maxSaveSlots - 1}");
                    return;
                }

                // Cập nhật metadata
                state.saveDate = DateTime.Now;
                state.saveSlotIndex = slotIndex;
                state.playTimeInSeconds = totalPlayTime;
                
                // Cập nhật current slot
                currentSlotIndex = slotIndex;

                string savePath = GetSavePathForSlot(slotIndex);
                
                if (useJsonFormat)
                {
                    SaveGameJSONToPath(savePath, state);
                }
                else
                {
                    SaveGameBinaryToPath(savePath, state);
                }
                
                Debug.Log($"Game saved to slot {slotIndex + 1}: {savePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game to slot {slotIndex}: {e.Message}");
            }
        }

        /// <summary>
        /// Load game từ slot cụ thể
        /// </summary>
        public GameState LoadGameFromSlot(int slotIndex)
        {
            try
            {
                if (slotIndex < 0 || slotIndex >= maxSaveSlots)
                {
                    Debug.LogError($"Invalid slot index: {slotIndex}. Must be between 0 and {maxSaveSlots - 1}");
                    return null;
                }

                string savePath = GetSavePathForSlot(slotIndex);
                
                if (!File.Exists(savePath))
                {
                    Debug.LogWarning($"Save file not found in slot {slotIndex}!");
                    return null;
                }

                GameState state;
                if (useJsonFormat)
                {
                    state = LoadGameJSONFromPath(savePath);
                }
                else
                {
                    state = LoadGameBinaryFromPath(savePath);
                }

                if (state != null)
                {
                    // Cập nhật current slot và play time
                    currentSlotIndex = slotIndex;
                    totalPlayTime = state.playTimeInSeconds;
                    gameStartTime = DateTime.Now;
                    
                    Debug.Log($"Game loaded from slot {slotIndex + 1}!");
                    ApplyGameState(state);
                }

                return state;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game from slot {slotIndex}: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra có file save trong slot không
        /// </summary>
        public bool HasSaveFileInSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSaveSlots)
                return false;
                
            string savePath = GetSavePathForSlot(slotIndex);
            return File.Exists(savePath);
        }

        /// <summary>
        /// Xóa file save trong slot
        /// </summary>
        public void DeleteSaveFileInSlot(int slotIndex)
        {
            try
            {
                if (slotIndex < 0 || slotIndex >= maxSaveSlots)
                {
                    Debug.LogError($"Invalid slot index: {slotIndex}");
                    return;
                }

                string savePath = GetSavePathForSlot(slotIndex);
                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                    Debug.Log($"Save file deleted from slot {slotIndex + 1}!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete save file in slot {slotIndex}: {e.Message}");
            }
        }

        /// <summary>
        /// Lấy thông tin tất cả save slots
        /// </summary>
        public List<SaveSlotInfo> GetAllSaveSlots()
        {
            List<SaveSlotInfo> slots = new List<SaveSlotInfo>();
            int latestSlotIndex = GetLatestSaveSlotIndex();
            
            for (int i = 0; i < maxSaveSlots; i++)
            {
                slots.Add(GetSaveSlotInfo(i));
            }
            
            // Đánh dấu save gần nhất
            if (latestSlotIndex >= 0 && latestSlotIndex < slots.Count)
            {
                slots[latestSlotIndex].isLatest = true;
            }
            
            return slots;
        }

        /// <summary>
        /// Lấy thông tin save slot
        /// </summary>
        public SaveSlotInfo GetSaveSlotInfo(int slotIndex)
        {
            var info = new SaveSlotInfo
            {
                slotIndex = slotIndex,
                isEmpty = !HasSaveFileInSlot(slotIndex),
                isLatest = false
            };
            
            if (!info.isEmpty)
            {
                try
                {
                    // Load metadata từ file (chỉ load metadata, không load toàn bộ game state)
                    GameState state = LoadGameMetadataFromSlot(slotIndex);
                    if (state != null)
                    {
                        info.saveDate = state.saveDate;
                        info.playTimeInSeconds = state.playTimeInSeconds;
                        info.currentDay = state.currentDay;
                        info.currentHour = state.currentHour;
                        info.currentMinute = state.currentMinute;
                        info.playerMoney = state.playerMoney;
                        info.playerName = state.playerName ?? "Player";
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load metadata from slot {slotIndex}: {e.Message}");
                }
            }
            
            return info;
        }

        /// <summary>
        /// Lấy slot index của save gần nhất
        /// </summary>
        public int GetLatestSaveSlotIndex()
        {
            int latestIndex = -1;
            DateTime latestDate = DateTime.MinValue;
            
            for (int i = 0; i < maxSaveSlots; i++)
            {
                if (HasSaveFileInSlot(i))
                {
                    var info = GetSaveSlotInfo(i);
                    if (info.saveDate > latestDate)
                    {
                        latestDate = info.saveDate;
                        latestIndex = i;
                    }
                }
            }
            
            return latestIndex;
        }

        /// <summary>
        /// Load chỉ metadata từ slot (nhanh hơn, không load toàn bộ game state)
        /// </summary>
        private GameState LoadGameMetadataFromSlot(int slotIndex)
        {
            string savePath = GetSavePathForSlot(slotIndex);
            
            if (!File.Exists(savePath))
                return null;
                
            try
            {
                if (useJsonFormat)
                {
                    // Với JSON, có thể đọc một phần file
                    string json = File.ReadAllText(savePath);
                    return JsonUtility.FromJson<GameState>(json);
                }
                else
                {
                    // Với Binary, phải load toàn bộ (nhưng vẫn nhanh)
                    return LoadGameBinaryFromPath(savePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load metadata from slot {slotIndex}: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lưu game (Binary format) vào đường dẫn cụ thể
        /// </summary>
        private void SaveGameBinaryToPath(string path, GameState state)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(stream, state);
            }
        }

        /// <summary>
        /// Load game (Binary format) từ đường dẫn cụ thể
        /// </summary>
        private GameState LoadGameBinaryFromPath(string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return (GameState)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Lưu game (JSON format) vào đường dẫn cụ thể
        /// </summary>
        private void SaveGameJSONToPath(string path, GameState state)
        {
            string json = JsonUtility.ToJson(state, true);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Load game (JSON format) từ đường dẫn cụ thể
        /// </summary>
        private GameState LoadGameJSONFromPath(string path)
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameState>(json);
        }

    }
}

