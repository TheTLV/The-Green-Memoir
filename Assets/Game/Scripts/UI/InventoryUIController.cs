using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class InventoryUIController : MonoBehaviour
{
    // Static reference for easy access to open/close
    public static InventoryUIController Instance;

    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel; // The main parent panel
    [SerializeField] private Transform contentContainer; // Parent for all slot UIs (has Vertical Layout Group)
    [SerializeField] private GameObject itemSlotPrefab; // Prefab for an item row

    [Header("Context Menu References")]
    [SerializeField] private GameObject contextMenuPanel;
    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private List<GameObject> activeSlotUIs = new List<GameObject>();
    private ItemData selectedItemData; // The item currently selected/clicked

    // --- INITIALIZATION ---

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Hide UI on start
        inventoryPanel.SetActive(false);
    }

    private void Start()
    {
        // 1. Subscribe to Inventory Changes
        InventoryManager.Instance.OnInventoryChanged += RefreshInventoryDisplay;

        // 2. Set up context menu button listeners
        useButton.onClick.AddListener(() => OnUseClicked());
        dropButton.onClick.AddListener(() => OnDropClicked());
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent errors when scene closes
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= RefreshInventoryDisplay;
        }
    }

    // --- MAIN UI CONTROL ---

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf)
        {
            RefreshInventoryDisplay();
        }
        else
        {
            HideContextMenu();
            ClearDescription();
        }
    }

    // --- REFRESH DISPLAY LOGIC ---

    public void RefreshInventoryDisplay()
    {
        // 1. Clear old slots
        foreach (var slotUI in activeSlotUIs)
        {
            Destroy(slotUI);
        }
        activeSlotUIs.Clear();

        // 2. Get current inventory data
        List<InventorySlot> slots = InventoryManager.Instance.GetInventorySlots();

        // 3. Instantiate new slots for each item
        foreach (var slot in slots)
        {
            GameObject slotUI = Instantiate(itemSlotPrefab, contentContainer);

            // Assume the prefab has a script/components to display data
            // You would normally have a separate ItemSlotUI.cs script here to handle display

            // For now, let's just update text components if you placed them directly:
            TextMeshProUGUI nameText = slotUI.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI quantityText = slotUI.transform.Find("QuantityText").GetComponent<TextMeshProUGUI>();
            Image iconImage = slotUI.transform.Find("IconImage").GetComponent<Image>();

            if (nameText != null) nameText.text = slot.item.itemName;
            if (quantityText != null) quantityText.text = $"x{slot.quantity}";
            if (iconImage != null) iconImage.sprite = slot.item.icon;

            // Attach interaction listeners to the slot UI
            Button button = slotUI.GetComponent<Button>();
            if (button != null)
            {
                // Pass the ItemData to the click handler
                button.onClick.AddListener(() => OnSlotClicked(slot.item));
            }

            // You would also add mouse hover listeners here for the description panel

            activeSlotUIs.Add(slotUI);
        }
    }

    // --- INTERACTION HANDLERS ---

    public void OnSlotClicked(ItemData item)
    {
        selectedItemData = item;
        ShowContextMenu(item);
        ShowDescription(item);
    }

    public void OnUseClicked()
    {
        if (selectedItemData == null) return;

        if (InventoryManager.Instance.CanPerformAction(selectedItemData, "Use"))
        {
            // TODO: Add actual 'Use' logic (e.g., consume item, apply effect)
            InventoryManager.Instance.RemoveItem(selectedItemData, 1);
            Debug.Log($"Used item: {selectedItemData.itemName}");
        }
        HideContextMenu();
    }

    public void OnDropClicked()
    {
        if (selectedItemData == null) return;

        if (InventoryManager.Instance.CanPerformAction(selectedItemData, "Drop"))
        {
            // TODO: Add logic to spawn the item prefab in the world
            InventoryManager.Instance.RemoveItem(selectedItemData, 1);
            Debug.Log($"Dropped item: {selectedItemData.itemName}");
        }
        HideContextMenu();
    }

    // --- CONTEXT MENU & DESCRIPTION VISUALS ---

    private void ShowContextMenu(ItemData item)
    {
        contextMenuPanel.SetActive(true);
        // Position the context menu next to the clicked slot (TODO: Implement proper positioning)

        // Disable/Enable buttons based on restrictions
        useButton.gameObject.SetActive(InventoryManager.Instance.CanPerformAction(item, "Use"));
        dropButton.gameObject.SetActive(InventoryManager.Instance.CanPerformAction(item, "Drop"));
    }

    private void HideContextMenu()
    {
        contextMenuPanel.SetActive(false);
        selectedItemData = null;
    }

    private void ShowDescription(ItemData item)
    {
        descriptionText.text = item.description;
        // TO DO: Make description panel visible/slide in
    }

    private void ClearDescription()
    {
        descriptionText.text = "";
    }
}