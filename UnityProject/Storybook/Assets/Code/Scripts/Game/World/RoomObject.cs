using UnityEngine;
using System.Collections;

//TODO: 

public abstract class RoomObject : MonoBehaviour{

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

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // What do we do immediately upon entering the room?
    protected virtual void OnRoomEnter()
    {
        return;
    }

    // What do we do as soon as all players reach the center of the room?
    protected virtual void OnRoomEvent()
    {
        return;
    }

    // What do we do immediately upon leaving the room?
    protected virtual void OnRoomExit()
    {
        return;
    }

    // Property for a Room's Location
    public Location RoomLocation
    {
        get { return m_roomLocation; }
        set { m_roomLocation = value; }
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

    public Genre RoomGenre
    {
        get { return m_roomGenre; }
        set { m_roomGenre = value; }
    }

    // Get Feature of a room
    public string RoomFeature
    {
        get { return m_roomFeature; }
        set { m_roomFeature = value; }
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
}
