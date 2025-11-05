using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject quickSelectorBar;
    [SerializeField] private QuickSlotUI currentToolDisplaySlot;
    [SerializeField] private List<QuickSlotUI> toolSlotUI = new List<QuickSlotUI>(4);
    [SerializeField] private RectTransform selectorArrow;

    [Header("Tool Logic Data")]
    [SerializeField] private List<ToolData> fixedToolData = new List<ToolData>(4);

    [Header("Tool Visuals (Icons/Names)")]
    [SerializeField] private List<ItemData> toolVisuals = new List<ItemData>(4);

    // ---------------------------------------------------
    [Header("Manager References")]
    [SerializeField] private ToolStateManager toolStateManager;
    // ---------------------------------------------------

    private int currentSelectedIndex = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (fixedToolData.Count != 4 || toolVisuals.Count != 4 || toolSlotUI.Count != 4)
        {
            Debug.LogError("QuickSlotManager: Lists must contain exactly 4 elements.");
            return;
        }


        if (toolStateManager == null)
        {
            toolStateManager = ToolStateManager.Instance;
        }

        RefreshToolItems();
        SetSelectedIndex(0);
    }

    private void Update()
    {
        HandleNumberKeyInput();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetSelectorBarVisible(!quickSelectorBar.activeSelf);
        }
    }

    // --- INPUT & SELECTION LOGIC ---

    private void HandleNumberKeyInput()
    {
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SetSelectedIndex(i);
                SetSelectorBarVisible(false);
                break;
            }
        }
    }

    public void SetSelectedIndex(int newIndex)
    {
        if (newIndex < 0 || newIndex >= toolSlotUI.Count) return;

        // 1. Update Index and get current data
        currentSelectedIndex = newIndex;
        ItemData selectedItemVisual = toolVisuals[currentSelectedIndex];
        ToolData selectedToolData = fixedToolData[currentSelectedIndex];

        // 2. Determine Uses (for Water Can)
        int uses = 1;
        if (selectedToolData.actionType == ToolActionType.Water)
        {

            if (toolStateManager != null)
            {
                uses = toolStateManager.GetToolState(selectedToolData).currentUses;
            }
        }

        // 3. Update Visual Slot (Large one)
        currentToolDisplaySlot.UpdateSlot(selectedItemVisual, uses);

        // 4. Update Arrow position
        if (selectorArrow != null)
        {
            selectorArrow.position = toolSlotUI[currentSelectedIndex].transform.position;
        }
    }

    public ToolData GetSelectedToolData()
    {
        if (currentSelectedIndex >= 0 && currentSelectedIndex < fixedToolData.Count)
        {
            return fixedToolData[currentSelectedIndex];
        }
        return null;
    }

    // --- VISUAL & DATA BINDING ---

    private void RefreshToolItems()
    {
        // Setup of the 4 small slots using ItemData visuals
        for (int i = 0; i < 4; i++)
        {
            toolSlotUI[i].UpdateSlot(toolVisuals[i], 1);
        }
    }

    public void SetSelectorBarVisible(bool isVisible)
    {
        quickSelectorBar.SetActive(isVisible);
    }
}