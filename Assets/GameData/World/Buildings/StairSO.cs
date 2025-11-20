using UnityEngine;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Stair SO - Định nghĩa cầu thang giữa các tầng
    /// </summary>
    [CreateAssetMenu(fileName = "Stair", menuName = "Game/Building/Stair", order = 40)]
    public class StairSO : ScriptableObject
    {
        [Header("Stair Info")]
        [Tooltip("ID của cầu thang (để reference)")]
        public string stairId;
        
        [Tooltip("Tên cầu thang")]
        public string stairName = "Stair1";

        [Header("Target")]
        [Tooltip("Target Floor ID (tầng đi đến)")]
        public string targetFloorId = "";

        [Header("Spawn")]
        [Tooltip("Vị trí spawn khi đi lên/xuống tầng")]
        public Vector3 spawnPosition = Vector3.zero;

        [Header("Settings")]
        [Tooltip("Cầu thang này có hoạt động không")]
        public bool isEnabled = true;
        
        [Tooltip("Phím để tương tác")]
        public KeyCode interactKey = KeyCode.E;
        
        [Tooltip("Khoảng cách trigger")]
        public float triggerDistance = 2f;
    }
}

