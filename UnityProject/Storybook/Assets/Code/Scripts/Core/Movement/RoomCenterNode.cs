using UnityEngine;
using System.Collections;

public class RoomCenterNode : MovementNode {

    protected override void OnEnter(NetworkNodeMover mover)
    {
        //Debug.Log("Entering node");
    }

    protected override void OnLeave(NetworkNodeMover mover)
    {
        //Debug.Log("Exiting node");
    }

}
