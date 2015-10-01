using UnityEngine;
using System.Collections.Generic;

public abstract class EnemyMove : CombatMove {

    /// <summary>
    /// The amount that the move costs
    /// The better the move, the more expensive it is
    /// </summary>
    [SerializeField]
    private int m_moveCost;

    [SerializeField]
    private double m_moveFrequency;

    public virtual void ChooseTargets(CombatPawn[] possibleTargets)
    {
        if (possibleTargets.Length <= NumberOfTargets)
        {
            List<CombatPawn> targetList = new List<CombatPawn>(possibleTargets);
            SetMoveTargets(targetList);
        }
    }
    
    public int MoveCost
    {
        get { return m_moveCost; }
    }

    public void SetMoveCost(int newMoveCost)
    {
        m_moveCost = newMoveCost;
    }

    public double MoveFrequency
    {
        get { return m_moveFrequency; }
    }
}
