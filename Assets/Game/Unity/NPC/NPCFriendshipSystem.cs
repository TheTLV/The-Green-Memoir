using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.NPC
{
    /// <summary>
    /// NPC Friendship System - Quản lý độ thân mật với NPC (offline mode)
    /// Thay thế FriendSystem cho offline game
    /// </summary>
    public class NPCFriendshipSystem : MonoBehaviour
    {
        private static NPCFriendshipSystem _instance;
        public static NPCFriendshipSystem Instance => _instance;

        [System.Serializable]
        public class NPCFriendshipData
        {
            public string npcId;
            public string npcName;
            public int friendshipPoints;
            public FriendshipLevel level;
            public System.DateTime lastInteraction;
            public List<string> unlockedEvents = new List<string>(); // Events đã mở khóa nhờ friendship

            public enum FriendshipLevel
            {
                Stranger,      // 0-99
                Acquaintance,  // 100-299
                Friend,        // 300-599
                GoodFriend,    // 600-799
                BestFriend,    // 800-999
                Soulmate       // 1000+
            }

            public FriendshipLevel GetLevel(int points)
            {
                if (points >= 1000) return FriendshipLevel.Soulmate;
                if (points >= 800) return FriendshipLevel.BestFriend;
                if (points >= 600) return FriendshipLevel.GoodFriend;
                if (points >= 300) return FriendshipLevel.Friend;
                if (points >= 100) return FriendshipLevel.Acquaintance;
                return FriendshipLevel.Stranger;
            }
        }

        private Dictionary<string, NPCFriendshipData> _npcFriendships = new Dictionary<string, NPCFriendshipData>();
        private GameSettingsSO _gameSettings;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
                LoadFriendships();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        [Header("Game Settings (optional)")]
        [Tooltip("Kéo GameSettingsSO vào đây. Nếu để trống, sẽ tự động tìm")]
        [SerializeField] private GameSettingsSO settingsOverride;

        private void LoadSettings()
        {
            // Dùng override nếu có
            if (settingsOverride != null)
            {
                _gameSettings = settingsOverride;
            }
            else
            {
                // Tìm GameSettingsSO (runtime - dùng Resources hoặc tìm trong scene)
                var allSettings = Resources.FindObjectsOfTypeAll<GameSettingsSO>();
                if (allSettings != null && allSettings.Length > 0)
                {
                    _gameSettings = allSettings[0];
                }
                
                #if UNITY_EDITOR
                // Editor mode - dùng AssetDatabase
                if (_gameSettings == null)
                {
                    string[] guids = UnityEditor.AssetDatabase.FindAssets("t:GameSettingsSO");
                    if (guids.Length > 0)
                    {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                        _gameSettings = UnityEditor.AssetDatabase.LoadAssetAtPath<GameSettingsSO>(path);
                    }
                }
                #endif
            }

            // Nếu không có settings, tắt system
            if (_gameSettings == null || !_gameSettings.CanUseNPCFriendship())
            {
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// Thêm điểm thân mật với NPC
        /// </summary>
        public void AddFriendship(string npcId, string npcName, int points)
        {
            if (!IsEnabled()) return;

            if (!_npcFriendships.ContainsKey(npcId))
            {
                _npcFriendships[npcId] = new NPCFriendshipData
                {
                    npcId = npcId,
                    npcName = npcName,
                    friendshipPoints = 0,
                    level = NPCFriendshipData.FriendshipLevel.Stranger
                };
            }

            var data = _npcFriendships[npcId];
            data.friendshipPoints = Mathf.Clamp(data.friendshipPoints + points, 0, _gameSettings.maxFriendshipPoints);
            data.level = data.GetLevel(data.friendshipPoints);
            data.lastInteraction = System.DateTime.Now;

            SaveFriendships();

            Debug.Log($"Added {points} friendship points to {npcName}. Total: {data.friendshipPoints} ({data.level})");
        }

        /// <summary>
        /// Giảm điểm thân mật với NPC
        /// </summary>
        public void RemoveFriendship(string npcId, int points)
        {
            if (!IsEnabled()) return;
            if (!_npcFriendships.ContainsKey(npcId)) return;

            var data = _npcFriendships[npcId];
            data.friendshipPoints = Mathf.Max(0, data.friendshipPoints - points);
            data.level = data.GetLevel(data.friendshipPoints);
            data.lastInteraction = System.DateTime.Now;

            SaveFriendships();
        }

        /// <summary>
        /// Lấy điểm thân mật với NPC
        /// </summary>
        public int GetFriendship(string npcId)
        {
            if (!IsEnabled()) return 0;
            if (!_npcFriendships.ContainsKey(npcId)) return 0;
            return _npcFriendships[npcId].friendshipPoints;
        }

        /// <summary>
        /// Lấy level thân mật với NPC
        /// </summary>
        public NPCFriendshipData.FriendshipLevel GetFriendshipLevel(string npcId)
        {
            if (!IsEnabled()) return NPCFriendshipData.FriendshipLevel.Stranger;
            if (!_npcFriendships.ContainsKey(npcId)) return NPCFriendshipData.FriendshipLevel.Stranger;
            return _npcFriendships[npcId].level;
        }

        /// <summary>
        /// Kiểm tra đã đạt level thân mật chưa
        /// </summary>
        public bool HasReachedLevel(string npcId, NPCFriendshipData.FriendshipLevel requiredLevel)
        {
            var currentLevel = GetFriendshipLevel(npcId);
            return currentLevel >= requiredLevel;
        }

        /// <summary>
        /// Mở khóa event nhờ friendship
        /// </summary>
        public void UnlockEvent(string npcId, string eventId)
        {
            if (!IsEnabled()) return;
            if (!_npcFriendships.ContainsKey(npcId)) return;

            var data = _npcFriendships[npcId];
            if (!data.unlockedEvents.Contains(eventId))
            {
                data.unlockedEvents.Add(eventId);
                SaveFriendships();
            }
        }

        /// <summary>
        /// Kiểm tra event đã mở khóa chưa
        /// </summary>
        public bool IsEventUnlocked(string npcId, string eventId)
        {
            if (!IsEnabled()) return false;
            if (!_npcFriendships.ContainsKey(npcId)) return false;
            return _npcFriendships[npcId].unlockedEvents.Contains(eventId);
        }

        /// <summary>
        /// Lấy danh sách NPC đã tương tác
        /// </summary>
        public List<NPCFriendshipData> GetAllFriendships()
        {
            if (!IsEnabled()) return new List<NPCFriendshipData>();
            return _npcFriendships.Values.ToList();
        }

        /// <summary>
        /// Lấy danh sách NPC theo level
        /// </summary>
        public List<NPCFriendshipData> GetNPCsByLevel(NPCFriendshipData.FriendshipLevel level)
        {
            if (!IsEnabled()) return new List<NPCFriendshipData>();
            return _npcFriendships.Values.Where(n => n.level == level).ToList();
        }

        private bool IsEnabled()
        {
            return _gameSettings != null && _gameSettings.CanUseNPCFriendship();
        }

        private void SaveFriendships()
        {
            string json = JsonUtility.ToJson(new FriendshipListWrapper { friendships = _npcFriendships.Values.ToList() });
            PlayerPrefs.SetString("NPCFriendships", json);
        }

        private void LoadFriendships()
        {
            string json = PlayerPrefs.GetString("NPCFriendships", "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var wrapper = JsonUtility.FromJson<FriendshipListWrapper>(json);
                    if (wrapper.friendships != null)
                    {
                        foreach (var friendship in wrapper.friendships)
                        {
                            _npcFriendships[friendship.npcId] = friendship;
                        }
                    }
                }
                catch
                {
                    _npcFriendships = new Dictionary<string, NPCFriendshipData>();
                }
            }
        }

        [System.Serializable]
        private class FriendshipListWrapper
        {
            public List<NPCFriendshipData> friendships;
        }
    }
}

