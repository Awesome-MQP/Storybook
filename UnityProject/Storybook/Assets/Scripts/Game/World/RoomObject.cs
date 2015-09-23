﻿using UnityEngine;
using System.Collections;

public class RoomObject : MonoBehaviour{
    [SerializeField]
    private Door[] m_roomDoors;
                         // Ordering for indices should be clockwise, starting from the north.
                         // In a standard 1x1 room, it would be like:
                         // 0 - North, 1 - East, 2 - South, 3 - West.
                         // In a larger room, it would probably be more like 0-N, 1-N, 2-E, 3-S, 4-S, and so on.
                         // If a door does not exist here, just use "null"
    [SerializeField]
    private Location m_roomLocation;
    [SerializeField]
    private int m_roomSize;
    [SerializeField]
    private int m_roomSizeConstraint; // Can be x1, x2, x4.
                                   // Did I handle this right? I'm not really sure what the difference is meant to be between
                                   // room size and room size constraint.

    // Set the location of the room.
    // Should only be used once, when placing the room.
    public void SetRoomLocation(Location loc)
    {
        m_roomLocation = loc;
    }

    // Grab the location of the room.
    public Location GetRoomLocation()
    {
        return m_roomLocation;
    }
}
