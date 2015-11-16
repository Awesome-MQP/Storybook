using UnityEngine;
using System.Collections;
using Photon;

public class TestRpc : PunBehaviour
{
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        photonView.TransferController(newPlayer);
    }

    public override void OnStartController(bool wasSpawn)
    {
        Debug.Log("I am controller.");
    }
}
