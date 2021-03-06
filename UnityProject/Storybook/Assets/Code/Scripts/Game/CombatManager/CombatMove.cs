﻿using UnityEngine;
using System.Collections.Generic;

public abstract class CombatMove : MonoBehaviour{

    private bool m_isMoveStarted = false;

    /// <summary>
    /// Carries out the effect that the move does
    /// Ex: Attack the enemies, heal the players, etc.
    /// </summary>
    protected abstract void DoMoveEffect();

    /// <summary>
    /// Called each frame during the execute state until the move is complete
    /// Handles the animation for the move as well as calling DoMoveEffect 
    /// </summary>
    public virtual void ExecuteMove()
    {
        NetExecuteState executeState = FindObjectOfType<NetExecuteState>();
        Animator playerAnimator = executeState.CurrentCombatPawn.GetComponent<Animator>();
        float bufferTime = 0.5f;
        if (!m_isMoveStarted)
        {
            // If the move is an attack, play the attack animation
            if (IsMoveAttack)
            {
                _SetAnimatorToAttack(playerAnimator);   
            }
            // Otherwise play the support animation
            else
            {
                _SetAnimatorToSupport(playerAnimator);
            }
            m_isMoveStarted = true;
        }

        AnimatorClipInfo[] allClips = playerAnimator.GetCurrentAnimatorClipInfo(0);
        float clipLength = allClips[0].clip.length;
        float waitTime = clipLength;

        if (IsMoveEffectCompleted)
        {
            float longestHurtAnim = _longestHurtAnim();
            if (longestHurtAnim > clipLength)
            {
                waitTime = longestHurtAnim;
            }
        }

        SetTimeSinceMoveStarted(TimeSinceMoveStarted + Time.deltaTime);

        if (TimeSinceMoveStarted >= 0.5f && !IsMoveEffectCompleted)
        {
            DoMoveEffect();
            SetIsMoveEffectCompleted(true);
        }
        else if (TimeSinceMoveStarted >= waitTime + bufferTime)
        {
            Debug.Log("Page move is complete");
            _SetAnimatorToIdle(playerAnimator);
            SetIsMoveComplete(true);
            m_isMoveStarted = false;
            SetTimeSinceMoveStarted(0);
            _switchTargetsToIdle();
        }
    }

    private float _longestHurtAnim()
    {
        NetExecuteState executeState = FindObjectOfType<NetExecuteState>();
        float longestClipTime = 0;
        foreach (CombatPawn pawn in m_targets)
        { 
            Animator playerAnimator = executeState.CurrentCombatPawn.GetComponent<Animator>();
            float clipTime = playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            if (clipTime > longestClipTime)
            {
                longestClipTime = clipTime;
            }
        }
        return longestClipTime;
    }

    private void _switchTargetsToIdle()
    {
        foreach(CombatPawn pawn in MoveTargets)
        {
            pawn.SwitchToIdleAnim();
        }
    }

    /// <summary>
    /// Sets the given player animator to play the attack animation
    /// </summary>
    /// <param name="playerAnimator">The animator to switch to the attack animation</param>
    private void _SetAnimatorToAttack(Animator playerAnimator)
    {
        playerAnimator.SetBool("IdleToIdle", false);
        playerAnimator.SetBool("WalkToIdle", false);
        playerAnimator.SetBool("AttackToIdle", false);
        playerAnimator.SetBool("IdleToAttack", true);
    }

    /// <summary>
    /// Sets the given player animator to play the support animation
    /// </summary>
    /// <param name="playerAnimator">The animator to switch to the support animation</param>
    private void _SetAnimatorToSupport(Animator playerAnimator)
    {
        playerAnimator.SetBool("IdleToIdle", false);
        playerAnimator.SetBool("SupportToIdle", false);
        playerAnimator.SetBool("IdleToSupport", true);
    }

    /// <summary>
    /// Sets the given player animator to the idle animation
    /// </summary>
    /// <param name="playerAnimator">the animator to switch to the idle animation</param>
    private void _SetAnimatorToIdle(Animator playerAnimator)
    {
        playerAnimator.SetBool("IdleToAttack", false);
        playerAnimator.SetBool("IdleToSupport", false);
        playerAnimator.SetBool("SupportToIdle", true);
        playerAnimator.SetBool("AttackToIdle", true);
        playerAnimator.SetBool("IdleToIdle", true);
    }

    /// <summary>
    /// The combat pawns that are being targeted by the move
    /// </summary>
    private List<CombatPawn> m_targets = new List<CombatPawn>();

    /// <summary>
    /// The maximum number of targets that the move has
    /// </summary>
    [SerializeField]
    private int m_numberOfTargets;

