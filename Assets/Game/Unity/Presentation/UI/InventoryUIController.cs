using UnityEngine;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Unity.Presentation.Views;
using TheGreenMemoir.Core.Application.Events;
using TheGreenMemoir.Core.Domain.ValueObjects;
using TheGreenMemoir.Core.Domain.Enums;

namespace TheGreenMemoir.Unity.Presentation.UI
{
    /// <summary>
    /// Controller để quản lý UI túi đồ
    /// Tự động cập nhật khi có sự kiện thay đổi inventory
    /// </summary>
    public class InventoryUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryView inventoryView;
        [SerializeField] private ItemInfoPanel itemInfoPanel;
        [SerializeField] private InventoryFilterUI filterUI;
        [SerializeField] private ActionMenuUI actionMenuUI;
        [SerializeField] private PlayerId playerId = PlayerId.Default;

        [Header("Settings")]
        [SerializeField] private KeyCode toggleKey = KeyCode.I;

        private bool _isVisible = false;
        private ItemTag _currentFilter = ItemTag.None;

        private void Start()
        {
            // Tự động tìm InventoryView nếu chưa gán
            if (inventoryView == null)
                inventoryView = GetComponentInChildren<InventoryView>();

            // Tự động tìm các components nếu chưa gán
            if (itemInfoPanel == null)
                itemInfoPanel = GetComponentInChildren<ItemInfoPanel>();

            if (filterUI == null)
                filterUI = GetComponentInChildren<InventoryFilterUI>();

            if (actionMenuUI == null)
                actionMenuUI = GetComponentInChildren<ActionMenuUI>();

            // Subscribe filter events
            if (filterUI != null)
            {
                filterUI.OnFilterChanged += OnFilterChanged;
            }

            // Subscribe events
            if (GameManager.EventBus != null)
            {
                GameManager.EventBus.Subscribe<ItemAddedEvent>(OnItemAdded);
                GameManager.EventBus.Subscribe<ItemRemovedEvent>(OnItemRemoved);
            }

            // Ẩn UI ban đầu
            if (inventoryView != null)
                inventoryView.gameObject.SetActive(_isVisible);
            if (itemInfoPanel != null)
                itemInfoPanel.Hide();

            // Refresh UI lần đầu
            RefreshUI();
        }

        private void Update()
        {
            // Toggle UI với phím
            if (UnityEngine.Input.GetKeyDown(toggleKey))
            {
                ToggleUI();
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe events
            if (GameManager.EventBus != null)
            {
                GameManager.EventBus.Unsubscribe<ItemAddedEvent>(OnItemAdded);
                GameManager.EventBus.Unsubscribe<ItemRemovedEvent>(OnItemRemoved);
            }
        }

        /// <summary>
        /// Hiển thị/ẩn UI
        /// </summary>
        public void ToggleUI()
        {
            _isVisible = !_isVisible;
            if (inventoryView != null)
                inventoryView.gameObject.SetActive(_isVisible);

            if (_isVisible)
                RefreshUI();
        }

        /// <summary>
        /// Hiển thị UI
        /// </summary>
        public void ShowUI()
        {
            _isVisible = true;
            if (inventoryView != null)
            {
                inventoryView.gameObject.SetActive(true);
                RefreshUI();
            }
        }

        /// <summary>
        /// Ẩn UI
        /// </summary>
        public void HideUI()
        {
            _isVisible = false;
            if (inventoryView != null)
                inventoryView.gameObject.SetActive(false);
        }

        /// <summary>
        /// Làm mới UI
        /// </summary>
        private void RefreshUI()
        {
            if (inventoryView == null || GameManager.InventoryService == null)
                return;

            var inventory = GameManager.InventoryService.GetInventory(playerId);
            if (inventory != null)
            {
                // Apply filter nếu có
                if (_currentFilter != ItemTag.None)
                {
                    inventoryView.DisplayFilteredInventory(inventory, _currentFilter);
                }
                else
                {
                    inventoryView.DisplayInventory(inventory);
                }
            }
        }

        /// <summary>
        /// Xử lý khi filter thay đổi
        /// </summary>
        private void OnFilterChanged(ItemTag filterTag)
        {
            _currentFilter = filterTag;
            RefreshUI();
        }

        /// <summary>
        /// Hiển thị thông tin item khi click vào slot
        /// </summary>
        public void OnSlotClicked(ItemId itemId, Vector3 position)
        {
            // Hiển thị info panel
            if (itemInfoPanel != null)
            {
                itemInfoPanel.ShowItem(itemId);
            }

            // Hiển thị action menu
            if (actionMenuUI != null)
            {
                actionMenuUI.ShowActions(itemId, position);
            }
        }

        /// <summary>
        /// Event handler khi có vật phẩm được thêm
        /// </summary>
        private void OnItemAdded(ItemAddedEvent evt)
        {
            if (evt.PlayerId == playerId)
            {
                RefreshUI();
            }
        }

        /// <summary>
        /// Event handler khi có vật phẩm bị xóa
        /// </summary>
        private void OnItemRemoved(ItemRemovedEvent evt)
        {
            if (evt.PlayerId == playerId)
            {
                RefreshUI();
            }
        }
    }
}

