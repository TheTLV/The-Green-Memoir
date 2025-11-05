using UnityEngine;
using System.Collections.Generic;

// Enum for tool actions remains the same
public enum ToolActionType
{
    None,
    Plow,
    Water,
    Refill,
    Harvest,
    Seed,
    Interact
}

// ScriptableObject for tool specific data
[CreateAssetMenu(fileName = "New Tool Data", menuName = "Inventory/Tool Data")]
public class ToolData : ScriptableObject
{
    [Header("Tool Information")]
    public string toolName = "New Tool";
    public Sprite icon;

    [Header("Functionality")]
    public ToolActionType actionType = ToolActionType.None;

    [Header("Refillable Properties")]
    [Tooltip("Max charges before needing refill (1 for Hoe/Sickle, 10 for Water Can)")]
    public int maxUses = 1;

    // Optional: Reference to ItemData if the tool is stored in inventory (we are ignoring this for now)
    // public ItemData itemDataReference;
}