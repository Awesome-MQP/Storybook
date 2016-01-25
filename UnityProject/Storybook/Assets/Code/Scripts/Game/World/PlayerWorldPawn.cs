using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Assertions;

// The PlayerWorldPawn extends the WorldPawn class to play animations depending on movement,
// as well as position the players depending on the number of players.
// PhotonNetwork.Player.ID

// Diamond positions:
// X 1 X
// 2 X 3
// X 4 X

// Construct the PlayerWorldPawn using a Transform - this transform is the node that the pawn will adhere to!
public class PlayerWorldPawn : WorldPawn, IConstructable<Transform>{

    private int m_playerNum = 0;
    private float m_playerSpeed = 1.0f;
    private Transform m_targetNode;

    // Use this for initialization
    void Start ()
    {
        m_playerNum = PhotonNetwork.player.ID;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    // if the player is moving, set the animation to moving
        if(!IsAtTarget)
        {
            SwitchCharacterToWalking();
        }

        // if the player has stopped moving, move to their position in the diamond.
        if(IsAtTarget)
        {
            //GetIntoPosition(m_playerNum);
            SwitchCharacterToIdle();
        }
	}

    private void GetIntoPosition(int id)
    {
        Vector3 newTargetPosition = Vector3.zero;

        Assert.IsTrue(IsMine);

        TargetPosition = newTargetPosition;
    }

    // Constructor granted by the IConstructable
    public void Construct(Transform parameter)
    {
        TargetPosition = parameter.position;
        m_targetNode = parameter;
    }

    // Animation transitions
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
