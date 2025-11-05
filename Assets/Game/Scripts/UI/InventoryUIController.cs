using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance;

    [Header("References")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject inventorySlotPrefab;

    [Header("Context Menu References")]
    [SerializeField] private GameObject contextMenuPanel;
    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private List<InventorySlotUI> inventorySlots = new List<InventorySlotUI>();
    private ItemData selectedItemData;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        inventoryPanel.SetActive(false);
    }

    private void Start()
    {
        if (inventoryManager == null)
            inventoryManager = InventoryManager.Instance;

        inventoryManager.OnInventoryChanged += UpdateInventoryUI;


        useButton.onClick.AddListener(OnUseClicked);
        dropButton.onClick.AddListener(OnDropClicked);


        InitializeInventoryUI(30);
    }

    private void OnDestroy()
    {
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged -= UpdateInventoryUI;
        }
    }

    private void InitializeInventoryUI(int numberOfSlots)
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
        inventorySlots.Clear();

        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slotGO = Instantiate(inventorySlotPrefab, contentContainer);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            inventorySlots.Add(slotUI);

            Button button = slotGO.GetComponent<Button>();
            if (button != null)
            {
                int index = i;
                button.onClick.AddListener(() => OnSlotClicked(inventorySlots[index].currentItemData));
            }
        }
    }

    public void UpdateInventoryUI()
    {
        List<InventorySlot> items = inventoryManager.GetInventorySlots();

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < items.Count)
            {
                inventorySlots[i].UpdateSlot(items[i].item, items[i].quantity);
            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }
    }

    // --- MAIN UI CONTROL ---

    public void ToggleInventoryUI()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf)
        {
            UpdateInventoryUI();
        }
        else
        {
            HideContextMenu();
            descriptionText.text = "";
        }
    }

    // --- INTERACTION HANDLERS ---

    public void OnSlotClicked(ItemData item)
    {
        if (item == null)
        {
            HideContextMenu();
            return;
        }

        selectedItemData = item;
        ShowContextMenu(item);
        descriptionText.text = item.description;
    }

    public void OnUseClicked()
    {
        if (selectedItemData == null) return;

        if (InventoryManager.Instance.CanPerformAction(selectedItemData, "Use"))
        {
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
            InventoryManager.Instance.RemoveItem(selectedItemData, 1);
            Debug.Log($"Dropped item: {selectedItemData.itemName}");
        }
        HideContextMenu();
    }

    // --- CONTEXT MENU VISUALS ---

    private void ShowContextMenu(ItemData item)
    {
        contextMenuPanel.SetActive(true);

        useButton.gameObject.SetActive(InventoryManager.Instance.CanPerformAction(item, "Use"));
        dropButton.gameObject.SetActive(InventoryManager.Instance.CanPerformAction(item, "Drop"));
    }

    private void HideContextMenu()
    {
        contextMenuPanel.SetActive(false);
        selectedItemData = null;
    }
}