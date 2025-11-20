using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Item Slot UI Component - Hiển thị item trong inventory
    /// Hỗ trợ hover description và click actions
    /// </summary>
    public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("UI References")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemQuantity;
        [SerializeField] private Image background;
        [SerializeField] private Image border;

        [Header("Colors")]
        [SerializeField] private Color normalColor = new Color(40, 40, 40, 255);
        [SerializeField] private Color hoverColor = new Color(60, 60, 60, 255);
        [SerializeField] private Color selectedColor = new Color(74, 144, 226, 255);

        private ItemDataSO _itemData;
        private int _quantity;
        private bool _isSelected;

        // Events
        public System.Action<ItemDataSO> OnItemSelected;
        public System.Action<ItemDataSO, Vector2> OnItemHover;
        public System.Action<ItemDataSO> OnItemHoverExit;

        /// <summary>
        /// Setup item slot với item data
        /// </summary>
        public void Setup(ItemDataSO itemData, int quantity, System.Action<ItemDataSO> onSelected = null, System.Action<ItemDataSO, Vector2> onHover = null)
        {
            _itemData = itemData;
            _quantity = quantity;
            OnItemSelected = onSelected;
            OnItemHover = onHover;

            UpdateDisplay();
        }

        /// <summary>
        /// Update hiển thị của slot
        /// </summary>
        private void UpdateDisplay()
        {
            if (_itemData == null)
            {
                // Empty slot
                if (itemIcon != null) itemIcon.sprite = null;
                if (itemIcon != null) itemIcon.color = new Color(1, 1, 1, 0);
                if (itemName != null) itemName.text = "";
                if (itemQuantity != null) itemQuantity.text = "";
                return;
            }

            // Set icon
            if (itemIcon != null)
            {
                itemIcon.sprite = _itemData.icon;
                itemIcon.color = Color.white;
            }

            // Set name
            if (itemName != null)
            {
                itemName.text = _itemData.itemName;
            }

            // Set quantity
            if (itemQuantity != null)
            {
                if (_quantity > 1)
                {
                    itemQuantity.text = $"x{_quantity}";
                    itemQuantity.gameObject.SetActive(true);
                }
                else
                {
                    itemQuantity.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Set selected state
        /// </summary>
        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            UpdateBackgroundColor();
        }

        private void UpdateBackgroundColor()
        {
            if (background == null) return;

            if (_isSelected)
            {
                background.color = selectedColor;
            }
            else
            {
                background.color = normalColor;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_itemData == null) return;

            if (background != null)
            {
                background.color = hoverColor;
            }

            if (OnItemHover != null)
            {
                OnItemHover(_itemData, eventData.position);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UpdateBackgroundColor();

            if (OnItemHoverExit != null)
            {
                OnItemHoverExit(_itemData);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_itemData == null) return;

            if (OnItemSelected != null)
            {
                OnItemSelected(_itemData);
            }
        }

        /// <summary>
        /// Get item data
        /// </summary>
        public ItemDataSO GetItemData()
        {
            return _itemData;
        }

        /// <summary>
        /// Get quantity
        /// </summary>
        public int GetQuantity()
        {
            return _quantity;
        }
    }
}

