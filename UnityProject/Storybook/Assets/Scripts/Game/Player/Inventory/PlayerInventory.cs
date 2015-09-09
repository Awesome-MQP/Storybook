using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {

    [SerializeField]
    private List<InventoryItem> m_Inventory = new List<InventoryItem>(); // List for all equipment and active items
                                                                       //public List<Page> Pages = new List<Page>(); // List for all pages the player has
                                                                       // ^^^Uncomment when Pages are added back in.
    [SerializeField]
    private const int DEFAULT_INVENTORY_SPACE = 4;
    [SerializeField]
    private const int DEFAULT_MAX_PAGES = 99;
    [SerializeField]
    private int m_ItemsInInventory = 0; // Current items in the player's inventory
    [SerializeField]
    private int m_PagesHeld = 0; // Number of pages the players has

    // Add an item to the Inventory, but only if there is enough space
    public void AddToInventory(InventoryItem TheItem)
    {
        if (m_Inventory.Count < DEFAULT_INVENTORY_SPACE)
        {
            m_Inventory.Add(TheItem);
            m_ItemsInInventory = m_Inventory.Count;
            // TODO : Notify player that the item is in their inventory
            // TODO : Code that removes the item from the room
        }
        else
        {
            // TODO : Notify the player that inventory is full
            // TODO : Code that drops the item on the floor (?)
        }
    }

    // Removes a specific item from the Inventory, using the List.Remove method to remove the first instance of a specific item.
    // This way, only one of an item is removed if the player has more than one.
    public void RemoveFromInventory(InventoryItem TheItem)
    {
        m_Inventory.Remove(TheItem);
        m_ItemsInInventory = m_Inventory.Count;
    }
}
