using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System; // Required for Action/Events


[System.Serializable]
public struct InventorySlot
{
    public ItemData item;
    public int quantity;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Player Currency")]
    public long moneyAmount = 10000; // Accessible by HUDManager

    // Key: ItemData Asset, Value: Quantity
    private Dictionary<ItemData, int> itemQuantities = new Dictionary<ItemData, int>();

    // Event triggered when the inventory or money changes (for UI refresh)
    public event Action OnInventoryChanged;
    public event Action OnMoneyChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Example: Initialize with some test data if needed
    }

    // --- CURRENCY MANAGEMENT ---

    public void AddMoney(long amount)
    {
        if (amount > 0)
        {
            moneyAmount += amount;
            OnMoneyChanged?.Invoke();
        }
    }

    public bool RemoveMoney(long amount)
    {
        if (amount > 0 && moneyAmount >= amount)
        {
            moneyAmount -= amount;
            OnMoneyChanged?.Invoke();
            return true;
        }
        return false;
    }

    // --- ITEM MANAGEMENT ---

    /// <summary>
    /// Adds an item to the inventory, handling stacking.
    /// </summary>
    public void AddItem(ItemData itemToAdd, int amount)
    {
        if (itemToAdd == null || amount <= 0) return;

        if (itemQuantities.ContainsKey(itemToAdd))
        {
            itemQuantities[itemToAdd] += amount;
        }
        else
        {
            itemQuantities.Add(itemToAdd, amount);
        }

        Debug.Log($"Added {amount} of {itemToAdd.itemName}. Total: {itemQuantities[itemToAdd]}");
        OnInventoryChanged?.Invoke(); // Notify UI to refresh
    }

    /// <summary>
    /// Removes a specified amount of an item from the inventory.
    /// </summary>
    public bool RemoveItem(ItemData itemToRemove, int amount)
    {
        if (itemToRemove == null || amount <= 0) return false;

        if (itemQuantities.ContainsKey(itemToRemove))
        {
            int currentQuantity = itemQuantities[itemToRemove];

            if (currentQuantity >= amount)
            {
                itemQuantities[itemToRemove] -= amount;

                // Remove the entry completely if quantity reaches zero
                if (itemQuantities[itemToRemove] <= 0)
                {
                    itemQuantities.Remove(itemToRemove);
                }

                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns the full list of occupied slots for UI display.
    /// </summary>
    public List<InventorySlot> GetInventorySlots()
    {
        return itemQuantities.Select(pair => new InventorySlot
        {
            item = pair.Key,
            quantity = pair.Value
        }).ToList();
    }

    // --- CONTEXT MENU LOGIC (DUMMY IMPLEMENTATION) ---

    /// <summary>
    /// Checks if a specific action (e.g., Use, Drop) is allowed for an item.
    /// </summary>
    public bool CanPerformAction(ItemData item, string action)
    {
        // Example logic based on your request:
        if (item == null) return false;

        if (action == "Drop" || action == "Sell")
        {
            // Mission items and Tools cannot be dropped/sold
            // (Assumes ItemData has public bool isMissionItem and public bool isTool)
            if (item.isMissionItem || item.isTool)
                return false;
        }

        if (action == "Use")
        {
            // Only consumables can be 'Used' (Assumes ItemData has public bool isConsumable)
            if (!item.isConsumable)
                return false;
        }

        // Default: Allow action unless specifically restricted
        return true;
    }
}