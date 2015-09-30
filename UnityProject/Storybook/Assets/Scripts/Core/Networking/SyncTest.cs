using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SyncTest : NetworkBehaviour
{
    [ServerCallback]
    void Update()
    {
        m_syncVar += Time.deltaTime;
    }

    [SyncVar, SerializeField]
    private float m_syncVar;
}
