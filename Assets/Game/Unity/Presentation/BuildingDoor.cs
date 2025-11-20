using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Unity.Managers;

namespace TheGreenMemoir.Unity.Presentation
{
    /// <summary>
    /// Building Door - Cửa vào tòa nhà (dùng DoorSO, không hardcode)
    /// Hỗ trợ trigger zone (không cần sprite cửa) - có thể tự động chuyển hoặc nhấn E
    /// </summary>
    public class BuildingDoor : MonoBehaviour
    {
        [Header("Door SO (Dùng SO thay vì hardcode)")]
        [Tooltip("Kéo DoorSO vào đây, hoặc để trống để dùng doorId")]
        [SerializeField] private DoorSO doorSO;
        
        [Tooltip("Door ID (nếu không có doorSO, sẽ tìm từ GameDatabase)")]
        [SerializeField] private string doorId = "";

        [Header("UI (Optional)")]
        [Tooltip("Text hiển thị prompt (ví dụ: 'Nhấn E để vào') - Optional")]
        [SerializeField] private TextMeshProUGUI promptText;
        
        [Tooltip("GameObject chứa prompt UI (sẽ hiện/ẩn) - Optional")]
        [SerializeField] private GameObject promptPanel;

        private Transform player;
        private DoorSO currentDoor;
        private bool playerInTrigger = false;
        private bool isTransitioning = false;
        private CameraController cameraController;

        void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;

            // Tìm CameraController (optional)
            cameraController = FindFirstObjectByType<CameraController>();

            // Load DoorSO
            if (doorSO == null && !string.IsNullOrWhiteSpace(doorId))
            {
                var database = GameDatabaseManager.GetDatabase();
                if (database != null)
                {
                    // Tìm DoorSO từ BuildingSO
                    foreach (var building in database.buildings)
                    {
                        if (building != null)
                        {
                            doorSO = building.GetDoor(doorId);
                            if (doorSO != null) break;
                        }
                    }
                }
            }

            currentDoor = doorSO;
            
            if (currentDoor == null)
            {
                Debug.LogWarning($"BuildingDoor: DoorSO not found for doorId: {doorId}");
            }
            else
            {
                // Đảm bảo có Collider2D trigger nếu cần
                Collider2D existingCol = GetComponent<Collider2D>();
                if (existingCol == null)
                {
                    // Tự động tạo trigger collider nếu chưa có
                    BoxCollider2D boxCol = gameObject.AddComponent<BoxCollider2D>();
                    boxCol.isTrigger = true;
                    boxCol.size = new Vector2(2f, 2f); // Kích thước mặc định
                    Debug.Log($"BuildingDoor: Auto-created trigger collider for {currentDoor.doorName}");
                }
                else if (!existingCol.isTrigger)
                {
                    // Nếu có collider nhưng chưa set trigger, set trigger
                    existingCol.isTrigger = true;
                    Debug.Log($"BuildingDoor: Set existing collider as trigger for {currentDoor.doorName}");
                }
            }

            // Ẩn prompt ban đầu
            ShowPrompt(false);
        }

        void Update()
        {
            if (player == null || currentDoor == null || !currentDoor.isEnabled || currentDoor.isLocked) return;

            // Nếu không dùng trigger zone, dùng distance-based
            if (!HasTriggerCollider())
            {
                float distance = Vector3.Distance(transform.position, player.position);
                if (distance <= currentDoor.triggerDistance)
                {
                    if (currentDoor.autoTransition)
                    {
                        HandleDoorInteraction();
                    }
                    else if (UnityEngine.Input.GetKeyDown(currentDoor.interactKey))
                    {
                        HandleDoorInteraction();
                    }
                }
                return;
            }

            // Nếu dùng trigger zone, xử lý trong OnTriggerEnter2D/OnTriggerStay2D
            if (playerInTrigger && !isTransitioning)
            {
                if (!currentDoor.autoTransition && UnityEngine.Input.GetKeyDown(currentDoor.interactKey))
                {
                    HandleDoorInteraction();
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && currentDoor != null && currentDoor.isEnabled && !currentDoor.isLocked && !isTransitioning)
            {
                playerInTrigger = true;
                ShowPrompt(true);
                
                // Nếu auto transition, chuyển ngay lập tức
                if (currentDoor.autoTransition)
                {
                    HandleDoorInteraction();
                }
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && currentDoor != null && currentDoor.isEnabled && !currentDoor.isLocked && !isTransitioning)
            {
                playerInTrigger = true;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInTrigger = false;
                ShowPrompt(false);
            }
        }

        private bool HasTriggerCollider()
        {
            Collider2D col = GetComponent<Collider2D>();
            return col != null && col.isTrigger;
        }

        private void ShowPrompt(bool show)
        {
            if (currentDoor == null || !currentDoor.showPrompt) return;

            if (promptPanel != null)
            {
                promptPanel.SetActive(show);
            }

            if (promptText != null)
            {
                promptText.gameObject.SetActive(show);
                if (show && !currentDoor.autoTransition)
                {
                    promptText.text = $"Nhấn {currentDoor.interactKey} để {currentDoor.doorName}";
                }
                else if (show && currentDoor.autoTransition)
                {
                    promptText.text = $"Đang vào {currentDoor.doorName}...";
                }
            }
        }

        private void HandleDoorInteraction()
        {
            if (isTransitioning) return; // Tránh gọi nhiều lần
            isTransitioning = true;
            
            // Ẩn prompt
            ShowPrompt(false);

            // Trigger camera delay nếu có
            if (cameraController != null)
            {
                cameraController.TriggerMapTransitionDelay();
            }
            
            // Lưu spawn position
            if (currentDoor.spawnPosition != Vector3.zero)
            {
                PlayerPrefs.SetFloat("TeleportX", currentDoor.spawnPosition.x);
                PlayerPrefs.SetFloat("TeleportY", currentDoor.spawnPosition.y);
                PlayerPrefs.SetFloat("TeleportZ", currentDoor.spawnPosition.z);
            }

            // Load scene từ DoorSO (không hardcode)
            string targetScene = currentDoor.targetSceneName;
            if (string.IsNullOrWhiteSpace(targetScene))
            {
                // Fallback: tìm từ BuildingSO
                var database = GameDatabaseManager.GetDatabase();
                if (database != null)
                {
                    foreach (var building in database.buildings)
                    {
                        if (building != null)
                        {
                            var door = building.GetDoor(currentDoor.doorId);
                            if (door != null)
                            {
                                targetScene = building.buildingScene;
                                break;
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(targetScene))
            {
                var sceneLoader = SceneLoader.Instance;
                if (sceneLoader != null)
                {
                    sceneLoader.LoadScene(targetScene);
                }
                else
                {
                    SceneManager.LoadScene(targetScene);
                }
            }
            else
            {
                Debug.LogWarning($"BuildingDoor: No target scene found for door {currentDoor.doorId}");
            }
        }
    }
}

