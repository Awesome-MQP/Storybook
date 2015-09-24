using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Dungeon Master class is the handler for all statistics, as well as handling
// the location of all room prefabs.
public class DungeonMaster : MonoBehaviour{

    // Location of all the rooms
    [SerializeField]
    private DirectoryInfo m_roomFilesLocation = new DirectoryInfo("Assets/Scripts/Game/Dungeon/Rooms");

    // List of room prefabs in the Dungeon/Rooms folder
    // These are all the rooms that can be spawned
    [SerializeField]
    private RoomObject[] m_rooms;


	// When the DungeonMaster is spawned in the world, have it immediately get all the room prefabs.
	void Awake () {
        m_rooms = Resources.LoadAll<RoomObject>("RoomPrefabs");
        Debug.Log("Number of room prefabs found: " + m_rooms.Length);
        TestGetRoomPrefab();
    }
	
    // Gets a room prefab to place in the world based on certain input criteria.
    RoomObject GetRoomPrefab(int size, Genre genre, RoomFeature feature)
    {
        // List of "good" rooms - ones that match the criteria passed in.
        List<RoomObject> goodRooms = new List<RoomObject>();

        // Check each room to see if there is a match
        foreach(RoomObject r in m_rooms)
        {
            if(r.GetRoomSize() != size)
            {
                continue;
            }
            else if(r.GetRoomGenre() != genre)
            {
                continue;
            }
            else if(r.GetRoomFeature() != feature)
            {
                continue;
            }
            else
            {
                // Room is good! Now add it to the candidates for rooms to place.
                goodRooms.Add(r);
            }
        }
        // How many matches did we get?
        if(goodRooms.Count == 0)
        {
            Debug.Log("No matches for room size: " + size + ", Genre: " + genre + ", Feature: " + feature);
            return null;
        }
        Debug.Log("Found " + goodRooms.Count + " rooms with size: " + size + ", Genre: " + genre + ", Feature: " + feature + ".");

        // Now that we have found all potential matching rooms, choose one to place.
        int roomChooser = Random.Range(0, goodRooms.Count);
        RoomObject roomToBuild = goodRooms[roomChooser];

        // TODO: Send to WorldManager.placeRoom()?
        return roomToBuild;
    }

    
    void TestGetRoomPrefab()
    {
        GetRoomPrefab(1, Genre.SciFi, RoomFeature.Curse);
        GetRoomPrefab(4, Genre.Horror, RoomFeature.Shop);
        GetRoomPrefab(2, Genre.Fantasy, RoomFeature.Treasure);
    }
}
