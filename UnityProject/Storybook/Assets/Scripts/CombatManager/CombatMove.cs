using UnityEngine;
using System.Collections.Generic;

public abstract class CombatMove {

    /// <summary>
    /// Carries out the effect that the move does
    /// Ex: Attack the enemies, heal the players, etc.
    /// </summary>
    protected abstract void DoMoveEffect();

    /// <summary>
    /// Called each frame during the execute state until the move is complete
    /// Handles the animation for the move as well as calling DoMoveEffect 
    /// </summary>
    public abstract void ExecuteMove();

    /// <summary>
    /// The combat pawns that are being targeted by the move
    /// </summary>
    private List<CombatPawn> m_targets;

    /// <summary>
    /// The maximum number of targets that the move has
    /// </summary>
    private int m_numberOfTargets;

    /// <summary>
    /// True if the move targets the pawns of the opposing side, false if it effects pawns of the same side
    /// Ex: true if it is a player move that deals damage to an enemy
    /// </summary>
    private bool m_isMoveAttack;

    private bool m_isMoveEffectDone = false;

    private bool m_isMoveCompleted = false;

    private float m_timeSinceMoveStarted = 0;

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
}
