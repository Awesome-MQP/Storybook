using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class OverworldUIHandler : UIHandler {

    private StorybookPlayerMover m_playerMover;

    [SerializeField]
    Button m_northButton;

    [SerializeField]
    Button m_eastButton;

    [SerializeField]
    Button m_southButton;

    [SerializeField]
    Button m_westButton;

    public override void PageButtonPressed(PageButton pageButton)
    {
        
    }

    /// <summary>
    /// Called by direction buttons in the overworld menu
    /// Submits the direction to the player mover
    /// </summary>
    /// <param name="selectedDirection">The direction that was pressed</param>
    public void DirectionButtonPressed(Door.Direction selectedDirection)
    {
        m_playerMover.SubmitDirection(selectedDirection);
        Destroy(gameObject);
    }

    public void RegisterPlayerMover(StorybookPlayerMover playerMover)
    {
        m_playerMover = playerMover;
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
