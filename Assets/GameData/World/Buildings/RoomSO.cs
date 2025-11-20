using UnityEngine;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Room SO - Định nghĩa phòng trong tòa nhà
    /// </summary>
    [CreateAssetMenu(fileName = "Room", menuName = "Game/Building/Room", order = 38)]
    public class RoomSO : ScriptableObject
    {
        [System.Serializable]
        public enum UnlockCondition
        {
            None,           // Không cần unlock
            Quest,          // Cần hoàn thành quest
            Level,          // Cần đạt level
            Item,           // Cần có item
            Money,          // Cần trả tiền
            Custom          // Điều kiện tùy chỉnh
        }

        [Header("Room Info")]
        [Tooltip("ID của phòng (để reference)")]
        public string roomId;
        
        [Tooltip("Tên phòng")]
        public string roomName = "Room1";

        [Header("Unlock")]
        [Tooltip("Phòng này có bị khóa không")]
        public bool isLocked = false;
        
        [Tooltip("Điều kiện unlock")]
        public UnlockCondition unlockCondition = UnlockCondition.None;
        
        [Tooltip("Giá trị unlock (quest ID, level, item ID, money amount)")]
        public string unlockValue = "";
        
        [Tooltip("Thông báo khi phòng bị khóa")]
        public string lockedMessage = "Phòng này đang phát triển!";

        [Header("Position")]
        [Tooltip("Vị trí phòng trong scene (nếu cần)")]
        public Vector3 position = Vector3.zero;

        [Header("Special Features")]
        [Tooltip("Có giường để ngủ không")]
        public bool hasBed = false;
        
        [Tooltip("Vị trí giường")]
        public Vector3 bedPosition = Vector3.zero;
        
        [Tooltip("Scene load sau khi ngủ (nếu có giường)")]
        public string sleepNextScene = "";
    }
}

