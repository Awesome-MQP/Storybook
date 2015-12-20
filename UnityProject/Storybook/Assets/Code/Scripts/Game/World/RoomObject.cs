using UnityEngine;
using System.Collections;

public abstract class RoomObject : MonoBehaviour {

    public enum DoorIndex { NorthDoor = 0, EastDoor, SouthDoor, WestDoor };

    [SerializeField]
    Transform m_cameraNode;

    [SerializeField]
    private Location m_roomLocation;

    [SerializeField]
    private int m_roomSize; // Can be x1, x2, x4.

    [SerializeField]
    private Genre m_roomGenre;

    [SerializeField]
    private string m_roomFeature;

    [SerializeField]
    private MovementNode m_player1Pos;

    [SerializeField]
    private MovementNode m_player2Pos;

    [SerializeField]
    private MovementNode m_player3Pos;

    [SerializeField]
    private MovementNode m_player4Pos;

    [SerializeField]
    private Door m_northDoor;

    [SerializeField]
    private Door m_eastDoor;

    [SerializeField]
    private Door m_southDoor;

    [SerializeField]
    private Door m_westDoor;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // What do we do immediately upon entering the room?
    public virtual void OnRoomEnter()
    {
        return;
    }

    // What do we do as soon as all players reach the center of the room?
    public virtual void OnRoomEvent()
    {
        return;
    }

    // What do we do immediately upon leaving the room?
    public virtual void OnRoomExit()
    {
        return;
    }

    // Property for a Room's Location
    public Location RoomLocation
    {
        get { return m_roomLocation; }
        set { m_roomLocation = value; }
    }

    public Door[] AllDoors
    {
        get
        {
            Door[] doorArray = new Door[4];
            doorArray[0] = m_northDoor;
            doorArray[1] = m_eastDoor;
            doorArray[2] = m_southDoor;
            doorArray[3] = m_westDoor;
            return doorArray;
        }
    }

    public Door NorthDoor
    {
        get { return m_northDoor; }
        set { m_northDoor = value; }
    }

    public Door EastDoor
    {
        get { return m_eastDoor; }
        set { m_eastDoor = value; }
    }

    public Door SouthDoor
    {
        get { return m_southDoor; }
        set { m_southDoor = value; }
    }

    public Door WestDoor
    {
        get { return m_westDoor; }
        set { m_westDoor = value; }
    }

    public Door GetDoorByIndex (DoorIndex doorIndex)
    {
        Door doorToReturn = null;
        switch (doorIndex)
        {
            case DoorIndex.NorthDoor:
                doorToReturn = m_northDoor;
                break;

            case DoorIndex.EastDoor:
                doorToReturn = m_eastDoor;
                break;

            case DoorIndex.SouthDoor:
                doorToReturn = m_southDoor;
                break;

            case DoorIndex.WestDoor:
                doorToReturn = m_westDoor;
                break;
        }
        return doorToReturn;
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

    public MovementNode Player1Node
    {
        get { return m_player1Pos; }
    }

    public MovementNode Player2Node
    {
        get { return m_player2Pos; }
    }

    public MovementNode Player3Node
    {
        get { return m_player3Pos; }
    }

    public MovementNode Player4Node
    {
        get { return m_player4Pos; }
    }
}
