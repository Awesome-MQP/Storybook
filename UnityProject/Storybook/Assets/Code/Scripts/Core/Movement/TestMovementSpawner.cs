using UnityEngine;
using System.Collections;

public class TestMovementSpawner : MonoBehaviour {

    [SerializeField]
    private MapManager m_mapManager;

    [SerializeField]
    private MusicManager m_musicManager;

    [SerializeField]
    private NetworkNodeMover m_worldPlayer;

    [SerializeField]
    private RoomMover m_playerGroup;

    [SerializeField]
    private GameManager m_gameManager;

    [SerializeField]
    private PlayerInventory m_playerInventoryPrefab;

    [SerializeField]
    private DungeonMaster m_dungeonMasterPrefab;

	// Use this for initialization
	void Awake () {
        GameObject mapObject = PhotonNetwork.Instantiate(m_mapManager.name, Vector3.zero, Quaternion.identity, 0);
        PhotonNetwork.Spawn(mapObject.GetComponent<PhotonView>());

        GameObject dungeonMaster = PhotonNetwork.Instantiate(m_dungeonMasterPrefab.name, Vector3.zero, Quaternion.identity, 0);
        PhotonNetwork.Spawn(dungeonMaster.GetComponent<PhotonView>());

        GameObject musicManager = PhotonNetwork.Instantiate(m_musicManager.name, Vector3.zero, Quaternion.identity, 0);
        PhotonNetwork.Spawn(musicManager.GetComponent<PhotonView>());

        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            GameObject playerObject = PhotonNetwork.Instantiate(m_worldPlayer.name, Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(playerObject.GetComponent<PhotonView>());

            GameObject playerInventoryObject = PhotonNetwork.Instantiate(m_playerInventoryPrefab.name, Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(playerInventoryObject.GetComponent<PhotonView>());
            PlayerInventory playerInventory = playerInventoryObject.GetComponent<PlayerInventory>();
            playerInventory.PlayerId = i;
            dungeonMaster.GetComponent<DungeonMaster>().InitializeInventory(playerInventory);
        }

        GameObject playerGroup = PhotonNetwork.Instantiate(m_playerGroup.name, Vector3.zero, Quaternion.identity, 0);
        PhotonNetwork.Spawn(playerGroup.GetComponent<PhotonView>());

        GameObject gameManager = PhotonNetwork.Instantiate(m_gameManager.name, Vector3.zero, Quaternion.identity, 0);
        PhotonNetwork.Spawn(gameManager.GetComponent<PhotonView>());
	}
	
}
