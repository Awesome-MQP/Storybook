using UnityEngine;
using System.Collections;
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
    private FileInfo[] m_rooms;


	// When the DungeonMaster is spawned in the world, have it immediately get all the room prefabs.
	void Awake () {
        m_rooms = m_roomFilesLocation.GetFiles(); // Fills "rooms" with all the prefabs in the Dungeon/Rooms folder.
    }
	
    // Gets a room prefab to place in the world based on certain input criteria.
    
}
