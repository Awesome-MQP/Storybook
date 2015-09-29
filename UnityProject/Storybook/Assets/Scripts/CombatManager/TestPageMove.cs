using UnityEngine;
using System.Collections;
using System;

public class TestPageMove : PageMove {

    private int MOVE_DAMAGE = 4;
    private int MOVE_TARGETS = 4;
    private bool IS_MOVE_ATTACK = true;

    public TestPageMove()
    {
        SetIsMoveAttack(IS_MOVE_ATTACK);
        SetNumberOfTargets(MOVE_TARGETS);
    }

    /// <summary>
    /// Deals damage to all the combat pawns in the target list
    /// </summary>
    protected override void DoMoveEffect()
    {
        foreach (CombatPawn enemyPawn in MoveTargets)
        {
            enemyPawn.DealDamageToPawn(MOVE_DAMAGE);
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
            Debug.Log("Page move is complete");
            SetIsMoveComplete(true);
        }
    }
}
