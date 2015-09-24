using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class InventoryItem
{
    [SerializeField]
    private string m_ItemName;
    [SerializeField]
    private Genre m_ItemGenre; // Item Genre
    [SerializeField]
    private RoomFeature m_ItemFeature; // Item Feature (property of spawned room/combat effect)
    [SerializeField]
    private int m_ItemLevel; // The level of the item, determines how deep in the dungeon it appears
    [SerializeField]
    private float m_BaseDrop; // The base drop rate of the item

    // Method for item usage. Configure more in-depth for child classes.
    protected void OnUse()
    {
        Use();
    }

    // Method for item dropping (from battle, chest, etc.) Configure more in-depth for child classes.
    protected void OnDrop()
    {
        Drop();
    }

    // Method for item pickup. Configure more in-depth from the child class.
    protected void OnPickup()
    {
        Pickup();
    }

    // Method to Use the item
    public abstract void Use();

    // Method to Drop the item
    public abstract void Drop();

    // Method to Pickup the item
    public abstract void Pickup();
}
