using UnityEngine;
using System.Collections.Generic;

public abstract class CombatEnemy : CombatPawn {

    private List<EnemyMove> m_enemyMoveList = new List<EnemyMove>();

    public List<EnemyMove> EnemyMoves
    {
        get { return m_enemyMoveList; }
        set { m_enemyMoveList = value; }
    }

    public abstract EnemyMove ChooseMove();
}
