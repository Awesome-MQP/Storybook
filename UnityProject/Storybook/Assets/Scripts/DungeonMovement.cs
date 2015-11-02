using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonMovement : Photon.PunBehaviour {

    [SerializeField]
    private GameObject m_characterPrefab;

    private int m_hostPlayerId;
    private static PhotonView m_scenePhotonView;
    private MapManager m_mapManager;
    private RoomObject m_currentRoom;
    private Location m_currentLoc;
    private Location m_newRoomLoc;
    private GameObject m_playerPawn = null;
    private List<GameObject> m_playerPawns = new List<GameObject>();

    // Use this for initialization
    void Start () {
        m_scenePhotonView = this.GetComponent<PhotonView>();
        m_scenePhotonView.RPC("SelectHostPlayer", PhotonTargets.MasterClient);
        m_mapManager = FindObjectOfType<MapManager>();
        m_currentLoc = new Location(0, 0);
        m_currentRoom = m_mapManager.PlaceRoom(m_currentLoc);
        m_scenePhotonView.RPC("PlacePlayer", PhotonTargets.All);
    }

    void Update()
    {
        // If a pawn is at the destination, stop all pawns from moving and move all players to the new room
        if (m_playerPawns.Count > 0 && m_playerPawns[0].GetComponent<CharacterAnimator>().IsAtDestination)
        {
            m_scenePhotonView.RPC("StopPlayer", PhotonTargets.All);
            m_scenePhotonView.RPC("MoveToRoomAtLoc", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);
        }
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
                m_newRoomLoc = new Location(m_currentLoc.X + 1, m_currentLoc.Y);
                m_scenePhotonView.RPC("SetNewRoomLoc", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);

                // If the selected door's room has not been spawned, create the room
                if (!northDoor.IsDoorRoomSpawned)
                {
                    m_scenePhotonView.RPC("CreateNewRoom", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);
                    northDoor.SetIsDoorRoomSpawned(true);
                }
                // Otherwise just move to the room
                m_scenePhotonView.RPC("MovePlayer", PhotonTargets.All, northDoor.transform.position);
            }

            // Only allow the player to select a door if the door is enabled for the current room 
            if (westDoor.IsDoorEnabled && GUILayout.Button("Move Left"))
            {
                m_newRoomLoc = new Location(m_currentLoc.X, m_currentLoc.Y - 1);
                m_scenePhotonView.RPC("SetNewRoomLoc", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);

                // If the selected door's room has not been spawned, create the room
                if (!westDoor.IsDoorRoomSpawned)
                {
                    m_scenePhotonView.RPC("CreateNewRoom", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);
                    westDoor.SetIsDoorRoomSpawned(true);
                }
                m_scenePhotonView.RPC("MovePlayer", PhotonTargets.All, westDoor.transform.position);
            }

            // Only allow the player to select a door if the door is enabled for the current room 
            if (eastDoor.IsDoorEnabled && GUILayout.Button("Move Right"))
            {
                m_newRoomLoc = new Location(m_currentLoc.X, m_currentLoc.Y + 1);
                m_scenePhotonView.RPC("SetNewRoomLoc", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);

                // If the selected door's room has not been spawned, create the room
                if (!eastDoor.IsDoorRoomSpawned)
                {
                    m_scenePhotonView.RPC("CreateNewRoom", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);
                    eastDoor.SetIsDoorRoomSpawned(true);
                }
                m_scenePhotonView.RPC("MovePlayer", PhotonTargets.All, eastDoor.transform.position);
            }

            // Only allow the player to select a door if the door is enabled for the current room 
            if (southDoor.IsDoorEnabled && GUILayout.Button("Move Down"))
            {
                m_newRoomLoc = new Location(m_currentLoc.X - 1, m_currentLoc.Y);
                m_scenePhotonView.RPC("SetNewRoomLoc", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);

                // If the selected door's room has not been spawned, create the room
                if (!southDoor.IsDoorRoomSpawned)
                {
                    m_scenePhotonView.RPC("CreateNewRoom", PhotonTargets.All, m_newRoomLoc.X, m_newRoomLoc.Y);
                    southDoor.SetIsDoorRoomSpawned(true);
                }
                m_scenePhotonView.RPC("MovePlayer", PhotonTargets.All, southDoor.transform.position);
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
        m_mapManager.PlaceRoom(newRoomLoc);
        m_newRoomLoc = newRoomLoc;
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
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            Vector3 playerLoc = Vector3.zero;
            // Place the player at the corresponding player node in the room
            if (i == 0)
            {
                playerLoc = m_currentRoom.Player1Node.position;
            }
            else if (i == 1)
            {
                playerLoc = m_currentRoom.Player2Node.position;
            }
            else if (i == 2)
            {
                playerLoc = m_currentRoom.Player3Node.position;
            }
            else if (i == 3)
            {
                playerLoc = m_currentRoom.Player4Node.position;
            }

            // If the player pawn has not been created, instantiate it
            if (m_playerPawns.Count <= i)
            {
                GameObject pawn = (GameObject) Instantiate(m_characterPrefab, playerLoc, Quaternion.identity);
                m_playerPawns.Add(pawn);
            }
            // Otherwise, just move the existing pawn
            else
            {
                m_playerPawns[i].transform.position = playerLoc;
            }
        }
    }

    /// <summary>
    /// Sets a destination for all of the player pawns
    /// </summary>
    /// <param name="targetLocation"></param>
    [PunRPC]
    private void MovePlayer(Vector3 targetLocation)
    {
        foreach (GameObject pawn in m_playerPawns)
        {
            pawn.GetComponent<CharacterAnimator>().SetDestination(targetLocation);
        }
    }

    /// <summary>
    /// Stops all the player pawns from moving
    /// </summary>
    [PunRPC]
    private void StopPlayer()
    {
        foreach (GameObject pawn in m_playerPawns)
        {
            pawn.GetComponent<CharacterAnimator>().StopMoving();
        }
    }

    /// <summary>
    /// Sets the newRoomLoc variable to the given location on all clients
    /// </summary>
    /// <param name="locX">The X coordinate of the new location</param>
    /// <param name="locY">The Y coordinate of the new location</param>
    [PunRPC]
    private void SetNewRoomLoc(int locX, int locY)
    {
        m_newRoomLoc = new Location(locX, locY);
    }
}
