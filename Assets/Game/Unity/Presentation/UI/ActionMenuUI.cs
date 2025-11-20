using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Enums;
using TheGreenMemoir.Core.Domain.Entities;

namespace TheGreenMemoir.Unity.Presentation.UI
{
    /// <summary>
    /// Menu hiển thị các actions có thể làm với item
    /// Tự động tạo buttons dựa trên item tags
    /// </summary>
    public class ActionMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private GameObject actionButtonPrefab;

        [Header("Settings")]
        [SerializeField] private float buttonSpacing = 5f;

        private List<GameObject> _actionButtons = new List<GameObject>();
        private ItemId _currentItemId;

        /// <summary>
        /// Các loại action có thể làm với item
        /// </summary>
        public enum ItemAction
        {
            Use,        // Sử dụng (Consumable)
            Plant,      // Trồng (Seed)
            Drop,       // Vứt (luôn có)
            Sell,       // Bán (Sellable)
            Gift,       // Tặng (Giftable)
            Craft       // Chế tạo (Craftable)
        }

        private void Awake()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        /// <summary>
        /// Hiển thị action menu cho item
        /// </summary>
        public void ShowActions(ItemId itemId, Vector3 position)
        {
            _currentItemId = itemId;

            var database = TheGreenMemoir.Unity.Managers.GameDatabaseManager.GetDatabase();
            if (database == null)
            {
                Debug.LogError("GameDatabase not found!");
                return;
            }

            var item = database.GetItem(itemId);
            if (item == null)
            {
                Debug.LogWarning($"Item {itemId} not found!");
                return;
            }

            // Lấy danh sách actions có thể làm
            var actions = GetAvailableActions(item);

            if (actions.Count == 0)
            {
                Debug.LogWarning("No actions available for this item");
                return;
            }

            // Hiển thị panel
            if (panel != null)
            {
                panel.SetActive(true);
                // Đặt vị trí panel (có thể điều chỉnh)
                panel.transform.position = position;
            }

            // Tạo buttons
            CreateActionButtons(actions);
        }

        /// <summary>
        /// Lấy danh sách actions có thể làm với item
        /// </summary>
        private List<ItemAction> GetAvailableActions(Item item)
        {
            var actions = new List<ItemAction>();

            // Use - nếu Consumable
            if (item.HasTag(ItemTag.Consumable) || item.HasTag(ItemTag.Edible) || item.HasTag(ItemTag.Drinkable))
            {
                actions.Add(ItemAction.Use);
            }

            // Plant - nếu Seed
            if (item.HasTag(ItemTag.Seed))
            {
                actions.Add(ItemAction.Plant);
            }

            // Sell - nếu Sellable
            if (item.HasTag(ItemTag.Sellable))
            {
                actions.Add(ItemAction.Sell);
            }

            // Gift - nếu Giftable
            if (item.HasTag(ItemTag.Giftable))
            {
                actions.Add(ItemAction.Gift);
            }

            // Craft - nếu Craftable
            if (item.HasTag(ItemTag.Craftable))
            {
                actions.Add(ItemAction.Craft);
            }

            // Drop - luôn có
            actions.Add(ItemAction.Drop);

            return actions;
        }

