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
public class PlayerWorldPawn : WorldPawn, IConstructable<PlayerEntity>
{
    private int m_playerNum = 0;
    private float m_playerSpeed = 1.0f;
    private Animator m_animator;
    private bool m_isIdle;
    private bool m_isWalking;

    [SyncProperty]
    public bool IsIdle
    {
        get { return m_isIdle; }
        protected set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_isIdle = value;
            _setAnimatorToIdle();
            PropertyChanged();
        }
    }

    [SyncProperty]
    public bool IsWalking
    {
        get { return m_isWalking; }
        protected set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_isWalking = value;
            _setAnimatorToWalking();
            PropertyChanged();
        }
    }

    protected override void Awake()
    {
        m_animator = GetComponent<Animator>();

        base.Awake();
    }

    public void Construct(PlayerEntity playerEntity)
    {

    }

    //Set the node that we want to go towards.
    public void SetHomeNode(MovementNode node)
    {
        TargetNode = node;
    }

    // Animation transitions

    /// <summary>
    /// Switches the character to walking animation for all clients
    /// </summary>
    public void SwitchCharacterToWalking()
    {
        IsWalking = true;
        IsIdle = false;

        _setAnimatorToWalking();
    }

    // Handle the animator bools for setting character to walking
    private void _setAnimatorToWalking()
    {
        m_animator.SetBool("IdleToIdle", false);
        m_animator.SetBool("WalkToWalk", true);
        m_animator.SetBool("WalkToIdle", false);
        m_animator.SetBool("IdleToWalk", true);
    }

    /// <summary>
    /// Switches character to idle animation for all clients
    /// </summary>
    public void SwitchCharacterToIdle()
    {
        IsWalking = false;
        IsIdle = true;

        _setAnimatorToIdle();
    }

    // Handle the animator bools for setting the character to walking
    private void _setAnimatorToIdle()
    {
        m_animator.SetBool("IdleToIdle", true);
        m_animator.SetBool("WalkToWalk", false);
        m_animator.SetBool("IdleToWalk", false);
        m_animator.SetBool("WalkToIdle", true);
    }
}
