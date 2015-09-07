using UnityEngine;
using System.Collections;

public class Page : InventoryItem {

    public enum Room // Enum for RoomTypes
    {
        Standard, Shop, Curse, Sanctuary, Exit, Speed, Multiply, Teleport
    };   
    public Room PageRoomType; // RoomType

    // Create a new Page
    public Page(string TheName, Genre TheColor, Room TheRoom)
    {
        this.ItemName = TheName;
        this.ItemColor = TheColor;
        this.PageRoomType = TheRoom;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Function to determine page action when used in the world
    public void UseInWorld()
    {
        /*
        * TODO: determine room to be built
        */
    }

    // Function to determine page action when used in combat
    public void UseInCombat()
    {
        /*
        * TODO: determine combat action
        */
    }
}
