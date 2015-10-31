using UnityEngine;
using System.Collections;

public class DungeonMovement : Photon.PunBehaviour {

    private int m_hostPlayerId;
    private static PhotonView m_scenePhotonView;
    private MapManager m_mapManager;
    private RoomObject m_currentRoom;
    private Location m_currentLoc;
    private GameObject m_playerPawn;

    // Use this for initialization
    void Start () {
        m_scenePhotonView = this.GetComponent<PhotonView>();
        m_scenePhotonView.RPC("SelectHostPlayer", PhotonTargets.MasterClient);
        m_mapManager = FindObjectOfType<MapManager>();
        m_currentLoc = new Location(0, 0);
        m_currentRoom = m_mapManager.PlaceRoom(m_currentLoc);
        m_scenePhotonView.RPC("PlacePlayer", PhotonTargets.All);
    }

    void OnGUI()
    {
        // Only the host can choose which direction to move in
        if (m_hostPlayerId == PhotonNetwork.player.ID)
        {
            Door northDoor = m_currentRoom.RoomDoors[m_currentRoom.NORTH_DOOR_INDEX];
            Door westDoor = m_currentRoom.RoomDoors[m_currentRoom.WEST_DOOR_INDEX];
            Door eastDoor = m_currentRoom.RoomDoors[m_currentRoom.EAST_DOOR_INDEX];
            Door southDoor = m_currentRoom.RoomDoors[m_currentRoom.SOUTH_DOOR_INDEX];
            
            // Only allow the player to select a door if the door is enabled for the current room 
            if (northDoor.IsDoorEnabled && GUILayout.Button("Move Up"))
            {
                Debug.Log("Moving up");
                Location newRoomLoc = new Location(m_currentLoc.X + 1, m_currentLoc.Y);

                // If the selected door's room has not been spawned, create the room
                if (!northDoor.IsDoorRoomSpawned)
                {
                    Debug.Log("North door room not spawned");
                    m_scenePhotonView.RPC("CreateNewRoom", PhotonTargets.All, newRoomLoc.X, newRoomLoc.Y);
                    northDoor.SetIsDoorRoomSpawned(true);
                }
                // Otherwise just move to the room
                else
                {
                    m_scenePhotonView.RPC("MoveToRoomAtLoc", PhotonTargets.All, newRoomLoc.X, newRoomLoc.Y);
                }
            }

            // Only allow the player to select a door if the door is enabled for the current room 
            if (westDoor.IsDoorEnabled && GUILayout.Button("Move Left"))
            {
                Debug.Log("Moving left");
                Location newRoomLoc = new Location(m_currentLoc.X, m_currentLoc.Y - 1);

                // If the selected door's room has not been spawned, create the room
                if (!westDoor.IsDoorRoomSpawned)
                {
                    m_scenePhotonView.RPC("CreateNewRoom", PhotonTargets.All, newRoomLoc.X, newRoomLoc.Y);
                    westDoor.SetIsDoorRoomSpawned(true);
                }
                // Otherwise just move to the room
                else
                {
                    m_scenePhotonView.RPC("MoveToRoomAtLoc", PhotonTargets.All, newRoomLoc.X, newRoomLoc.Y);
                }
            }

            // Only allow the player to select a door if the door is enabled for the current room 
            if (eastDoor.IsDoorEnabled && GUILayout.Button("Move Right"))
            {
                Debug.Log("Moving right");
                Location newRoomLoc = new Location(m_currentLoc.X, m_currentLoc.Y + 1);

                // If the selected door's room has not been spawned, create the room
                if (!eastDoor.IsDoorRoomSpawned)
                {
                    m_scenePhotonView.RPC("CreateNewRoom", PhotonTargets.All, newRoomLoc.X, newRoomLoc.Y);
                    eastDoor.SetIsDoorRoomSpawned(true);
                }
                // Otherwise just move to the room
                else
                {
                    m_scenePhotonView.RPC("MoveToRoomAtLoc", PhotonTargets.All, newRoomLoc.X, newRoomLoc.Y);
                }
            }

            // Only allow the player to select a door if the door is enabled for the current room 
            if (southDoor.IsDoorEnabled && GUILayout.Button("Move Down"))
            {
                Debug.Log("Moving down");
                Location newRoomLoc = new Location(m_currentLoc.X - 1, m_currentLoc.Y);

                // If the selected door's room has not been spawned, create the room
                if (!southDoor.IsDoorRoomSpawned)
                {
                    m_scenePhotonView.RPC("CreateNewRoom", PhotonTargets.All, newRoomLoc.X, newRoomLoc.Y);
                    southDoor.SetIsDoorRoomSpawned(true);
                }
                // Otherwise just move to the room
                else
                {
                    m_scenePhotonView.RPC("MoveToRoomAtLoc", PhotonTargets.All, newRoomLoc.X, newRoomLoc.Y);
                }
            }
        }
    }

