using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Equipment : InventoryItem {

    public string EquipSlot;
    public List<int> EquipMods; // Stat mods for the item. Index-Stat relationships are as follows:
    // 0 : HP, 1 : Strength, 2 : Special, 3 : Speed, 4 : Defence, 5 : Luck

    public Equipment(string TheName, Genre TheColor, string TheSlot, List<int> TheMods)
    {
        this.ItemName = TheName;
        this.ItemColor = TheColor;
        this.EquipSlot = TheSlot;
        this.EquipMods = TheMods;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
