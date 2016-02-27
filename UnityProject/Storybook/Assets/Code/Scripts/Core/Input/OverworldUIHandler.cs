using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class OverworldUIHandler : UIHandler {

    [SerializeField]
    Button m_northButton;

    [SerializeField]
    Button m_eastButton;

    [SerializeField]
    Button m_southButton;

    [SerializeField]
    Button m_westButton;

    /// <summary>
    /// Called by direction buttons in the overworld menu
    /// Submits the direction to the player mover
    /// </summary>
    /// <param name="selectedDirection">The direction that was pressed</param>
    public void DirectionButtonPressed(Door.Direction selectedDirection)
    {
        PlayClickSound();
        EventDispatcher.GetDispatcher<OverworldEventDispatcher>().SubmitDirection(selectedDirection);
        Destroy(gameObject);
    }

    /// <summary>
    /// Sets which buttons are active based on the doors in the current room
    /// </summary>
    /// <param name="currentRoom">The current room that the players are in</param>
    public void PopulateMenu(RoomObject currentRoom)
    {
        m_northButton.gameObject.SetActive(currentRoom.NorthDoor.IsDoorEnabled);
        m_eastButton.gameObject.SetActive(currentRoom.EastDoor.IsDoorEnabled);
        m_southButton.gameObject.SetActive(currentRoom.SouthDoor.IsDoorEnabled);
        m_westButton.gameObject.SetActive(currentRoom.WestDoor.IsDoorEnabled);
    }
}
