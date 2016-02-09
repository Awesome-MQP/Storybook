using UnityEngine;
using System.Collections;

public class GameStartup : Photon.PunBehaviour {

	// Use this for initialization
	void Start () {
        PhotonNetwork.ConnectUsingSettings(SystemInfo.deviceUniqueIdentifier);
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
        PhotonNetwork.LoadLevel("MainMenu");
    }
}
