using UnityEngine;
using System.Collections;

public class MatchMaker : Photon.PunBehaviour {

    private bool isFirst = false;
    private static PhotonView ScenePhotonView;

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
        ScenePhotonView = this.GetComponent<PhotonView>();
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        if (isFirst && GUILayout.Button("Start Game"))
        {
            ScenePhotonView.RPC("StartGame", PhotonTargets.All);
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.playerList.Length == 1)
        {
            isFirst = true;
        }
    }

    [PunRPC]
    private void StartGame()
    {
        Application.LoadLevel("TestRoomPlacement");
    }
}
