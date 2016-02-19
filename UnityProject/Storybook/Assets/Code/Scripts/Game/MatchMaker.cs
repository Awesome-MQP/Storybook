using UnityEngine;
using System.Collections;

public class MatchMaker : Photon.PunBehaviour {

    private int[] m_idMap = new int[4];

    private bool isFirst = false;
    private static PhotonView ScenePhotonView;

    void Start()
    {
        // TODO
        PhotonNetwork.ConnectUsingSettings(SystemInfo.deviceUniqueIdentifier);
        ScenePhotonView = this.GetComponent<PhotonView>();
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        if (isFirst && GUILayout.Button("Start Game"))
        {
            PhotonNetwork.LoadLevel("TestRoomPlacement");
        }
        if (isFirst && GUILayout.Button("Start Combat"))
        {
            PhotonNetwork.LoadLevel("DemoCombatScene");
            Destroy(this);
        }
        if (isFirst && GUILayout.Button("Start World Movement"))
        {
            PhotonNetwork.LoadLevel("WorldMovementTest");
            Destroy(this);
        }
        if (isFirst && GUILayout.Button("Start Map Test"))
        {
            PhotonNetwork.LoadLevel("TestMapGeneration");
            Destroy(this);
        }
        if (isFirst && GUILayout.Button("Start Scene Fade"))
        {
            PhotonNetwork.LoadLevel("FaderScene1");
        }
        if (isFirst && GUILayout.Button("Load Level Bug"))
        {
            PhotonNetwork.LoadLevel("LoadLevelBugScene");
            Destroy(this);
        }
        if (isFirst && GUILayout.Button("Start Demo"))
        {
            PhotonNetwork.LoadLevel("TestingLevel");
            Destroy(this);
        }
    }

    /// <summary>
    /// When a player connects to master, have them join the lobby
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// When a player joins a lobby, join a random room
    /// </summary>
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// Create a room if the player can't join a random room
    /// </summary>
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null);
    }

    /// <summary>
    /// When the player joins a room, check to see if they are the first player in the room
    /// </summary>
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.playerList.Length == 1)
        {
            isFirst = true;
        }
    }

    /// <summary>
    /// Loads the TestRoomPlacement scene for all players
    /// </summary>
    [PunRPC]
    private void StartGame()
    {
        Application.LoadLevel("TestRoomPlacement");
    }

    /// <summary>
    /// Loads the DemoCombatScene for all players
    /// </summary>
    [PunRPC]
    private void StartCombat()
    {
        Application.LoadLevel("DemoCombatScene");
    }
}
