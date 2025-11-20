using UnityEngine;
using UnityEngine.SceneManagement;
using TheGreenMemoir.Unity.Data;
using System.Collections.Generic;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Building Controller - Quản lý tòa nhà từ BuildingSO (nhà, siêu thị, trung tâm thương mại, v.v.)
    /// Hỗ trợ nhiều tầng, nhiều cửa, như miniscene
    /// Tất cả cấu hình qua SO
    /// </summary>
    public class BuildingController : MonoBehaviour
    {
        [Header("Building SO")]
        [Tooltip("Kéo BuildingSO vào đây")]
        [SerializeField] private BuildingSO buildingSO;

        [Header("Room References (Tự động tìm nếu để trống)")]
        [Tooltip("Danh sách phòng (tự động tìm theo tên trong SO)")]
        [SerializeField] private List<GameObject> rooms = new List<GameObject>();
        
        [Tooltip("Giường (nếu có)")]
        [SerializeField] private GameObject bed;
        
        [Tooltip("Button ngủ (nếu có)")]
        [SerializeField] private UnityEngine.UI.Button sleepButton;

        [Header("UI")]
        [Tooltip("Text hiển thị thông báo đang phát triển")]
        [SerializeField] private TMPro.TextMeshProUGUI developingText;

        [Header("Door References")]
        [Tooltip("Danh sách cửa (tự động tìm nếu để trống)")]
        [SerializeField] private List<GameObject> doors = new List<GameObject>();

        private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();

        void Start()
        {
            // Load BuildingSO
            if (buildingSO == null)
            {
                #if UNITY_EDITOR
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BuildingSO");
                if (guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    buildingSO = UnityEditor.AssetDatabase.LoadAssetAtPath<BuildingSO>(path);
                }
                #endif
            }

            if (buildingSO == null)
            {
                Debug.LogWarning("BuildingSO not found!");
                return;
            }

            // Setup rooms theo SO
            SetupRooms();
            
            // Setup doors theo SO
            SetupDoors();

            // Link sleep button (nếu có)
            if (sleepButton != null && buildingSO.hasBed)
            {
                sleepButton.onClick.AddListener(OnSleepClicked);
            }
        }

        void SetupRooms()
        {
            // Tự động tìm rooms nếu chưa gán
            if (rooms.Count == 0)
            {
                foreach (var floor in buildingSO.floors)
                {
                    foreach (var roomData in floor.rooms)
                    {
                        GameObject roomObj = GameObject.Find(roomData.roomName);
                        if (roomObj != null)
                        {
                            rooms.Add(roomObj);
                            roomDict[roomData.roomName] = roomObj;
                        }
                    }
                }
            }
            else
            {
                // Build dict từ rooms đã gán
                foreach (var room in rooms)
                {
                    if (room != null)
                        roomDict[room.name] = room;
                }
            }

            // Enable/disable rooms theo SO
            foreach (var floor in buildingSO.floors)
            {
                if (floor == null) continue;
                foreach (var roomData in floor.rooms)
                {
                    if (roomData == null) continue;
                    if (roomDict.ContainsKey(roomData.roomName))
                    {
                        // enabled = !isLocked
                        roomDict[roomData.roomName].SetActive(!roomData.isLocked);
                    }
                }
            }
        }

        void SetupDoors()
        {
            // Tự động tìm doors nếu chưa gán
            if (doors.Count == 0)
            {
                foreach (var doorData in buildingSO.doors)
                {
                    if (doorData == null || !doorData.isEnabled) continue;
                    
                    GameObject doorObj = GameObject.Find(doorData.doorName);
                    if (doorObj != null)
                    {
                        doors.Add(doorObj);
                        
                        // Gắn BuildingDoor component nếu chưa có
                        var buildingDoor = doorObj.GetComponent<Presentation.BuildingDoor>();
                        if (buildingDoor == null)
                        {
                            buildingDoor = doorObj.AddComponent<Presentation.BuildingDoor>();
                        }
                        
                        // Set DoorSO vào BuildingDoor (không cần set targetScene/spawnPosition trực tiếp)
                        // BuildingDoor sẽ tự động đọc từ DoorSO
                        #if UNITY_EDITOR
                        // Trong Editor, có thể set doorSO trực tiếp
                        var doorSOField = typeof(Presentation.BuildingDoor).GetField("doorSO", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (doorSOField != null)
                        {
                            doorSOField.SetValue(buildingDoor, doorData);
                        }
                        #endif
                    }
                }
            }
        }

        public void OnSleepClicked()
        {
            if (!buildingSO.hasBed) return;

            // Qua ngày bằng cheat (thay vì SkipDay riêng)
            // Tìm InputActionSO có SkipDay cheat
            var inputManager = TheGreenMemoir.Unity.Input.InputActionManager.Instance;
            if (inputManager != null)
            {
                // Trigger cheat SkipDay (nếu có InputActionSO với actionId "CheatF4_SkipDay")
                inputManager.TriggerById("CheatF4_SkipDay");
            }

            // Load next scene từ SO
            SceneManager.LoadScene(buildingSO.sleepNextScene);
        }

        public void OnRoomClicked(string roomName)
        {
            // Tìm room data trong SO
            foreach (var floor in buildingSO.floors)
            {
                if (floor == null) continue;
                foreach (var roomData in floor.rooms)
                {
                    if (roomData == null) continue;
                    if (roomData.roomName == roomName)
                    {
                        if (roomData.isLocked)
                        {
                            // Hiển thị thông báo đang phát triển
                            if (developingText != null)
                            {
                                developingText.text = roomData.lockedMessage;
                                developingText.gameObject.SetActive(true);
                                Invoke(nameof(HideDevelopingText), 2f);
                            }
                        }
                        return;
                    }
                }
            }
        }

        void HideDevelopingText()
        {
            if (developingText != null)
                developingText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Lấy spawn position từ door name
        /// </summary>
        public Vector3 GetSpawnPosition(string doorName)
        {
            foreach (var doorData in buildingSO.doors)
            {
                if (doorData != null && doorData.doorName == doorName)
                {
                    return doorData.spawnPosition;
                }
            }
            return buildingSO.defaultSpawnPosition;
        }
    }
}