    /// <summary>
    /// The genre of the move
    /// </summary>
    [SerializeField]
    private Genre m_moveGenre;

    ///<summary>
    /// The type of the move
    /// </summary>
    [SerializeField]
    private MoveType m_moveType;

    /// <summary>
    /// The level of the move
    /// </summary>
    [SerializeField]
    private int m_moveLevel;

    /// <summary>
    /// The rarity of the move
    /// </summary>
    [SerializeField]
    private bool m_moveRarity;

    /// <summary>
    /// True if the move targets the pawns of the opposing side, false if it effects pawns of the same side
    /// Ex: true if it is a player move that deals damage to an enemy
    /// </summary>
    [SerializeField]
    private bool m_isMoveAttack;

    private bool m_isMoveEffectDone = false;

    private bool m_isMoveCompleted = false;

    private float m_timeSinceMoveStarted = 0;

    private CombatPawn m_moveOwner;

    /// <summary>
    /// Initializes move variables
    /// </summary>
    public void InitializeMove()
    {
        m_isMoveEffectDone = false;
        m_isMoveCompleted = false;
        m_timeSinceMoveStarted = 0;
    }

    /// <summary>
    /// Removes the given target from the target list if it is in the target list
    /// </summary>
    /// <param name="targetToRemove">The CombatPawn to remove from the list of targets</param>
    public void RemoveTarget(CombatPawn targetToRemove)
    {
        if (m_targets.Contains(targetToRemove))
        {
            m_targets.Remove(targetToRemove);
        }
    }

    /// <summary>
    /// Property for the targets of the move
    /// </summary>
    public CombatPawn[] MoveTargets
    {
        get { return m_targets.ToArray(); }
    }

    /// <summary>
    /// Setter for the move targets
    /// </summary>
    /// <param name="newMoveTargets">The new move targets list of CombatPawn</param>
    public void SetMoveTargets(List<CombatPawn> newMoveTargets)
    {
        m_targets = newMoveTargets;
    }

    /// <summary>
    /// Property getter for the IsMoveAttack boolean
    /// </summary>
    public bool IsMoveAttack
    {
        get { return m_isMoveAttack; }
    }

    /// <summary>
    /// Setter for the IsMoveAttack boolean
    /// </summary>
    /// <param name="newIsMoveAttack">The new value for the IsMoveAttack boolean</param>
    public void SetIsMoveAttack(bool newIsMoveAttack)
    {
        m_isMoveAttack = newIsMoveAttack;
    }

    /// <summary>
    /// Property getter for the NumberOfTargets int
    /// </summary>
    public int NumberOfTargets
    {
        get { return m_numberOfTargets; }
    }

    /// <summary>
    /// Setter for the NumberOfTargets int
    /// </summary>
    /// <param name="newNumberOfTargets">The new value for the number of targets</param>
    public void SetNumberOfTargets(int newNumberOfTargets)
    {
        m_numberOfTargets = newNumberOfTargets;
    }

    /// <summary>
    /// True if the effect of the move has been done, false otherwise
    /// </summary>
    public bool IsMoveEffectCompleted
    {
        get { return m_isMoveEffectDone; }
    }

    public void SetIsMoveEffectCompleted(bool newIsEffectCompleted)
    {
        m_isMoveEffectDone = newIsEffectCompleted;
    }

    /// <summary>
    /// True if the move is done executing, false otherwise
    /// </summary>
    public bool IsMoveComplete
    {
        get { return m_isMoveCompleted; }
    }

    public void SetIsMoveComplete(bool newIsMoveCompleted)
    {
        m_isMoveCompleted = newIsMoveCompleted;
    }

    /// <summary>
    /// The time elapsed since starting the move, used as a placeholder for actual animation code
    /// </summary>
    public float TimeSinceMoveStarted
    {
        get { return m_timeSinceMoveStarted; }
    }

    public void SetTimeSinceMoveStarted(float newTimeSinceMoveStarted)
    {
        m_timeSinceMoveStarted = newTimeSinceMoveStarted;
    }

    public void SetMoveOwner(CombatPawn moveOwner)
    {
        m_moveOwner = moveOwner;
    }

    public CombatPawn MoveOwner
    {
        get { return m_moveOwner; }
    }

    public Genre MoveGenre
    {
        get { return m_moveGenre; }
        set { m_moveGenre = value; }
    }

    public MoveType MoveType
    {
        get { return m_moveType; }
        set { m_moveType = value; }
    }

    public int MoveLevel
    {
        get { return m_moveLevel; }
        set { m_moveLevel = value; }
    }

    public bool MoveRarity
    {
        get { return m_moveRarity; }
        set { m_moveRarity = value; }
    }
}
