using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {

    [SerializeField]
    private List<InventoryItem> Inventory = new List<InventoryItem>(); // List for all equipment and active items
                                                                       //public List<Page> Pages = new List<Page>(); // List for all pages the player has
                                                                       // ^^^Uncomment when Pages are added back in.
    [SerializeField]
    private const int DEFAULT_INVENTORY_SPACE = 4;
    [SerializeField]
    private const int DEFAULT_MAX_PAGES = 99;
    [SerializeField]
    private int ItemsInInventory = 0; // Current items in the player's inventory
    [SerializeField]
    private int PagesHeld = 0; // Number of pages the players has

    // Add an item to the Inventory, but only if there is enough space
    public void AddToInventory(InventoryItem TheItem)
    {
        if (Inventory.Count < DEFAULT_INVENTORY_SPACE)
        {
            Inventory.Add(TheItem);
            ItemsInInventory = Inventory.Count;
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
        Inventory.Remove(TheItem);
        ItemsInInventory = Inventory.Count;
    }
}
