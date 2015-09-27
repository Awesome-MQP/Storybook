using UnityEngine;
using System.Collections.Generic;

public abstract class CombatMove {

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
    public List<CombatPawn> MoveTargets
    {
        get { return m_targets; }
    }

    public void SetMoveTargets(List<CombatPawn> newMoveTargets)
    {
        m_targets = newMoveTargets;
    }

    public bool IsMoveAttack
    {
        get { return m_isMoveAttack; }
    }

    public void SetIsMoveAttack(bool newIsMoveAttack)
    {
        m_isMoveAttack = newIsMoveAttack;
    }

    public int NumberOfTargets
    {
        get { return m_numberOfTargets; }
    }

    public void SetNumberOfTargets(int newNumberOfTargets)
    {
        m_numberOfTargets = newNumberOfTargets;
    }
}
