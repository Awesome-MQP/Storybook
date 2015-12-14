using UnityEngine;
using System.Collections;

public class TestMovementSpawner : MonoBehaviour {

    [SerializeField]
    private MapManager m_mapManager;

    [SerializeField]
    private NetworkNodeMover m_worldPlayer;

    [SerializeField]
    private PlayerMover m_playerGroup;



	// Use this for initialization
	void Awake () {
        GameObject mapObject = PhotonNetwork.Instantiate(m_mapManager.name, Vector3.zero, Quaternion.identity, 0);
        PhotonNetwork.Spawn(mapObject.GetComponent<PhotonView>());

        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            GameObject playerObject = PhotonNetwork.Instantiate(m_worldPlayer.name, Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(playerObject.GetComponent<PhotonView>());
        }

        GameObject playerGroup = PhotonNetwork.Instantiate(m_playerGroup.name, Vector3.zero, Quaternion.identity, 0);
        PhotonNetwork.Spawn(playerGroup.GetComponent<PhotonView>());
	}
	
}
