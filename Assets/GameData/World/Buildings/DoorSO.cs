using UnityEngine;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Door SO - Định nghĩa cửa trong tòa nhà
    /// </summary>
    [CreateAssetMenu(fileName = "Door", menuName = "Game/Building/Door", order = 39)]
    public class DoorSO : ScriptableObject
    {
        [Header("Door Info")]
        [Tooltip("ID của cửa (để reference)")]
        public string doorId;
        
        [Tooltip("Tên cửa")]
        public string doorName = "MainDoor";

        [Header("Target")]
        [Tooltip("Target Room ID (nếu đi vào phòng)")]
        public string targetRoomId = "";
        
        [Tooltip("Target Building ID (nếu đi vào tòa nhà khác)")]
        public string targetBuildingId = "";
        
        [Tooltip("Target Scene Name (nếu đi vào scene khác)")]
        public string targetSceneName = "";

        [Header("Spawn")]
        [Tooltip("Vị trí spawn khi đi qua cửa này")]
        public Vector3 spawnPosition = Vector3.zero;

        [Header("Settings")]
        [Tooltip("Cửa này có mở được không")]
        public bool isEnabled = true;
        
        [Tooltip("Cửa này có bị khóa không")]
        public bool isLocked = false;
        
        [Tooltip("Phím để tương tác (chỉ dùng nếu autoTransition = false)")]
        public KeyCode interactKey = KeyCode.E;
        
        [Tooltip("Khoảng cách trigger (chỉ dùng nếu không có Collider2D trigger)")]
        public float triggerDistance = 2f;
        
        [Tooltip("Tự động chuyển phòng khi vào trigger zone (true) hoặc cần nhấn phím (false)")]
        public bool autoTransition = false;
        
        [Tooltip("Hiển thị prompt khi vào trigger zone (ví dụ: 'Nhấn E để vào')")]
        public bool showPrompt = true;
    }
}

