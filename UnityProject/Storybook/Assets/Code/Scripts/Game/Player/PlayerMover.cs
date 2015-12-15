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
    private bool m_areButtonsEnabled = false;

    /// <summary>
    /// Spawns all the players in the given room object
    /// Called only to place the players in the starting room of the floor
    /// </summary>
    /// <param name="room">The room to spawn the players in</param>
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

        AreButtonsEnabled = true;
    }

    /// <summary>
    /// Moves all of the players through the given door
    /// It is a coroutine so that it can update the player positions once they reach the door
    /// </summary>
    /// <param name="door">The door for the players to go through</param>
    /// <returns></returns>
    public IEnumerator MoveThroughDoor(Door door)
    {
        AreButtonsEnabled = false;
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
        m_currentRoom.OnRoomExit();

        Location newRoomLoc = door.RoomThroughDoorLoc;
        MapManager mapManager = FindObjectOfType<MapManager>();

        RoomObject nextRoom = mapManager.GetRoom(newRoomLoc);
        RoomData nextRoomData = mapManager.GetRoomData(newRoomLoc.X, newRoomLoc.Y);
        Door newDoor = mapManager.GetDoorPartner(m_currentRoomLoc, door);
    
        foreach (NetworkNodeMover player in m_worldPlayers)
        {
            player.Position = newDoor.DoorNode.transform.position;
        }

        photonView.RPC("SwitchCameraRoom", PhotonTargets.All, nextRoom.CameraNode.transform.position, nextRoom.CameraNode.transform.rotation);
        m_currentRoomLoc = newRoomLoc;
        m_currentRoom = nextRoom;
        m_currentRoomData = nextRoomData;

        photonView.RPC("SendRoomData", PhotonTargets.Others, m_currentRoomData.IsNorthDoorActive, m_currentRoomData.IsEastDoorActive,
            m_currentRoomData.IsSouthDoorActive, m_currentRoomData.IsWestDoorActive, m_currentRoomData.RoomType);
        photonView.RPC("SendRoomLoc", PhotonTargets.Others, m_currentRoomLoc.X, m_currentRoomLoc.Y);

        m_currentRoom.OnRoomEnter();

        _SetTargetNodesForPlayers();

        while (!_areAllPlayersAtTarget())
        {
            yield return new WaitForSeconds(0.1f);
        }

        m_currentRoom.OnRoomEvent();

        AreButtonsEnabled = true;
    }

    /// <summary>
    /// Creates a new room at the given coordinates in the room grid and moves the players to this new room
    /// </summary>
    /// <param name="newRoomLocX">The X coordinate of the room to create</param>
    /// <param name="newRoomLocY">The Y coordinate of the room to create</param>
    private void CreateNewRoom(int newRoomLocX, int newRoomLocY)
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        Location newRoomLoc = new Location(newRoomLocX, newRoomLocY);
        mapManager.PlaceRoom(newRoomLoc);
    }

    void OnGUI()
    {
        // Only allow the leader to choose the direction when the buttons are enabled
        if (m_leaderId == PhotonNetwork.player.ID && AreButtonsEnabled)
        {
            bool isNorthDoorEnabled = m_currentRoomData.IsNorthDoorActive;
            bool isEastDoorEnabled = m_currentRoomData.IsEastDoorActive;
            bool isSouthDoorEnabled = m_currentRoomData.IsSouthDoorActive;
            bool isWestDoorEnabled = m_currentRoomData.IsWestDoorActive;

            // Only allow the player to select a door if the door is enabled for the current room 
            if (isNorthDoorEnabled && GUILayout.Button("Move Up"))
            {
                int newRoomLocX = m_currentRoomLoc.X - 1;
                int newRoomLocY = m_currentRoomLoc.Y;
                photonView.RPC("CreateRoomOnMaster", PhotonTargets.MasterClient, newRoomLocX, newRoomLocY, RoomObject.DoorIndex.NorthDoor);
            }

            // Only allow the player to select a door if the door is enabled for the current room 
            if (isWestDoorEnabled && GUILayout.Button("Move Left"))
            {
                int newRoomLocX = m_currentRoomLoc.X;
                int newRoomLocY = m_currentRoomLoc.Y - 1;
                photonView.RPC("CreateRoomOnMaster", PhotonTargets.MasterClient, newRoomLocX, newRoomLocY, RoomObject.DoorIndex.WestDoor);
            }

            // Only allow the player to select a door if the door is enabled for the current room 
            if (isEastDoorEnabled && GUILayout.Button("Move Right"))
            {
                int newRoomLocX = m_currentRoomLoc.X;
                int newRoomLocY = m_currentRoomLoc.Y + 1;
                photonView.RPC("CreateRoomOnMaster", PhotonTargets.MasterClient, newRoomLocX, newRoomLocY, RoomObject.DoorIndex.EastDoor);
            }

            // Only allow the player to select a door if the door is enabled for the current room 
            if (isSouthDoorEnabled && GUILayout.Button("Move Down"))
            {
                int newRoomLocX = m_currentRoomLoc.X + 1;
                int newRoomLocY = m_currentRoomLoc.Y;
                photonView.RPC("CreateRoomOnMaster", PhotonTargets.MasterClient, newRoomLocX, newRoomLocY, RoomObject.DoorIndex.SouthDoor);
            }
        }
    }

    /// <summary>
    /// The list of players that are currently in the dungeon
    /// </summary>
    public List<NetworkNodeMover> WorldPlayers
    {
        get { return m_worldPlayers; }
        set { m_worldPlayers = value; }
    }

    /// <summary>
    /// Sets the target nodes of the all the players to the corresponding player nodes
    /// </summary>
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

    /// <summary>
    /// Moves the camera to the new position and rotation
    /// Called when the players switch rooms
    /// </summary>
    /// <param name="newCamPos">The new position for the camera</param>
    /// <param name="newCamRot">The new rotation for the camera</param>
    [PunRPC]
    public void SwitchCameraRoom(Vector3 newCamPos, Quaternion newCamRot)
    {
        Camera.main.transform.position = newCamPos;
        Camera.main.transform.rotation = newCamRot;
    }

    /// <summary>
    /// Sends the current room data to the clients
    /// </summary>
    /// <param name="isNorthDoorActive">True if the north door is active for the current room, false otherwise</param>
    /// <param name="isEastDoorActive">True if the east door is active for the current room, false otherwise</param>
    /// <param name="isSouthDoorActive">True if the south door is active for the current room, false otherwise</param>
    /// <param name="isWestDoorActive">True if the west door is active for the current room, false otherwise</param>
    /// <param name="roomType">The type of the room (MapManager.RoomType)</param>
    [PunRPC]
    public void SendRoomData(bool isNorthDoorActive, bool isEastDoorActive, bool isSouthDoorActive, bool isWestDoorActive, int roomType)
    {
        m_currentRoomData = new RoomData(isNorthDoorActive, isEastDoorActive, isSouthDoorActive, isWestDoorActive, (MapManager.RoomType) roomType);
    }

    /// <summary>
    /// Sends the location of the current room to the clients
    /// </summary>
    /// <param name="roomLocX">The x-coordinate of the current room</param>
    /// <param name="roomLocY">The y-coordinate of the current room</param>
    [PunRPC]
    public void SendRoomLoc(int roomLocX, int roomLocY)
    {
        m_currentRoomLoc = new Location(roomLocX, roomLocY);
    }

    /// <summary>
    /// Called on clients to create a room on the server
    /// </summary>
    /// <param name="roomLocX">The x-coordinate of the room to create</param>
    /// <param name="roomLocY">The y-coordinate of the room to create</param>
    /// <param name="doorIndex">The door that was selected by the client</param>
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

    /// <summary>
    /// The photon player id of the current leader
    /// </summary>
    [SyncProperty]
    public int LeaderId
    {
        get { return m_leaderId; }
        set
        {
            m_leaderId = value;
            PropertyChanged();
        }
    }

    /// <summary>
    /// True when the players are waiting in the center of the room, false while they are moving
    /// If it is true, the buttons will appear to the leader, if it false, they will not appear
    /// </summary>
    [SyncProperty]
    public bool AreButtonsEnabled
    {
        get { return m_areButtonsEnabled; }
        set
        {
            m_areButtonsEnabled = value;
            PropertyChanged();
        }
    }

    /// <summary>
    /// Chooses a new party leader to select the door of the current room
    /// </summary>
    public void ChooseNewLeader()
    {
        if (m_leaderId < PhotonNetwork.playerList.Length)
        {
            LeaderId = m_leaderId + 1;
        }
        else
        {
            LeaderId = 1;
        }
    }

    private bool _areAllPlayersAtTarget()
    {
        foreach(NetworkNodeMover player in m_worldPlayers)
        {
            if (!player.IsAtTarget)
            {
                return false;
            }
        }
        return true;
    }

    public void ReturnCameraToDungeon()
    {
        photonView.RPC("SwitchCameraRoom", PhotonTargets.All, m_currentRoom.CameraNode.transform.position, m_currentRoom.CameraNode.transform.rotation);
    }

    /*
    /// <summary>
    /// Registers the current room on local clients from the given coordinates
    /// </summary>
    /// <param name="roomLocX">The x-coordinate of the room to register</param>
    /// <param name="roomLocY">The y-coordinate of the room to register</param>
    [PunRPC]
    public void RegisterCurrentRoom(int roomLocX, int roomLocY)
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        Location roomLoc = new Location(roomLocX, roomLocY);
        m_currentRoomData = mapManager.GetRoomData(roomLocX, roomLocY);
        m_currentRoomLoc = roomLoc;
    }

    /// <summary>
    /// Calls register current room on the clients after some waiting
    /// </summary>
    /// <returns></returns>
    public IEnumerator SendRoom()
    {
        yield return new WaitForSeconds(2.0f);
        photonView.RPC("RegisterCurrentRoom", PhotonTargets.Others, m_currentRoomLoc.X, m_currentRoomLoc.Y);
    }
    */
}
