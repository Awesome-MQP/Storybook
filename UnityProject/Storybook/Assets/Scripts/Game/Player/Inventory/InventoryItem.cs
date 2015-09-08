using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class InventoryItem {

    [SerializeField]
    private enum Genre // Enum for color values
    {
        None, Red, Orange, Yellow, Green, Blue, Grey
    };

    [SerializeField]
    private string ItemName;
    [SerializeField]
    private Genre ItemColor; // Item Color
    [SerializeField]
    private int ItemLevel; // The level of the item, determines how deep in the dungeon it appears
    [SerializeField]
    private float BaseDrop; // The base drop rate of the item

    // Abstract method for item usage. Configure more in-depth for child classes.
    public void OnUse()
    {

    }

    // Abstract method for item dropping (from battle, chest, etc.) Configure more in-depth for child classes.
    public void OnDrop()
    {

    }

    // Method to Use the item
    public void Use()
    {
        // TODO: Remove from the inventory
    }

    // Method to Drop the item
    public void Drop()
    {

    }
}
