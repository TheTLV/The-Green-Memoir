using UnityEngine;


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

    // public bool isTool;
    // public ToolActionType actionType;
    // ------------------------------------

    [Header("Seed Properties")]
    public bool isSeed = false;
    public SeedData plantedCropData; 

    [Header("Context Menu Restrictions")]
    public bool isMissionItem = false;
    public bool isConsumable = false;
}