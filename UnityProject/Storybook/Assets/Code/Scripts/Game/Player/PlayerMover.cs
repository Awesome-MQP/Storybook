using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class for handling player movement.
/// </summary>
public class PlayerMover : NetworkMover
{

    private RoomObject m_currentRoom;
    private RoomData m_currentRoomData;
    private Location m_currentRoomLoc;
    private List<NetworkNodeMover> m_worldPlayers = new List<NetworkNodeMover>();
    private int m_leaderId = 1;

    public void SpawnInRoom(RoomObject room)
    {
        m_currentRoom = room;
        m_currentRoomLoc = room.RoomLocation;
        MapManager mapManager = FindObjectOfType<MapManager>();
        m_currentRoomData = mapManager.GetRoomData(room.RoomLocation.X, room.RoomLocation.Y);

        photonView.RPC("SendRoomData", PhotonTargets.Others, m_currentRoomData.IsNorthDoorActive, m_currentRoomData.IsEastDoorActive,
            m_currentRoomData.IsSouthDoorActive, m_currentRoomData.IsWestDoorActive, m_currentRoomData.RoomType);
        photonView.RPC("SendRoomLoc", PhotonTargets.Others, m_currentRoomLoc.X, m_currentRoomLoc.Y);

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

    public IEnumerator MoveThroughDoor(Door door)
    {
        ChooseNewLeader();
        foreach (NetworkNodeMover player in m_worldPlayers)
        {
            player.TargetNode = door.DoorNode;
            player.IsAtTarget = false;
        }

        bool areAllPlayersAtDoor = false;
        while (!areAllPlayersAtDoor)
        {
            areAllPlayersAtDoor = true;
            foreach (NetworkNodeMover player in m_worldPlayers)
            {
                if (!player.IsAtTarget)
                {
                    areAllPlayersAtDoor = false;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Switching rooms");
        Location newRoomLoc = door.RoomThroughDoorLoc;
        MapManager mapManager = FindObjectOfType<MapManager>();
        RoomObject nextRoom = mapManager.GetRoom(newRoomLoc);
        RoomData nextRoomData = mapManager.GetRoomData(newRoomLoc.X, newRoomLoc.Y);
        Door newDoor = mapManager.GetDoorPartner(m_currentRoomLoc, door);
    
        foreach (NetworkNodeMover player in m_worldPlayers)
        {
            player.Position = newDoor.DoorNode.transform.position;
            Debug.Log(newDoor.DoorNode.transform.position);
        }

        photonView.RPC("SwitchCameraRoom", PhotonTargets.All, nextRoom.CameraNode.transform.position, nextRoom.CameraNode.transform.rotation);
        m_currentRoomLoc = newRoomLoc;
        m_currentRoom = nextRoom;
        m_currentRoomData = nextRoomData;

        photonView.RPC("SendRoomData", PhotonTargets.Others, m_currentRoomData.IsNorthDoorActive, m_currentRoomData.IsEastDoorActive,
            m_currentRoomData.IsSouthDoorActive, m_currentRoomData.IsWestDoorActive, m_currentRoomData.RoomType);
        photonView.RPC("SendRoomLoc", PhotonTargets.Others, m_currentRoomLoc.X, m_currentRoomLoc.Y);

        _SetTargetNodesForPlayers();
    }

    /// <summary>
    /// Creates a new room at the given coordinates in the room grid and moves the players to this new room
    /// </summary>
    /// <param name="newRoomLocX">The X coordinate of the room to create</param>
    /// <param name="newRoomLocY">The Y coordinate of the room to create</param>
    private void CreateNewRoom(int newRoomLocX, int newRoomLocY)
    {
        Debug.Log("Creating new room");
        MapManager mapManager = FindObjectOfType<MapManager>();
        Location newRoomLoc = new Location(newRoomLocX, newRoomLocY);
        mapManager.PlaceRoom(newRoomLoc);
    }

    void OnGUI()
    {
        if (m_leaderId == PhotonNetwork.player.ID)
        {
            bool isNorthDoorEnabled = m_currentRoomData.IsNorthDoorActive;
            bool isEastDoorEnabled = m_currentRoomData.IsEastDoorActive;
            bool isSouthDoorEnabled = m_currentRoomData.IsSouthDoorActive;
            bool isWestDoorEnabled = m_currentRoomData.IsWestDoorActive;

            // Only allow the player to select a door if the door is enabled for the current room 
            if (isNorthDoorEnabled && GUILayout.Button("Move Up"))
            {
                int newRoomLocX = m_currentRoomLoc.X + 1;
                int newRoomLocY = m_currentRoomLoc.Y;
                photonView.RPC("CreateRoomOnMaster", PhotonTargets.MasterClient, newRoomLocX, newRoomLocY, 0);
            }

            // Only allow the player to select a door if the door is enabled for the current room 
            if (isWestDoorEnabled && GUILayout.Button("Move Left"))
            {
                Location m_newRoomLoc = new Location(m_currentRoomLoc.X, m_currentRoomLoc.Y - 1);
                int newRoomLocX = m_currentRoomLoc.X;
                int newRoomLocY = m_currentRoomLoc.Y - 1;
                photonView.RPC("CreateRoomOnMaster", PhotonTargets.MasterClient, newRoomLocX, newRoomLocY, 3);
            }

            // Only allow the player to select a door if the door is enabled for the current room 
            if (isEastDoorEnabled && GUILayout.Button("Move Right"))
            {
                int newRoomLocX = m_currentRoomLoc.X;
                int newRoomLocY = m_currentRoomLoc.Y + 1;
                photonView.RPC("CreateRoomOnMaster", PhotonTargets.MasterClient, newRoomLocX, newRoomLocY, 1);
            }

            // Only allow the player to select a door if the door is enabled for the current room 
            if (isSouthDoorEnabled && GUILayout.Button("Move Down"))
            {
                Location m_newRoomLoc = new Location(m_currentRoomLoc.X - 1, m_currentRoomLoc.Y);
                int newRoomLocX = m_currentRoomLoc.X - 1;
                int newRoomLocY = m_currentRoomLoc.Y;
                photonView.RPC("CreateRoomOnMaster", PhotonTargets.MasterClient, newRoomLocX, newRoomLocY, 2);
            }
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

    [PunRPC]
    public void SwitchCameraRoom(Vector3 newCamPos, Quaternion newCamRot)
    {
        Camera.main.transform.position = newCamPos;
        Camera.main.transform.rotation = newCamRot;
    }

    [PunRPC]
    public void RegisterCurrentRoom(int roomLocX, int roomLocY)
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        Location roomLoc = new Location(roomLocX, roomLocY);
        m_currentRoomData = mapManager.GetRoomData(roomLocX, roomLocY);
        m_currentRoomLoc = roomLoc;
    }

    public IEnumerator SendRoom()
    {
        yield return new WaitForSeconds(2.0f);
        photonView.RPC("RegisterCurrentRoom", PhotonTargets.Others, m_currentRoomLoc.X, m_currentRoomLoc.Y);
    }

    [PunRPC]
    public void SendRoomData(bool isNorthDoorActive, bool isEastDoorActive, bool isSouthDoorActive, bool isWestDoorActive, int roomType)
    {
        m_currentRoomData = new RoomData(isNorthDoorActive, isEastDoorActive, isSouthDoorActive, isWestDoorActive, (MapManager.RoomType) roomType);
    }

    [PunRPC]
    public void SendRoomLoc(int roomLocX, int roomLocY)
    {
        m_currentRoomLoc = new Location(roomLocX, roomLocY);
    }

    [PunRPC]
    public void CreateRoomOnMaster(int roomLocX, int roomLocY, int doorIndex)
    {
        Door selectedDoor = m_currentRoom.RoomDoors[doorIndex];

        // If the selected door's room has not been spawned, create the room
        if (!selectedDoor.IsDoorRoomSpawned)
        {
            CreateNewRoom(roomLocX, roomLocY);
            selectedDoor.SetIsDoorRoomSpawned(true);
        }

        StartCoroutine(MoveThroughDoor(selectedDoor));
    }

    public void ChooseNewLeader()
    {
        if (m_leaderId < PhotonNetwork.playerList.Length)
        {
            m_leaderId++;
        }
        else
        {
            m_leaderId = 1;
        }

        photonView.RPC("SendNewLeader", PhotonTargets.Others, m_leaderId);
    }

    [PunRPC]
    public void SendNewLeader(int newLeaderId)
    {
        m_leaderId = newLeaderId;
    }
}
