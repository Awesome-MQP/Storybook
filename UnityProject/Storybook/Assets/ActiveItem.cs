using UnityEngine;
using System.Collections;

public class ActiveItem : InventoryItem {

    // Create a new Active Item
    public ActiveItem(string TheName, Genre TheColor)
    {
        this.ItemName = TheName;
        this.ItemColor = TheColor;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Called when the item is used to determine the effect
    public void Activate()
    {
        // TODO : Code that perform's the item's effect when used.
    }
}
