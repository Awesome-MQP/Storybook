using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    [SerializeField]
    private DoorSpawnNode m_doorNode;

    private bool m_isDoorEnabled = true;
    private bool m_isDoorRoomSpawned = false;

    private Location m_roomThroughDoorLoc;

    // TODO : Add implementation for passing through doors.
    //        (Requires world manager and more complete level-building stuff first.)


    /// <summary>
    /// Disables the collider and renderer on the door, as well as setting the enabled boolean to false
    /// </summary>
    public void DisableDoor()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        m_isDoorEnabled = false;
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
    public bool IsDoorEnabled
    {
        get { return m_isDoorEnabled; }
    }

    /// <summary>
    /// The door node that corresponds to this door
    /// </summary>
    public DoorSpawnNode DoorNode
    {
        get { return m_doorNode; }
    }
}
