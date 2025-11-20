using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGreenMemoir.Unity.Managers
{
    /// <summary>
    /// Helper để teleport player sau khi load scene
    /// </summary>
    public class TeleportHelper : MonoBehaviour
    {
        private void Start()
        {
            // Kiểm tra có teleport position không
            if (PlayerPrefs.HasKey("TeleportX"))
            {
                float x = PlayerPrefs.GetFloat("TeleportX");
                float y = PlayerPrefs.GetFloat("TeleportY");
                float z = PlayerPrefs.GetFloat("TeleportZ", 0f);
                
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    player.transform.position = new Vector3(x, y, z);
                    Debug.Log($"Teleported player to {x}, {y}, {z}");
                }
                
                // Xóa teleport data
                PlayerPrefs.DeleteKey("TeleportX");
                PlayerPrefs.DeleteKey("TeleportY");
                PlayerPrefs.DeleteKey("TeleportZ");
            }
        }
    }
}

