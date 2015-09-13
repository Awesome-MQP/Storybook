using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {

    protected const int WORLD_MAX_X_SIZE = 12;
    protected const int WORLD_MAX_Y_SIZE = 12;

    protected RoomObject[,] m_WorldGrid = new RoomObject[WORLD_MAX_X_SIZE, WORLD_MAX_Y_SIZE]; // Creates a 12x12 array to place rooms

	// Use this for initialization
	void Start () {
        TestMapMgr(); // run tests on map manager code once on startup.
	}
	
    // Place a new room in the world.
    // The MapMgr is not concerned with the contents of the room, just that it can be placed.
    // 
    protected RoomObject PlaceRoom(Location gridPosition, RoomObject theRoom) {
        int placeX = gridPosition.getX();
        int placeY = gridPosition.getY();
        // First, check to make sure the location is valid! Can't have rooms hanging off the edge of the map.
        if ((placeX < 0 || placeX >= WORLD_MAX_X_SIZE) || 
            (placeY < 0 || placeY >= WORLD_MAX_Y_SIZE)) {
            Debug.Log("Cannot place a room at position: " + placeX + "," + placeY + ". Position is invalid.");
            return null;
        }
        // Now check to make sure there isn't a room already in the spot.
        // The player cannot overwrite rooms that have already been placed.
        if(m_WorldGrid[placeX, placeY] != null) {
            Debug.Log("Cannot place a room at position: " + placeX + "," + placeY + " A room already exists here.");
            return null;
        }
        // If we got here, then the location is assumed to be valid.
        // Place the room.
        m_WorldGrid[placeX, placeY] = theRoom;
        Debug.Log("Room was placed successfully at position: " + placeX + "," + placeY);
        return theRoom;
    }

	// Update is called once per frame
	void Update () {
	
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
