using UnityEngine;
using System.Collections;

public class Door : Photon.PunBehaviour {

    [SerializeField]
    private DoorNode m_doorNode;

    [SerializeField]
    private bool m_isDoorEnabled = true;

    private bool m_isDoorRoomSpawned = false;

    private Location m_roomThroughDoorLoc;

    void Update()
    {
        if (!m_isDoorEnabled)
        {
            DisableDoor();
        }
    }

    // TODO : Add implementation for passing through doors.
    //        (Requires world manager and more complete level-building stuff first.)

    //TODO: This can be changed to just disable the entire object
    /// <summary>
    /// Disables the collider and renderer on the door, as well as setting the enabled boolean to false
    /// </summary>
    public void DisableDoor()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        IsDoorEnabled = false;
    }

    /// <summary>
    /// The grid location of the room that is through the door
    /// </summary>
    public Location RoomThroughDoorLoc
    {
        get { return m_roomThroughDoorLoc; }
    }

    public void SetRoomThroughDoorLoc(Location newRoomThroughDoorLoc)
    {
        m_roomThroughDoorLoc = newRoomThroughDoorLoc;
    }

    /// <summary>
    /// True if the room that the door leads to has spawned, false otherwise
    /// </summary>
    public bool IsDoorRoomSpawned
    {
        get { return m_isDoorRoomSpawned; }
    }

    public void SetIsDoorRoomSpawned(bool newIsDoorRoomSpawned)
    {
        m_isDoorRoomSpawned = newIsDoorRoomSpawned;
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
            PropertyChanged();
        }
    }

    /// <summary>
    /// The door node that corresponds to this door
    /// </summary>
    public DoorNode DoorNode
    {
        get { return m_doorNode; }
    }
}
