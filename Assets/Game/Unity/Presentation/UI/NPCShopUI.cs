using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.Presentation.UI
{
    /// <summary>
    /// UI để mua/bán với NPC
    /// </summary>
    public class NPCShopUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform buyItemContainer;
        [SerializeField] private Transform sellItemContainer;
        [SerializeField] private GameObject shopItemPrefab;
        [SerializeField] private Button buyTabButton;
        [SerializeField] private Button sellTabButton;
        [SerializeField] private TextMeshProUGUI playerMoneyText;
        [SerializeField] private TextMeshProUGUI messageText; // For temporary display

        [Header("Settings")]
        [SerializeField] private NPCId npcId = NPCId.Default;
        [SerializeField] private PlayerId playerId = PlayerId.Default;

        private bool _isBuyTab = true;
        private List<GameObject> _shopItems = new List<GameObject>();

        private void Awake()
        {
            if (panel != null)
                panel.SetActive(false);

            // Setup tab buttons
            if (buyTabButton != null)
                buyTabButton.onClick.AddListener(() => SwitchToBuyTab());

            if (sellTabButton != null)
                sellTabButton.onClick.AddListener(() => SwitchToSellTab());
        }

        /// <summary>
        /// Hiển thị shop UI
        /// </summary>
        public void ShowShop(NPCId npc)
        {
            npcId = npc;
            
            if (panel != null)
                panel.SetActive(true);

            RefreshUI();
        }

        /// <summary>
        /// Ẩn shop UI
        /// </summary>
        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);

            ClearShopItems();
        }

        /// <summary>
        /// Chuyển sang tab Buy
        /// </summary>
        private void SwitchToBuyTab()
        {
            _isBuyTab = true;
            RefreshUI();
        }

        /// <summary>
        /// Chuyển sang tab Sell
        /// </summary>
        private void SwitchToSellTab()
        {
            _isBuyTab = false;
            RefreshUI();
        }

        /// <summary>
        /// Làm mới UI
        /// </summary>
        private void RefreshUI()
        {
            // Update money display
            UpdateMoneyDisplay();

            // Clear old items
            ClearShopItems();

            if (_isBuyTab)
            {
                DisplayBuyItems();
            }
            else
            {
                DisplaySellItems();
            }
        }

        /// <summary>
        /// Hiển thị items có thể mua
        /// </summary>
        private void DisplayBuyItems()
        {
            // TODO: Lấy items từ NPC inventory
            // Tạm thời hiển thị message
            // if (messageText != null)
            //     messageText.text = "NPC Shop - Buy Tab\n(TODO: Load NPC items)";
        }

        /// <summary>
        /// Hiển thị items có thể bán (từ player inventory)
        /// </summary>
        private void DisplaySellItems()
        {
            var inventory = GameManager.InventoryService.GetInventory(playerId);
            if (inventory == null)
                return;

            var database = GameDatabaseManager.GetDatabase();
            if (database == null)
                return;

            // Lọc items có thể bán
            var sellableItems = inventory.GetAllSlots()
                .Where(slot => !slot.IsEmpty && slot.Item.HasTag(Core.Domain.Enums.ItemTag.Sellable))
                .Select(slot => slot.Item)
                .Distinct()
                .ToList();

            if (sellableItems.Count == 0)
            {
                // Hiển thị message không có gì để bán
                return;
            }

            // Tạo UI items
            foreach (var item in sellableItems)
            {
                var itemData = database.GetItemData(item.Id);
                if (itemData == null)
                    continue;

                int quantity = inventory.GetQuantity(item.Id);
                CreateSellItem(item, itemData, quantity);
            }
        }

        /// <summary>
        /// Tạo UI item để bán
        /// </summary>
        private void CreateSellItem(Item item, ItemDataSO itemData, int quantity)
        {
            GameObject itemObj;

            if (shopItemPrefab != null && sellItemContainer != null)
            {
                itemObj = Instantiate(shopItemPrefab, sellItemContainer);
            }
            else
            {
                itemObj = new GameObject($"SellItem_{item.Id}");
                if (sellItemContainer != null)
                    itemObj.transform.SetParent(sellItemContainer);
            }

            // Setup button
            var button = itemObj.GetComponent<Button>();
            if (button == null)
                button = itemObj.AddComponent<Button>();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnSellItem(item.Id, 1));

            // Setup text (có thể thêm icon, giá, v.v.)
            var text = itemObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"{item.Name} x{quantity} - {itemData.sellPrice}G";
            }

            _shopItems.Add(itemObj);
        }

        /// <summary>
        /// Xử lý khi bán item
        /// </summary>
        private void OnSellItem(ItemId itemId, int quantity)
        {
            var result = GameManager.ShopService.SellItem(npcId, itemId, quantity, playerId);
            
            if (result.IsSuccess)
            {
                Debug.Log($"Sold {quantity} of {itemId} for {result.MoneyReceived}G");
                RefreshUI(); // Refresh để cập nhật inventory và money
            }
            else
            {
                Debug.LogWarning($"Failed to sell item: {result.ErrorMessage}");
                if (messageText != null)
                    messageText.text = result.ErrorMessage;
            }
        }

        /// <summary>
        /// Cập nhật hiển thị tiền
        /// </summary>
        private void UpdateMoneyDisplay()
        {
            if (playerMoneyText == null)
                return;

            var player = GameManager.PlayerRepository.GetPlayer(playerId);
            if (player != null)
            {
                playerMoneyText.text = $"Money: {player.Money}G";
            }
        }

        /// <summary>
        /// Xóa tất cả shop items
        /// </summary>
        private void ClearShopItems()
        {
            foreach (var item in _shopItems)
            {
                if (item != null)
                    Destroy(item);
            }
            _shopItems.Clear();
        }

        // [SerializeField] private TextMeshProUGUI messageText; // For temporary display
    }
}

