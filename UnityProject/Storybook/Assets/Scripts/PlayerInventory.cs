using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {

    public List<InventoryItem> Inventory = new List<InventoryItem>(); // List for all equipment and active items
    public List<Page> Pages = new List<Page>(); // List for all pages the player has
    public const int DEFAULT_INVENTORY_SPACE = 4;
    public const int DEFAULT_MAX_PAGES = 99;
    public int ItemsInInventory = 0; // Current items in the player's inventory
    public int PagesHeld = 0; // Number of pages the players has

	// Use this for initialization
	void Start () {
        Equipment TestEquip = new Equipment("Test", InventoryItem.Genre.Blue, "Head", new List<int>() { 0, 0, 0, 0, 0, 0 });
        Debug.Log("The number of items in the Inventory is : " + ItemsInInventory);
        Debug.Log("The number of pages held is : " + PagesHeld);
        this.AddToInventory(TestEquip);
        Debug.Log("The number of items in the Inventory is : " + ItemsInInventory);
        this.AddToInventory(TestEquip);
        this.AddToInventory(TestEquip);
        this.AddToInventory(TestEquip);
        Debug.Log("The number of items in the Inventory is : " + ItemsInInventory);
        this.AddToInventory(TestEquip);
        this.RemoveFromInventory(TestEquip);
        Debug.Log("The number of items in the Inventory is : " + ItemsInInventory);
    }

    // Update is called once per frame
    void Update () {
	
	}

    // Add an item to the Inventory, but only if there is enough space
    private void AddToInventory(InventoryItem TheItem)
    {
        if (Inventory.Count < DEFAULT_INVENTORY_SPACE)
        {
            Inventory.Add(TheItem);
            Debug.Log("Added item: " + TheItem.ItemName);
            ItemsInInventory = Inventory.Count;
            // TODO : Code that removes the item from the room
        }
        else
        {
            Debug.Log("Too many items in the bag already!");
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
