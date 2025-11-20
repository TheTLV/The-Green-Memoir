using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Enums;
using TheGreenMemoir.Core.Application.Commands;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.Presentation.UI
{
    /// <summary>
    /// UI để chọn hạt giống khi trồng cây
    /// Tự động lọc seeds từ inventory và hiển thị
    /// </summary>
    public class SeedSelectionUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform seedContainer;
        [SerializeField] private GameObject seedItemPrefab;
        [SerializeField] private TextMeshProUGUI messageText;

        [Header("Settings")]
        [SerializeField] private PlayerId playerId = PlayerId.Default;

        private TilePosition _targetTile;
        private List<GameObject> _seedItems = new List<GameObject>();

        private void Awake()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        /// <summary>
        /// Hiển thị UI chọn hạt giống
        /// </summary>
        public void ShowSeedSelection(TilePosition tilePos)
        {
            _targetTile = tilePos;

            // Lấy inventory
            var inventory = GameManager.InventoryService.GetInventory(playerId);
            if (inventory == null)
            {
                ShowMessage("Inventory not found!");
                return;
            }

            // Lọc seeds từ inventory
            var seeds = InventoryFilter.GetItemsByTag(inventory, ItemTag.Seed).ToList();

            if (seeds.Count == 0)
            {
                ShowMessage("Không có hạt nào để gieo");
                return;
            }

            // Hiển thị panel
            if (panel != null)
                panel.SetActive(true);

            if (messageText != null)
                messageText.text = "Chọn hạt giống:";

            // Hiển thị danh sách seeds
            DisplaySeeds(seeds);
        }

        /// <summary>
        /// Hiển thị danh sách seeds
        /// </summary>
        private void DisplaySeeds(IEnumerable<Core.Domain.Entities.Item> seeds)
        {
            // Clear old items
            ClearSeedItems();

            var database = GameDatabaseManager.GetDatabase();
            if (database == null)
            {
                Debug.LogError("GameDatabase not found!");
                return;
            }

            foreach (var seed in seeds)
            {
                var seedData = database.GetItemData(seed.Id);
                if (seedData == null)
                    continue;

                // Lấy số lượng seed trong inventory
                var inventory = GameManager.InventoryService.GetInventory(playerId);
                int quantity = inventory.GetQuantity(seed.Id);

                // Tạo UI item
                CreateSeedItem(seed, seedData, quantity);
            }
        }

        /// <summary>
        /// Tạo UI item cho một seed
        /// </summary>
        private void CreateSeedItem(Core.Domain.Entities.Item seed, ItemDataSO seedData, int quantity)
        {
            GameObject itemObj;
            
            if (seedItemPrefab != null && seedContainer != null)
            {
                itemObj = Instantiate(seedItemPrefab, seedContainer);
            }
            else
            {
                // Tạo đơn giản nếu không có prefab
                itemObj = new GameObject($"SeedItem_{seed.Id}");
                if (seedContainer != null)
                    itemObj.transform.SetParent(seedContainer);

                // Thêm Image và Text
                var image = itemObj.AddComponent<Image>();
                var textObj = new GameObject("Text");
                textObj.transform.SetParent(itemObj.transform);
                var text = textObj.AddComponent<TextMeshProUGUI>();
                text.text = $"{seed.Name} x{quantity}";
            }

            // Setup button
            var button = itemObj.GetComponent<Button>();
            if (button == null)
                button = itemObj.AddComponent<Button>();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnSeedSelected(seed.Id));

            // Setup icon
            var iconImage = itemObj.GetComponentInChildren<Image>();
            if (iconImage != null && seedData.icon != null)
            {
                iconImage.sprite = seedData.icon;
            }

            _seedItems.Add(itemObj);
        }

        /// <summary>
        /// Xử lý khi chọn seed
        /// </summary>
        private void OnSeedSelected(ItemId seedId)
        {
            var database = GameDatabaseManager.GetDatabase();
            if (database == null)
            {
                Debug.LogError("GameDatabase not found!");
                return;
            }

            // Lấy CropData từ seed
            var cropData = database.GetCropDataFromSeed(seedId);
            if (cropData == null)
            {
                Debug.LogWarning($"CropData not found for seed {seedId}");
                Hide();
                return;
            }

            // Kiểm tra còn seed trong inventory không
            var inventory = GameManager.InventoryService.GetInventory(playerId);
            if (!inventory.HasItem(seedId, 1))
            {
                ShowMessage("Không còn hạt giống này!");
                return;
            }

            // Trừ seed khỏi inventory
            GameManager.InventoryService.RemoveItem(playerId, seedId, 1);

            // Trồng cây
            var command = new PlantSeedByIdCommand(
                GameManager.FarmingService,
                _targetTile,
                new CropId(cropData.cropId),
                playerId
            );

            var result = GameManager.CommandInvoker.ExecuteCommand(command);

            if (result.IsSuccess)
            {
                Hide();
            }
            else
            {
                // Trả lại seed nếu trồng thất bại
                GameManager.InventoryService.AddItemById(playerId, seedId, 1);
                ShowMessage($"Trồng thất bại: {result.ErrorMessage}");
            }
        }

        /// <summary>
        /// Hiển thị message
        /// </summary>
        private void ShowMessage(string message)
        {
            if (messageText != null)
                messageText.text = message;

            if (panel != null)
                panel.SetActive(true);
        }

        /// <summary>
        /// Ẩn UI
        /// </summary>
        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);

            ClearSeedItems();
        }

        /// <summary>
        /// Xóa tất cả seed items
        /// </summary>
        private void ClearSeedItems()
        {
            foreach (var item in _seedItems)
            {
                if (item != null)
                    Destroy(item);
            }
            _seedItems.Clear();
        }
    }
}

