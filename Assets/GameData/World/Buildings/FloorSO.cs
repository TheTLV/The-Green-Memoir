using UnityEngine;
using System.Collections.Generic;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Floor SO - Định nghĩa tầng trong tòa nhà
    /// </summary>
    [CreateAssetMenu(fileName = "Floor", menuName = "Game/Building/Floor", order = 37)]
    public class FloorSO : ScriptableObject
    {
        [Header("Floor Info")]
        [Tooltip("ID của tầng (để reference)")]
        public string floorId;
        
        [Tooltip("Tên tầng (vd: Tầng 1, Tầng 2)")]
        public string floorName = "Tầng 1";
        
        [Tooltip("Level của tầng (0 = tầng trệt, 1 = tầng 1, -1 = tầng hầm)")]
        public int floorLevel = 0;

        [Header("Rooms")]
        [Tooltip("Danh sách phòng trong tầng này")]
        public List<RoomSO> rooms = new List<RoomSO>();

        [Header("Stairs")]
        [Tooltip("Danh sách cầu thang trong tầng này")]
        public List<StairSO> stairs = new List<StairSO>();

        [Header("Scene")]
        [Tooltip("Scene của tầng này (nếu mỗi tầng là scene riêng)")]
        public string sceneName = "";
        
        [Tooltip("Spawn position mặc định khi vào tầng này")]
        public Vector3 defaultSpawnPosition = Vector3.zero;
    }
}

