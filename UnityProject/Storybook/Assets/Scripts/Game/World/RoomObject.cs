using UnityEngine;
using System.Collections;

abstract public class RoomObject : MonoBehaviour {

    public readonly int NORTH_DOOR_INDEX = 0;
    public readonly int EAST_DOOR_INDEX = 1;
    public readonly int SOUTH_DOOR_INDEX = 2;
    public readonly int WEST_DOOR_INDEX = 3;

<<<<<<< HEAD
    // Property for a Room's location
    public Location RoomLocation
=======
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
    private Location m_roomLocation;
    [SerializeField]
    private int m_roomSize; // Can be x1, x2, x4.
    [SerializeField]
    private Genre m_roomGenre;
    [SerializeField]
    private RoomFeature m_roomFeature;

    [SerializeField]
    private Transform m_player1Pos;

    [SerializeField]
    private Transform m_player2Pos;

    [SerializeField]
    private Transform m_player3Pos;

    [SerializeField]
    private Transform m_player4Pos;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

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
>>>>>>> a8fb1d08f2c2857c6dbd70251c4d196f92639e87
    {
        get { return m_roomLocation; }
        set { m_roomLocation = value; }
    }

    // Property for a Room's Genre
    public Genre RoomGenre
    {
        get { return m_roomGenre; }
        set { m_roomGenre = value; }
    }

    // Property for a Room's Feature
    public string RoomFeature
    {
        get { return m_roomFeature; }
        set { m_roomFeature = value; }
    }

    // The list of room doors
    public Door[] RoomDoors
    {
        get { return m_roomDoors; }
        set { m_roomDoors = value; }
    }

    // Property for the size of a room
    public int RoomSize
    {
        get { return m_roomSize; }
        set { m_roomSize = value; }
    }

    // What to do immediately when we enter the room
    // Parent class does nothing special, so just return
    public void OnRoomEnter()
    {
        return;
    }

    // What to do immediately when we exit the room
    // Parent class does nothing special, so just return
    public void OnRoomExit()
    {
        return;
    }

    // What to do when the room's event activates
    // Parent class does nothing special, so just return
    public void OnRoomEvent()
    {
        return;
    }

    public Transform CameraNode
    {
        get { return m_cameraNode; }
    }

    public Transform Player1Node
    {
        get { return m_player1Pos; }
    }

    public Transform Player2Node
    {
        get { return m_player2Pos; }
    }

    public Transform Player3Node
    {
        get { return m_player3Pos; }
    }

    public Transform Player4Node
    {
        get { return m_player4Pos; }
    }

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
    private Location m_roomLocation;
    [SerializeField]
    private int m_roomSize; // Can be x1, x2, x4.
    [SerializeField]
    private Genre m_roomGenre;
    [SerializeField]
    private string m_roomFeature;

    [SerializeField]
    private Transform m_player1Pos;

    [SerializeField]
    private Transform m_player2Pos;

    [SerializeField]
    private Transform m_player3Pos;

    [SerializeField]
    private Transform m_player4Pos;
}
