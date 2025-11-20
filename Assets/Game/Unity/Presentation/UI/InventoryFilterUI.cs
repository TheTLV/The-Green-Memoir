using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheGreenMemoir.Core.Domain.Enums;

namespace TheGreenMemoir.Unity.Presentation.UI
{
    /// <summary>
    /// UI để chọn tag filter cho inventory
    /// </summary>
    public class InventoryFilterUI : MonoBehaviour
    {
        [Header("Filter Buttons")]
        [SerializeField] private Button allItemsButton;
        [SerializeField] private Button seedsButton;
        [SerializeField] private Button toolsButton;
        [SerializeField] private Button questItemsButton;
        [SerializeField] private Button foodButton;
        [SerializeField] private Button materialsButton;

        [Header("Settings")]
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private Color normalColor = Color.white;

        private ItemTag _currentFilter = ItemTag.None;
        private Button _selectedButton;

        public System.Action<ItemTag> OnFilterChanged;

        private void Start()
        {
            // Setup buttons
            if (allItemsButton != null)
                allItemsButton.onClick.AddListener(() => SetFilter(ItemTag.None, allItemsButton));

            if (seedsButton != null)
                seedsButton.onClick.AddListener(() => SetFilter(ItemTag.Seed, seedsButton));

            if (toolsButton != null)
                toolsButton.onClick.AddListener(() => SetFilter(ItemTag.Tool, toolsButton));

            if (questItemsButton != null)
                questItemsButton.onClick.AddListener(() => SetFilter(ItemTag.QuestItem, questItemsButton));

            if (foodButton != null)
                foodButton.onClick.AddListener(() => SetFilter(ItemTag.Food, foodButton));

            if (materialsButton != null)
                materialsButton.onClick.AddListener(() => SetFilter(ItemTag.Material, materialsButton));

            // Mặc định chọn "All Items"
            SetFilter(ItemTag.None, allItemsButton);
        }

        private void SetFilter(ItemTag filter, Button button)
        {
            _currentFilter = filter;

            // Update button colors
            if (_selectedButton != null)
            {
                UpdateButtonColor(_selectedButton, normalColor);
            }

            _selectedButton = button;
            if (_selectedButton != null)
            {
                UpdateButtonColor(_selectedButton, selectedColor);
            }

            // Notify
            OnFilterChanged?.Invoke(filter);
        }

        private void UpdateButtonColor(Button button, Color color)
        {
            var colors = button.colors;
            colors.normalColor = color;
            button.colors = colors;
        }

        public ItemTag GetCurrentFilter()
        {
            return _currentFilter;
        }
    }
}

