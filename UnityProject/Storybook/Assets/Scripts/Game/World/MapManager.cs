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

    private int m_defaultRoomSize = 20; // Default room size (in blocks in Unity editor)

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
        int placeX = gridPosition.X;
        int placeY = gridPosition.Y;
        // First, check to make sure the location is valid! Can't have rooms hanging off the edge of the map.
        if ((placeX < 0 || placeX >= m_worldMaxXSize) || 
            (placeY < 0 || placeY >= m_worldMaxYSize)) {
            return null;
        }
        // Now check to make sure there isn't a room already in the spot.
        // The player cannot overwrite rooms that have already been placed.
        if(m_worldGrid[placeX, placeY] != null) {
            return null;
        }
        // If we got here, then the location is assumed to be valid.
        // Place the room.
        Vector3 roomGridLocation = new Vector3(m_defaultRoomSize * placeY, 0, m_defaultRoomSize * placeX);
        RoomObject room = (RoomObject) Instantiate(m_roomPrefab, roomGridLocation, new Quaternion());
        room.SetRoomLocation(gridPosition);
        _determineDoorPlacement(gridPosition, room);
        _checkDoorRooms(gridPosition, room);
        m_worldGrid[placeX, placeY] = room;
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
        theRoom.RoomDoors[theRoom.NORTH_DOOR_INDEX].SetRoomThroughDoorLoc(new Location(gridPosition.X + 1, gridPosition.Y));
        theRoom.RoomDoors[theRoom.EAST_DOOR_INDEX].SetRoomThroughDoorLoc(new Location(gridPosition.X, gridPosition.Y + 1));
        theRoom.RoomDoors[theRoom.SOUTH_DOOR_INDEX].SetRoomThroughDoorLoc(new Location(gridPosition.X - 1, gridPosition.Y));
        theRoom.RoomDoors[theRoom.WEST_DOOR_INDEX].SetRoomThroughDoorLoc(new Location(gridPosition.X, gridPosition.Y - 1));

        if (gridPosition.X - 1 < 0)
        {
            theRoom.RoomDoors[theRoom.SOUTH_DOOR_INDEX].DisableDoor();
        }

        else if (gridPosition.X + 1 >= m_worldMaxXSize)
        {
            theRoom.RoomDoors[theRoom.NORTH_DOOR_INDEX].DisableDoor();
        }

        if (gridPosition.Y - 1 < 0)
        {
            theRoom.RoomDoors[theRoom.WEST_DOOR_INDEX].DisableDoor();
        }
        else if (gridPosition.Y + 1 >= m_worldMaxYSize)
        {
            theRoom.RoomDoors[theRoom.EAST_DOOR_INDEX].DisableDoor();
        }

        return theRoom;
    }

    /// <summary>
    /// Checks to see if the rooms that correspond to the doors have been spawned already and sets the DoorRoomSpawned
    /// variables accordingly
    /// </summary>
    /// <param name="roomLoc">The location of the room doors to be checked</param>
    /// <param name="currentRoom">The room with the doors to be checked</param>
    /// <returns></returns>
    private RoomObject _checkDoorRooms(Location roomLoc, RoomObject currentRoom)
    {
        Location northDoorLoc = new Location(roomLoc.X + 1, roomLoc.Y);
        Location eastDoorLoc = new Location(roomLoc.X, roomLoc.Y + 1);
        Location southDoorLoc = new Location(roomLoc.X - 1, roomLoc.Y);
        Location westDoorLoc = new Location(roomLoc.X, roomLoc.Y - 1);

        if (_isLocationOccupied(northDoorLoc))
        {
            currentRoom.RoomDoors[currentRoom.NORTH_DOOR_INDEX].SetIsDoorRoomSpawned(true);
        }

        if (_isLocationOccupied(eastDoorLoc))
        {
            currentRoom.RoomDoors[currentRoom.EAST_DOOR_INDEX].SetIsDoorRoomSpawned(true);
        }

        if (_isLocationOccupied(southDoorLoc))
        {
            currentRoom.RoomDoors[currentRoom.SOUTH_DOOR_INDEX].SetIsDoorRoomSpawned(true);
        }

        if (_isLocationOccupied(westDoorLoc))
        {
            currentRoom.RoomDoors[currentRoom.WEST_DOOR_INDEX].SetIsDoorRoomSpawned(true);
        }

        return currentRoom;
    }

    /// <summary>
    /// Checks to see if the given location is occupied by a room
    /// </summary>
    /// <param name="locToCheck">The location in the grid to check</param>
    /// <returns>True if the room is occupied, false otherwise</returns>
    private bool _isLocationOccupied(Location locToCheck)
    {
        if (locToCheck.X >= 0 && locToCheck.X < m_worldMaxXSize && locToCheck.Y >= 0 && locToCheck.Y < m_worldMaxYSize)
        {
            RoomObject roomAtLoc = m_worldGrid[locToCheck.X, locToCheck.Y];
            return (roomAtLoc != null);
        }
        return false;
    }

    /// <summary>
    /// Takes in a door and returns the door that is paired with it
    /// </summary>
    /// <param name="currentLoc">The location in the grid of the given door</param>
    /// <param name="entryDoor">The door to find the pairing of</param>
    /// <returns>The door the is linked to the given door</returns>
    public Door GetDoorPartner(Location currentLoc, Door entryDoor)
    {
        RoomObject currentRoom = m_worldGrid[currentLoc.X, currentLoc.Y];
        Door[] roomDoors = currentRoom.RoomDoors;
        int doorIndex;

        // Iterate through all of the doors in the room to determine its index
        for(doorIndex = 0; doorIndex < roomDoors.Length; doorIndex++)
        {
            if (roomDoors[doorIndex] == entryDoor)
            {
                break;
            }
        }

        Door exitDoor = null;

        if (doorIndex == currentRoom.NORTH_DOOR_INDEX)
        {
            Location exitDoorLoc = new Location(currentLoc.X + 1, currentLoc.Y);
            RoomObject exitRoom = m_worldGrid[exitDoorLoc.X, exitDoorLoc.Y];
            exitDoor = exitRoom.RoomDoors[exitRoom.SOUTH_DOOR_INDEX];
        }

        else if (doorIndex == currentRoom.EAST_DOOR_INDEX)
        {
            Location exitDoorLoc = new Location(currentLoc.X, currentLoc.Y + 1);
            RoomObject exitRoom = m_worldGrid[exitDoorLoc.X, exitDoorLoc.Y];
            exitDoor = exitRoom.RoomDoors[exitRoom.WEST_DOOR_INDEX];
        }

        else if (doorIndex == currentRoom.SOUTH_DOOR_INDEX)
        {
            Location exitDoorLoc = new Location(currentLoc.X - 1, currentLoc.Y);
            RoomObject exitRoom = m_worldGrid[exitDoorLoc.X, exitDoorLoc.Y];
            exitDoor = exitRoom.RoomDoors[exitRoom.NORTH_DOOR_INDEX];
        }

        else if (doorIndex == currentRoom.WEST_DOOR_INDEX)
        {
            Location exitDoorLoc = new Location(currentLoc.X, currentLoc.Y - 1);
            RoomObject exitRoom = m_worldGrid[exitDoorLoc.X, exitDoorLoc.Y];
            exitDoor = exitRoom.RoomDoors[exitRoom.EAST_DOOR_INDEX];
        }

        return exitDoor;
    }

    /// <summary>
    /// Moves the camera to the given location, called when a player switches rooms
    /// </summary>
    /// <param name="newCameraLoc">The grid location to move the camera to</param>
    public void MoveCamera(Location newCameraLoc)
    {
        RoomObject newRoom = m_worldGrid[newCameraLoc.X, newCameraLoc.Y];
        CameraNode roomCamNode = newRoom.GetComponentInChildren<CameraNode>();
        Camera.main.transform.position = roomCamNode.transform.position;
    }
}
