using UnityEngine;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.NPC
{
    /// <summary>
    /// NPC Gift Giver - NPC tặng item khi player đến gần
    /// Cấu hình qua Inspector, không cần code
    /// </summary>
    public class NPCGiftGiver : MonoBehaviour
    {
        [Header("Gift Settings")]
        [Tooltip("Item ID để tặng (vd: seed_corn)")]
        public string itemId = "seed_corn";
        
        [Tooltip("Số lượng tặng")]
        public int quantity = 5;
        
        [Tooltip("Chỉ tặng 1 lần (sau đó không tặng nữa)")]
        public bool giveOnce = true;
        
        [Tooltip("Khoảng cách trigger (units)")]
        public float triggerDistance = 2f;

        [Header("UI")]
        [Tooltip("Hiển thị thông báo khi tặng")]
        public bool showMessage = true;
        
        [Tooltip("Thông báo tùy chỉnh")]
        public string customMessage = "NPC đã tặng bạn {0} {1}!";

        private bool hasGiven = false;
        private Transform player;

        void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        void Update()
        {
            if (hasGiven && giveOnce) return;
            if (player == null) return;

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= triggerDistance)
            {
                GiveGift();
            }
        }

        void GiveGift()
        {
            if (hasGiven && giveOnce) return;

            if (GameManager.InventoryService != null)
            {
                var itemIdVO = new ItemId(itemId);
                // Dùng AddItemById thay vì AddItem (vì AddItem cần Item entity, không phải ItemId)
                GameManager.InventoryService.AddItemById(PlayerId.Default, itemIdVO, quantity);
                
                hasGiven = true;
                
                string message = customMessage.Replace("{0}", quantity.ToString())
                                              .Replace("{1}", itemId);
                Debug.Log(message);
            }
        }
    }
}

