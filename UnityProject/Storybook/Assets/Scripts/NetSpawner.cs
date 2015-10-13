using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetSpawner : NetworkBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameObject newGameObject = Instantiate(m_prefab);
            NetworkServerExtended.SpawnWithChildren(newGameObject);
        }
    }

    [SerializeField]
    private GameObject m_prefab;
}
