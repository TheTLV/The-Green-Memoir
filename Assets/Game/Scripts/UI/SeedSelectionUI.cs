using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SeedSelectionUI : MonoBehaviour
{
    public static SeedSelectionUI Instance;

    [Header("UI References")]
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject seedButtonPrefab;

    [Header("Required Managers")]
    [SerializeField] private FarmingManager farmingManager;
    [SerializeField] private InventoryManager inventoryManager;

    // List to hold UI elements for cleanup
    private List<GameObject> activeSeedButtons = new List<GameObject>();

    private Vector3Int targetGridPosition;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        selectionPanel.SetActive(false);
    }

    // --- MAIN CONTROL ---

    public void OpenSelectionUI(Vector3Int targetPos)
    {
        if (farmingManager == null) farmingManager = FarmingManager.Instance;
        if (inventoryManager == null) inventoryManager = InventoryManager.Instance;

        targetGridPosition = targetPos;
        selectionPanel.SetActive(true);

        PopulateSeedList();
    }

    public void CloseSelectionUI()
    {
        selectionPanel.SetActive(false);
    }

    // --- UI POPULATION ---

    private void PopulateSeedList()
    {
        // 1. Clear old buttons
        foreach (var button in activeSeedButtons)
        {
            Destroy(button);
        }
        activeSeedButtons.Clear();

        // 2. Get all available seed items from Inventory
        List<InventorySlot> inventorySlots = inventoryManager.GetInventorySlots();

        foreach (var slot in inventorySlots)
        {
            // Filter: Only show items that are marked as Seed and have data
            if (slot.item.isSeed && slot.item.plantedCropData != null && slot.quantity > 0)
            {
                // Instantiate the UI button/row
                GameObject seedButtonGO = Instantiate(seedButtonPrefab, contentContainer);
                activeSeedButtons.Add(seedButtonGO);

                // Assuming the prefab has a Button and TextMeshPro component
                Button button = seedButtonGO.GetComponent<Button>();
                TextMeshProUGUI text = seedButtonGO.GetComponentInChildren<TextMeshProUGUI>();

                // Set text (e.g., "Corn Seeds x5")
                if (text != null) text.text = $"{slot.item.itemName} x{slot.quantity}";

                // Attach the planting action to the button, passing the ItemData
                if (button != null) button.onClick.AddListener(() => OnSeedSelected(slot.item));
            }
        }
    }

    // --- PLANTING ACTION ---

    public void OnSeedSelected(ItemData seedItem)
    {
        if (farmingManager.IsGroundPlowed(targetGridPosition) && !farmingManager.IsCropPlanted(targetGridPosition))
        {
            farmingManager.SeedTile(targetGridPosition, seedItem.plantedCropData);
            inventoryManager.RemoveItem(seedItem, 1);
        }
        else
        {
            Debug.Log("Planting failed: Ground state changed or already planted.");
        }

        CloseSelectionUI();
    }
}