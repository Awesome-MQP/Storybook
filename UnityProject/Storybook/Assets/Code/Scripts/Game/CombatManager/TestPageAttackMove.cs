using UnityEngine;
using System.Collections;
using System;

public class TestPageAttackMove : PageMove {

    private const bool IS_MOVE_ATTACK = true;

    void Awake()
    {
        Debug.Log("Setting is move attack");
        SetIsMoveAttack(IS_MOVE_ATTACK);
    }

    /// <summary>
    /// Deals damage to all the combat pawns in the target list
    /// </summary>
    protected override void DoMoveEffect()
    {
        foreach (CombatPawn enemyPawn in MoveTargets)
        {
            int moveDamage = StatsManager.CalcDamage(MoveOwner.PawnGenre, enemyPawn.PawnGenre, MoveGenre, MoveLevel, MoveOwner.Attack, enemyPawn.Defense);
            enemyPawn.DealDamageToPawn(moveDamage);
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
