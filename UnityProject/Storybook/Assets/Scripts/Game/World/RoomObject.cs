using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class RoomObject : NetworkBehaviour {

    public readonly int NORTH_DOOR_INDEX = 0;
    public readonly int EAST_DOOR_INDEX = 1;
    public readonly int SOUTH_DOOR_INDEX = 2;
    public readonly int WEST_DOOR_INDEX = 3;

    [SerializeField]
    Transform m_cameraNode;

    [SerializeField]
    private Door[] m_roomDoors;
                         // Ordering for indices should be clockwise, starting from the north.
                         // In a standard 1x1 room, it would be like:
                         // 0 - North, 1 - East, 2 - South, 3 - West.
                         // In a larger room, it would probably be more like 0-N, 1-N, 2-E, 3-S, 4-S, and so on.
                         // If a door does not exist here, just use "null"
    [SerializeField]
    [SyncVar]
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

    /// <summary>
    /// The list of room doors
    /// </summary>
    public Door[] RoomDoors
    {
        get { return m_roomDoors; }
    }

    public void SetRoomDoors(Door[] newDoorList)
    {
        m_roomDoors = newDoorList;
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

    public Transform CameraNode
    {
        get { return m_cameraNode; }
    }
}
