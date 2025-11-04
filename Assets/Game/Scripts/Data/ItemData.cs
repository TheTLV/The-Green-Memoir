using UnityEngine;

public enum ToolActionType
{
    None,
    Plow,
    Water,
    Seed,
    Harvest,
    Interact
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Item Infomation")]
    public string itemName = "New Item";
    [TextArea(3, 10)]
    public string description = "A default item.";
    public Sprite icon;

    [Header("Stackable")]
    public bool isStackable = true;
    public int maxStackSize = 99;

    [Header("Tool-Seed")]
    public bool isTool = false;
    public bool isSeed = false;
    public ToolActionType actionType = ToolActionType.None;

    public SeedData plantedCropData;


    [Header("Context Menu Restrictions")]
    public bool isMissionItem = false; // Cannot be dropped/sold
    public bool isConsumable = false;  // Can be 'Used' (e.g., food, potion)
    // --------------------------------------------------------
}