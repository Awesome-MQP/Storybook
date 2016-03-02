using UnityEngine;
using System.Collections;

public class TestLobby : Photon.PunBehaviour
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("Benny");
    }

    public override void OnConnectedToMaster()
    {
#if !UNITY_EDITOR
        PhotonNetwork.CreateRoom("AlphaRoom");
#else
        PhotonNetwork.JoinRoom("Benny");
#endif
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
    }
}
