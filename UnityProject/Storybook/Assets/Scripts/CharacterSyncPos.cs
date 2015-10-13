using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CharacterSyncPos : NetworkBehaviour {

    [SyncVar(hook = "SyncPositionValues")]
    private Vector3 syncPos;

    private Vector3 lastPos;

    private float threshold = 0.5f;

    void FixedUpdate()
    {
        TransmitPosition();
    }

    [Client]
    void SyncPositionValues(Vector3 latestPos)
    {
        syncPos = latestPos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer && Vector3.Distance(transform.position, lastPos) > threshold)
        {
            CmdProvidePositionToServer(transform.position);
            lastPos = transform.position;
        }
    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
    }
}
