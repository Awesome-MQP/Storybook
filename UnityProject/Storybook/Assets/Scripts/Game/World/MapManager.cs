using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {

    [SerializeField]
    private int m_worldMaxXSize = 12;

    [SerializeField]
    private int m_worldMaxYSize = 12;

    [SerializeField]
    private RoomObject[,] m_worldGrid; // Creates a 2D array to place rooms

    [SerializeField]
    private RoomObject m_roomPrefab;

    private int m_defaultRoomSize = 50; // Default room size (in blocks in Unity editor)

    // Initialize
    void Awake()
    {
        m_worldGrid = new RoomObject[m_worldMaxXSize, m_worldMaxYSize];
        Location startingRoomLoc = new Location(0, 0);
        PlaceRoom(startingRoomLoc);
    }
	
    // Place a new room in the world.
    // The MapMgr is not concerned with the contents of the room, just that it can be placed.
    // 
    public RoomObject PlaceRoom(Location gridPosition)
    {
        Debug.Log("Placing room");
        int placeX = gridPosition.X;
        int placeY = gridPosition.Y;
        // First, check to make sure the location is valid! Can't have rooms hanging off the edge of the map.
        if ((placeX < 0 || placeX >= m_worldMaxXSize) || 
            (placeY < 0 || placeY >= m_worldMaxYSize)) {
            Debug.Log("Room is outside of map");
            return null;
        }
        // Now check to make sure there isn't a room already in the spot.
        // The player cannot overwrite rooms that have already been placed.
        if(m_worldGrid[placeX, placeY] != null) {
            Debug.Log("Room is already placed here");
            Debug.Log("X = " + placeX.ToString());
            Debug.Log("Y = " + placeY.ToString());
            return null;
        }
        // If we got here, then the location is assumed to be valid.
        // Place the room.
        m_roomPrefab.SetRoomLocation(gridPosition);
        m_worldGrid[placeX, placeY] = m_roomPrefab;
        Vector3 roomGridLocation = new Vector3(m_defaultRoomSize * placeY, 0, m_defaultRoomSize * placeX);
        RoomObject room = (RoomObject) Instantiate(m_roomPrefab, roomGridLocation, new Quaternion());
        _determineDoorPlacement(gridPosition, room);
        return room;
    }

    // Get a room from the world
    public RoomObject GetRoom(Location roomLoc)
    {
        return m_worldGrid[roomLoc.X, roomLoc.Y];
    }

    // Tests the Map Manager code. Called once on startup.
    void TestMapMgr()
    {
        Location OOB_X = new Location(-1, 0);
        Location OOB_Y = new Location(0, 13);
        Location OOB_Both = new Location(25, 25);
        Location Good_1 = new Location(5, 5);
        Location Good_2 = new Location(5, 6);
        PlaceRoom(OOB_X); // try placing a room out of bounds in X (expect fail)
        PlaceRoom(OOB_Y); // try placing a room OOB in Y (expect fail)
        PlaceRoom(OOB_Both); // try placing a room OOB in both directions (expect fail)
        PlaceRoom(Good_1); // try placing a room in a good location (expect pass)
        PlaceRoom(Good_2); // try placing a room in another good location (expect pass)
        PlaceRoom(Good_1); // try placing a room where another room has already been placed (expect fail)
    }

    void TestMapMgrPlacement()
    {
        Location Good_1 = new Location(5, 5);
        Location Good_2 = new Location(5, 6);
        GameObject aBlankRoom = GameObject.Find("TestRoom");
    }

    /// <summary>
    /// Determines which of the doors of the room need to appear based on the room's position in the grid
    /// </summary>
    /// <param name="gridPosition">The position of the room in the grid</param>
    /// <param name="theRoom">The room object being placed</param>
    /// <returns>Returns the room object with the altered doors</returns>
    private RoomObject _determineDoorPlacement(Location gridPosition, RoomObject theRoom)
    {
        // Initialize the room through door locations
        theRoom.RoomDoors[0].SetRoomThroughDoorLoc(new Location(gridPosition.X + 1, gridPosition.Y));
        theRoom.RoomDoors[1].SetRoomThroughDoorLoc(new Location(gridPosition.X, gridPosition.Y + 1));
        theRoom.RoomDoors[2].SetRoomThroughDoorLoc(new Location(gridPosition.X - 1, gridPosition.Y));
        theRoom.RoomDoors[3].SetRoomThroughDoorLoc(new Location(gridPosition.X, gridPosition.Y - 1));

        if (gridPosition.X - 1 < 0)
        {
            theRoom.RoomDoors[2].DisableDoor();
        }

        else if (gridPosition.X + 1 >= m_worldMaxXSize)
        {
            theRoom.RoomDoors[0].DisableDoor();
        }

        if (gridPosition.Y - 1 < 0)
        {
            theRoom.RoomDoors[3].DisableDoor();
        }
        else if (gridPosition.Y + 1 >= m_worldMaxYSize)
        {
            theRoom.RoomDoors[1].DisableDoor();
        }

        return theRoom;
    }
}
