using UnityEngine;

namespace TheGreenMemoir.Unity.NPC
{
    /// <summary>
    /// NPC Friendship SO - Cấu hình độ thân mật cho từng NPC
    /// Gán vào NPCDefinitionSO để cấu hình
    /// </summary>
    [CreateAssetMenu(fileName = "NPCFriendship", menuName = "Game/NPC/Friendship Config", order = 35)]
    public class NPCFriendshipSO : ScriptableObject
    {
        [Header("Friendship Settings")]
        [Tooltip("NPC này có thể tăng độ thân mật không")]
        public bool canGainFriendship = true;

        [Tooltip("Điểm thân mật ban đầu")]
        public int startingFriendship = 0;

        [Header("Friendship Rewards")]
        [Tooltip("Events mở khóa khi đạt level thân mật")]
        public FriendshipReward[] rewards;

        [System.Serializable]
        public class FriendshipReward
        {
            public NPCFriendshipSystem.NPCFriendshipData.FriendshipLevel requiredLevel;
            public string eventId; // Event mở khóa
            public string questId; // Quest mở khóa (optional)
            public string dialogueId; // Dialogue mở khóa (optional)
        }

        /// <summary>
        /// Lấy reward cho level thân mật
        /// </summary>
        public FriendshipReward GetRewardForLevel(NPCFriendshipSystem.NPCFriendshipData.FriendshipLevel level)
        {
            if (rewards == null) return null;
            return System.Array.Find(rewards, r => r.requiredLevel == level);
        }
    }
}

