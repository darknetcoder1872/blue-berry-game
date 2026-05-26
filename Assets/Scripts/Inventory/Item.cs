using UnityEngine;
using System;

/// <summary>
/// Represents a single inventory item
/// </summary>
[System.Serializable]
public class Item
{
    public string itemId;
    public string itemName;
    public string description;
    public Sprite icon;
    public int stackSize;
    public float weight;
    public ItemRarity rarity;
    public ItemType itemType;

    public enum ItemRarity { Common, Uncommon, Rare, Legendary }
    public enum ItemType { Weapon, Tool, Food, Medical, Crafting, Misc }

    public Item(string id, string name, Sprite icon, int stack, float weight, ItemRarity rarity, ItemType type)
    {
        itemId = id;
        itemName = name;
        this.icon = icon;
        stackSize = stack;
        this.weight = weight;
        this.rarity = rarity;
        itemType = type;
    }
}

/// <summary>
/// Represents a stack of items in inventory
/// </summary>
[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int quantity;

    public InventorySlot(Item item, int quantity = 1)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public float GetTotalWeight() => item.weight * quantity;
}