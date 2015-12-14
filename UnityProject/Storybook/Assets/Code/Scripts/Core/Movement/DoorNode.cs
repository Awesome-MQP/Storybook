using UnityEngine;
using System.Collections;
using System;

public class DoorNode : MovementNode
{

    [SerializeField]
    Door m_nodeDoor;

    protected override void OnEnter(NetworkNodeMover mover)
    {
        Debug.Log("Entering door node");
        PlayerMover playerMover = FindObjectOfType<PlayerMover>();

        if (PhotonNetwork.isMasterClient)
        {
            playerMover.MoveThroughDoor(m_nodeDoor, true);
        }
    }

    protected override void OnLeave(NetworkNodeMover mover)
    {
        Debug.Log("Exiting door node");
    }
}
