using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class for handling player movement.
/// </summary>
public class PlayerMover : NetworkMover
{

    private RoomObject m_currentRoom;
    private Location m_currentRoomLoc;
    private List<NetworkNodeMover> m_worldPlayers = new List<NetworkNodeMover>();

    public void SpawnInRoom(RoomObject room)
    {
        m_currentRoom = room;
        m_currentRoomLoc = room.RoomLocation;

        foreach(NetworkNodeMover player in m_worldPlayers)
        {
            player.IsAtTarget = true;
        }

        if (m_worldPlayers.Count >= 1)
        {
            m_worldPlayers[0].transform.position = room.Player1Node.transform.position;
            m_worldPlayers[0].TargetNode = room.Player1Node;
        }
        if (m_worldPlayers.Count >= 2)
        {
            m_worldPlayers[1].transform.position = room.Player2Node.transform.position;
            m_worldPlayers[1].TargetNode = room.Player2Node;
        }
        if (m_worldPlayers.Count >= 3)
        {
            m_worldPlayers[2].transform.position = room.Player3Node.transform.position;
            m_worldPlayers[2].TargetNode = room.Player3Node;
        }
        if (m_worldPlayers.Count >= 4)
        {
            m_worldPlayers[3].transform.position = room.Player4Node.transform.position;
            m_worldPlayers[3].TargetNode = room.Player4Node;
        }
    }

    public void MoveThroughDoor(Door door, bool arePlayersAtDoor)
    {
        if (!arePlayersAtDoor)
        {
            foreach (NetworkNodeMover player in m_worldPlayers)
            {
                player.TargetNode = door.DoorNode;
                player.IsAtTarget = false;
            }
        }
        else
        {
            Debug.Log("Switching rooms");
            Location newRoomLoc = door.RoomThroughDoorLoc;
            MapManager mapManager = FindObjectOfType<MapManager>();
            RoomObject nextRoom = mapManager.GetRoom(newRoomLoc);
            Door newDoor = mapManager.GetDoorPartner(m_currentRoomLoc, door);

            foreach (NetworkNodeMover player in m_worldPlayers)
            {
                player.transform.position = newDoor.DoorNode.transform.position;
                Debug.Log(newDoor.DoorNode.transform.position);
            }

            Camera.main.transform.position = nextRoom.CameraNode.transform.position;
            Camera.main.transform.rotation = nextRoom.CameraNode.transform.rotation;
            m_currentRoomLoc = newRoomLoc;
            m_currentRoom = nextRoom;
            _SetTargetNodesForPlayers();
        }
    }

    /// <summary>
    /// Creates a new room at the given coordinates in the room grid and moves the players to this new room
    /// </summary>
    /// <param name="newRoomLocX">The X coordinate of the room to create</param>
    /// <param name="newRoomLocY">The Y coordinate of the room to create</param>
    [PunRPC]
    private void CreateNewRoom(int newRoomLocX, int newRoomLocY)
    {
        Debug.Log("Creating new room");
        MapManager mapManager = FindObjectOfType<MapManager>();
        Location newRoomLoc = new Location(newRoomLocX, newRoomLocY);
        mapManager.PlaceRoom(newRoomLoc);
    }

    void OnGUI()
    {
        Door northDoor = m_currentRoom.RoomDoors[m_currentRoom.NORTH_DOOR_INDEX];
        Door westDoor = m_currentRoom.RoomDoors[m_currentRoom.WEST_DOOR_INDEX];
        Door eastDoor = m_currentRoom.RoomDoors[m_currentRoom.EAST_DOOR_INDEX];
        Door southDoor = m_currentRoom.RoomDoors[m_currentRoom.SOUTH_DOOR_INDEX];

        // Only allow the player to select a door if the door is enabled for the current room 
        if (northDoor.IsDoorEnabled && GUILayout.Button("Move Up"))
        {
            Location m_newRoomLoc = new Location(m_currentRoomLoc.X + 1, m_currentRoomLoc.Y);

            // If the selected door's room has not been spawned, create the room
            if (!northDoor.IsDoorRoomSpawned)
            {
                photonView.RPC("CreateNewRoom", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);
                northDoor.SetIsDoorRoomSpawned(true);
            }

            MoveThroughDoor(northDoor, false);
        }

        // Only allow the player to select a door if the door is enabled for the current room 
        if (westDoor.IsDoorEnabled && GUILayout.Button("Move Left"))
        {
            Location m_newRoomLoc = new Location(m_currentRoomLoc.X, m_currentRoomLoc.Y - 1);

            // If the selected door's room has not been spawned, create the room
            if (!westDoor.IsDoorRoomSpawned)
            {
                photonView.RPC("CreateNewRoom", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);
                westDoor.SetIsDoorRoomSpawned(true);
            }

            MoveThroughDoor(westDoor, false);
        }

        // Only allow the player to select a door if the door is enabled for the current room 
        if (eastDoor.IsDoorEnabled && GUILayout.Button("Move Right"))
        {
            Location m_newRoomLoc = new Location(m_currentRoomLoc.X, m_currentRoomLoc.Y + 1);

            // If the selected door's room has not been spawned, create the room
            if (!eastDoor.IsDoorRoomSpawned)
            {
                photonView.RPC("CreateNewRoom", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);
                eastDoor.SetIsDoorRoomSpawned(true);
            }

            MoveThroughDoor(eastDoor, false);
        }

        // Only allow the player to select a door if the door is enabled for the current room 
        if (southDoor.IsDoorEnabled && GUILayout.Button("Move Down"))
        {
            Location m_newRoomLoc = new Location(m_currentRoomLoc.X - 1, m_currentRoomLoc.Y);

            // If the selected door's room has not been spawned, create the room
            if (!southDoor.IsDoorRoomSpawned)
            {
                photonView.RPC("CreateNewRoom", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);
                southDoor.SetIsDoorRoomSpawned(true);
            }

            MoveThroughDoor(southDoor, false);
        }
    }

    public List<NetworkNodeMover> WorldPlayers
    {
        get { return m_worldPlayers; }
        set { m_worldPlayers = value; }
    }

    private void _SetTargetNodesForPlayers()
    {
        if (m_worldPlayers.Count >= 1)
        {
            m_worldPlayers[0].TargetNode = m_currentRoom.Player1Node;
            m_worldPlayers[0].IsAtTarget = false;
        }
        if (m_worldPlayers.Count >= 2)
        {
            m_worldPlayers[1].TargetNode = m_currentRoom.Player2Node;
            m_worldPlayers[1].IsAtTarget = false;
        }
        if (m_worldPlayers.Count >= 3)
        {
            m_worldPlayers[2].TargetNode = m_currentRoom.Player3Node;
            m_worldPlayers[2].IsAtTarget = false;
        }
        if (m_worldPlayers.Count >= 4)
        {
            m_worldPlayers[3].TargetNode = m_currentRoom.Player4Node;
            m_worldPlayers[3].IsAtTarget = false;
        }
    }
}
