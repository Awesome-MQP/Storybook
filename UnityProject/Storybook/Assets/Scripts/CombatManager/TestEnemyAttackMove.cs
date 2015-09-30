using UnityEngine;
using System.Collections.Generic;
using System;

public class TestEnemyAttackMove : EnemyMove {

    private int MOVE_DAMAGE = 3;
    private int MOVE_TARGETS = 4;

    public TestEnemyAttackMove()
    {
        SetIsMoveAttack(true);
        SetNumberOfTargets(MOVE_TARGETS);
    }

    /// <summary>
    /// Deals combat damage to all the player pawns in the target list
    /// </summary>
    protected override void DoMoveEffect()
    {
        foreach (CombatPawn combatPawn in MoveTargets)
        {
            combatPawn.DealDamageToPawn(MOVE_DAMAGE);
        }
    }

    public override void ExecuteMove()
    {
        SetTimeSinceMoveStarted(TimeSinceMoveStarted + Time.deltaTime);
        if (TimeSinceMoveStarted >= 0.5 && !IsMoveEffectCompleted)
        {
            DoMoveEffect();
            SetIsMoveEffectCompleted(true);
        }
        else if (TimeSinceMoveStarted >= 1)
        {
            Debug.Log("Enemy move is complete");
            SetIsMoveComplete(true);
        }
    }

}
