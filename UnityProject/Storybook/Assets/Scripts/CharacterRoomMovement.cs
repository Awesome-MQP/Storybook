using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CharacterRoomMovement : NetworkBehaviour {

    [SyncVar]
    private Location m_playerLoc;

    void Start()
    {
        m_playerLoc = new Location(0, 0);
    }

    [ServerCallback]
    void FixedUpdate () {
        // Check to see if the player is at a door
        Door d = _isAtRoomDoor();

        // If the player is at a door, spawn the room for the door if it has not already been spawned
        if (d != null)
        {
            MapManager mapManager = FindObjectOfType<MapManager>();
            if (!d.IsDoorRoomSpawned)
            {
                SpawnNewRoom(d);
            }
            else
            {
                Door exitDoor = mapManager.GetDoorPartner(m_playerLoc, d);
                DoorSpawnNode doorNode = exitDoor.DoorNode;
                RoomObject newRoom = mapManager.GetRoom(d.RoomThroughDoorLoc);
                Transform camNode = newRoom.CameraNode;
                Vector3 newCamPosition = camNode.position;
                RpcMovePlayerToNextRoom(doorNode.transform.position, newCamPosition, d.RoomThroughDoorLoc.X, d.RoomThroughDoorLoc.Y);
            }
        }
    }

    /// <summary>
    /// Checks the distance between the player and all of the doors and returns a door if the player is close enough
    /// </summary>
    /// <returns>Returns a door object if the player is close enough to a door, returns null other wise</returns>
    private Door _isAtRoomDoor()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        RoomObject currentRoom = mapManager.GetRoom(m_playerLoc);
        Door[] roomDoors = currentRoom.RoomDoors;
        foreach (Door d in roomDoors)
        {
            // Only check doors that are enabled
            if (d.IsDoorEnabled)
            {
                Vector3 doorPos = d.transform.position;
                float distanceToDoor = Vector3.Distance(doorPos, transform.position);
                if (distanceToDoor < 1.5)
                {
                    return d;
                }
            }
        }
        return null;
    }

    [ClientRpc]
    public void RpcMovePlayerToNextRoom(Vector3 doorSpawnPos, Vector3 camSpawnPos, int locX, int locY)
    {
        Location newPlayerLoc = new Location(locX, locY);
        m_playerLoc = newPlayerLoc;
        if (isLocalPlayer)
        {
            Camera.main.transform.position = camSpawnPos;
        }
        transform.position = doorSpawnPos;
    }

    [ServerCallback]
    void SpawnNewRoom(Door d)
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        Location newRoomLoc = d.RoomThroughDoorLoc;
        mapManager.PlaceRoom(newRoomLoc);
        d.SetIsDoorRoomSpawned(true);
    }
}
