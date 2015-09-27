using UnityEngine;
using System.Collections.Generic;

public class TestEnemyMove : EnemyMove {

    private int MOVE_DAMAGE = 3;
    private int MOVE_TARGETS = 4;

    public TestEnemyMove()
    {
        SetIsMoveAttack(true);
        SetNumberOfTargets(MOVE_TARGETS);
    }

    /// <summary>
    /// Deals combat damage to all the player pawns in the target list
    /// </summary>
    public override void DoMove()
    {
        foreach (CombatPawn combatPawn in MoveTargets)
        {
            combatPawn.DealDamageToPawn(MOVE_DAMAGE);
        }
    }
	
}
