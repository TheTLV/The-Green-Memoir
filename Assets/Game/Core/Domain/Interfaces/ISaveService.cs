using System;
using System.Collections.Generic;

namespace TheGreenMemoir.Core.Domain.Interfaces
{
    /// <summary>
    /// Dịch vụ lưu/load game
    /// </summary>
    public interface ISaveService
    {
        void SaveGame(GameState state);
        GameState LoadGame();
        bool HasSaveFile();
        void DeleteSaveFile();
        
        // Multiple save slots
        void SaveGameToSlot(int slotIndex, GameState state);
        GameState LoadGameFromSlot(int slotIndex);
        bool HasSaveFileInSlot(int slotIndex);
        void DeleteSaveFileInSlot(int slotIndex);
        List<SaveSlotInfo> GetAllSaveSlots();
        SaveSlotInfo GetSaveSlotInfo(int slotIndex);
        int GetLatestSaveSlotIndex();
    }

    /// <summary>
    /// Thông tin về save slot
    /// </summary>
    [System.Serializable]
    public class SaveSlotInfo
    {
        public int slotIndex;
        public bool isEmpty;
        public DateTime saveDate;
        public float playTimeInSeconds; // Thời gian chơi (giây)
        public int currentDay;
        public int currentHour;
        public int currentMinute;
        public int playerMoney;
        public string playerName;
        public bool isLatest; // Save gần nhất
        
        public string GetPlayTimeFormatted()
        {
            int hours = (int)(playTimeInSeconds / 3600);
            int minutes = (int)((playTimeInSeconds % 3600) / 60);
            return $"{hours:00}:{minutes:00}";
        }
        
        public string GetSaveDateFormatted()
        {
            return saveDate.ToString("dd/MM/yyyy HH:mm");
        }
    }

    /// <summary>
    /// Trạng thái game để lưu
    /// </summary>
    [System.Serializable]
    public class GameState
    {
        /// <summary>
        /// Cho phép input hay không (false khi pause, cutscene, dialogue, etc)
        /// </summary>
        public static bool AllowInput { get; set; } = true;

        // Player Data
        public int playerMoney;
        public int playerEnergy;
        public int playerMaxEnergy;
        public float playerPositionX;
        public float playerPositionY;
        
        // Scene & Location
        public string currentSceneName; // Scene hiện tại (ví dụ: "Game", "House", "Farm")
        public string currentRoomName; // Room hiện tại (nếu có room system)

        // Inventory
        [System.Serializable]
        public class InventoryItemData
        {
            public string itemId;
            public int quantity;
        }
        public System.Collections.Generic.List<InventoryItemData> inventoryItems;

        // Farm Tiles
        [System.Serializable]
        public class FarmTileData
        {
            public int x;
            public int y;
            public string stateId;
            public string cropId;
            public int growthStage;
            public float growthProgress;
        }
        public System.Collections.Generic.List<FarmTileData> farmTiles;

        // Time
        public int currentDay;
        public int currentHour;
        public int currentMinute;

        // Save Metadata
        public DateTime saveDate;
        public float playTimeInSeconds; // Thời gian chơi (giây)
        public int saveSlotIndex; // Slot number (0-based)

        // Settings
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;
        public bool fullscreen;
        public int qualityLevel;

        // Multiplayer (nếu có)
        public string playerName;
        public string playerId;
        public System.Collections.Generic.List<string> friendIds;
        
        // Quest System
        [System.Serializable]
        public class QuestStateData
        {
            public string questId;
            public string questStatus; // "NotStarted", "InProgress", "Completed", "Failed"
            public System.Collections.Generic.List<QuestObjectiveData> objectives;
            public bool isRewarded;
        }
        public System.Collections.Generic.List<QuestStateData> questStates;
        
        [System.Serializable]
        public class QuestObjectiveData
        {
            public string objectiveId;
            public int currentProgress;
            public bool isCompleted;
        }
        
        // World Items (Items trong map - pickup items, dropped items)
        [System.Serializable]
        public class WorldItemData
        {
            public string itemId;
            public int quantity;
            public float positionX;
            public float positionY;
            public string sceneName; // Scene chứa item này
            public string uniqueId; // ID duy nhất để track item (ví dụ: "quest_item_1", "pickup_rock_1")
            public bool isPickedUp; // Đã nhặt chưa
        }
        public System.Collections.Generic.List<WorldItemData> worldItems;
        
        // NPC States (NPC đã tặng quà, dialogue states, inventory, money, etc.)
        [System.Serializable]
        public class NPCStateData
        {
            public string npcId; // ID của NPC (ví dụ: "npc_merchant", "npc_quest_giver")
            public string sceneName; // Scene chứa NPC này
            public bool hasGivenGift; // Đã tặng quà chưa (nếu giveOnce = true)
            public int dialogueIndex; // Dialogue index hiện tại
            public System.Collections.Generic.List<string> completedDialogues; // Các dialogue đã hoàn thành
            
            // Trading System (nếu NPC có trading)
            public bool hasTradingSystem; // NPC có hệ thống giao dịch không
            public int money; // Số tiền NPC có
            [System.Serializable]
            public class NPCInventoryItem
            {
                public string itemId; // Item ID
                public int quantity; // Số lượng
                public int buyPrice; // Giá NPC mua từ player
                public int sellPrice; // Giá NPC bán cho player
            }
            public System.Collections.Generic.List<NPCInventoryItem> inventoryItems; // Items trong túi NPC
        }
        public System.Collections.Generic.List<NPCStateData> npcStates;
        
        // Building States (Building đã xây, đã upgrade, etc.)
        [System.Serializable]
        public class BuildingStateData
        {
            public string buildingId; // ID của building
            public float positionX;
            public float positionY;
            public string sceneName; // Scene chứa building này
            public int level; // Level của building (nếu có upgrade)
            public bool isBuilt; // Đã xây chưa
        }
        public System.Collections.Generic.List<BuildingStateData> buildingStates;
        
        // Door States (Door locked/unlocked, etc.)
        [System.Serializable]
        public class DoorStateData
        {
            public string doorId; // ID của door (ví dụ: "maindoor", "housedoor")
            public string sceneName; // Scene chứa door này
            public bool isLocked; // Đã khóa chưa
            public bool isEnabled; // Có enabled không
        }
        public System.Collections.Generic.List<DoorStateData> doorStates;
        
        // Quest Item Giver States (QuestItemGiver đã tặng chưa)
        [System.Serializable]
        public class QuestItemGiverStateData
        {
            public string giverId; // Unique ID của QuestItemGiver (có thể dùng GameObject name)
            public string sceneName; // Scene chứa giver này
            public bool hasGiven; // Đã tặng chưa
        }
        public System.Collections.Generic.List<QuestItemGiverStateData> questItemGiverStates;
        
        // Tool States (Tool usage, level, etc.)
        [System.Serializable]
        public class ToolStateData
        {
            public string toolId; // Tool ID từ ToolDataSO
            public int currentUses; // Số lần sử dụng hiện tại (-1 = infinite)
            public int toolLevel; // Level của tool (nếu có thể nâng cấp)
            public bool isUnlocked; // Tool đã unlock chưa
        }
        public System.Collections.Generic.List<ToolStateData> toolStates;
    }
}

