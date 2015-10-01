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

    /// <summary>
    /// Chooses the targets of the move based on a variety of criteria
    /// This function in the base class just checks to see if the number of possible targets is less than the move's
    /// number of targets
    /// </summary>
    /// <param name="possibleTargets">The list of combat pawn that are the possible targets for the move</param>
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

    /// <summary>
    /// The likelihood of the move being chosen (value is between 0-1)
    /// </summary>
    public double MoveFrequency
    {
        get { return m_moveFrequency; }
    }
}
