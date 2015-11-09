using UnityEngine;
using System.Collections;

public class TestLobby : Photon.PunBehaviour
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("alpha");
    }

    public override void OnConnectedToMaster()
    {
#if UNITY_EDITOR
        PhotonNetwork.CreateRoom("AlphaRoom");
#else
        PhotonNetwork.JoinRoom("AlphaRoom");
#endif
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
    }
}
