using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {

    [SerializeField]
    private int m_worldMaxXSize = 12;

    [SerializeField]
    private int m_worldMaxYSize = 12;

    [SerializeField]
    private RoomObject[,] m_worldGrid; // Creates a 2D array to place rooms

    // Initialize
    void Awake()
    {
        m_worldGrid = new RoomObject[m_worldMaxXSize, m_worldMaxYSize];
    }
	
    // Place a new room in the world.
    // The MapMgr is not concerned with the contents of the room, just that it can be placed.
    // 
    public RoomObject PlaceRoom(Location gridPosition, RoomObject theRoom)
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
        m_worldGrid[placeX, placeY] = theRoom;
        return theRoom;
    }

    // Tests the Map Manager code. Called once on startup.
    void TestMapMgr()
    {
        RoomObject testRoom = new RoomObject();
        Location OOB_X = new Location(-1, 0);
        Location OOB_Y = new Location(0, 13);
        Location OOB_Both = new Location(25, 25);
        Location Good_1 = new Location(5, 5);
        Location Good_2 = new Location(5, 6);
        PlaceRoom(OOB_X, testRoom); // try placing a room out of bounds in X (expect fail)
        PlaceRoom(OOB_Y, testRoom); // try placing a room OOB in Y (expect fail)
        PlaceRoom(OOB_Both, testRoom); // try placing a room OOB in both directions (expect fail)
        PlaceRoom(Good_1, testRoom); // try placing a room in a good location (expect pass)
        PlaceRoom(Good_2, testRoom); // try placing a room in another good location (expect pass)
        PlaceRoom(Good_1, testRoom); // try placing a room where another room has already been placed (expect fail)
    }
}
