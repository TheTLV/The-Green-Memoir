using UnityEngine;
using System.Collections.Generic;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Building SO - Cấu hình tòa nhà (nhà, siêu thị, trung tâm thương mại, v.v.)
    /// Dùng FloorSO, RoomSO, DoorSO, StairSO thay vì nested classes
    /// Tất cả cấu hình qua Inspector, không cần code
    /// </summary>
    [CreateAssetMenu(fileName = "Building", menuName = "Game/Building", order = 35)]
    public class BuildingSO : ScriptableObject
    {
        [Header("Building Info")]
        [Tooltip("ID của tòa nhà (để reference)")]
        public string buildingId;
        
        [Tooltip("Tên tòa nhà (vd: Nhà Chính, Siêu Thị, Trung Tâm Thương Mại)")]
        public string buildingName = "Nhà Chính";
        
        [Tooltip("Loại tòa nhà (vd: House, Supermarket, Mall)")]
        public string buildingType = "House";
        
        [Tooltip("Scene của tòa nhà này (nếu toàn bộ tòa nhà là 1 scene)")]
        public string buildingScene = "House";

        [Header("Floors (Nhiều Tầng)")]
        [Tooltip("Danh sách tầng trong tòa nhà (dùng FloorSO)")]
        public List<FloorSO> floors = new List<FloorSO>();

        [Header("Doors (Nhiều Cửa)")]
        [Tooltip("Danh sách cửa trong tòa nhà (dùng DoorSO)")]
        public List<DoorSO> doors = new List<DoorSO>();

        [Header("Special Features")]
        [Tooltip("Có giường để ngủ không (chỉ cho nhà)")]
        public bool hasBed = true;
        
        [Tooltip("Vị trí giường trong scene (nếu có)")]
        public Vector3 bedPosition = new Vector3(0, 0, 0);
        
        [Tooltip("Scene load sau khi ngủ (chỉ cho nhà)")]
        public string sleepNextScene = "Game";

        [Header("Teleport Points")]
        [Tooltip("Vị trí spawn mặc định khi vào tòa nhà")]
        public Vector3 defaultSpawnPosition = new Vector3(0, 0, 0);
        
        [Tooltip("Vị trí quay về (vd: vườn trong Game scene)")]
        public Vector3 returnPosition = new Vector3(0, 0, 0);

        /// <summary>
        /// Lấy FloorSO từ floor ID
        /// </summary>
        public FloorSO GetFloor(string floorId)
        {
            return floors.Find(f => f != null && f.floorId == floorId);
        }

        /// <summary>
        /// Lấy RoomSO từ room ID (tìm trong tất cả floors)
        /// </summary>
        public RoomSO GetRoom(string roomId)
        {
            foreach (var floor in floors)
            {
                if (floor == null) continue;
                var room = floor.rooms.Find(r => r != null && r.roomId == roomId);
                if (room != null) return room;
            }
            return null;
        }

        /// <summary>
        /// Lấy DoorSO từ door ID
        /// </summary>
        public DoorSO GetDoor(string doorId)
        {
            return doors.Find(d => d != null && d.doorId == doorId);
        }

        /// <summary>
        /// Lấy StairSO từ stair ID (tìm trong tất cả floors)
        /// </summary>
        public StairSO GetStair(string stairId)
        {
            foreach (var floor in floors)
            {
                if (floor == null) continue;
                var stair = floor.stairs.Find(s => s != null && s.stairId == stairId);
                if (stair != null) return stair;
            }
            return null;
        }
    }
}
