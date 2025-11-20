using System.Collections.Generic;
using UnityEngine;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.Managers
{
    /// <summary>
    /// Tool Manager - Quản lý trạng thái tools (currentUses, level, etc.)
    /// Hệ thống flex: Chỉ cần thêm ToolDataSO là tự động handle được
    /// </summary>
    public class ToolManager : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Auto-initialize tools từ GameDatabase khi Start")]
        [SerializeField] private bool autoInitializeTools = true;

        private Dictionary<string, ToolState> toolStates = new Dictionary<string, ToolState>();

        /// <summary>
        /// Tool State - Lưu trạng thái của tool
        /// </summary>
        [System.Serializable]
        public class ToolState
        {
            public string toolId;
            public int currentUses;
            public int toolLevel;
            public bool isUnlocked;
            public bool isInfiniteUses; // Cache từ ToolDataSO

            public ToolState(string id, bool infiniteUses, int maxUses, int level = 1, bool unlocked = true)
            {
                toolId = id;
                isInfiniteUses = infiniteUses;
                currentUses = infiniteUses ? -1 : maxUses; // -1 = infinite
                toolLevel = level;
                isUnlocked = unlocked;
            }

            public bool CanUse()
            {
                return isInfiniteUses || currentUses > 0;
            }

            public void Use()
            {
                if (!isInfiniteUses && currentUses > 0)
                {
                    currentUses--;
                }
            }

            public void Refill(int maxUses)
            {
                if (!isInfiniteUses)
                {
                    currentUses = maxUses;
                }
            }

            public void RefillAmount(int amount)
            {
                if (!isInfiniteUses && amount > 0)
                {
                    // Will be capped by maxUses when saving/loading
                    currentUses += amount;
                }
            }

            public void Upgrade(int newLevel, int newMaxUses)
            {
                toolLevel = newLevel;
                if (!isInfiniteUses)
                {
                    // Khi upgrade, refill về max
                    currentUses = newMaxUses;
                }
            }
        }

        private void Start()
        {
            // GameDatabaseManager không có Instance, chỉ có static GetDatabase()
            if (autoInitializeTools)
            {
                InitializeTools();
            }
        }

        /// <summary>
        /// Khởi tạo tools từ GameDatabase
        /// </summary>
        public void InitializeTools()
        {
            toolStates.Clear();
            
            var database = GameDatabaseManager.GetDatabase();
            if (database == null || database.tools == null)
            {
                Debug.LogWarning("GameDatabase not found or no tools!");
                return;
            }

            foreach (var toolData in database.tools)
            {
                if (toolData == null)
                    continue;

                // Chỉ init tools mặc định (isDefaultTool = true)
                if (toolData.isDefaultTool)
                {
                    var toolState = new ToolState(
                        toolData.toolId,
                        toolData.isInfiniteUses,
                        toolData.maxUses,
                        toolData.toolLevel,
                        true // Mặc định unlock
                    );
                    toolStates[toolData.toolId] = toolState;
                }
            }

            Debug.Log($"ToolManager: Initialized {toolStates.Count} tools");
        }

        /// <summary>
        /// Lấy tool state
        /// </summary>
        public ToolState GetToolState(string toolId)
        {
            if (toolStates.TryGetValue(toolId, out var state))
            {
                return state;
            }

            // Nếu chưa có, tạo mới từ ToolDataSO
            var database = GameDatabaseManager.GetDatabase();
            if (database != null)
            {
                var toolData = database.GetToolData(new ToolId(toolId));
                if (toolData != null)
                {
                    var newState = new ToolState(
                        toolId,
                        toolData.isInfiniteUses,
                        toolData.maxUses,
                        toolData.toolLevel,
                        toolData.isDefaultTool // Unlock nếu là default tool
                    );
                    toolStates[toolId] = newState;
                    return newState;
                }
            }

            return null;
        }

        /// <summary>
        /// Sử dụng tool (giảm currentUses)
        /// </summary>
        public bool UseTool(string toolId)
        {
            var state = GetToolState(toolId);
            if (state == null || !state.CanUse())
                return false;

            state.Use();
            return true;
        }

        /// <summary>
        /// Refill tool (bơm lại)
        /// </summary>
        public void RefillTool(string toolId)
        {
            var state = GetToolState(toolId);
            if (state == null || state.isInfiniteUses)
                return;

            var database = GameDatabaseManager.GetDatabase();
            if (database != null)
            {
                var toolData = database.GetToolData(new ToolId(toolId));
                if (toolData != null)
                {
                    state.Refill(toolData.maxUses);
                }
            }
        }

        /// <summary>
        /// Refill tool với số lượng cụ thể
        /// </summary>
        public void RefillToolAmount(string toolId, int amount)
        {
            var state = GetToolState(toolId);
            if (state != null && !state.isInfiniteUses)
            {
                var database = GameDatabaseManager.GetDatabase();
                if (database != null)
                {
                    var toolData = database.GetToolData(new ToolId(toolId));
                    if (toolData != null)
                    {
                        state.RefillAmount(amount);
                        // Cap tại maxUses
                        if (state.currentUses > toolData.maxUses)
                        {
                            state.currentUses = toolData.maxUses;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Nâng cấp tool
        /// </summary>
        public void UpgradeTool(string toolId, int newLevel)
        {
            var state = GetToolState(toolId);
            if (state == null)
                return;

            var database = GameDatabaseManager.GetDatabase();
            if (database != null)
            {
                var toolData = database.GetToolData(new ToolId(toolId));
                if (toolData != null && toolData.isUpgradeable)
                {
                    // TODO: Tính newMaxUses dựa trên level (ví dụ: level 2 = maxUses * 1.5)
                    int newMaxUses = toolData.maxUses + (newLevel - 1) * 10; // Tạm thời
                    state.Upgrade(newLevel, newMaxUses);
                }
            }
        }

        /// <summary>
        /// Unlock tool
        /// </summary>
        public void UnlockTool(string toolId)
        {
            var state = GetToolState(toolId);
            if (state == null)
            {
                // Tạo mới nếu chưa có
                var database = GameDatabaseManager.GetDatabase();
                if (database != null)
                {
                    var toolData = database.GetToolData(new ToolId(toolId));
                    if (toolData != null)
                    {
                        state = new ToolState(
                            toolId,
                            toolData.isInfiniteUses,
                            toolData.maxUses,
                            toolData.toolLevel,
                            true
                        );
                        toolStates[toolId] = state;
                    }
                }
            }
            else
            {
                state.isUnlocked = true;
            }
        }

        /// <summary>
        /// Lưu tool states vào GameState
        /// </summary>
        public void SaveToolStates(GameState gameState)
        {
            if (gameState.toolStates == null)
            {
                gameState.toolStates = new List<GameState.ToolStateData>();
            }
            else
            {
                gameState.toolStates.Clear();
            }

            foreach (var kvp in toolStates)
            {
                var state = kvp.Value;
                gameState.toolStates.Add(new GameState.ToolStateData
                {
                    toolId = state.toolId,
                    currentUses = state.currentUses,
                    toolLevel = state.toolLevel,
                    isUnlocked = state.isUnlocked
                });
            }
        }

        /// <summary>
        /// Load tool states từ GameState
        /// </summary>
        public void LoadToolStates(GameState gameState)
        {
            if (gameState.toolStates == null)
                return;

            toolStates.Clear();
            var database = GameDatabaseManager.GetDatabase();
            if (database == null)
                return;

            foreach (var toolStateData in gameState.toolStates)
            {
                var toolData = database.GetToolData(new ToolId(toolStateData.toolId));
                if (toolData != null)
                {
                    var state = new ToolState(
                        toolStateData.toolId,
                        toolData.isInfiniteUses,
                        toolData.maxUses,
                        toolStateData.toolLevel,
                        toolStateData.isUnlocked
                    );
                    state.currentUses = toolStateData.currentUses;
                    toolStates[toolStateData.toolId] = state;
                }
            }
        }

        /// <summary>
        /// Lấy số lần sử dụng hiện tại
        /// </summary>
        public int GetCurrentUses(string toolId)
        {
            var state = GetToolState(toolId);
            return state != null ? (state.isInfiniteUses ? -1 : state.currentUses) : 0;
        }

        /// <summary>
        /// Lấy level của tool
        /// </summary>
        public int GetToolLevel(string toolId)
        {
            var state = GetToolState(toolId);
            return state != null ? state.toolLevel : 1;
        }

        /// <summary>
        /// Kiểm tra tool đã unlock chưa
        /// </summary>
        public bool IsToolUnlocked(string toolId)
        {
            var state = GetToolState(toolId);
            return state != null && state.isUnlocked;
        }
    }
}

