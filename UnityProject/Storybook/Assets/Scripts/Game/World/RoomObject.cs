using UnityEngine;
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
    private int m_roomSize; // Can be x1, x2, x4.
    [SerializeField]
    private Genre m_roomGenre;
    [SerializeField]
    private RoomFeature m_roomFeature;

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

    // Set size of a room
    public void SetRoomSize(int size)
    {
        m_roomSize = size;
    }

    // Get size of a room
    public int GetRoomSize()
    {
        return m_roomSize;
    }

    // Set Genre of a room
    public void SetRoomGenre(Genre genre)
    {
        m_roomGenre = genre;
    }

    // Get Genre of a room
    public Genre GetRoomGenre()
    {
        return m_roomGenre;
    }

    // Set Feature of a room
    public void SetRoomFeature(RoomFeature feature)
    {
        m_roomFeature = feature;
    }

    // Get Feature of a room
    public RoomFeature GetRoomFeature()
    {
        return m_roomFeature;
    }
}
