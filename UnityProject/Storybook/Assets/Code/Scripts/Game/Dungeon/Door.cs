using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using Random = System.Random;

public class Door : MovementNode
{
    public enum Direction
    {
        North,
        East,
        South,
        West,
        Unknown
    }

    private bool m_isDoorEnabled = true;

    private bool m_isDoorOpen;

    private RoomObject m_parentRoomObject;

    private Location m_doorLocation;

    private Direction m_direction = Direction.Unknown;

    private Location m_nextRoomLocation;

    protected override void Awake()
    {
        m_parentRoomObject = this.GetComponentInParent<RoomObject>(true);

        base.Awake();
    }

    /// <summary>
    /// True if the door is enabled in the room, false otherwise
    /// </summary>
    [SyncProperty]
    public bool IsDoorEnabled
    {
        get { return m_isDoorEnabled; }
        set
        {
            m_isDoorEnabled = value;
            GetComponent<CapsuleCollider>().enabled = value;
            GetComponent<MeshRenderer>().enabled = value;
            PropertyChanged();
        }
    }

    /// <summary>
    /// If true then the door is open and useable, otherwise false.
    /// </summary>
    [SyncProperty]
    public bool IsDoorOpen
    {
        get { return m_isDoorOpen; }
        protected set
        {
            m_isDoorOpen = value;

            if (m_isDoorOpen)
            {
                OnDoorOpen();
            }
            else
            {
                OnDoorClose();
            }

            PropertyChanged();
        }
    }

    /// <summary>
    /// The direction that this door goes in.
    /// </summary>
    [SyncProperty]
    public Direction DoorDirection
    {
        get { return m_direction; }
        set
        {
            Assert.AreEqual(Direction.Unknown, m_direction);

            m_direction = value;

            m_nextRoomLocation = m_doorLocation + m_direction;

            PropertyChanged();
        }
    }

    /// <summary>
    /// The room object that owns this door.
    /// </summary>
    public RoomObject ParentRoom
    {
        get { return m_parentRoomObject; }
    }

    /// <summary>
    /// The next location to go to.
    /// </summary>
    public Location NextRoomLocation
    {
        get { return m_nextRoomLocation; }
    }

    /// <summary>
    /// The location of the room that the door is in
    /// </summary>
    public Location DoorLocation
    {
        get { return m_doorLocation; }
        set { m_doorLocation = value; }
    }

    /// <summary>
    /// The door that this door links to.
    /// </summary>
    public Door LinkedDoor
    {
        get { return MapManager.Instance.GetDoorPartner(ParentRoom.RoomLocation, this); }
    }

    /// <summary>
    /// Disables the collider and renderer on the door, as well as setting the enabled boolean to false
    /// </summary>
    public void DisableDoor()
    {
        IsDoorEnabled = false;
    }

    public void OpenDoor()
    {
        if (IsMine)
        {
            IsDoorOpen = true;
        }
    }

    public void CloseDoor()
    {
        if (IsMine)
        {
            IsDoorOpen = false;
        }
    }

    protected void OnDoorOpen()
    {
#if UNITY_EDITOR
        Debug.Log("Door open {0}.", this);
#endif
    }

    protected void OnDoorClose()
    {
        
    }

    protected override void OnEnter(NetworkNodeMover mover)
    { 
    }

    protected override void OnLeave(NetworkNodeMover mover)
    {
    }
}