        /// <summary>
        /// Tạo buttons cho các actions
        /// </summary>
        private void CreateActionButtons(List<ItemAction> actions)
        {
            // Clear old buttons
            ClearButtons();

            float yOffset = 0f;

            foreach (var action in actions)
            {
                GameObject buttonObj;

                if (actionButtonPrefab != null && buttonContainer != null)
                {
                    buttonObj = Instantiate(actionButtonPrefab, buttonContainer);
                }
                else
                {
                    // Tạo đơn giản nếu không có prefab
                    buttonObj = new GameObject($"ActionButton_{action}");
                    if (buttonContainer != null)
                        buttonObj.transform.SetParent(buttonContainer);

                    buttonObj.AddComponent<RectTransform>();
                    var textObj = new GameObject("Text");
                    textObj.transform.SetParent(buttonObj.transform);
                    var text = textObj.AddComponent<TextMeshProUGUI>();
                    text.text = GetActionName(action);
                    text.alignment = TextAlignmentOptions.Center;
                }

                // Setup button
                var actionButton = buttonObj.GetComponent<Button>();
                if (actionButton == null)
                    actionButton = buttonObj.AddComponent<Button>();

                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(() => OnActionSelected(action));

                // Đặt vị trí
                var rectTransform = buttonObj.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(0, -yOffset);
                    yOffset += rectTransform.sizeDelta.y + buttonSpacing;
                }

                _actionButtons.Add(buttonObj);
            }

            // Điều chỉnh kích thước panel theo số lượng buttons
            AdjustPanelSize(actions.Count);
        }

        /// <summary>
        /// Lấy tên hiển thị của action
        /// </summary>
        private string GetActionName(ItemAction action)
        {
            return action switch
            {
                ItemAction.Use => "Sử dụng",
                ItemAction.Plant => "Trồng",
                ItemAction.Drop => "Vứt",
                ItemAction.Sell => "Bán",
                ItemAction.Gift => "Tặng",
                ItemAction.Craft => "Chế tạo",
                _ => action.ToString()
            };
        }

        /// <summary>
        /// Xử lý khi chọn action
        /// </summary>
        private void OnActionSelected(ItemAction action)
        {
            Debug.Log($"Action selected: {action} for item {_currentItemId}");

            // Xử lý từng action
            switch (action)
            {
                case ItemAction.Use:
                    HandleUse();
                    break;

                case ItemAction.Plant:
                    HandlePlant();
                    break;

                case ItemAction.Drop:
                    HandleDrop();
                    break;

                case ItemAction.Sell:
                    HandleSell();
                    break;

                case ItemAction.Gift:
                    HandleGift();
                    break;

                case ItemAction.Craft:
                    HandleCraft();
                    break;
            }

            Hide();
        }

        private void HandleUse()
        {
            // TODO: Implement use item
            Debug.Log("Use item");
        }

        private void HandlePlant()
        {
            // TODO: Switch to plant tool và hiển thị seed selection
            Debug.Log("Plant item");
        }

        private void HandleDrop()
        {
            // TODO: Remove item from inventory
            TheGreenMemoir.Unity.Managers.GameManager.InventoryService.RemoveItem(
                TheGreenMemoir.Core.Domain.ValueObjects.PlayerId.Default,
                _currentItemId,
                1
            );
            Debug.Log("Drop item");
        }

        private void HandleSell()
        {
            // TODO: Open shop UI
            Debug.Log("Sell item");
        }

        private void HandleGift()
        {
            // TODO: Open gift UI
            Debug.Log("Gift item");
        }

        private void HandleCraft()
        {
            // TODO: Open craft UI
            Debug.Log("Craft item");
        }

        /// <summary>
        /// Điều chỉnh kích thước panel theo số lượng buttons
        /// </summary>
        private void AdjustPanelSize(int buttonCount)
        {
            if (panel == null)
                return;

            var rectTransform = panel.GetComponent<RectTransform>();
            if (rectTransform != null && actionButtonPrefab != null)
            {
                var buttonRect = actionButtonPrefab.GetComponent<RectTransform>();
                if (buttonRect != null)
                {
                    float height = (buttonRect.sizeDelta.y + buttonSpacing) * buttonCount;
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
                }
            }
        }

        /// <summary>
        /// Ẩn menu
        /// </summary>
        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);

            ClearButtons();
        }

        /// <summary>
        /// Xóa tất cả buttons
        /// </summary>
        private void ClearButtons()
        {
            foreach (var button in _actionButtons)
            {
                if (button != null)
                    Destroy(button);
            }
            _actionButtons.Clear();
        }
    }
}

