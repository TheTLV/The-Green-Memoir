using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGreenMemoir.Unity.Presentation
{
    /// <summary>
    /// House Door - Cửa vào nhà (DEPRECATED: Dùng BuildingDoor thay thế)
    /// </summary>
    [System.Obsolete("Use BuildingDoor instead")]
    public class HouseDoor : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Tên scene nhà")]
        public string houseSceneName = "House";
        
        [Tooltip("Phím để vào nhà")]
        public KeyCode interactKey = KeyCode.E;
        
        [Tooltip("Khoảng cách trigger")]
        public float triggerDistance = 2f;

        private Transform player;

        void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        void Update()
        {
            if (player == null) return;

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= triggerDistance && UnityEngine.Input.GetKeyDown(interactKey))
            {
                SceneManager.LoadScene(houseSceneName);
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && UnityEngine.Input.GetKeyDown(interactKey))
            {
                SceneManager.LoadScene(houseSceneName);
            }
        }
    }
}

