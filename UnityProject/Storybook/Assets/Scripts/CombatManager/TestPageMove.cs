using UnityEngine;
using System.Collections;
using System;

public class TestPageMove : PageMove {

    private int MOVE_DAMAGE = 5;
    private int MOVE_TARGETS = 4;
    private bool IS_MOVE_ATTACK = true;

    public TestPageMove()
    {
        SetIsMoveAttack(true);
        SetNumberOfTargets(MOVE_TARGETS);
    }

    /// <summary>
    /// Deals damage to all the combat pawns in the target list
    /// </summary>
    public override void DoMove()
    {
        foreach (CombatPawn enemyPawn in MoveTargets)
        {
            enemyPawn.DealDamageToPawn(MOVE_DAMAGE);
        }
    }

}
