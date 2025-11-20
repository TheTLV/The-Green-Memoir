using UnityEngine;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.NPC
{
    /// <summary>
    /// Quest Item Giver - Item tặng cho tutorial (thay vì NPC)
    /// Cấu hình qua Inspector, không cần code
    /// </summary>
    public class QuestItemGiver : MonoBehaviour
    {
        [Header("Quest Item Settings")]
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
        public string customMessage = "Bạn đã nhận được {0} {1} từ quest item!";

        [Header("Visual (Optional)")]
        [Tooltip("Sprite để hiển thị (nếu không có thì dùng default)")]
        public Sprite itemSprite;
        
        [Tooltip("Có hiệu ứng khi nhận không")]
        public bool hasEffect = true;

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
                GiveQuestItem();
            }
        }

        void GiveQuestItem()
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
                
                // TODO: Hiển thị UI notification nếu có
            }
        }

        void OnDrawGizmosSelected()
        {
            // Vẽ trigger distance trong editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, triggerDistance);
        }
    }
}

