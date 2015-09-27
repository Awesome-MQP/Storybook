using UnityEngine;
using System.Collections.Generic;

public abstract class CombatMove {

    /// <summary>
    /// Carries out the action that the move does
    /// Ex: Attack the enemies, heal the players, etc.
    /// </summary>
    public abstract void DoMove();

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
}
