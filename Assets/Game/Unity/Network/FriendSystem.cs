using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TheGreenMemoir.Unity.Network
{
    /// <summary>
    /// Hệ thống bạn bè - Quản lý danh sách bạn, add/remove friend
    /// </summary>
    public class FriendSystem : MonoBehaviour
    {
        private static FriendSystem _instance;
        public static FriendSystem Instance => _instance;

        [System.Serializable]
        public class FriendData
        {
            public string friendId;
            public string friendName;
            public bool isOnline;
            public System.DateTime lastSeen;
            public FriendStatus status;

            public enum FriendStatus
            {
                Pending,    // Đang chờ chấp nhận
                Accepted,  // Đã chấp nhận
                Blocked    // Đã chặn
            }
        }

        private List<FriendData> _friends = new List<FriendData>();
        private List<string> _pendingRequests = new List<string>(); // Friend requests đang chờ

        [Header("Game Settings (optional)")]
        [Tooltip("Kéo GameSettingsSO vào đây. Chỉ hoạt động nếu online mode")]
        [SerializeField] private TheGreenMemoir.Unity.Data.GameSettingsSO gameSettings;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                CheckIfEnabled();
                LoadFriends();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void CheckIfEnabled()
        {
            // Load settings nếu chưa có
            if (gameSettings == null)
            {
                #if UNITY_EDITOR
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:GameSettingsSO");
                if (guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    gameSettings = UnityEditor.AssetDatabase.LoadAssetAtPath<TheGreenMemoir.Unity.Data.GameSettingsSO>(path);
                }
                #endif
            }

            // Chỉ hoạt động nếu online mode và showFriendList = true
            if (gameSettings != null)
            {
                if (!gameSettings.CanUseOnlineFeatures() || !gameSettings.showFriendList)
                {
                    enabled = false;
                    gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Gửi lời mời kết bạn
        /// </summary>
        public void SendFriendRequest(string friendId, string friendName = "")
        {
            if (string.IsNullOrEmpty(friendId)) return;
            if (_friends.Any(f => f.friendId == friendId)) return; // Đã là bạn

            // Thêm vào pending
            if (!_pendingRequests.Contains(friendId))
            {
                _pendingRequests.Add(friendId);
            }

            // Gửi request đến server
            if (NetworkManager.Instance != null && NetworkManager.Instance.IsConnected)
            {
                NetworkManager.Instance.SendMessageToServer("FriendRequest", new
                {
                    friendId = friendId,
                    friendName = friendName
                });
            }

            Debug.Log($"Friend request sent to {friendName} ({friendId})");
        }

        /// <summary>
        /// Chấp nhận lời mời kết bạn
        /// </summary>
        public void AcceptFriendRequest(string friendId)
        {
            if (!_pendingRequests.Contains(friendId)) return;

            var friend = new FriendData
            {
                friendId = friendId,
                friendName = "Friend", // Sẽ được cập nhật từ server
                isOnline = false,
                lastSeen = System.DateTime.Now,
                status = FriendData.FriendStatus.Accepted
            };

            _friends.Add(friend);
            _pendingRequests.Remove(friendId);

            SaveFriends();

            Debug.Log($"Friend request accepted: {friendId}");
        }

        /// <summary>
        /// Từ chối lời mời kết bạn
        /// </summary>
        public void DeclineFriendRequest(string friendId)
        {
            _pendingRequests.Remove(friendId);
            Debug.Log($"Friend request declined: {friendId}");
        }

        /// <summary>
        /// Xóa bạn
        /// </summary>
        public void RemoveFriend(string friendId)
        {
            _friends.RemoveAll(f => f.friendId == friendId);
            SaveFriends();
            Debug.Log($"Friend removed: {friendId}");
        }

        /// <summary>
        /// Chặn bạn
        /// </summary>
        public void BlockFriend(string friendId)
        {
            var friend = _friends.FirstOrDefault(f => f.friendId == friendId);
            if (friend != null)
            {
                friend.status = FriendData.FriendStatus.Blocked;
                SaveFriends();
                Debug.Log($"Friend blocked: {friendId}");
            }
        }

        /// <summary>
        /// Bỏ chặn bạn
        /// </summary>
        public void UnblockFriend(string friendId)
        {
            var friend = _friends.FirstOrDefault(f => f.friendId == friendId);
            if (friend != null && friend.status == FriendData.FriendStatus.Blocked)
            {
                friend.status = FriendData.FriendStatus.Accepted;
                SaveFriends();
                Debug.Log($"Friend unblocked: {friendId}");
            }
        }

        /// <summary>
        /// Lấy danh sách bạn
        /// </summary>
        public List<FriendData> GetFriends()
        {
            return _friends.Where(f => f.status == FriendData.FriendStatus.Accepted).ToList();
        }

        /// <summary>
        /// Lấy danh sách bạn online
        /// </summary>
        public List<FriendData> GetOnlineFriends()
        {
            return _friends.Where(f => f.status == FriendData.FriendStatus.Accepted && f.isOnline).ToList();
        }

        /// <summary>
        /// Lấy danh sách lời mời đang chờ
        /// </summary>
        public List<string> GetPendingRequests()
        {
            return new List<string>(_pendingRequests);
        }

        /// <summary>
        /// Kiểm tra có phải bạn không
        /// </summary>
        public bool IsFriend(string friendId)
        {
            return _friends.Any(f => f.friendId == friendId && f.status == FriendData.FriendStatus.Accepted);
        }

        /// <summary>
        /// Lưu danh sách bạn
        /// </summary>
        private void SaveFriends()
        {
            string json = JsonUtility.ToJson(new FriendListWrapper { friends = _friends });
            PlayerPrefs.SetString("Friends", json);
        }

        /// <summary>
        /// Load danh sách bạn
        /// </summary>
        private void LoadFriends()
        {
            string json = PlayerPrefs.GetString("Friends", "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var wrapper = JsonUtility.FromJson<FriendListWrapper>(json);
                    _friends = wrapper.friends ?? new List<FriendData>();
                }
                catch
                {
                    _friends = new List<FriendData>();
                }
            }
        }

        [System.Serializable]
        private class FriendListWrapper
        {
            public List<FriendData> friends;
        }
    }
}

