using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Core inventory management system with slots, stacking, and weight limits.
/// </summary>
public class InventorySystem : MonoBehaviour
{
    [SerializeField] private int maxSlots = 20;
    [SerializeField] private float maxWeight = 30f; // kg
    [SerializeField] private int quickSlots = 4;

    private List<InventorySlot> inventorySlots = new List<InventorySlot>();
    private float currentWeight = 0f;

    // Events
    public static event Action<int> OnInventoryChanged; // slot index
    public static event Action OnWeightChanged;
    public static event Action<string> OnInventoryFull;

    private void Start()
    {
        // Initialize empty slots
        for (int i = 0; i < maxSlots; i++)
        {
            inventorySlots.Add(null);
        }
    }

    /// <summary>
    /// Add item to inventory
    /// </summary>
    public bool AddItem(Item item, int quantity = 1)
    {
        // Try to stack with existing items
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i] != null && inventorySlots[i].item.itemId == item.itemId)
            {
                inventorySlots[i].quantity += quantity;
                OnInventoryChanged?.Invoke(i);
                OnWeightChanged?.Invoke();
                return true;
            }
        }

        // Add to empty slot
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i] == null)
            {
                float itemWeight = item.weight * quantity;
                if (currentWeight + itemWeight > maxWeight)
                {
                    OnInventoryFull?.Invoke("Inventory too heavy!");
                    return false;
                }

                inventorySlots[i] = new InventorySlot(item, quantity);
                currentWeight += itemWeight;
                OnInventoryChanged?.Invoke(i);
                OnWeightChanged?.Invoke();
                return true;
            }
        }

        OnInventoryFull?.Invoke("Inventory full!");
        return false;
    }

    /// <summary>
    /// Remove item from inventory
    /// </summary>
    public bool RemoveItem(int slotIndex, int quantity = 1)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Count)
            return false;

        if (inventorySlots[slotIndex] == null)
            return false;

        inventorySlots[slotIndex].quantity -= quantity;
        if (inventorySlots[slotIndex].quantity <= 0)
        {
            currentWeight -= inventorySlots[slotIndex].GetTotalWeight();
            inventorySlots[slotIndex] = null;
        }

        OnInventoryChanged?.Invoke(slotIndex);
        OnWeightChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Get item from slot
    /// </summary>
    public InventorySlot GetSlot(int index)
    {
        if (index >= 0 && index < inventorySlots.Count)
            return inventorySlots[index];
        return null;
    }

    // Getters
    public float GetCurrentWeight() => currentWeight;
    public float GetMaxWeight() => maxWeight;
    public float GetWeightPercent() => currentWeight / maxWeight;
    public int GetEmptySlots() 
    {
        int empty = 0;
        foreach (var slot in inventorySlots)
            if (slot == null) empty++;
        return empty;
    }
    public List<InventorySlot> GetAllItems() => inventorySlots;
}