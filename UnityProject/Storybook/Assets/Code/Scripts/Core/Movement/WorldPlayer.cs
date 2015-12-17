using UnityEngine;
using System.Collections;

public class WorldPlayer : NetworkNodeMover {

    public void SwitchCharacterToWalking()
    {
        photonView.RPC("RPCSwitchCharacterToWalking", PhotonTargets.All);
    }

    public void SwitchCharacterToIdle()
    {
        photonView.RPC("RPCSwitchCharacterToIdle", PhotonTargets.All);
    }

    [PunRPC]
    public void RPCSwitchCharacterToWalking()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetBool("IdleToIdle", false);
        animator.SetBool("WalkToIdle", false);
        animator.SetBool("IdleToWalk", true);
    }

    [PunRPC]
    public void RPCSwitchCharacterToIdle()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetBool("WalkToWalk", false);
        animator.SetBool("IdleToWalk", false);
        animator.SetBool("WalkToIdle", true);
    }

}
