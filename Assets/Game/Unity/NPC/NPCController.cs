using UnityEngine;
using System.Collections.Generic;
using TheGreenMemoir.Unity.Data;
using TheGreenMemoir.Unity.NPC;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.NPC
{
    /// <summary>
    /// NPC Controller - Quản lý NPC với inventory và trading
    /// Flexible: Không lỗi nếu thiếu components
    /// </summary>
    public class NPCController : MonoBehaviour
    {
        [Header("NPC Data")]
        [SerializeField] private NPCDefinitionSO npcDefinition;
        [SerializeField] private NPCFriendshipSO friendshipData;

        [Header("NPC Settings")]
        [SerializeField] private float interactionDistance = 2f;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private bool canTrade = true;
        [SerializeField] private bool canTalk = true;

        // Public properties để check từ bên ngoài
        public bool CanTrade => canTrade;
        public bool CanTalk => canTalk;

        [Header("Inventory")]
        [SerializeField] private int maxInventorySize = 20;
        [SerializeField] private List<NPCTradeItem> inventory = new List<NPCTradeItem>();
        [SerializeField] private int money = 1000;

        [Header("Auto Refill")]
        [SerializeField] private bool autoRefillInventory = true;
        [SerializeField] private float refillInterval = 300f; // 5 minutes
        [SerializeField] private int refillAmount = 5;
        [SerializeField] private List<ItemDataSO> refillItems = new List<ItemDataSO>();

        [Header("UI (Optional)")]
        [SerializeField] private GameObject interactionPrompt;
        [SerializeField] private GameObject shopUI;

        private float lastRefillTime = 0f;
        private bool playerInRange = false;
        private GameObject player;

        [System.Serializable]
        public class NPCTradeItem
        {
            public ItemDataSO itemData;
            public int quantity;
            public int buyPrice; // Giá NPC mua từ player
            public int sellPrice; // Giá NPC bán cho player

            public NPCTradeItem(ItemDataSO item, int qty, int buy, int sell)
            {
                itemData = item;
                quantity = qty;
                buyPrice = buy;
                sellPrice = sell;
            }
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if (npcDefinition == null)
            {
                Debug.LogWarning($"NPCController on {gameObject.name} has no NPCDefinitionSO!");
                return;
            }

            // Initialize inventory với random items
            InitializeInventory();

            // Setup UI
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);

            if (shopUI != null)
                shopUI.SetActive(false);
        }

        private void Update()
        {
            if (player == null) return;

            float distance = Vector2.Distance(transform.position, player.transform.position);
            playerInRange = distance <= interactionDistance;

            if (playerInRange)
            {
                if (interactionPrompt != null && !interactionPrompt.activeSelf)
                    interactionPrompt.SetActive(true);

                if (UnityEngine.Input.GetKeyDown(interactKey))
                {
                    OnInteract();
                }
            }
            else
            {
                if (interactionPrompt != null && interactionPrompt.activeSelf)
                    interactionPrompt.SetActive(false);
            }

            // Auto refill inventory
            if (autoRefillInventory && Time.time - lastRefillTime >= refillInterval)
            {
                RefillInventory();
                lastRefillTime = Time.time;
            }
        }

        /// <summary>
        /// Initialize inventory với random items
        /// </summary>
        private void InitializeInventory()
        {
            if (npcDefinition == null) return;

            inventory.Clear();

            // Add random items
            var database = Managers.GameDatabaseManager.GetDatabase();
            if (database != null && database.items != null && database.items.Count > 0)
            {
                int itemCount = Random.Range(3, Mathf.Min(8, database.items.Count)); // 3-7 items
                var availableItems = new List<ItemDataSO>(database.items);

                for (int i = 0; i < itemCount && availableItems.Count > 0; i++)
                {
                    int randomIndex = Random.Range(0, availableItems.Count);
                    var item = availableItems[randomIndex];
                    availableItems.RemoveAt(randomIndex);

                    int quantity = Random.Range(1, 10);
                    int buyPrice = CalculateBuyPrice(item);
                    int sellPrice = CalculateSellPrice(item);

                    inventory.Add(new NPCTradeItem(item, quantity, buyPrice, sellPrice));
                }
            }

            // Initialize money
            money = Random.Range(500, 2000);
        }

        /// <summary>
        /// Refill inventory tự động
        /// </summary>
        private void RefillInventory()
        {
            if (refillItems == null || refillItems.Count == 0) return;

            foreach (var itemData in refillItems)
            {
                if (itemData == null) continue;

                // Tìm item trong inventory
                var existingItem = inventory.Find(i => i.itemData == itemData);
                if (existingItem != null)
                {
                    // Thêm số lượng
                    existingItem.quantity += refillAmount;
                }
                else if (inventory.Count < maxInventorySize)
                {
                    // Thêm item mới
                    int buyPrice = CalculateBuyPrice(itemData);
                    int sellPrice = CalculateSellPrice(itemData);
                    inventory.Add(new NPCTradeItem(itemData, refillAmount, buyPrice, sellPrice));
                }
            }

            // Refill money
            money += Random.Range(100, 500);
        }

        private int CalculateBuyPrice(ItemDataSO item)
        {
            if (item == null) return 0;
            int basePrice = item.sellPrice;
            if (npcDefinition != null && npcDefinition.isShop)
            {
                basePrice = Mathf.RoundToInt(basePrice * (npcDefinition.baseBuyMultiplier / 100f));
            }
            return Mathf.Max(1, basePrice); // Tối thiểu 1
        }

        private int CalculateSellPrice(ItemDataSO item)
        {
            if (item == null) return 0;
            int basePrice = item.buyPrice;
            if (npcDefinition != null && npcDefinition.isShop)
            {
                basePrice = Mathf.RoundToInt(basePrice * (npcDefinition.baseSellMultiplier / 100f));
            }
            return Mathf.Max(1, basePrice); // Tối thiểu 1
        }

        private void OnInteract()
        {
            if (canTrade && npcDefinition != null && npcDefinition.isShop)
            {
                OpenShop();
            }
            else if (canTalk)
            {
                StartDialogue();
            }
        }

        private void OpenShop()
        {
            if (shopUI != null)
            {
                shopUI.SetActive(true);
                var shopController = shopUI.GetComponent<Presentation.UI.NPCShopUI>();
                if (shopController != null)
                {
                    // Setup shop với NPC inventory và money
                    // TODO: Implement Setup method trong NPCShopUI
                }
            }
        }

        private void StartDialogue()
        {
            // TODO: Implement dialogue system
            Debug.Log($"Talking to {npcDefinition?.displayName ?? "NPC"}");
        }

        /// <summary>
        /// Buy item from player (NPC mua từ player) - Giới hạn bởi số tiền NPC có
        /// </summary>
        public bool BuyFromPlayer(ItemDataSO item, int quantity, out int totalPrice, out int actualQuantity)
        {
            totalPrice = 0;
            actualQuantity = 0;

            if (item == null || quantity <= 0) return false;

            var tradeItem = inventory.Find(i => i.itemData == item);
            if (tradeItem == null)
            {
                // Tạo item mới
                int buyPrice = CalculateBuyPrice(item);
                tradeItem = new NPCTradeItem(item, 0, buyPrice, 0);
                inventory.Add(tradeItem);
            }

            totalPrice = tradeItem.buyPrice * quantity;

            // Kiểm tra NPC có đủ tiền không - CHỈ MUA ĐƯỢC VỚI SỐ TIỀN CÓ
            if (money < totalPrice)
            {
                // Tính lại quantity dựa trên số tiền có
                actualQuantity = money / tradeItem.buyPrice;
                if (actualQuantity <= 0) return false;
                totalPrice = tradeItem.buyPrice * actualQuantity;
            }
            else
            {
                actualQuantity = quantity;
            }

            // Thực hiện giao dịch
            tradeItem.quantity += actualQuantity;
            money -= totalPrice;

            return true;
        }

        /// <summary>
        /// Sell item to player (NPC bán cho player)
        /// </summary>
        public bool SellToPlayer(ItemDataSO item, int quantity, out int totalPrice)
        {
            totalPrice = 0;
            if (item == null || quantity <= 0) return false;

            var tradeItem = inventory.Find(i => i.itemData == item);
            if (tradeItem == null || tradeItem.quantity < quantity)
            {
                return false; // NPC không có đủ
            }

            totalPrice = tradeItem.sellPrice * quantity;
            tradeItem.quantity -= quantity;
            money += totalPrice;

            if (tradeItem.quantity <= 0)
            {
                inventory.Remove(tradeItem);
            }

            return true;
        }

        /// <summary>
        /// Get NPC money
        /// </summary>
        public int GetMoney()
        {
            return money;
        }

        /// <summary>
        /// Get NPC inventory
        /// </summary>
        public List<NPCTradeItem> GetInventory()
        {
            return new List<NPCTradeItem>(inventory);
        }

        /// <summary>
        /// Set NPC money (dùng cho load game)
        /// </summary>
        public void SetMoney(int newMoney)
        {
            money = Mathf.Max(0, newMoney);
        }

        /// <summary>
        /// Set NPC inventory (dùng cho load game)
        /// </summary>
        public void SetInventory(List<NPCTradeItem> newInventory)
        {
            if (newInventory != null)
            {
                inventory = new List<NPCTradeItem>(newInventory);
            }
        }

        /// <summary>
        /// Get NPC ID (dùng cho save/load)
        /// </summary>
        public string GetNPCId()
        {
            if (npcDefinition != null && !string.IsNullOrEmpty(npcDefinition.npcId))
            {
                return npcDefinition.npcId;
            }
            return gameObject.name; // Fallback: dùng GameObject name
        }
    }
}

