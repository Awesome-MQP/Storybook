using UnityEngine;
using System.Collections;

public class TestInventoryCreator : MonoBehaviour {

    [SerializeField]
    private Inventory m_inventoryPrefab;

	// Use this for initialization
	void Start () {
        GameObject inventoryObject = PhotonNetwork.Instantiate(m_inventoryPrefab.name, Vector3.zero, Quaternion.identity, 0);
        PhotonNetwork.Spawn(inventoryObject.GetComponent<PhotonView>());
	}

}
