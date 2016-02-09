using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;
using System;
using UnityEngine.Assertions;

public abstract class RoomObject : PunBehaviour, IConstructable<RoomData>
{
    private HashSet<string> m_featureSet;

    [SerializeField]
    Transform m_cameraNode;

    [SerializeField]
    private Location m_roomLocation;

    [SerializeField]
    [Obsolete("We no longer use room size with rooms.")]
    private int m_roomSize; // Can be x1, x2, x4.

    [SerializeField]
    private string[] m_roomFeatures = new string[0];

    [SerializeField]
    private Door m_northDoor;

    [SerializeField]
    private Door m_eastDoor;

    [SerializeField]
    private Door m_southDoor;

    [SerializeField]
    private Door m_westDoor;

    [SerializeField]
    private MovementNode m_centerNode;

    [SerializeField]
    protected Transform m_floorObject;

    private PageData m_roomPageData;

    protected override void Awake()
    {
        base.Awake();

        m_featureSet = new HashSet<string>();
        foreach (string feature in m_roomFeatures)
        {
            m_featureSet.Add(feature.ToUpper());
        }
    }

    public void Construct(RoomData room)
    {
        Assert.IsTrue(IsMine);

        RoomLocation = new Location(room.X, room.Y);

        m_northDoor.IsDoorEnabled = room.IsNorthDoorActive;
        m_northDoor.DoorLocation = RoomLocation;
        m_northDoor.DoorDirection = Door.Direction.North;

        m_eastDoor.IsDoorEnabled = room.IsEastDoorActive;
        m_eastDoor.DoorLocation = RoomLocation;
        m_eastDoor.DoorDirection = Door.Direction.East;

        m_southDoor.IsDoorEnabled = room.IsSouthDoorActive;
        m_southDoor.DoorLocation = RoomLocation;
        m_southDoor.DoorDirection = Door.Direction.South;

        m_westDoor.IsDoorEnabled = room.IsWestDoorActive;
        m_westDoor.DoorLocation = RoomLocation;
        m_westDoor.DoorDirection = Door.Direction.West;
    }

    public void RoomEntered(RoomMover mover)
    {
        OnRoomEnter(mover);
    }

    public IEnumerable RoomEvent(RoomMover mover)
    {
        BasePlayerMover playerMover = mover as BasePlayerMover;
        if(playerMover != null)
            EventDispatcher.GetDispatcher<PlayerControlEventDispatcher>().OnPreRoomEvent(playerMover, this);

        foreach (var v in OnRoomEvent(mover))
        {
            yield return v;
        }

        if (playerMover != null)
            EventDispatcher.GetDispatcher<PlayerControlEventDispatcher>().OnPostRoomEvent(playerMover, this);
    }

    public void RoomExit(RoomMover mover)
    {
        OnRoomExit(mover);
    }

    [SyncProperty]
    public Location RoomLocation
    {
        get { return m_roomLocation; }
        protected set
        {
            m_roomLocation = value;
            MapManager.Instance.RegisterRoom(this);
            PropertyChanged();
        }
    }

    //TODO: Array allocation every time this is used, might be better to just have getters.

    public Door[] AllDoors
    {
        get
        {
            Door[] doorArray = {
                m_northDoor,
                m_eastDoor,
                m_southDoor,
                m_westDoor
            };
            return doorArray;
        }
    }

    public Door NorthDoor
    {
        get { return m_northDoor; }
    }

    public Door EastDoor
    {
        get { return m_eastDoor; }
    }

    public Door SouthDoor
    {
        get { return m_southDoor; }
    }

    public Door WestDoor
    {
        get { return m_westDoor; }
    }

    // Property for the size of a room

    [Obsolete]
    public int RoomSize
    {
        get { return m_roomSize; }
    }

    public MovementNode CenterNode
    {
        get { return m_centerNode; }
    }

    public Transform CameraNode
    {
        get { return m_cameraNode; }
    }

    public PageData RoomPageData
    {
        get { return m_roomPageData; }
        set { m_roomPageData = value; }
    }

    /// <summary>
    /// Get the door in a direction.
    /// </summary>
    /// <param name="direction">The direction to get a door in.</param>
    /// <returns>The door in that direction, or null if there is no door in that direction.</returns>
    public Door GetDoorByDirection (Door.Direction direction)
    {
        Door doorToReturn;
        switch (direction)
        {
            case Door.Direction.North:
                doorToReturn = m_northDoor;
                break;

            case Door.Direction.East:
                doorToReturn = m_eastDoor;
                break;

            case Door.Direction.South:
                doorToReturn = m_southDoor;
                break;

            case Door.Direction.West:
                doorToReturn = m_westDoor;
                break;

            default:
                throw new ArgumentOutOfRangeException("direction", direction, null);
        }

        return doorToReturn.IsDoorEnabled ? doorToReturn : null;
    }

    /// <summary>
    /// Checks to see if the room contains a feature.
    /// </summary>
    /// <param name="feature">The feature to check for.</param>
    /// <returns>True if the room contains this feature, false otherwise.</returns>
    public bool ContainsFeature(string feature)
    {
        return feature == null || m_featureSet.Contains(feature.ToUpper());
    }

    /// <summary>
    /// Checks to see if the room contains a list of features.
    /// </summary>
    /// <param name="features">The list of features to check for.</param>
    /// <returns>True if all features are contained, otherwise false.</returns>
    public bool ContainsFeatures(params string[] features)
    {
        foreach (string feature in features)
        {
            if (!ContainsFeature(feature))
                return false;
        }

        return true;
    }

    protected abstract void OnRoomEnter(RoomMover mover);

    protected abstract IEnumerable OnRoomEvent(RoomMover mover);

    protected abstract void OnRoomExit(RoomMover mover);
}
