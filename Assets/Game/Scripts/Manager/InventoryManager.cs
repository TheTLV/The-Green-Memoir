using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

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
    public long moneyAmount = 10000;

    private Dictionary<ItemData, int> itemQuantities = new Dictionary<ItemData, int>();

    public event Action OnInventoryChanged;
    public event Action OnMoneyChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
        OnInventoryChanged?.Invoke();
    }

    public bool RemoveItem(ItemData itemToRemove, int amount)
    {
        if (itemToRemove == null || amount <= 0) return false;

        if (itemQuantities.ContainsKey(itemToRemove))
        {
            int currentQuantity = itemQuantities[itemToRemove];

            if (currentQuantity >= amount)
            {
                itemQuantities[itemToRemove] -= amount;

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

    public List<InventorySlot> GetInventorySlots()
    {
        return itemQuantities.Select(pair => new InventorySlot
        {
            item = pair.Key,
            quantity = pair.Value
        }).ToList();
    }

    // --- CONTEXT MENU LOGIC ---
    public bool CanPerformAction(ItemData item, string action)
    {
        if (item == null) return false;

        if (action == "Drop" || action == "Sell")
        {
            if (item.isMissionItem)
                return false;
        }

        if (action == "Use")
        {
            if (!item.isConsumable)
                return false;
        }
        return true;
    }
}