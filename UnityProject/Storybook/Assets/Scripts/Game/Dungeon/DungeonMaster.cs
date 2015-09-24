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
    }

    /// <summary>
    /// Gets a room prefab to place in the world based on certain input criteria.
    /// </summary>
    /// <param name="size">The specified size of the room to search for.</param>
    /// <param name="genre">The specified Genre of the room to search for.</param>
    /// <param name="feature">The specified Feature of the room to search for.</param>
    /// <returns>The room if a match is found, or null if no match is found.</returns>
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
            return null;
        }

        // Now that we have found all potential matching rooms, choose one to place.
        int roomChooser = Random.Range(0, goodRooms.Count);
        RoomObject roomToBuild = goodRooms[roomChooser];

        // TODO: Send to WorldManager.placeRoom()?
        return roomToBuild;
    }

    /// <summary>
    /// Tests the GetRoomPrefab function.
    /// </summary>
    void TestGetRoomPrefab()
    {
        GetRoomPrefab(1, Genre.SciFi, RoomFeature.Curse);
        GetRoomPrefab(4, Genre.Horror, RoomFeature.Shop);
        GetRoomPrefab(2, Genre.Fantasy, RoomFeature.Treasure);
    }
}