    /// <summary>
    /// Randomly chooses the host player
    /// The host player is the one that chooses where to move through the dungeon
    /// </summary>
    [PunRPC]
    private void SelectHostPlayer()
    {
        m_hostPlayerId = UnityEngine.Random.Range(1, PhotonNetwork.playerList.Length);
        m_scenePhotonView.RPC("SendOutHostId", PhotonTargets.Others, m_hostPlayerId);
        Debug.Log("Host player = " + m_hostPlayerId);
    }

    /// <summary>
    /// Sends out the chosen hostId to all the other players in the game
    /// </summary>
    /// <param name="hostId">The photon player ID to send out</param>
    [PunRPC]
    private void SendOutHostId(int hostId)
    {
        m_hostPlayerId = hostId;
    }

    /// <summary>
    /// Creates a new room at the given coordinates in the room grid and moves the players to this new room
    /// </summary>
    /// <param name="newRoomLocX">The X coordinate of the room to create</param>
    /// <param name="newRoomLocY">The Y coordinate of the room to create</param>
    [PunRPC]
    private void CreateNewRoom(int newRoomLocX, int newRoomLocY)
    {
        Location newRoomLoc = new Location(newRoomLocX, newRoomLocY);
        m_currentRoom = m_mapManager.PlaceRoom(newRoomLoc);
        m_currentLoc = newRoomLoc;
        Transform roomCamNode = m_currentRoom.CameraNode;
        Camera.main.transform.position = roomCamNode.transform.position;
        PlacePlayer();
    }

    /// <summary>
    /// Moves the players to the room at the given coordinates in the room grid
    /// </summary>
    /// <param name="roomLocX">The X coordinate of the room to move to</param>
    /// <param name="roomLocY">The Y coordinate of the room to move to</param>
    [PunRPC]
    private void MoveToRoomAtLoc(int roomLocX, int roomLocY)
    {
        Location roomLoc = new Location(roomLocX, roomLocY);
        m_currentRoom = m_mapManager.GetRoom(roomLoc);
        m_currentLoc = roomLoc;
        Transform roomCamNode = m_currentRoom.CameraNode;
        Camera.main.transform.position = roomCamNode.transform.position;
        PlacePlayer();
    }

    /// <summary>
    /// Places the player at the corresponding player node in the current room
    /// </summary>
    [PunRPC]
    private void PlacePlayer()
    {
        Vector3 playerLoc = Vector3.zero;
        // Place the player at the corresponding player node in the room
        if (PhotonNetwork.player.ID == 1)
        {
            playerLoc = m_currentRoom.Player1Node.position;
        }
        else if (PhotonNetwork.player.ID == 2)
        {
            playerLoc = m_currentRoom.Player2Node.position;
        }
        else if (PhotonNetwork.player.ID == 3)
        {
            playerLoc = m_currentRoom.Player3Node.position;
        }
        else if (PhotonNetwork.player.ID == 4)
        {
            playerLoc = m_currentRoom.Player4Node.position;
        }

        // If the player pawn has not been created, instantiate it
        if (m_playerPawn == null)
        {
            m_playerPawn = PhotonNetwork.Instantiate("PlayerCharacter", playerLoc, Quaternion.identity, 0);
        }
        // Otherwise, just move the existing pawn
        else
        {
            m_playerPawn.transform.position = playerLoc;
        }
    }
}
