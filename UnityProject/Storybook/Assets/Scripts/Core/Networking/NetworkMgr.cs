using UnityEngine;
using System.Collections;

public class NetworkMgr : Photon.MonoBehaviour {

	// Use this for initialization
	void Awake () {
    }

    void OnJoinedRoom()
    {
        //Spawn player prefab
        GameObject player = PhotonNetwork.Instantiate("PrototypePlayer", new Vector3(0, 1, 0), Quaternion.identity, 0);
    }

}
