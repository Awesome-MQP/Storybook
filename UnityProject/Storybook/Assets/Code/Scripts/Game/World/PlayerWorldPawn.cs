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
public class PlayerWorldPawn : WorldPawn
{
    private int m_playerNum = 0;
    private float m_playerSpeed = 1.0f;
    private Animator m_animator;
    private bool m_isIdle;
    private bool m_isWalking;

    public bool IsIdle
    {
        get { return m_isIdle; }
        protected set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_isIdle = value;
            m_animator.SetBool("IsIdle", value);
            PropertyChanged();
        }
    }

    public bool IsWalking
    {
        get { return m_isIdle; }
        protected set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_isWalking = value;
            m_animator.SetBool("IsWalking", value);
            PropertyChanged();
        }
    }

    // Use this for initialization
    void Start ()
    {
        m_playerNum = PhotonNetwork.player.ID;
        m_animator = GetComponent<Animator>();
    }

    private void GetIntoPosition(int id)
    {
        Vector3 newTargetPosition = Vector3.zero;

        Assert.IsTrue(IsMine);

        TargetPosition = newTargetPosition;
    }

    //Set the node that we want to go towards.
    public void SetHomeNode(MovementNode node)
    {
        TargetNode = node;
    }

    // Animation transitions
    public void SwitchCharacterToWalking()
    {
        IsWalking = true;
        IsIdle = false;
    }

    public void SwitchCharacterToIdle()
    {
        IsWalking = false;
        IsIdle = true;
    }
}
