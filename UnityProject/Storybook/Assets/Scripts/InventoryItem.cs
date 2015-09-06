using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryItem : MonoBehaviour {

    public enum Genre // Enum for color values
    {
        None, Red, Orange, Yellow, Green, Blue, Grey
    };

    public string ItemName;
    public Genre ItemColor; // Item Color
    public int ItemLevel; // The level of the item, determines how deep in the dungeon it appears
    public float BaseDrop; // The base drop rate of the item

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
