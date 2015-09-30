using UnityEngine;
using System.Collections.Generic;

public abstract class EnemyMove : CombatMove {

    /// <summary>
    /// The amount that the move costs
    /// The better the move, the more expensive it is
    /// </summary>
    private int m_moveCost;
    
    public int MoveCost
    {
        get { return m_moveCost; }
    }

    public void SetMoveCost(int newMoveCost)
    {
        m_moveCost = newMoveCost;
    }
}
